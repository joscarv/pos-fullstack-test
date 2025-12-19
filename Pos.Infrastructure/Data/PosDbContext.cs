using Microsoft.EntityFrameworkCore;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Data
{
    public sealed class PosDbContext : DbContext
    {
        public PosDbContext(DbContextOptions<PosDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder b)
        {
            // Product
            b.Entity<Product>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Sku).HasMaxLength(13).IsRequired();
                e.Property(x => x.Name).HasMaxLength(150).IsRequired();
                e.Property(x => x.Price).HasColumnType("decimal(18,2)");
                e.Property(x => x.IepsRate).HasColumnType("decimal(5,4)");
                e.HasIndex(x => x.Sku).IsUnique();
                e.HasOne(x => x.Inventory)
                 .WithOne(x => x.Product)
                 .HasForeignKey<Inventory>(x => x.ProductId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // Inventory
            b.Entity<Inventory>(e =>
            {
                e.HasKey(x => x.ProductId);
            });

            // Sale
            b.Entity<Sale>(e =>
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
            b.Entity<SaleItem>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
                e.Property(x => x.LineSubtotal).HasColumnType("decimal(18,2)");
                e.Property(x => x.LineIeps).HasColumnType("decimal(18,2)");
                e.Property(x => x.SkuSnapshot).HasMaxLength(50).IsRequired();
                e.Property(x => x.NameSnapshot).HasMaxLength(150).IsRequired();
            });

            // Payment
            b.Entity<Payment>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Amount).HasColumnType("decimal(18,2)");
                e.Property(x => x.Reference).HasMaxLength(100);
                e.Property(x => x.AuthCode).HasMaxLength(30);
                e.Property(x => x.Message).HasMaxLength(200).IsRequired();
            });

            // Seed simple
            // Seed Productos
            var p1 = new Product { Id = 1, Sku = "112233", Name = "Coca Cola 600ml", Price = 18.00m, IepsRate = 0.26m };
            var p2 = new Product { Id = 2, Sku = "223344", Name = "Sabritas 45g", Price = 22.00m, IepsRate = 0.26m };
            var p3 = new Product { Id = 3, Sku = "334455", Name = "Agua Natural 1L", Price = 15.00m, IepsRate = 0.00m };
            var p4 = new Product { Id = 4, Sku = "445566", Name = "Jugo Jumex Mango 450ml", Price = 17.00m, IepsRate = 0.26m };
            var p5 = new Product { Id = 5, Sku = "556677", Name = "Galletas Oreo 117g", Price = 26.00m, IepsRate = 0.26m };
            var p6 = new Product { Id = 6, Sku = "667788", Name = "Pan Bimbo Blanco", Price = 38.00m, IepsRate = 0.00m };
            var p7 = new Product { Id = 7, Sku = "778899", Name = "Leche Lala Entera 1L", Price = 25.00m, IepsRate = 0.00m };
            var p8 = new Product { Id = 8, Sku = "889900", Name = "Red Bull 250ml", Price = 42.00m, IepsRate = 0.26m };
            var p9 = new Product { Id = 9, Sku = "990011", Name = "Chocolate Carlos V", Price = 14.00m, IepsRate = 0.26m };
            var p10 = new Product { Id = 10, Sku = "101112", Name = "Chicles Trident", Price = 12.00m, IepsRate = 0.26m };

            var p11 = new Product { Id = 11, Sku = "121314", Name = "Cerveza Corona 355ml", Price = 28.00m, IepsRate = 0.26m };
            var p12 = new Product { Id = 12, Sku = "141516", Name = "Papas Pringles 71g", Price = 39.00m, IepsRate = 0.26m };
            var p13 = new Product { Id = 13, Sku = "161718", Name = "Atún Dolores 140g", Price = 24.00m, IepsRate = 0.00m };
            var p14 = new Product { Id = 14, Sku = "181920", Name = "Arroz SOS 1kg", Price = 32.00m, IepsRate = 0.00m };
            var p15 = new Product { Id = 15, Sku = "212223", Name = "Frijoles Isadora 430g", Price = 19.00m, IepsRate = 0.00m };
            var p16 = new Product { Id = 16, Sku = "242526", Name = "Aceite Nutrioli 850ml", Price = 58.00m, IepsRate = 0.00m };
            var p17 = new Product { Id = 17, Sku = "272829", Name = "Café Nescafé Clásico", Price = 85.00m, IepsRate = 0.00m };
            var p18 = new Product { Id = 18, Sku = "303132", Name = "Yogurt Yoplait Fresa", Price = 13.00m, IepsRate = 0.00m };
            var p19 = new Product { Id = 19, Sku = "333435", Name = "Gansito Marinela", Price = 15.00m, IepsRate = 0.26m };
            var p20 = new Product { Id = 20, Sku = "363738", Name = "Powerade 500ml", Price = 21.00m, IepsRate = 0.26m };

            b.Entity<Product>().HasData(
                p1, p2, p3, p4, p5,
                p6, p7, p8, p9, p10,
                p11, p12, p13, p14, p15,
                p16, p17, p18, p19, p20
            );

            // Seed Inventario
            b.Entity<Inventory>().HasData(
                new Inventory { ProductId = 1, Stock = 50 },
                new Inventory { ProductId = 2, Stock = 40 },
                new Inventory { ProductId = 3, Stock = 60 },
                new Inventory { ProductId = 4, Stock = 35 },
                new Inventory { ProductId = 5, Stock = 45 },
                new Inventory { ProductId = 6, Stock = 30 },
                new Inventory { ProductId = 7, Stock = 25 },
                new Inventory { ProductId = 8, Stock = 20 },
                new Inventory { ProductId = 9, Stock = 55 },
                new Inventory { ProductId = 10, Stock = 70 },

                new Inventory { ProductId = 11, Stock = 80 },
                new Inventory { ProductId = 12, Stock = 15 },
                new Inventory { ProductId = 13, Stock = 40 },
                new Inventory { ProductId = 14, Stock = 22 },
                new Inventory { ProductId = 15, Stock = 28 },
                new Inventory { ProductId = 16, Stock = 18 },
                new Inventory { ProductId = 17, Stock = 12 },
                new Inventory { ProductId = 18, Stock = 34 },
                new Inventory { ProductId = 19, Stock = 48 },
                new Inventory { ProductId = 20, Stock = 27 }
            );
        }
    }
}
