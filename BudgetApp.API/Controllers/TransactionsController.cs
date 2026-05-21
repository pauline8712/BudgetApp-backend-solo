using BudgetApp.Application.Features.Transactions.Commands;
using BudgetApp.Application.Features.Transactions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/transactions")]
public class TransactionsController : ControllerBase
{
    // MediatR injiceras via konstruktorn — används för att skicka Commands och Queries
    private readonly IMediator _mediator;

    public TransactionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Hämtar alla transaktioner för en specifik kategori
    [HttpGet("{categoryId}")]
    public async Task<IActionResult> GetAll(Guid categoryId)
    {
        try
        {
            var result = await _mediator.Send(new GetTransactionsByCategoryQuery { CategoryId = categoryId });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // Hämtar en specifik transaktion baserat på Id
    // Returnerar 404 om transaktionen inte hittas
    [HttpGet("single/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetTransactionByIdQuery { Id = id });
            if (result == null)
                return NotFound($"Transaction with id {id} not found");
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // Skapar en ny transaktion
    // Returnerar 201 Created med den skapade transaktionen
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTransactionCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // Uppdaterar en befintlig transaktion
    // Returnerar 404 om transaktionen inte hittas
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTransactionCommand command)
    {
        try
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            if (result == null)
                return NotFound($"Transaction with id {id} not found");
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // Tar bort en transaktion baserat på Id
    // Returnerar 404 om transaktionen inte hittas
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteTransactionCommand { Id = id });
            if (!result)
                return NotFound($"Transaction with id {id} not found");
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // Hämtar alla transaktioner för en specifik kategori och vecka
    // Används av frontend för att visa veckovy på statistiksidan
    [HttpGet("byweek/{categoryId}/{year}/{weekNumber}")]
    public async Task<IActionResult> GetByWeek(Guid categoryId, int year, int weekNumber)
    {
        try
        {
            var result = await _mediator.Send(new GetTransactionsByWeekQuery
            {
                CategoryId = categoryId,
                Year = year,
                WeekNumber = weekNumber
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
         }
    }
}