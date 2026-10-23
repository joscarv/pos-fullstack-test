namespace Pos.Domain.Entities
{
    public sealed class Inventory
    {
        public Guid ProductId { get; set; }
        public int Stock { get; set; }
    }
}
