using CatalogService.Core.Domain.Models;
using CatalogService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace CatalogService.Api.Infrastructure.EntityConfiguration;

public class CatalogItemEntityTypeConfiguration : IEntityTypeConfiguration<CatalogItem>
{
    public void Configure(EntityTypeBuilder<CatalogItem> builder)
    {
        builder.ToTable(nameof(CatalogItem), CatalogContext.DEFAULT_SCHEMA);

        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.Id)
            .UseHiLo("catalog_item_hilo")
            .IsRequired();

        builder.Property(ci => ci.Name)
           .IsRequired()
           .HasMaxLength(50);

        builder.Property(ci => ci.Price)
            .IsRequired().HasPrecision(18, 4);

        builder.Property(ci => ci.Description)
            .IsRequired(false)
            .HasMaxLength(1000);

        builder.Ignore(ci => ci.PictureUrl);

        builder.HasOne(ci => ci.CatalogBrand)
            .WithMany()
            .HasForeignKey(ci => ci.CatalogBrandId);

        builder.HasOne(ci => ci.CatalogType)
            .WithMany()
            .HasForeignKey(ci => ci.CatalogTypeId);
    }
}
