using MediatR;
using Microsoft.AspNetCore.Mvc;
using PensionSystem.Application.Contributions.Commands;
using PensionSystem.Application.Contributions.Queries;

namespace PensionSystem.API.Controllers;

/// <summary>
/// Manages pension contributions (monthly mandatory and voluntary).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ContributionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContributionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Add a new contribution for a member.
    /// Monthly contributions are limited to one per calendar month.
    /// Voluntary contributions can be made any time.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddContribution([FromBody] AddContributionCommand command)
    {
        var result = await _mediator.Send(command);
        return Created($"api/contributions/{result.Id}", result);
    }

    /// <summary>
    /// Retrieve all contributions for a specific member.
    /// </summary>
    [HttpGet("member/{memberId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetContributionsByMember(Guid memberId)
    {
        var result = await _mediator.Send(new GetContributionsByMemberQuery(memberId));
        return Ok(result);
    }
}
