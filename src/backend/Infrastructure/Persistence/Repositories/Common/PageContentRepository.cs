using Domain.Entities.Common;
using Domain.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using MyLeague.Infrastructure.Persistence.Contexts;
using MyLeague.Infrastructure.Persistence.Repositories;

namespace MyLeague.Infrastructure.Persistence.Repositories.Common;

public class PageContentRepository : RepositoryBase<PageContent, CommonDbContext>, IPageContentRepository
{
    public PageContentRepository(CommonDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<PageContent?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _entities.FirstOrDefaultAsync(x => x.PageSlug == slug, cancellationToken);
    }

    public async Task AddAsync(PageContent pageContent, CancellationToken cancellationToken = default)
    {
        await _entities.AddAsync(pageContent, cancellationToken);
    }

    public async Task UpdateAsync(PageContent pageContent, CancellationToken cancellationToken = default)
    {
        _dbContext.Entry(pageContent).State = EntityState.Modified;
        await Task.CompletedTask;
    }
}

