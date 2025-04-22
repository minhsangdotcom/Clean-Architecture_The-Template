using Application.Common.Interfaces.UnitOfWorks;
using Contracts.Dtos.Responses;
using Domain.Aggregates.Carts;
using Domain.Aggregates.Carts.Enums;
using Domain.Aggregates.Carts.Specifications;
using Domain.Aggregates.Orders;
using Domain.Aggregates.Tickets;
using Mediator;

namespace Application.Features.Tickets.Carts.Pays;

public class PayCartHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<PayCartPayload, QueueResponse<PayCartResponse>>
{
    public async ValueTask<QueueResponse<PayCartResponse>> Handle(
        PayCartPayload request,
        CancellationToken cancellationToken
    )
    {
        Cart? cart = await unitOfWork
            .DynamicReadOnlyRepository<Cart>()
            .FindByConditionAsync(
                new FindCartByIdIncludeSpecification(Ulid.Parse(request.Payload!.CartId)),
                cancellationToken
            );

        if (cart == null)
        {
            return new QueueResponse<PayCartResponse>()
            {
                ErrorType = QueueErrorType.Persistent,
                IsSuccess = false,
                PayloadId = request.PayloadId,
                Error = new
                {
                    Message = "Car has not found",
                    Type = QueueErrorType.Persistent,
                    request.Payload!.CartId,
                },
            };
        }

        if (cart.IsPaid || cart.CartStatus == CartStatus.Expired)
        {
            return new QueueResponse<PayCartResponse>()
            {
                ErrorType = QueueErrorType.Persistent,
                IsSuccess = false,
                PayloadId = request.PayloadId,
                Error = new
                {
                    Message = "Car is invalid",
                    Type = QueueErrorType.Persistent,
                    request.Payload!.CartId,
                },
            };
        }

        List<CartItem> cartItems = [.. cart.CartItems!];
        if (cartItems.Any(x => x.Ticket!.UsedQuantity + x.Quantity > x.Ticket.TotalQuantity))
        {
            return new QueueResponse<PayCartResponse>()
            {
                ErrorType = QueueErrorType.Persistent,
                IsSuccess = false,
                PayloadId = request.PayloadId,
                Error = new
                {
                    Message = $"pay has failed for ticket is not enough quantity",
                    Type = QueueErrorType.Persistent,
                    request.Payload!.CartId,
                },
            };
        }
        // fake payment processing
        Random random = new();
        int fakeTime = random.Next(1, 6);
        await Task.Delay(fakeTime * 1000, cancellationToken);

        if (fakeTime % 2 != 0)
        {
            string error = "Payment service is unavailable.";
            cart.CartStatus = CartStatus.Failed;

            cart.PaymentResult = error;
            return new QueueResponse<PayCartResponse>()
            {
                ErrorType = QueueErrorType.Transient,
                IsSuccess = false,
                PayloadId = request.PayloadId,
                Error = new
                {
                    Message = error,
                    Type = QueueErrorType.Transient,
                    request.Payload!.CartId,
                },
            };
        }

        cart.IsPaid = true;
        cart.PaymentResult = "paying has been success";
        cart.CartStatus = CartStatus.Paid;
        await unitOfWork.Repository<Cart>().UpdateAsync(cart);

        List<Ticket> tickets = [];
        foreach (var cartItem in cartItems)
        {
            Ticket ticket = cartItem.Ticket!;
            ticket.UsedQuantity += cartItem.Quantity;
            tickets.Add(ticket);
        }
        await unitOfWork.Repository<Ticket>().UpdateRangeAsync(tickets);

        Order order =
            new()
            {
                CustomerId = request.Payload.CustomerId,
                PhoneNumber = cart.PhoneNumber,
                ShippingAddress = cart.ShippingAddress,
                ShippingFee = cart.ShippingFee,
                TotalAmount = cart.Total,
                OrderItems = cartItems.ToListOrderItem(),
            };
        await unitOfWork.Repository<Order>().AddAsync(order, cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);

        return new()
        {
            IsSuccess = true,
            PayloadId = request.PayloadId,
            ResponseData = cart.ToPayCartResponse(),
        };
    }
}
