using Laboratoire.Application.Services.CashFlowServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.CashFlowServices
{
    public class CashFlowGetterByIdServiceTest
    {
        private readonly Mock<ICashFlowRepository> _cashFlowRepositoryMock;
        private readonly Mock<ILogger<CashFlowGetterByIdService>> _loggerMock;
        private readonly CashFlowGetterByIdService _service;

        public CashFlowGetterByIdServiceTest()
        {
            _cashFlowRepositoryMock = new Mock<ICashFlowRepository>();
            _loggerMock = new Mock<ILogger<CashFlowGetterByIdService>>();

            _service = new CashFlowGetterByIdService(
                _cashFlowRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetCashFlowByIdAsync_ShouldReturnNull_WhenIdIsNull()
        {
            // Act
            var result = await _service.GetCashFlowByIdAsync(null);

            // Assert
            Assert.Null(result);
            _cashFlowRepositoryMock.Verify(r => r.GetCashFlowByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetCashFlowByIdAsync_ShouldReturnCashFlow_WhenCashFlowExists()
        {
            // Arrange
            var cashFlow = new CashFlow { CashFlowId = 10 };

            _cashFlowRepositoryMock
                .Setup(r => r.GetCashFlowByIdAsync(10))
                .ReturnsAsync(cashFlow);

            // Act
            var result = await _service.GetCashFlowByIdAsync(10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result!.CashFlowId);
            _cashFlowRepositoryMock.Verify(r => r.GetCashFlowByIdAsync(10), Times.Once);
        }

        [Fact]
        public async Task GetCashFlowByIdAsync_ShouldReturnNull_WhenCashFlowDoesNotExist()
        {
            // Arrange
            _cashFlowRepositoryMock
                .Setup(r => r.GetCashFlowByIdAsync(99))
                .ReturnsAsync((CashFlow?)null);

            // Act
            var result = await _service.GetCashFlowByIdAsync(99);

            // Assert
            Assert.Null(result);
            _cashFlowRepositoryMock.Verify(r => r.GetCashFlowByIdAsync(99), Times.Once);
        }
    }
}
