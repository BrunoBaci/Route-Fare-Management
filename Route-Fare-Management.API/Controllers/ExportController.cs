using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Route_Fare_Management.Application.Export;

namespace Route_Fare_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ExportController : ControllerBase
    {
        private readonly ISender _mediator;
        private readonly ILogger<ExportController> _logger;
        private readonly IConfiguration _configuration;
        private const string ContentType =
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public ExportController(ISender mediator, ILogger<ExportController> logger, IConfiguration configuration)
        {
            _mediator = mediator;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("pricing")]
        [Produces(ContentType)]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ExportPricing(
            [FromBody] ExportPricesCommand command,
            CancellationToken ct)
        {
            // Handler runs the export and returns the saved filename
            var fileName = await _mediator.Send(command, ct);
            var filePath = Path.Combine("exports", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                _logger.LogError("Export completed but file not found: {Path}", filePath);
                return NotFound("Export completed but file could not be located.");
            }

            var stream = System.IO.File.OpenRead(filePath);
            return File(stream, ContentType, fileName);
        }

    }
}
