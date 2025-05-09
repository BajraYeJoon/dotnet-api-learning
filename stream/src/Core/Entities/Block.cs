namespace Core.Entities
{
    public class Block
    {
        public Guid Id { get; set; }
        public string BlockName { get; set; } = string.Empty;
        public PropertyType PropertyType { get; set; }
        public Guid ManagerId { get; set; }
        public List<Floor> Floors { get; set; } = [];
        public List<House> Houses { get; set; } = [];

    }
}
