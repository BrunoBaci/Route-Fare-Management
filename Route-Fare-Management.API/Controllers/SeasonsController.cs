using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Route_Fare_Management.Application.Season;
using Route_Fare_Management.Application.Season.Commands;
using Route_Fare_Management.Application.Season.Queries;

namespace Route_Fare_Management.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class SeasonsController : ControllerBase
    {
        private readonly ISender _mediator;

        public SeasonsController(ISender mediator) => _mediator = mediator;

        /// <summary>List seasons. Optionally filter by year.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<SeasonDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? year = null, CancellationToken ct = default)
            => Ok(await _mediator.Send(new GetSeasonsQuery(year), ct));

        /// <summary>Get a season by ID.</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(SeasonDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
            => Ok(await _mediator.Send(new GetSeasonByIdQuery(id), ct));

        /// <summary>
        /// Create a season (Winter or Summer for a given year)
        /// Admin only.</summary>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(SeasonDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            [FromBody] CreateSeasonCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
    }

}
