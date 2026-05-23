using BudgetApp.Application.Features.Users.Commands;
using BudgetApp.Application.Features.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BudgetApp.API.Controllers;



[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    // MediatR injiceras via konstruktorn — används för att skicka Commands och Queries
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Hämtar alla användare
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _mediator.Send(new GetAllUsersQuery());
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // Hämtar en specifik användare baserat på Id
    // Returnerar 404 om användaren inte hittas
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetUserByIdQuery { Id = id });
            if (result == null)
                return NotFound($"User with id {id} not found");
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // Skapar en ny användare
    // Returnerar 201 Created med den skapade användaren
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
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

    // Uppdaterar en befintlig användare
    // Returnerar 404 om användaren inte hittas
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommand command)
    {
        try
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            if (result == null)
                return NotFound($"User with id {id} not found");
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // Tar bort en användare baserat på Id
    // Returnerar 404 om användaren inte hittas
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteUserCommand { Id = id });
            if (!result)
                return NotFound($"User with id {id} not found");
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}