namespace Core.Entities
{
    public class Floor
    {
        public Guid Id { get; set; }
        public int FloorNumber { get; set; }
        public Guid BlockId { get; set; }

        public Block Block { get; set; } = null!;
        public List<Flat> Flats { get; set; } = [];

    }
}
