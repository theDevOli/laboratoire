using Laboratoire.Application.Services.UtilServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.UtilServices
{
    public class StateGetterServiceTest
    {
        private readonly Mock<IUtilsRepository> _utilsRepoMock;
        private readonly Mock<ILogger<StateGetterService>> _loggerMock;
        private readonly StateGetterService _service;

        public StateGetterServiceTest()
        {
            _utilsRepoMock = new Mock<IUtilsRepository>();
            _loggerMock = new Mock<ILogger<StateGetterService>>();
            _service = new StateGetterService(_utilsRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllStatesAsync_ShouldReturnListOfStates_WhenStateExist()
        {
            // Arrange
            var states = new List<State>
            {
                new() { StateId = 1, StateName = "New Brunswick" },
                new() { StateId = 2, StateName = "Quebec" }
            };

            _utilsRepoMock.Setup(r => r.GetAllStatesAsync())
                          .ReturnsAsync(states);

            // Act
            var result = await _service.GetAllStatesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Collection
            (
                result,
                item => Assert.Equal(item.StateName, states[0].StateName),
                item => Assert.Equal(item.StateName, states[1].StateName)
            );
            _utilsRepoMock.Verify(r => r.GetAllStatesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllStatesAsync_ShouldReturnEmpty_WhenNoStatesExist()
        {
            // Arrange
            _utilsRepoMock.Setup(r => r.GetAllStatesAsync())
                          .ReturnsAsync(Enumerable.Empty<State>());

            // Act
            var result = await _service.GetAllStatesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _utilsRepoMock.Verify(r => r.GetAllStatesAsync(), Times.Once);
        }
    }
}
