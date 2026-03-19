using Application.Common;
using Application.DTOs.Common;
using Application.Features.Common.PageContent.Commands;
using Domain.Repositories.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Security.Principal;

namespace Application.Features.Common.PageContent.Handlers;

public class UpdatePageContentHandler : IRequestHandler<UpdatePageContentCommand, Result<PageContentDto>>
{
    private readonly IPageContentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdatePageContentHandler> _logger;
    private readonly ClaimsPrincipal? _user;

    public UpdatePageContentHandler(
        IPageContentRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<UpdatePageContentHandler> logger,
        ClaimsPrincipal? user = null)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _user = user;
    }

    public async Task<Result<PageContentDto>> Handle(UpdatePageContentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Slug))
            {
                return Result<PageContentDto>.Failure("Slug is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return Result<PageContentDto>.Failure("Title is required.");
            }

            if (string.IsNullOrWhiteSpace(request.ContentHtml))
            {
                return Result<PageContentDto>.Failure("ContentHtml is required.");
            }

            _logger.LogInformation("Upserting page content for slug: {Slug}", request.Slug);

            Domain.Entities.Common.PageContent? existing = await _repository.GetBySlugAsync(request.Slug, cancellationToken);

            string? modifier = _user?.FindFirst(ClaimTypes.Email)?.Value;

            if (existing is null)
            {
                Domain.Entities.Common.PageContent entity = new()
                {
                    PageSlug = request.Slug,
                    Title = request.Title,
                    ContentHtml = request.ContentHtml,
                    LastModifiedBy = modifier
                };

                await _repository.AddAsync(entity, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

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

            existing.Title = request.Title;
            existing.ContentHtml = request.ContentHtml;
            existing.LastModifiedBy = modifier;

            await _repository.UpdateAsync(existing, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            PageContentDto existingDto = new()
            {
                Id = existing.Id,
                PageSlug = existing.PageSlug,
                Title = existing.Title,
                ContentHtml = existing.ContentHtml,
                LastModifiedBy = existing.LastModifiedBy,
                UpdatedAt = existing.UpdatedAt ?? existing.CreatedAt
            };

            return Result<PageContentDto>.Success(existingDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while upserting page content for slug: {Slug}", request.Slug);
            return Result<PageContentDto>.Failure("An error occurred while updating the page content.");
        }
    }
}

