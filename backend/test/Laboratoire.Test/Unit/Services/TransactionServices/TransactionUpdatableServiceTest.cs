using Laboratoire.Application.Services.TransactionServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.TransactionServices
{
    public class TransactionUpdatableServiceTest
    {
        private readonly Mock<ITransactionRepository> _transactionRepoMock;
        private readonly Mock<ILogger<TransactionUpdatableService>> _loggerMock;
        private readonly TransactionUpdatableService _service;

        public TransactionUpdatableServiceTest()
        {
            _transactionRepoMock = new Mock<ITransactionRepository>();
            _loggerMock = new Mock<ILogger<TransactionUpdatableService>>();
            _service = new TransactionUpdatableService(_transactionRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task UpdateTransactionAsync_ShouldReturnNotFound_WhenTransactionDoesNotExist()
        {
            // Arrange
            var transaction = new Transaction { TransactionId = 1, TransactionType = "Deposit" };
            _transactionRepoMock.Setup(r => r.DoesTransactionExistByIdAsync(It.IsAny<Transaction>()))
                                .ReturnsAsync(false);

            // Act
            var result = await _service.UpdateTransactionAsync(transaction);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(404, result.StatusCode);
            _transactionRepoMock.Verify(r => r.DoesTransactionExistByIdAsync(It.IsAny<Transaction>()), Times.Once);
            _transactionRepoMock.Verify(r => r.DoesTransactionExistByUniqueAsync(It.IsAny<Transaction>()), Times.Never);
            _transactionRepoMock.Verify(r => r.UpdateTransactionAsync(It.IsAny<Transaction>()), Times.Never);
        }

        [Fact]
        public async Task UpdateTransactionAsync_ShouldReturnConflict_WhenTransactionUniqueConflict()
        {
            // Arrange
            var transaction = new Transaction { TransactionId = 1, TransactionType = "Deposit" };
            _transactionRepoMock.Setup(r => r.DoesTransactionExistByIdAsync(It.IsAny<Transaction>()))
                                .ReturnsAsync(true);
            _transactionRepoMock.Setup(r => r.DoesTransactionExistByUniqueAsync(It.IsAny<Transaction>()))
                                .ReturnsAsync(true);

            // Act
            var result = await _service.UpdateTransactionAsync(transaction);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(409, result.StatusCode);
            _transactionRepoMock.Verify(r => r.DoesTransactionExistByIdAsync(It.IsAny<Transaction>()), Times.Once);
            _transactionRepoMock.Verify(r => r.DoesTransactionExistByUniqueAsync(It.IsAny<Transaction>()), Times.Once);
            _transactionRepoMock.Verify(r => r.UpdateTransactionAsync(It.IsAny<Transaction>()), Times.Never);
        }

        [Fact]
        public async Task UpdateTransactionAsync_ShouldReturnDbError_WhenUpdateFails()
        {
            // Arrange
            var transaction = new Transaction { TransactionId = 1, TransactionType = "Deposit", BankName = "Bank A" };
            _transactionRepoMock.Setup(r => r.DoesTransactionExistByIdAsync(transaction)).ReturnsAsync(true);
            _transactionRepoMock.Setup(r => r.DoesTransactionExistByUniqueAsync(transaction)).ReturnsAsync(false);
            _transactionRepoMock.Setup(r => r.UpdateTransactionAsync(transaction)).ReturnsAsync(false);

            // Act
            var result = await _service.UpdateTransactionAsync(transaction);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            _transactionRepoMock.Verify(r => r.DoesTransactionExistByIdAsync(It.IsAny<Transaction>()), Times.Once);
            _transactionRepoMock.Verify(r => r.DoesTransactionExistByUniqueAsync(It.IsAny<Transaction>()), Times.Once);
            _transactionRepoMock.Verify(r => r.UpdateTransactionAsync(It.IsAny<Transaction>()), Times.Once);
        }

        [Fact]
        public async Task UpdateTransactionAsync_ShouldReturnSuccess_WhenUpdateSucceeds()
        {
            // Arrange
            var transaction = new Transaction { TransactionId = 1, TransactionType = "Deposit", BankName = "Bank A" };
            _transactionRepoMock.Setup(r => r.DoesTransactionExistByIdAsync(It.IsAny<Transaction>())).ReturnsAsync(true);
            _transactionRepoMock.Setup(r => r.DoesTransactionExistByUniqueAsync(It.IsAny<Transaction>())).ReturnsAsync(false);
            _transactionRepoMock.Setup(r => r.UpdateTransactionAsync(It.IsAny<Transaction>())).ReturnsAsync(true);

            // Act
            var result = await _service.UpdateTransactionAsync(transaction);

            // Assert
            Assert.False(result.IsNotSuccess());
            Assert.Equal(0, result.StatusCode);
            _transactionRepoMock.Verify(r => r.DoesTransactionExistByIdAsync(It.IsAny<Transaction>()), Times.Once);
            _transactionRepoMock.Verify(r => r.DoesTransactionExistByUniqueAsync(It.IsAny<Transaction>()), Times.Once);
            _transactionRepoMock.Verify(r => r.UpdateTransactionAsync(It.IsAny<Transaction>()), Times.Once);
        }
    }
}
