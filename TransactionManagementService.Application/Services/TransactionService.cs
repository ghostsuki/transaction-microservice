using TransactionManagementService.Application.DTOs;
using TransactionManagementService.Domain.Entities;
using TransactionManagementService.Domain.Interfaces;

namespace TransactionManagementService.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMessageService _messageService;

        public TransactionService(ITransactionRepository transactionRepository, IMessageService messageService)
        {
            _transactionRepository = transactionRepository;
            _messageService = messageService;
        }

        public async Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync()
        {
            var transactions = await _transactionRepository.GetAllAsync();
            return transactions.Select(MapToDto);
        }

        public async Task<TransactionDto?> GetTransactionByIdAsync(string id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            return transaction != null ? MapToDto(transaction) : null;
        }

        public async Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto createTransactionDto)
        {
            var transaction = new Transaction
            {
                AccountId = createTransactionDto.AccountId,
                Amount = createTransactionDto.Amount,
                Type = createTransactionDto.Type,
                Description = createTransactionDto.Description,
                CreatedAt = DateTime.UtcNow,
                Status = TransactionStatus.Pending
            };

            var createdTransaction = await _transactionRepository.CreateAsync(transaction);

            // Enviar mensagem para Service Bus
            var transactionMessage = new
            {
                TransactionId = createdTransaction.Id,
                AccountId = createdTransaction.AccountId,
                Amount = createdTransaction.Amount,
                Type = createdTransaction.Type.ToString(),
                CreatedAt = createdTransaction.CreatedAt
            };

            await _messageService.SendTransactionMessageAsync(transactionMessage);

            return MapToDto(createdTransaction);
        }

        private static TransactionDto MapToDto(Transaction transaction)
        {
            return new TransactionDto
            {
                Id = transaction.Id,
                AccountId = transaction.AccountId,
                Amount = transaction.Amount,
                Type = transaction.Type,
                Description = transaction.Description,
                CreatedAt = transaction.CreatedAt,
                Status = transaction.Status
            };
        }
    }
}