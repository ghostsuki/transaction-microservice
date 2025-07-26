// TransactionManagementService.Domain/Entities/Transaction.cs
namespace TransactionManagementService.Domain.Entities
{
    public class Transaction
    {
        public string Id { get; set; } = string.Empty;
        public string AccountId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    }

    public enum TransactionType
    {
        Debit,
        Credit
    }

    public enum TransactionStatus
    {
        Pending,
        Completed,
        Failed
    }
}