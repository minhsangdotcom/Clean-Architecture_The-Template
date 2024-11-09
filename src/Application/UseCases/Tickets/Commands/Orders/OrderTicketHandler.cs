using Application.Common.Interfaces.UnitOfWorks;
using Contracts.Dtos.Responses;
using Domain.Aggregates.Tickets;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Tickets.Commands.Orders;

public class OrderTicketHandler(IDbContext dbContext)
{
}
