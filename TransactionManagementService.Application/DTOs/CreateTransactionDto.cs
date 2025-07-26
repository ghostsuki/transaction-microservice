// TransactionManagementService.Application/DTOs/CreateTransactionDto.cs
using TransactionManagementService.Domain.Entities;

namespace TransactionManagementService.Application.DTOs
{
    public class CreateTransactionDto
    {
        public string AccountId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}