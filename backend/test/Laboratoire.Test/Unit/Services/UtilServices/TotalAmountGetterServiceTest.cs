using Laboratoire.Application.Services.UtilServices;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.UtilServices
{
    public class TotalAmountGetterServiceTest
    {
        private readonly Mock<ICashFlowRepository> _cashFlowRepoMock;
        private readonly Mock<ILogger<TotalAmountGetterService>> _loggerMock;
        private readonly TotalAmountGetterService _service;

        public TotalAmountGetterServiceTest()
        {
            _cashFlowRepoMock = new Mock<ICashFlowRepository>();
            _loggerMock = new Mock<ILogger<TotalAmountGetterService>>();
            _service = new TotalAmountGetterService(_cashFlowRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAmountAsync_ShouldReturnCorrectAmount()
        {
            // Arrange
            var expected = 1500.75m;
            _cashFlowRepoMock.Setup(r => r.GetAmountAsync(2025, 9, "INCOME", 1))
                             .ReturnsAsync(expected);

            // Act
            var result = await _service.GetAmountAsync(2025, 9, "INCOME", 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected, result);
            _cashFlowRepoMock
                .Verify(r => r.GetAmountAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task GetAmountAsync_ShouldReturnNull_WhenRepositoryReturnsNull()
        {
            // Arrange
            _cashFlowRepoMock.Setup(r => r.GetAmountAsync(null, null, null, null))
                             .ReturnsAsync((decimal?)null);

            // Act
            var result = await _service.GetAmountAsync(null, null, null, null);

            // Assert
            Assert.Null(result);
            _cashFlowRepoMock
                .Verify(r => r.GetAmountAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }
    }
}
