using Core.Entities;

namespace Core.DTOs
{
    public class BlockDto
    {
        public Guid Id { get; set; }
        public string BlockName { get; set; } = string.Empty;

        public PropertyType PropertyType { get; set; }
    }
}