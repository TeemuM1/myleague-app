using Application.Common;
using Application.DTOs.Common;
using Application.Features.Common.PageContent.Commands;
using Application.Features.Common.PageContent.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models.Common;

namespace WebAPI.Controllers.Common
{
    /// <summary>
    /// Controller for managing page content (HTML) by page slug.
    /// </summary>
    [ApiController]
    [Route("api/page-content")]
    [Produces("application/json")]
    public class PageContentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PageContentController> _logger;

        /// <summary>
        /// Initializes a new instance of the PageContentController class.
        /// </summary>
        /// <param name="mediator">The mediator for handling commands and queries</param>
        /// <param name="logger">The logger for this controller</param>
        public PageContentController(IMediator mediator, ILogger<PageContentController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get page content by slug.
        /// </summary>
        /// <param name="slug">The page identifier, e.g. "saannot"</param>
        /// <returns>The page content</returns>
        [HttpGet("{slug}")]
        [ProducesResponseType(typeof(ApiResponse<PageContentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PageContentDto>>> GetBySlug(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                return BadRequest(ApiResponse<PageContentDto>.ErrorResponse("Slug is required."));
            }

            _logger.LogInformation("Getting page content by slug: {Slug}", slug);

            Result<PageContentDto> result = await _mediator.Send(new GetPageContentBySlugQuery(slug));

            if (result.IsSuccess && result.Data != null)
            {
                return Ok(ApiResponse<PageContentDto>.SuccessResponse(result.Data, "Page content retrieved successfully"));
            }

            string errorMessage = result.Error ?? result.GetErrorsString();
            if (errorMessage.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(ApiResponse<PageContentDto>.ErrorResponse(errorMessage));
            }

            return StatusCode(500, ApiResponse<PageContentDto>.ErrorResponse(errorMessage));
        }

        /// <summary>
        /// Updates or creates (upsert) page content by slug.
        /// </summary>
        /// <param name="slug">The page identifier, e.g. "saannot"</param>
        /// <param name="request">The updated page title and HTML content</param>
        /// <returns>The updated page content</returns>
        [HttpPut("{slug}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<PageContentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PageContentDto>>> Upsert(string slug, [FromBody] PageContentUpdateDto request)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                return BadRequest(ApiResponse<PageContentDto>.ErrorResponse("Slug is required."));
            }

            _logger.LogInformation("Upserting page content by slug: {Slug}", slug);

            Result<PageContentDto> result = await _mediator.Send(new UpdatePageContentCommand(
                slug,
                request.Title,
                request.ContentHtml));

            if (result.IsSuccess && result.Data != null)
            {
                return Ok(ApiResponse<PageContentDto>.SuccessResponse(result.Data, "Page content updated successfully"));
            }

            string errorMessage = result.Error ?? result.GetErrorsString();
            List<string> errorList = result.ValidationFailures.Select(x => x.ErrorMessage).ToList();
            return BadRequest(ApiResponse<PageContentDto>.ErrorResponse(errorMessage, errorList));
        }
    }
}

