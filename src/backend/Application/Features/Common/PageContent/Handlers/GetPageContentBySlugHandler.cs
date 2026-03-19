using Application.Common;
using Application.DTOs.Common;
using Application.Features.Common.PageContent.Queries;
using Domain.Repositories.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Common.PageContent.Handlers;

public class GetPageContentBySlugHandler : IRequestHandler<GetPageContentBySlugQuery, Result<PageContentDto>>
{
    private readonly IPageContentRepository _repository;
    private readonly ILogger<GetPageContentBySlugHandler> _logger;

    public GetPageContentBySlugHandler(IPageContentRepository repository, ILogger<GetPageContentBySlugHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<PageContentDto>> Handle(GetPageContentBySlugQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Slug))
            {
                return Result<PageContentDto>.Failure("Slug is required.");
            }

            _logger.LogInformation("Retrieving page content for slug: {Slug}", request.Slug);

            Domain.Entities.Common.PageContent? entity = await _repository.GetBySlugAsync(request.Slug, cancellationToken);
            if (entity == null)
            {
                _logger.LogInformation("Page content not found for slug: {Slug}", request.Slug);
                return Result<PageContentDto>.NotFound(nameof(PageContent), request.Slug);
            }

            PageContentDto dto = new()
            {
                Id = entity.Id,
                PageSlug = entity.PageSlug,
                Title = entity.Title,
                ContentHtml = entity.ContentHtml,
                LastModifiedBy = entity.LastModifiedBy,
                UpdatedAt = entity.UpdatedAt ?? entity.CreatedAt
            };

            return Result<PageContentDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving page content for slug: {Slug}", request.Slug);
            return Result<PageContentDto>.Failure("An error occurred while retrieving the page content.");
        }
    }
}

