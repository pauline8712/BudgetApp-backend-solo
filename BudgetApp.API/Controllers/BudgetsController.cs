using BudgetApp.Application.Features.Budgets.Commands;
using BudgetApp.Application.Features.Budgets.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BudgetApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/budgets")]
public class BudgetsController : ControllerBase
{
    // MediatR injiceras via konstruktorn — används för att skicka Commands och Queries
    private readonly IMediator _mediator;

    public BudgetsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetAll(Guid userId)
    {
        try
        {
            var result = await _mediator.Send(new GetAllBudgetsQuery { UserId = userId });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    //Hämtar en specifik budget baserat på Id
    //Returnerar 404 om budgeten inte hittas 
    [HttpGet("single/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetBudgetByIdQuery { Id = id });
            if (result == null)
                return NotFound($"Budget with id {id} not found");
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    //Skapar en ny budget
    //Returnerar 201 Created med den skapade budgeten
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBudgetCommand command)
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

    //Uppdaterar en befintligt budget
    //Returnerar 404 om budgeten inte hittas
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBudgetCommand command)
    {
        try
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            if (result == null)
                return NotFound($"Budget with id {id} not found");
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    //Tar bort en budget baserat på Id
    //Returnerar 404 om budgeten inte hittas

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteBudgetCommand { Id = id });
            if (!result)
                return NotFound($"Budget with id {id} not found");
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // Hämtar en budgetsammanfattning med kategorier och beräknade saldo
    // Returnerar 404 om budgeten inte hittas
    [HttpGet("summary/{budgetId}")]
    public async Task<IActionResult> GetSummary(Guid budgetId)
    {
        try
        {
            var result = await _mediator.Send(new GetBudgetSummaryQuery { BudgetId = budgetId });
            if (result == null)
                return NotFound($"Budget with id {budgetId} not found");
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // Hämtar ALLA budgetar — endast tillgänglig för Admin
    // Returnerar 403 Forbidden om användaren inte är Admin
    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllAdmin()
    {
        try
        {
            var result = await _mediator.Send(new GetAllBudgetsAdminQuery());
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}