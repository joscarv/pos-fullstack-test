namespace Pos.Domain.Entities
{
    public sealed class Product
    {
        public Guid Id { get; private set; }
        public long Ean { get; private set; }
        public string Sku { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public decimal IvaRate { get; private set; }
        public decimal IepsRate { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; }


        private Product() { }

        public Product(long ean, string sku, string name, string? description, decimal price, decimal ivaRate, decimal iepsRate)
        {
            SetCommercialInformation(ean, sku, name, description, price, ivaRate, iepsRate);
            Id = Guid.NewGuid();
            IsActive = true;
            CreatedAtUtc = DateTime.UtcNow;
            UpdatedAtUtc = null;
        }

        public void Update(long ean, string sku, string name, string? description, decimal price, decimal ivaRate, decimal iepsRate)
        {
            SetCommercialInformation(ean, sku, name, description, price, ivaRate, iepsRate);
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        private void SetCommercialInformation(long ean, string sku, string name, string? description, decimal price, decimal ivaRate, decimal iepsRate)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ean, nameof(ean));
            ArgumentException.ThrowIfNullOrEmpty(sku, nameof(sku));
            ArgumentException.ThrowIfNullOrEmpty(name, nameof(name));
            if (description?.Trim().Length > 200)
            {
                throw new ArgumentException("Description cannot exceed 200 characters.", nameof(description));
            }
            ArgumentOutOfRangeException.ThrowIfNegative(price, nameof(price));
            ArgumentOutOfRangeException.ThrowIfNegative(ivaRate, nameof(ivaRate));
            ArgumentOutOfRangeException.ThrowIfNegative(iepsRate, nameof(iepsRate));

            Ean = ean;
            Sku = sku;
            Name = name;
            Description = description;
            Price = price;
            IvaRate = ivaRate;
            IepsRate = iepsRate;
        }
    }
}