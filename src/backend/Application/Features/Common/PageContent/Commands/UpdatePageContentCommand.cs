using Application.Common;
using Application.DTOs.Common;
using MediatR;

namespace Application.Features.Common.PageContent.Commands;

public record UpdatePageContentCommand(
    string Slug,
    string Title,
    string ContentHtml) : IRequest<Result<PageContentDto>>;

