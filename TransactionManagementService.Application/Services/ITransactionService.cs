// TransactionManagementService.Application/Services/ITransactionService.cs
using TransactionManagementService.Application.DTOs;

namespace TransactionManagementService.Application.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync();
        Task<TransactionDto?> GetTransactionByIdAsync(string id);
        Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto createTransactionDto);
    }
}
