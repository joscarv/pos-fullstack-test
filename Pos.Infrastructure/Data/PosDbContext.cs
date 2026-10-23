using Microsoft.EntityFrameworkCore;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Data
{
    public sealed class PosDbContext : DbContext
    {
        public PosDbContext(DbContextOptions<PosDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Inventory> Inventories => Set<Inventory>();
        public DbSet<Sale> Sales => Set<Sale>();
        public DbSet<SaleItem> SaleItems => Set<SaleItem>();
        public DbSet<Payment> Payments => Set<Payment>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(PosDbContext).Assembly);

            // Inventory
            builder.Entity<Inventory>(e =>
            {
                e.HasKey(x => x.ProductId);
            });

            // Sale
            builder.Entity<Sale>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Folio).HasMaxLength(40).IsRequired();
                e.HasIndex(x => x.Folio).IsUnique();
                e.Property(x => x.Subtotal).HasColumnType("decimal(18,2)");
                e.Property(x => x.TaxIva).HasColumnType("decimal(18,2)");
                e.Property(x => x.TaxIeps).HasColumnType("decimal(18,2)");
                e.Property(x => x.Total).HasColumnType("decimal(18,2)");
            });

            // SaleItem
            builder.Entity<SaleItem>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
                e.Property(x => x.LineSubtotal).HasColumnType("decimal(18,2)");
                e.Property(x => x.LineIeps).HasColumnType("decimal(18,2)");
                e.Property(x => x.SkuSnapshot).HasMaxLength(50).IsRequired();
                e.Property(x => x.NameSnapshot).HasMaxLength(150).IsRequired();
            });

            // Payment
            builder.Entity<Payment>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Amount).HasColumnType("decimal(18,2)");
                e.Property(x => x.Reference).HasMaxLength(100);
                e.Property(x => x.AuthCode).HasMaxLength(30);
                e.Property(x => x.Message).HasMaxLength(200).IsRequired();
            });

            // Seed simple
            // Seed Productos
            //var products = new List<Product>
            //{
            //    new (1111111111111, "111111", "Coca Cola 600ml", "Refresco Coca Cola de 600ml en botella de plastico", 18.00m, 0.16m, 0.00m),
            //    new (1111111111112, "111112", "Sabritas 45g", "Papas sabritas de sal de 45g", 22.00m, 0.16m, 0.26m),
            //    new (1111111111113, "111113", "Agua Natural 1L", "Agua embotellada bonafon de 1L", 15.00m, 0.00m, 0.00m),
            //    new (1111111111114, "111114", "Jugo Jumex Mango 450ml", "Jugo de mango de 450ml", 17.00m, 0.16m, 0.26m),
            //    new (1111111111115, "111115", "Galletas Oreo 117g", "Galletas Oreo de 117g", 26.00m, 0.16m, 0.26m),
            //    new (1111111111116, "111116", "Pan Bimbo Blanco", "Pan Bimbo Blanco de 500g", 38.00m, 0.16m, 0.00m),
            //    new (1111111111117, "111117", "Leche Lala Entera 1L", "Leche Lala Entera de 1L", 25.00m, 0.00m, 0.00m),
            //    new (1111111111118, "111118", "Red Bull 250ml", "Red Bull de 250ml", 42.00m, 0.16m, 0.26m),
            //    new (1111111111119, "111119", "Chocolate Carlos V", "Chocolate Carlos V", 14.00m, 0.16m, 0.26m),
            //    new (111111111120, "111120", "Chicles Trident", "Chicles Trident", 12.00m, 0.16m, 0.26m),
            //    new (111111111121, "111121", "Cerveza Corona 355ml", "Cerveza Corona en lata de 255ml", 28.00m, 0.16m, 0.26m),
            //    new (111111111122, "111122", "Papas Pringles 71g", "", 39.00m, 0.16m, 0.00m),
            //    new (111111111123, "111123", "Atún Dolores 140g", "", 24.00m, 0.16m, 0.00m),
            //    new (111111111124, "111124", "Arroz SOS 1kg", "Arroz SOS de 1kg", 32.00m, 0.16m, 0.00m),
            //    new (111111111125, "111125", "Frijoles Isadora 430g", "Frijoles Isadora de 430g", 19.00m, 0.16m, 0.00m),
            //    new (111111111126, "111126", "Aceite Nutrioli 850ml", "Aceite Nutrioli de 850ml", 58.00m, 0.16m, 0.00m),
            //    new (111111111127, "111127", "Café Nescafé Clásico", "Café Nescafé Clásico de 25g", 85.00m, 0.16m, 0.00m),
            //    new (111111111128, "111128", "Yogurt Yoplait Fresa", "Yogurt Yoplait Fresa de 25g", 13.00m, 0.16m, 0.00m),
            //    new (111111111129, "111129", "Gansito Marinela", "Gansito Marinela de 45g", 15.00m, 0.16m, 0.26m),
            //    new (111111111130, "111130", "Powerade 500ml", "Powerade 500ml", 21.00m, 0.16m, 0.26m)
            //};
            //builder.Entity<Product>().HasData([.. products]);

            // Seed Inventario
            //var inventories = products.Select(p => new Inventory { ProductId = p.Id, Stock = new Random().Next(0, 75) }).ToList();
            //builder.Entity<Inventory>().HasData([.. inventories]);
        }
    }
}
