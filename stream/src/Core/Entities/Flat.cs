namespace Core.Entities
{
    public class Flat
    {
        public Guid Id { get; set; }
        public string FlatName { get; set; } = string.Empty;
        public Guid FloorId { get; set; }
        public Guid? ResidentId { get; set; }

        public Floor Floor { get; set; } = null!;
    }
}
