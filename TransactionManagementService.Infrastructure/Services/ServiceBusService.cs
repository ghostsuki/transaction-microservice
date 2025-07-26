// TransactionManagementService.Infrastructure/Services/ServiceBusService.cs
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using System.Text.Json;
using TransactionManagementService.Domain.Interfaces;
using TransactionManagementService.Infrastructure.Configuration;

namespace TransactionManagementService.Infrastructure.Services
{
    public class ServiceBusService : IMessageService, IAsyncDisposable
    {
        private readonly ServiceBusClient? _client;
        private readonly ServiceBusSender? _sender;
        private readonly ServiceBusSettings _settings;
        private readonly bool _isEnabled;

        public ServiceBusService(IOptions<ServiceBusSettings> settings)
        {
            _settings = settings.Value;
            
            try
            {
                // Verificar se a connection string está configurada
                if (string.IsNullOrEmpty(_settings.ConnectionString) || 
                    _settings.ConnectionString.Contains("localhost") ||
                    _settings.ConnectionString.Contains("sua-key-aqui"))
                {
                    _isEnabled = false;
                    Console.WriteLine("Azure Service Bus não configurado - rodando em modo mock");
                    return;
                }

                _client = new ServiceBusClient(_settings.ConnectionString);
                _sender = _client.CreateSender(_settings.QueueName);
                _isEnabled = true;
                Console.WriteLine("Azure Service Bus conectado com sucesso!");
            }
            catch (Exception ex)
            {
                _isEnabled = false;
                Console.WriteLine($"Erro ao conectar com Azure Service Bus: {ex.Message}");
                Console.WriteLine("Rodando em modo mock - mensagens serão logadas apenas");
            }
        }

        public async Task SendTransactionMessageAsync<T>(T message) where T : class
        {
            try
            {
                var messageBody = JsonSerializer.Serialize(message, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (!_isEnabled || _sender == null)
                {
                    // Modo mock - apenas log a mensagem
                    Console.WriteLine("=== MOCK SERVICE BUS MESSAGE ===");
                    Console.WriteLine($"Queue: {_settings.QueueName}");
                    Console.WriteLine($"Message: {messageBody}");
                    Console.WriteLine("=== END MOCK MESSAGE ===");
                    return;
                }

                var serviceBusMessage = new ServiceBusMessage(messageBody)
                {
                    ContentType = "application/json",
                    MessageId = Guid.NewGuid().ToString()
                };

                await _sender.SendMessageAsync(serviceBusMessage);
                Console.WriteLine($"Mensagem enviada para Service Bus: {serviceBusMessage.MessageId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar mensagem para Service Bus: {ex.Message}");
                
                // Fallback para modo mock
                var messageBody = JsonSerializer.Serialize(message, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                Console.WriteLine("=== FALLBACK MOCK MESSAGE ===");
                Console.WriteLine($"Message: {messageBody}");
                Console.WriteLine("=== END FALLBACK MESSAGE ===");
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_sender != null)
                await _sender.DisposeAsync();
            
            if (_client != null)
                await _client.DisposeAsync();
        }
    }
}