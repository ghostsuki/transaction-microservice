// TransactionManagementService.Infrastructure/Configuration/ServiceBusSettings.cs
namespace TransactionManagementService.Infrastructure.Configuration
{
    public class ServiceBusSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string QueueName { get; set; } = string.Empty;
    }
}