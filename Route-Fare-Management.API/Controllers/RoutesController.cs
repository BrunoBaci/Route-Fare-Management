using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Route_Fare_Management.Application.RouteFunctionality.Commands;
using Route_Fare_Management.Application.RouteFunctionality.DTOs;
using Route_Fare_Management.Application.RouteFunctionality.Queries;

namespace Route_Fare_Management.API.Controllers
{
    public class RoutesController : Controller
    {
        private readonly ISender _mediator;

        public RoutesController(ISender mediator) => _mediator = mediator;

        /// <summary>Get a paginated, searchable list of active routes.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<RouteDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            CancellationToken ct = default)
            => Ok(await _mediator.Send(new GetRoutesQuery(search), ct));

        /// <summary>Get a single route by ID.</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(RouteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
            => Ok(await _mediator.Send(new GetRouteByIdQuery(id), ct));

        /// <summary>Create a new route. Admin only.</summary>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(RouteDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create(
            [FromBody] CreateRouteCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>Update an existing route. Admin only.</summary>
        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(RouteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            Guid id, [FromBody] UpdateRouteCommand command, CancellationToken ct)
            => Ok(await _mediator.Send(command with { Id = id }, ct));
    }
}

