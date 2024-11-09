using Application.Common.Interfaces.UnitOfWorks;
using Domain.Aggregates.Customers;
using Domain.Aggregates.Tickets;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Data;

public class TicketSeeding
{
    public static async Task SeedingAsync(IServiceProvider provider)
    {
        IUnitOfWork unitOfWork = provider.GetRequiredService<IUnitOfWork>();

        if (await unitOfWork.Repository<Customer>().AnyAsync() && await unitOfWork.Repository<Ticket>().AnyAsync())
        {
            return;
        }

        List<Customer> customers =
        [
            new Customer()
            {
                FullName = "john corner",
                Address = "nyc,us",
                PhoneNumber = "0912356788",
            },
            new Customer()
            {
                FullName = "Rose kim",
                Address = "la,us",
                PhoneNumber = "0912356700",
            },
        ];

        await unitOfWork.Repository<Customer>().AddRangeAsync(customers);
        List<Ticket> tickets =
        [
            new Ticket()
            {
                EventName = "concert atsh 1",
                EventDate = new DateTime(2024, 12, 01),
                TotalQuantity = 10,
                Price = 200,
            },
            new Ticket()
            {
                EventName = "concert atsh 2",
                EventDate = new DateTime(2024, 12, 02),
                TotalQuantity = 5,
                Price = 250,
            },
        ];

        await unitOfWork.Repository<Ticket>().AddRangeAsync(tickets);
        await unitOfWork.SaveAsync();
    }
}
