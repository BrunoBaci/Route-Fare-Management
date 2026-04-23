using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Route_Fare_Management.Application.TourOperator.Commands;
using Route_Fare_Management.Application.TourOperator.DTOs;
using Route_Fare_Management.Application.TourOperator.Queries;

namespace Route_Fare_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class TourOperatorsController : ControllerBase
    {
        private readonly ISender _mediator;

        public TourOperatorsController(ISender mediator) => _mediator = mediator;

        /// <summary>List tour operators 
        /// Admin only
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(List<TourOperatorDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] bool? activeOnly = true,
            CancellationToken ct = default)
            => Ok(await _mediator.Send(
                new GetTourOperatorsQuery(activeOnly), ct));

        /// <summary>Get a tour operator by ID
        /// Admin only
        /// </summary>
        [HttpGet("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(TourOperatorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
            => Ok(await _mediator.Send(new GetTourOperatorByIdQuery(id), ct));

        /// <summary>Create a tour operator
        /// Admin only
        /// </summary>
        //[HttpPost]
        //[Authorize(Policy = "AdminOnly")]
        //[ProducesResponseType(typeof(TourOperatorDto), StatusCodes.Status201Created)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> Create(
        //    [FromBody] CreateTourOperatorCommand command, CancellationToken ct)
        //{
        //    var result = await _mediator.Send(command, ct);
        //    return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        //}

        /// <summary>Update a tour operator's name and booking classes
        /// Admin only
        /// </summary>
        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(TourOperatorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            Guid id, [FromBody] UpdateTourOperatorCommand command, CancellationToken ct)
            => Ok(await _mediator.Send(command with { Id = id }, ct));

        /// <summary>List routes assigned to an operator
        /// optional seasonfilter 
        /// </summary>
        [HttpGet("{operatorId:guid}/routes")]
        [Authorize(Policy = "OperatorOrAdmin")]
        [ProducesResponseType(typeof(List<TourOperatorRouteDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRoutes(
            Guid operatorId,
            [FromQuery] Guid? seasonId = null,
            CancellationToken ct = default)
            => Ok(await _mediator.Send(
                new GetTourOperatorRoutesQuery(operatorId, seasonId), ct));

        /// <summary>
        /// Assign a route to a season for this operator
        /// </summary>
        [HttpPost("{operatorId:guid}/routes")]
        [Authorize(Policy = "OperatorOrAdmin")]
        [ProducesResponseType(typeof(TourOperatorRouteDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignRoute(
            Guid operatorId,
            [FromBody] AssignRouteToSeasonCommand command,
            CancellationToken ct)
        {
            var result = await _mediator.Send(
                command with { TourOperatorId = operatorId }, ct);
            return CreatedAtAction(nameof(GetRoutes), new { operatorId }, result);
        }
    }
}
