using MediatR;
using Microsoft.AspNetCore.Mvc;
using PensionSystem.Application.Benefits.Commands;

namespace PensionSystem.API.Controllers;

/// <summary>
/// Manages pension benefit withdrawals.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BenefitsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BenefitsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Request a benefit withdrawal from a member's available balance.
    /// Member must have made at least 12 monthly contributions.
    /// </summary>
    [HttpPost("withdraw")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> WithdrawBenefit([FromBody] WithdrawBenefitCommand command)
    {
        var result = await _mediator.Send(command);
        return Created($"api/benefits/{result.Id}", result);
    }
}
