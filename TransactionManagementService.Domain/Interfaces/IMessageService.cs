// TransactionManagementService.Domain/Interfaces/IMessageService.cs
namespace TransactionManagementService.Domain.Interfaces
{
    public interface IMessageService
    {
        Task SendTransactionMessageAsync<T>(T message) where T : class;
    }
}