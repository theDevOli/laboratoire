using Laboratoire.Application.Services.CashFlowServices;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.CashFlowServices
{
    public class CashFlowUpdatableServiceTest
    {
        private readonly Mock<ICashFlowRepository> _cashFlowRepositoryMock;
        private readonly Mock<ILogger<CashFlowUpdatableService>> _loggerMock;
        private readonly CashFlowUpdatableService _service;

        public CashFlowUpdatableServiceTest()
        {
            _cashFlowRepositoryMock = new Mock<ICashFlowRepository>();
            _loggerMock = new Mock<ILogger<CashFlowUpdatableService>>();

            _service = new CashFlowUpdatableService(
                _cashFlowRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task UpdateCashFlowAsync_ShouldReturnNotFound_WhenCashFlowDoesNotExist()
        {
            // Arrange
            var cashFlow = new CashFlow { CashFlowId = 1 };

            _cashFlowRepositoryMock
                .Setup(r => r.DoesCashFlowExistsAsync(cashFlow))
                .ReturnsAsync(false);

            // Act
            var result = await _service.UpdateCashFlowAsync(cashFlow);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(ErrorMessage.NotFound, result.Message);
            Assert.Equal(404, result.StatusCode);

            _cashFlowRepositoryMock.Verify(r => r.DoesCashFlowExistsAsync(cashFlow), Times.Once);
            _cashFlowRepositoryMock.Verify(r => r.UpdateCashFlowAsync(It.IsAny<CashFlow>()), Times.Never);
        }

        [Fact]
        public async Task UpdateCashFlowAsync_ShouldReturnDbError_WhenUpdateFails()
        {
            // Arrange
            var cashFlow = new CashFlow { CashFlowId = 2 };

            _cashFlowRepositoryMock
                .Setup(r => r.DoesCashFlowExistsAsync(cashFlow))
                .ReturnsAsync(true);

            _cashFlowRepositoryMock
                .Setup(r => r.UpdateCashFlowAsync(cashFlow))
                .ReturnsAsync(false);

            // Act
            var result = await _service.UpdateCashFlowAsync(cashFlow);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(ErrorMessage.DbError, result.Message);
            Assert.Equal(500, result.StatusCode);

            _cashFlowRepositoryMock.Verify(r => r.DoesCashFlowExistsAsync(cashFlow), Times.Once);
            _cashFlowRepositoryMock.Verify(r => r.UpdateCashFlowAsync(cashFlow), Times.Once);
        }

        [Fact]
        public async Task UpdateCashFlowAsync_ShouldReturnSuccess_WhenUpdateSucceeds()
        {
            // Arrange
            var cashFlow = new CashFlow { CashFlowId = 3 };

            _cashFlowRepositoryMock
                .Setup(r => r.DoesCashFlowExistsAsync(cashFlow))
                .ReturnsAsync(true);

            _cashFlowRepositoryMock
                .Setup(r => r.UpdateCashFlowAsync(cashFlow))
                .ReturnsAsync(true);

            // Act
            var result = await _service.UpdateCashFlowAsync(cashFlow);

            // Assert
            Assert.False(result.IsNotSuccess());
            Assert.Null(result.Message);
            Assert.Equal(0, result.StatusCode);

            _cashFlowRepositoryMock.Verify(r => r.DoesCashFlowExistsAsync(cashFlow), Times.Once);
            _cashFlowRepositoryMock.Verify(r => r.UpdateCashFlowAsync(cashFlow), Times.Once);
        }
    }
}
