using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Data.Configuration
{
    internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products", table =>
            {
                table.HasCheckConstraint("CK_Product_Price", "[Price] >= 0");
                table.HasCheckConstraint("CK_Product_IvaRate", "[IvaRate] >= 0.00 AND [IvaRate] <= 1.00");
                table.HasCheckConstraint("CK_Product_IepsRate", "[IepsRate] >= 0.00 AND [IepsRate] <= 1.00");
            });

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Ean).HasMaxLength(13).IsRequired();
            builder.Property(p => p.Sku).HasMaxLength(20).IsRequired();
            builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
            builder.Property(p => p.Description).HasMaxLength(250);
            builder.Property(p => p.Price).HasPrecision(18, 2).IsRequired();
            builder.Property(p => p.IvaRate).HasPrecision(3, 2).IsRequired();
            builder.Property(p => p.IepsRate).HasPrecision(3, 2).IsRequired();
            builder.Property(p => p.IsActive).IsRequired();
            builder.Property(p => p.CreatedAtUtc).HasColumnType("datetime2").IsRequired();
            builder.Property(p => p.UpdatedAtUtc).HasColumnType("datetime2");

            builder.HasIndex(p => p.Ean).IsUnique();
            builder.HasIndex(p => p.Sku).IsUnique();
            builder.HasIndex(p => p.CreatedAtUtc);
        }
    }
}
