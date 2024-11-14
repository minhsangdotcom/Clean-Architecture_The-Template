using Application.Common.Interfaces.UnitOfWorks;
using AutoMapper;
using Contracts.Dtos.Responses;
using Domain.Aggregates.Carts;
using Domain.Aggregates.Carts.Specifications;
using Domain.Aggregates.Orders;
using Domain.Aggregates.Tickets;
using Mediator;

namespace Application.UseCases.Tickets.Carts.Pays;

public class PayCartHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<PayCartPayload, QueueResponse<PayCartResponse>>
{
    public async ValueTask<QueueResponse<PayCartResponse>> Handle(
        PayCartPayload request,
        CancellationToken cancellationToken
    )
    {
        Cart? cart = await unitOfWork
            .Repository<Cart>()
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

        cart.IsPaid = true;
        cart.PaymentResult = "paying has been success";
        cart.CartStatus = Domain.Aggregates.Carts.Enums.CartStatus.Completed;
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
                CustomerId = request.Payload.Payload!.CustomerId,
                PhoneNumber = cart.PhoneNumber,
                ShippingAddress = cart.ShippingAddress,
                ShippingFee = cart.ShippingFee,
                TotalAmount = cart.Total,
                OrderItems = mapper.Map<List<OrderItem>>(cartItems),
            };
        await unitOfWork.Repository<Order>().AddAsync(order, cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);

        return new()
        {
            IsSuccess = true,
            PayloadId = request.PayloadId,
            ResponseData = mapper.Map<PayCartResponse>(cart),
        };
    }
}
