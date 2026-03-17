using Application.Common;
using Application.DTOs.Common;
using MediatR;

namespace Application.Features.Common.PageContent.Queries;

public record GetPageContentBySlugQuery(string Slug) : IRequest<Result<PageContentDto>>;

