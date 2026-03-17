using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyLeague.Infrastructure.Persistence.Configurations.Common
{
    public class PageContentConfiguration : IEntityTypeConfiguration<PageContent>
    {
        public void Configure(EntityTypeBuilder<PageContent> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .IsRequired()
                .ValueGeneratedNever();

            builder.Property(x => x.PageSlug)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(x => x.PageSlug)
                .IsUnique();

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            // Store as unbounded text (PostgreSQL: "text")
            builder.Property(x => x.ContentHtml)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(x => x.LastModifiedBy)
                .HasMaxLength(200);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt);
        }
    }
}

