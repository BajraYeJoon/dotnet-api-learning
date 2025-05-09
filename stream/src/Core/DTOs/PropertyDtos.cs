namespace Core.DTOs
{
    public class FloorDto
    {
        public Guid Id { get; set; }
        public int FloorNumber { get; set; }
        public Guid BlockId { get; set; }
        public List<FlatDto> Flats { get; set; } = [];
    }

    public class FlatDto
    {
        public Guid Id { get; set; }
        public string FlatName { get; set; } = string.Empty;
        public Guid FloorId { get; set; }
    }

    public class HouseDto
    {
        public Guid Id { get; set; }
        public string HouseName { get; set; } = string.Empty;
        public Guid BlockId { get; set; }
    }
}