using Laboratoire.Application.Services.TransactionServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.TransactionServices
{
    public class TransactionGetterByIdServiceTest
    {
        private readonly Mock<ITransactionRepository> _transactionRepoMock;
        private readonly Mock<ILogger<TransactionGetterByIdService>> _loggerMock;
        private readonly TransactionGetterByIdService _service;

        public TransactionGetterByIdServiceTest()
        {
            _transactionRepoMock = new Mock<ITransactionRepository>();
            _loggerMock = new Mock<ILogger<TransactionGetterByIdService>>();
            _service = new TransactionGetterByIdService(_transactionRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetTransactionByIdAsync_ShouldReturnNull_WhenTransactionIdIsNull()
        {
            // Act
            var result = await _service.GetTransactionByIdAsync(null);

            // Assert
            Assert.Null(result);
            _transactionRepoMock.Verify(r => r.GetTransactionByIdAsync(It.IsAny<int>()), Times.Never);

        }

        [Fact]
        public async Task GetTransactionByIdAsync_ShouldReturnTransaction_WhenTransactionExists()
        {
            // Arrange
            int transactionId = 1;
            var transaction = new Transaction
            {
                TransactionId = transactionId,
                TransactionType = "Deposit",
                BankName = "Bank A"
            };

            _transactionRepoMock.Setup(r => r.GetTransactionByIdAsync(transactionId))
                .ReturnsAsync(transaction);

            // Act
            var result = await _service.GetTransactionByIdAsync(transactionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(transactionId, result.TransactionId);
            Assert.Equal("Deposit", result.TransactionType);
            _transactionRepoMock.Verify(r => r.GetTransactionByIdAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task GetTransactionByIdAsync_ShouldReturnNull_WhenTransactionDoesNotExist()
        {
            // Arrange
            _transactionRepoMock.Setup(r => r.GetTransactionByIdAsync(99))
                .ReturnsAsync((Transaction?)null);

            // Act
            var result = await _service.GetTransactionByIdAsync(99);

            // Assert
            Assert.Null(result);
            _transactionRepoMock.Verify(r => r.GetTransactionByIdAsync(It.IsAny<int>()), Times.Once);
        }
    }
}
