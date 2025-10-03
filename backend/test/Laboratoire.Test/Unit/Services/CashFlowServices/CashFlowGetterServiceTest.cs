using Laboratoire.Application.Services.CashFlowServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.CashFlowServices
{
    public class CashFlowGetterServiceTest
    {
        private readonly Mock<ICashFlowRepository> _cashFlowRepositoryMock;
        private readonly Mock<ILogger<CashFlowGetterService>> _loggerMock;
        private readonly CashFlowGetterService _service;

        public CashFlowGetterServiceTest()
        {
            _cashFlowRepositoryMock = new Mock<ICashFlowRepository>();
            _loggerMock = new Mock<ILogger<CashFlowGetterService>>();

            _service = new CashFlowGetterService(
                _cashFlowRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetAllCashFlowAsync_ShouldReturnCashFlows_WhenTheyExist()
        {
            // Arrange
            var cashFlows = new List<CashFlow>
            {
                new CashFlow { CashFlowId = 1 },
                new CashFlow { CashFlowId = 2 }
            };

            _cashFlowRepositoryMock
                .Setup(r => r.GetAllCashFlowAsync())
                .ReturnsAsync(cashFlows);

            // Act
            var result = await _service.GetAllCashFlowAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _cashFlowRepositoryMock.Verify(r => r.GetAllCashFlowAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllCashFlowAsync_ShouldReturnEmpty_WhenNoCashFlowsExist()
        {
            // Arrange
            _cashFlowRepositoryMock
                .Setup(r => r.GetAllCashFlowAsync())
                .ReturnsAsync(Enumerable.Empty<CashFlow>());

            // Act
            var result = await _service.GetAllCashFlowAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _cashFlowRepositoryMock.Verify(r => r.GetAllCashFlowAsync(), Times.Once);
        }
    }
}
