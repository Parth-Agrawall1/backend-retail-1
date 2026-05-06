namespace RetailAPI.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string Action { get; set; } = string.Empty;

        public int PerformedBy { get; set; }

        public int? PrId { get; set; }
        public string? Details { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
