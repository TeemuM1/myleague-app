using Domain.Entities.Common;

namespace Domain.Repositories.Common;

public interface IPageContentRepository
{
    Task<PageContent?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task AddAsync(PageContent pageContent, CancellationToken cancellationToken = default);
    Task UpdateAsync(PageContent pageContent, CancellationToken cancellationToken = default);
}

