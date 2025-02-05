using CatalogService.Core.Domain.Models;
using CatalogService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace CatalogService.Api.Infrastructure.EntityConfiguration;

public class CatalogTypeEntityTypeConfiguration : IEntityTypeConfiguration<CatalogType>
{
    public void Configure(EntityTypeBuilder<CatalogType> builder)
    {
        builder.ToTable(nameof(CatalogType), CatalogContext.DEFAULT_SCHEMA);

        builder.HasKey(ct => ct.Id);

        builder.Property(ct => ct.Id)
            .UseHiLo("catalog_type_hilo")
            .IsRequired();

        builder.Property(ct => ct.Name)
            .IsRequired()
            .HasMaxLength(100);
    }
}
