// TransactionManagementService.Infrastructure/Configuration/DatabaseSettings.cs
namespace TransactionManagementService.Infrastructure.Configuration
{
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string TransactionsCollectionName { get; set; } = string.Empty;
    }
}