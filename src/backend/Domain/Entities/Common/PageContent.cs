using Domain.Entities;

namespace Domain.Entities.Common
{
    public class PageContent : BaseEntity
    {
        public string PageSlug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ContentHtml { get; set; } = string.Empty;
        public string? LastModifiedBy { get; set; }
    }
}

