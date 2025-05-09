using Core.Entities;

namespace Core.DTOs
{
    public class ResidencyDto
    {
        public string BlockName { get; set; } = string.Empty;
        public int? FloorNumber { get; set; }
        public string UnitNumber { get; set; } = string.Empty;
        public PropertyType PropertyType { get; set; }
    }
}