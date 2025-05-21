using Core.Entities;

namespace Core.DTOs
{
    public class BlockDto
    {
        public Guid Id { get; set; }
        public string BlockName { get; set; } = string.Empty;
        public PropertyType PropertyType { get; set; }
        public Guid? ManagerId { get; set; }

        public string? ManagerName { get; set; }
    }

    public class CreateBlockDto
    {
        public string BlockName { get; set; } = string.Empty;
        public PropertyType PropertyType { get; set; }
        public Guid? ManagerId { get; set; }
    }

    public class AssignManagerDto
    {
        public Guid ManagerId { get; set; }
    }
}