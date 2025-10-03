using Laboratoire.Application.DTO;
using Laboratoire.Application.Services.TransactionServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.TransactionServices
{
    public class TransactionAdderServiceTest
    {
        private readonly Mock<ITransactionRepository> _transactionRepoMock;
        private readonly Mock<ILogger<TransactionAdderService>> _loggerMock;
        private readonly TransactionAdderService _service;

        public TransactionAdderServiceTest()
        {
            _transactionRepoMock = new Mock<ITransactionRepository>();
            _loggerMock = new Mock<ILogger<TransactionAdderService>>();
            _service = new TransactionAdderService(_transactionRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AddTransactionAsync_ShouldReturnConflict_WhenTransactionExists()
        {
            // Arrange
            var transactionDto = new TransactionDtoAdd
            {
                TransactionType = "Deposit",
                BankName = "Bank A"
            };
            _transactionRepoMock
                .Setup(r => r.DoesTransactionExistByUniqueAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.AddTransactionAsync(transactionDto);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(409, result.StatusCode);
            _transactionRepoMock.Verify(r => r.DoesTransactionExistByUniqueAsync(It.IsAny<Transaction>()), Times.Once);
            _transactionRepoMock.Verify(r => r.AddTransactionAsync(It.IsAny<Transaction>()), Times.Never);
        }

        [Fact]
        public async Task AddTransactionAsync_ShouldReturnDbError_WhenAddFails()
        {
            // Arrange
            var transactionDto = new TransactionDtoAdd
            {
                TransactionType = "Deposit",
                BankName = "Bank A"
            };
            _transactionRepoMock.Setup(r => r.DoesTransactionExistByUniqueAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(false);
            _transactionRepoMock.Setup(r => r.AddTransactionAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.AddTransactionAsync(transactionDto);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            _transactionRepoMock.Verify(r => r.DoesTransactionExistByUniqueAsync(It.IsAny<Transaction>()), Times.Once);
            _transactionRepoMock.Verify(r => r.AddTransactionAsync(It.IsAny<Transaction>()), Times.Once);
        }

        [Fact]
        public async Task AddTransactionAsync_ShouldReturnSuccess_WhenAddSucceeds()
        {
            // Arrange
            var transactionDto = new TransactionDtoAdd
            {
                TransactionType = "Deposit",
                BankName = "Bank A"
            };
            _transactionRepoMock.Setup(r => r.DoesTransactionExistByUniqueAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(false);
            _transactionRepoMock.Setup(r => r.AddTransactionAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.AddTransactionAsync(transactionDto);

            // Assert
            Assert.False(result.IsNotSuccess());
            Assert.Equal(0, result.StatusCode);
            _transactionRepoMock.Verify(r => r.DoesTransactionExistByUniqueAsync(It.IsAny<Transaction>()), Times.Once);
            _transactionRepoMock.Verify(r => r.AddTransactionAsync(It.IsAny<Transaction>()), Times.Once);
        }
    }
}
