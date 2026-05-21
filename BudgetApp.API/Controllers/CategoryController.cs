using BudgetApp.Application.Features.Categories.Commands;
using BudgetApp.Application.Features.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BudgetApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/categories")]

public class CategoryController : ControllerBase
{
    // MediatR injiceras via konstruktorn — används för att skicka Commands och Queries
    private readonly IMediator _mediator;

    public CategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    //Hämtar alla kategorier för ett specifikt budget
    [HttpGet("{budgetId}")]
    public async Task<IActionResult> GetAll(Guid budgetId)
    {
        try
        {
            var result = await _mediator.Send(new GetCategoriesQuery { BudgetId = budgetId });
            return Ok(result);
        }
        catch (Exception ex)
        {
            // Logga felet här
            return StatusCode(500, ex.Message);
        }
    }
    // Hämtar en specifik kategori baserat på Id
    // Returnerar 404 om kategorin inte hittas
    [HttpGet("single/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetCategoryByIdQuery { Id = id });
            if (result == null)
                return NotFound($"Category with id {id} not found");
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // Skapar en ny kategori
    // Returnerar 201 Created med den skapade kategorin
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
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


    // Uppdaterar en befintlig kategori
    // Returnerar 404 om kategorin inte hittas
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryCommand command)
    {
        try
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // Tar bort en kategori baserat på Id
    // Returnerar 204 No Content om borttagningen lyckades
    // Returnerar 404 om kategorin inte hittas
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteCategoryCommand { Id = id });
            if (!result)
                return NotFound($"Category with id {id} not found");
            return NoContent();


        }

        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}