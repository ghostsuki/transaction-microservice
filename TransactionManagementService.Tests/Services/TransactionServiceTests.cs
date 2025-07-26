// TransactionManagementService.Tests/Services/TransactionServiceTests.cs
using Xunit;
using FluentAssertions;
using Moq;
using TransactionManagementService.Application.DTOs;
using TransactionManagementService.Application.Services;
using TransactionManagementService.Domain.Entities;
using TransactionManagementService.Domain.Interfaces;

namespace TransactionManagementService.Tests.Services
{
    public class TransactionServiceTests
    {
        private readonly Mock<ITransactionRepository> _mockRepository;
        private readonly Mock<IMessageService> _mockMessageService;
        private readonly TransactionService _service;

        public TransactionServiceTests()
        {
            _mockRepository = new Mock<ITransactionRepository>();
            _mockMessageService = new Mock<IMessageService>();
            _service = new TransactionService(_mockRepository.Object, _mockMessageService.Object);
        }

        [Fact]
        public async Task GetAllTransactionsAsync_ShouldReturnAllTransactions()
        {
            // Arrange
            var transactions = new List<Transaction>
            {
                new() {
                    Id = "1",
                    AccountId = "ACC001",
                    Amount = 100.50m,
                    Type = TransactionType.Credit,
                    Description = "Test transaction",
                    Status = TransactionStatus.Completed
                },
                new() {
                    Id = "2",
                    AccountId = "ACC002",
                    Amount = -50.25m,
                    Type = TransactionType.Debit,
                    Description = "Another test transaction",
                    Status = TransactionStatus.Pending
                }
            };

            _mockRepository.Setup(r => r.GetAllAsync())
                          .ReturnsAsync(transactions);

            // Act
            var result = await _service.GetAllTransactionsAsync();

            // Assert
            result.Should().HaveCount(2);
            result.First().Id.Should().Be("1");
            result.First().Amount.Should().Be(100.50m);
        }

        [Fact]
        public async Task GetTransactionByIdAsync_WithValidId_ShouldReturnTransaction()
        {
            // Arrange
            var transaction = new Transaction
            {
                Id = "1",
                AccountId = "ACC001",
                Amount = 100.50m,
                Type = TransactionType.Credit,
                Description = "Test transaction",
                Status = TransactionStatus.Completed
            };

            _mockRepository.Setup(r => r.GetByIdAsync("1"))
                          .ReturnsAsync(transaction);

            // Act
            var result = await _service.GetTransactionByIdAsync("1");

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be("1");
            result.Amount.Should().Be(100.50m);
        }

        [Fact]
        public async Task GetTransactionByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync("invalid"))
                          .ReturnsAsync((Transaction?)null);

            // Act
            var result = await _service.GetTransactionByIdAsync("invalid");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateTransactionAsync_ShouldCreateTransactionAndSendMessage()
        {
            // Arrange
            var createDto = new CreateTransactionDto
            {
                AccountId = "ACC001",
                Amount = 100.50m,
                Type = TransactionType.Credit,
                Description = "Test transaction"
            };

            var createdTransaction = new Transaction
            {
                Id = "generated-id",
                AccountId = "ACC001",
                Amount = 100.50m,
                Type = TransactionType.Credit,
                Description = "Test transaction",
                Status = TransactionStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Transaction>()))
                          .ReturnsAsync(createdTransaction);

            _mockMessageService.Setup(m => m.SendTransactionMessageAsync(It.IsAny<object>()))
                              .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateTransactionAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be("generated-id");
            result.AccountId.Should().Be("ACC001");
            result.Amount.Should().Be(100.50m);
            result.Status.Should().Be(TransactionStatus.Pending);

            _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Transaction>()), Times.Once);
            _mockMessageService.Verify(m => m.SendTransactionMessageAsync(It.IsAny<object>()), Times.Once);
        }
    }
}