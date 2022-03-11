namespace src.Domain.Abstract
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public string TenantId { get; set; }
    }
}