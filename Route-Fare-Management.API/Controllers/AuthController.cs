using MediatR;
using Microsoft.AspNetCore.Mvc;
using Route_Fare_Management.Application.Auth;

namespace Route_Fare_Management.API.Controllers
{
    public class AuthController : Controller
    {
        private readonly ISender _mediator;
        public AuthController(ISender mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Register a new user account
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(
            [FromBody] RegisterCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>Login with email and password. 
        /// Returns a JWT token
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Login(
        [FromBody] LoginCommand command, CancellationToken ct)
        {
            return Ok(await _mediator.Send(command, ct));
        }
    }
}
