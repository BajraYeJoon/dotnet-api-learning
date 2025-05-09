namespace Core.Entities
{
    public class House
    {
        public Guid Id { get; set; }
        public string HouseName { get; set; } = string.Empty;
        public Guid BlockId { get; set; }
        public Guid? ResidentId { get; set; }

        public Block Block { get; set; } = null!;
    }
}
