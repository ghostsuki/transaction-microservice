// TransactionManagementService.Domain/Interfaces/ITransactionRepository.cs
using TransactionManagementService.Domain.Entities;

namespace TransactionManagementService.Domain.Interfaces
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetAllAsync();
        Task<Transaction?> GetByIdAsync(string id);
        Task<Transaction> CreateAsync(Transaction transaction);
        Task<Transaction?> UpdateAsync(string id, Transaction transaction);
        Task<bool> DeleteAsync(string id);
    }
}

