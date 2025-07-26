// TransactionManagementService.Infrastructure/Repositories/TransactionRepository.cs
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using TransactionManagementService.Domain.Entities;
using TransactionManagementService.Domain.Interfaces;
using TransactionManagementService.Infrastructure.Configuration;

namespace TransactionManagementService.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IMongoCollection<TransactionDocument>? _transactions;
        private readonly bool _isConnected;
        private readonly List<TransactionDocument> _mockData = new();

        public TransactionRepository(IOptions<DatabaseSettings> databaseSettings)
        {
            try
            {
                var connectionString = databaseSettings.Value.ConnectionString;
                Console.WriteLine($"🔗 Tentando conectar no MongoDB: {connectionString}");

                var mongoClient = new MongoClient(connectionString);
                var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
                _transactions = mongoDatabase.GetCollection<TransactionDocument>(databaseSettings.Value.TransactionsCollectionName);

                // Testar conexão
                var ping = mongoDatabase.RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                _isConnected = true;
                
                Console.WriteLine("✅ MongoDB LOCAL conectado com sucesso!");
                Console.WriteLine($"📁 Database: {databaseSettings.Value.DatabaseName}");
                Console.WriteLine($"📄 Collection: {databaseSettings.Value.TransactionsCollectionName}");
                
                // Inicializar dados de exemplo se collection estiver vazia
                InitializeDataIfEmpty().Wait();
            }
            catch (Exception ex)
            {
                _isConnected = false;
                Console.WriteLine($"❌ Erro ao conectar MongoDB Local: {ex.Message}");
                Console.WriteLine("🔄 Usando dados mock como fallback");
                InitializeMockData();
            }
        }

        private async Task InitializeDataIfEmpty()
        {
            try
            {
                var count = await _transactions!.CountDocumentsAsync(new BsonDocument());
                if (count == 0)
                {
                    Console.WriteLine("📊 Collection vazia - inserindo dados de exemplo...");
                    
                    var sampleData = new List<TransactionDocument>
                    {
                        new() {
                            Id = ObjectId.GenerateNewId().ToString(),
                            AccountId = "ACC001",
                            Amount = 1500.00m,
                            Type = TransactionType.Credit,
                            Description = "Salário mensal",
                            CreatedAt = DateTime.UtcNow.AddDays(-3),
                            Status = TransactionStatus.Completed
                        },
                        new() {
                            Id = ObjectId.GenerateNewId().ToString(),
                            AccountId = "ACC001",
                            Amount = -250.75m,
                            Type = TransactionType.Debit,
                            Description = "Compra supermercado",
                            CreatedAt = DateTime.UtcNow.AddDays(-2),
                            Status = TransactionStatus.Completed
                        },
                        new() {
                            Id = ObjectId.GenerateNewId().ToString(),
                            AccountId = "ACC002",
                            Amount = 500.00m,
                            Type = TransactionType.Credit,
                            Description = "Transferência recebida",
                            CreatedAt = DateTime.UtcNow.AddDays(-1),
                            Status = TransactionStatus.Completed
                        }
                    };

                    await _transactions.InsertManyAsync(sampleData);
                    Console.WriteLine($"✅ Inseridos {sampleData.Count} registros de exemplo no MongoDB");
                }
                else
                {
                    Console.WriteLine($"📊 Collection já contém {count} documento(s)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Erro ao verificar/inicializar dados: {ex.Message}");
            }
        }

        private void InitializeMockData()
        {
            _mockData.Clear();
            _mockData.AddRange(new List<TransactionDocument>
            {
                new() {
                    Id = ObjectId.GenerateNewId().ToString(),
                    AccountId = "ACC001",
                    Amount = 250.75m,
                    Type = TransactionType.Credit,
                    Description = "Depósito inicial (MOCK FALLBACK)",
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    Status = TransactionStatus.Completed
                },
                new() {
                    Id = ObjectId.GenerateNewId().ToString(),
                    AccountId = "ACC002", 
                    Amount = -89.50m,
                    Type = TransactionType.Debit,
                    Description = "Compra online (MOCK FALLBACK)",
                    CreatedAt = DateTime.UtcNow.AddHours(-5),
                    Status = TransactionStatus.Completed
                }
            });
            Console.WriteLine($"💾 Fallback: {_mockData.Count} registros mock carregados");
        }

        public async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            if (!_isConnected)
            {
                Console.WriteLine("📊 [MOCK] Retornando todas as transações");
                return _mockData.Select(MapToEntity);
            }

            try
            {
                Console.WriteLine("📊 [MONGODB] Buscando todas as transações");
                var documents = await _transactions!.Find(_ => true).ToListAsync();
                Console.WriteLine($"📊 [MONGODB] Encontradas {documents.Count} transações");
                return documents.Select(MapToEntity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [MONGODB] Erro ao buscar: {ex.Message}");
                return _mockData.Select(MapToEntity);
            }
        }

        public async Task<Transaction?> GetByIdAsync(string id)
        {
            if (!_isConnected)
            {
                Console.WriteLine($"🔍 [MOCK] Buscando transação: {id}");
                var mockDocument = _mockData.FirstOrDefault(x => x.Id == id);
                return mockDocument != null ? MapToEntity(mockDocument) : null;
            }

            try
            {
                Console.WriteLine($"🔍 [MONGODB] Buscando transação: {id}");
                var document = await _transactions!.Find(x => x.Id == id).FirstOrDefaultAsync();
                return document != null ? MapToEntity(document) : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [MONGODB] Erro ao buscar por ID: {ex.Message}");
                var mockDocument = _mockData.FirstOrDefault(x => x.Id == id);
                return mockDocument != null ? MapToEntity(mockDocument) : null;
            }
        }

        public async Task<Transaction> CreateAsync(Transaction transaction)
        {
            var document = MapToDocument(transaction);
            document.Id = ObjectId.GenerateNewId().ToString();
            document.CreatedAt = DateTime.UtcNow;
            document.Status = TransactionStatus.Pending;

            if (!_isConnected)
            {
                Console.WriteLine("✅ [MOCK] Criando nova transação");
                _mockData.Add(document);
                Console.WriteLine($"💾 [MOCK] Transação criada: {document.Id}");
                return MapToEntity(document);
            }

            try
            {
                Console.WriteLine("✅ [MONGODB] Criando nova transação");
                await _transactions!.InsertOneAsync(document);
                Console.WriteLine($"💾 [MONGODB] Transação salva: {document.Id}");
                return MapToEntity(document);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [MONGODB] Erro ao criar, usando mock: {ex.Message}");
                _mockData.Add(document);
                return MapToEntity(document);
            }
        }

        public async Task<Transaction?> UpdateAsync(string id, Transaction transactionIn)
        {
            var document = MapToDocument(transactionIn);
            document.Id = id;

            if (!_isConnected)
            {
                var existingIndex = _mockData.FindIndex(x => x.Id == id);
                if (existingIndex >= 0)
                {
                    _mockData[existingIndex] = document;
                    Console.WriteLine($"📝 [MOCK] Transação atualizada: {id}");
                    return MapToEntity(document);
                }
                return null;
            }

            try
            {
                Console.WriteLine($"📝 [MONGODB] Atualizando transação: {id}");
                await _transactions!.ReplaceOneAsync(x => x.Id == id, document);
                return MapToEntity(document);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [MONGODB] Erro ao atualizar: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            if (!_isConnected)
            {
                var removed = _mockData.RemoveAll(x => x.Id == id);
                if (removed > 0)
                {
                    Console.WriteLine($"🗑️ [MOCK] Transação removida: {id}");
                }
                return removed > 0;
            }

            try
            {
                Console.WriteLine($"🗑️ [MONGODB] Removendo transação: {id}");
                var result = await _transactions!.DeleteOneAsync(x => x.Id == id);
                return result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [MONGODB] Erro ao deletar: {ex.Message}");
                return false;
            }
        }

        private static Transaction MapToEntity(TransactionDocument document)
        {
            return new Transaction
            {
                Id = document.Id,
                AccountId = document.AccountId,
                Amount = document.Amount,
                Type = document.Type,
                Description = document.Description,
                CreatedAt = document.CreatedAt,
                Status = document.Status
            };
        }

        private static TransactionDocument MapToDocument(Transaction entity)
        {
            return new TransactionDocument
            {
                Id = entity.Id,
                AccountId = entity.AccountId,
                Amount = entity.Amount,
                Type = entity.Type,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                Status = entity.Status
            };
        }
    }

    internal class TransactionDocument
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