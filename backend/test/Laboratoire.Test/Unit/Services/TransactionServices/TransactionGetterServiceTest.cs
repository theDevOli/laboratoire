using Laboratoire.Application.Services.TransactionServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.TransactionServices
{
    public class TransactionGetterServiceTest
    {
        private readonly Mock<ITransactionRepository> _transactionRepoMock;
        private readonly Mock<ILogger<TransactionGetterService>> _loggerMock;
        private readonly TransactionGetterService _service;

        public TransactionGetterServiceTest()
        {
            _transactionRepoMock = new Mock<ITransactionRepository>();
            _loggerMock = new Mock<ILogger<TransactionGetterService>>();
            _service = new TransactionGetterService(_transactionRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllTransactionsAsync_ShouldReturnTransactions()
        {
            // Arrange
            var transactions = new List<Transaction>
            {
                new Transaction { TransactionId = 1, TransactionType = "Deposit", BankName = "Bank A" },
                new Transaction { TransactionId = 2, TransactionType = "Withdrawal", BankName = "Bank B" }
            };

            _transactionRepoMock.Setup(r => r.GetAllTransactionsAsync())
                .ReturnsAsync(transactions);

            // Act
            var result = await _service.GetAllTransactionsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Collection
                (
                    result,
                    item => Assert.Equal(1, item.TransactionId),
                    item => Assert.Equal(2, item.TransactionId)
                );
            _transactionRepoMock.Verify(r => r.GetAllTransactionsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllTransactionsAsync_ShouldReturnEmptyList_WhenNoTransactions()
        {
            // Arrange
            _transactionRepoMock.Setup(r => r.GetAllTransactionsAsync())
                .ReturnsAsync(new List<Transaction>());

            // Act
            var result = await _service.GetAllTransactionsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _transactionRepoMock.Verify(r => r.GetAllTransactionsAsync(), Times.Once);
        }
    }
}
