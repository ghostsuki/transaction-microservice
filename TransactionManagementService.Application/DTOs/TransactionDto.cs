using TransactionManagementService.Domain.Entities;

namespace TransactionManagementService.Application.DTOs
{
    public class TransactionDto
    {
        public string Id { get; set; } = string.Empty;
        public string AccountId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public TransactionStatus Status { get; set; }
    }
}