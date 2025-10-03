using Laboratoire.Application.Services.CashFlowServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.CashFlowServices
{
    public class CashFlowGetterByYearAndMonthServiceTest
    {
        private readonly Mock<ICashFlowRepository> _cashFlowRepositoryMock;
        private readonly Mock<ILogger<CashFlowGetterByYearAndMonthService>> _loggerMock;
        private readonly CashFlowGetterByYearAndMonthService _service;

        public CashFlowGetterByYearAndMonthServiceTest()
        {
            _cashFlowRepositoryMock = new Mock<ICashFlowRepository>();
            _loggerMock = new Mock<ILogger<CashFlowGetterByYearAndMonthService>>();

            _service = new CashFlowGetterByYearAndMonthService(
                _cashFlowRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetCashFlowByYearAndMonthAsync_ShouldReturnEmpty_WhenYearIsNull()
        {
            // Act
            var result = await _service.GetCashFlowByYearAndMonthAsync(null, 5);

            // Assert
            Assert.Empty(result);
            _cashFlowRepositoryMock.Verify(r => r.GetCashFlowByYearAndMonthAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetCashFlowByYearAndMonthAsync_ShouldReturnEmpty_WhenMonthIsNull()
        {
            // Act
            var result = await _service.GetCashFlowByYearAndMonthAsync(2024, null);

            // Assert
            Assert.Empty(result);
            _cashFlowRepositoryMock.Verify(r => r.GetCashFlowByYearAndMonthAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetCashFlowByYearAndMonthAsync_ShouldReturnCashFlows_WhenTheyExist()
        {
            // Arrange
            var cashFlows = new List<CashFlow>
            {
                new CashFlow { CashFlowId = 1 },
                new CashFlow { CashFlowId = 2 }
            };

            _cashFlowRepositoryMock
                .Setup(r => r.GetCashFlowByYearAndMonthAsync(2024, 7))
                .ReturnsAsync(cashFlows);

            // Act
            var result = await _service.GetCashFlowByYearAndMonthAsync(2024, 7);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count());
            _cashFlowRepositoryMock.Verify(r => r.GetCashFlowByYearAndMonthAsync(2024, 7), Times.Once);
        }

        [Fact]
        public async Task GetCashFlowByYearAndMonthAsync_ShouldReturnEmpty_WhenNoCashFlowsExist()
        {
            // Arrange
            _cashFlowRepositoryMock
                .Setup(r => r.GetCashFlowByYearAndMonthAsync(2024, 8))
                .ReturnsAsync(Enumerable.Empty<CashFlow>());

            // Act
            var result = await _service.GetCashFlowByYearAndMonthAsync(2024, 8);

            // Assert
            Assert.Empty(result);
            _cashFlowRepositoryMock.Verify(r => r.GetCashFlowByYearAndMonthAsync(2024, 8), Times.Once);
        }
    }
}
