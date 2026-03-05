using MediatR;
using Microsoft.AspNetCore.Mvc;
using PensionSystem.Application.Members.Commands;
using PensionSystem.Application.Members.Queries;

namespace PensionSystem.API.Controllers;

/// <summary>
/// Manages pension member registration and profile operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MembersController : ControllerBase
{
    private readonly IMediator _mediator;

    public MembersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Register a new pension member.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateMember([FromBody] CreateMemberCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetMember), new { id = result.Id }, result);
    }

    /// <summary>
    /// Retrieve a member by their unique identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMember(Guid id)
    {
        var result = await _mediator.Send(new GetMemberByIdQuery(id));
        if (result == null) return NotFound(new { error = "Member not found.", statusCode = 404 });
        return Ok(result);
    }

    /// <summary>
    /// Update an existing member's details.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMember(Guid id, [FromBody] UpdateMemberCommand command)
    {
        // Ensure the route ID matches the command ID
        if (id != command.Id)
            return BadRequest(new { error = "Route ID does not match body ID.", statusCode = 400 });

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Soft-delete a member (marks as deleted but retains data).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMember(Guid id)
    {
        await _mediator.Send(new DeleteMemberCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Gets a comprehensive statement for the member including current balance and recent transactions.
    /// </summary>
    [HttpGet("{id:guid}/statement")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMemberStatement(Guid id)
    {
        var result = await _mediator.Send(new GetMemberStatementQuery(id));
        return Ok(result);
    }
}
