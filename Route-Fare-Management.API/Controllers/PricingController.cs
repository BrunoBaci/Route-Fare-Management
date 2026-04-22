using MediatR;
using Microsoft.AspNetCore.Mvc;
using Route_Fare_Management.Application.PricingFunctionality.Commands;
using Route_Fare_Management.Application.PricingFunctionality.DTOs;
using Route_Fare_Management.Application.PricingFunctionality.Queries;

namespace Route_Fare_Management.API.Controllers
{
    public class PricingController : Controller
    {
        private readonly ISender _mediator;

        public PricingController(ISender mediator) => _mediator = mediator;

        /// <summary>
        /// Get the full pricing table for a specific Operator + Route + Season combination.
        /// Operators can only view their own pricing.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PricingTableDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPricing(
            [FromQuery] Guid tourOperatorId,
            [FromQuery] Guid routeId,
            [FromQuery] Guid seasonId,
            CancellationToken ct)
            => Ok(await _mediator.Send(
                new GetPricingQuery(tourOperatorId, routeId, seasonId), ct));

        /// <summary>
        /// Upsert pricing rows. Matches rows by Date — creates new or updates existing.
        /// </summary>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpsertPricing(
            [FromBody] UpsertPricingCommand command, CancellationToken ct)
        {
            await _mediator.Send(command, ct);
            return NoContent();
        }

        /// <summary>
        /// Initialize the pricing table: creates one empty row per season day.
        /// Returns the number of rows created.
        /// </summary>
        [HttpPost("initialize")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Initialize(
            [FromBody] InitializePricingTableCommand command, CancellationToken ct)
        {
            var rowsCreated = await _mediator.Send(command, ct);
            return Ok(new { rowsCreated });
        }
    }
}
