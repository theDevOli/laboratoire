using Microsoft.AspNetCore.Mvc;
using Moq;

using Laboratoire.Application.Utils;
using Laboratoire.UI.Controllers;
using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;

namespace Laboratoire.Tests.Controllers
{
    public class ChemicalControllerTests
    {
        private readonly Mock<IChemicalGetterService> _mockChemicalGetter;
        private readonly Mock<IChemicalGetterByIdService> _mockChemicalGetterById;
        private readonly Mock<IChemicalAdderService> _mockChemicalAdder;
        private readonly Mock<IChemicalUpdatableService> _mockChemicalUpdatable;
        private readonly ChemicalController _controller;

        public ChemicalControllerTests()
        {
            _mockChemicalGetter = new Mock<IChemicalGetterService>();
            _mockChemicalGetterById = new Mock<IChemicalGetterByIdService>();
            _mockChemicalAdder = new Mock<IChemicalAdderService>();
            _mockChemicalUpdatable = new Mock<IChemicalUpdatableService>();
            _controller = new ChemicalController
            (_mockChemicalGetter.Object, _mockChemicalGetterById.Object, _mockChemicalUpdatable.Object, _mockChemicalAdder.Object);
        }

        [Fact]
        public async Task GetAllChemicalsAsync_ShouldReturnOkResult_WithListOfChemicals()
        {
            // Arrange
            var chemicals = new List<ChemicalDtoGetUpdate>
            {
                new ChemicalDtoGetUpdate { ChemicalId=1,ChemicalName="Name1",Concentration="Concentration1",Quantity=1.0,Unit="Unit1",IsPoliceControlled=true,IsArmyControlled=false,EntryDate=new DateTime(),ExpireDate=new DateTime()},
                new ChemicalDtoGetUpdate { ChemicalId=2,ChemicalName="Name2",Concentration="Concentration2",Quantity=2.0,Unit="Unit2",IsPoliceControlled=true,IsArmyControlled=false,EntryDate=new DateTime(),ExpireDate=new DateTime()},

            };

            _mockChemicalGetter.Setup(repo => repo.GetAllChemicalsAsync()).ReturnsAsync(chemicals);

            // Act
            var result = await _controller.GetAllChemicals();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<ChemicalDtoGetUpdate>>>(okResult.Value);
            Assert.Equal(chemicals.Count, response.Data?.Count());
        }

        [Fact]
        public async Task GetAllChemicalsAsync_ShouldReturnServerError_WhenSeverErrorOccurs()
        {
            // Arrange
            _mockChemicalGetter.Setup(repo => repo.GetAllChemicalsAsync()).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetAllChemicals();

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);

            Assert.Equal(500, response.Error?.Code);
            Assert.Null(response.Data);
        }

        [Fact]
        public async Task GetChemicalByIdAsync_ShouldReturnOk_WhenChemicalExist()
        {
            // Arrange
            int ChemicalId = 1;
            ChemicalDtoGetUpdate chemical = new ChemicalDtoGetUpdate { ChemicalId = 1, ChemicalName = "Name1", Concentration = "Concentration1", Quantity = 1.0, Unit = "Unit1", IsPoliceControlled = true, IsArmyControlled = false, EntryDate = new DateTime(), ExpireDate = new DateTime() };
            _mockChemicalGetterById.Setup(repo => repo.GetChemicalByIdAsync(It.IsAny<int>())).ReturnsAsync(chemical);

            // Act
            var result = await _controller.GetChemicalByIdAsync(ChemicalId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<ChemicalDtoGetUpdate>>(okResult.Value);
            Assert.Null(response.Error);
            Assert.Equal(chemical.ChemicalId, response.Data?.ChemicalId);
        }

        [Fact]
        public async Task GetChemicalByIdAsync_ShouldReturnNotFound_WhenChemicalDoesNotExist()
        {
            // Arrange
            int ChemicalId = 1;
            ChemicalDtoGetUpdate? Chemical = null;
            _mockChemicalGetterById.Setup(repo => repo.GetChemicalByIdAsync(It.IsAny<int>())).ReturnsAsync(Chemical);

            // Act
            var result = await _controller.GetChemicalByIdAsync(ChemicalId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Null(response.Data);
            Assert.Equal(ErrorMessage.NotFound, response.Error?.Message);
            Assert.Equal(404, response.Error?.Code);
        }

        [Fact]
        public async Task GetChemicalByIdAsync_ShouldReturnServerError_WhenSeverErrorOccurs()
        {
            // Arrange
            _mockChemicalGetterById.Setup(repo => repo.GetChemicalByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetChemicalByIdAsync(1);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);

            Assert.Equal(500, response.Error?.Code);
            Assert.Null(response.Data);
        }

        [Fact]
        public async Task AddChemicalAsync_ShouldReturnOkResult_WhenChemicalIsAddedSuccessfully()
        {
            // Arrange
            var expectedError = Error.SetSuccess();
            var chemicalDto = new ChemicalDtoAdd { ChemicalName = "Name1", Concentration = "Concentration1", Quantity = 1.0, Unit = "Unit1", IsPoliceControlled = true, IsArmyControlled = false, EntryDate = new DateTime(), ExpireDate = new DateTime() };
            _mockChemicalAdder.Setup(repo => repo.AddChemicalAsync(It.IsAny<ChemicalDtoAdd>())).ReturnsAsync(expectedError);

            // Act
            var result = await _controller.AddChemicalAsync(chemicalDto);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal(SuccessMessage.Added, response.Data);
            Assert.Null(response.Error);
        }

        [Fact]
        public async Task AddChemicalAsync_ShouldReturnConflict_WhenChemicalAlreadyExists()
        {
            // Arrange
            var expectedError = Error.SetError(ErrorMessage.ConflictPost, 409);
            var chemicalDto = new ChemicalDtoAdd { ChemicalName = "Name1", Concentration = "Concentration1", Quantity = 1.0, Unit = "Unit1", IsPoliceControlled = true, IsArmyControlled = false, EntryDate = new DateTime(), ExpireDate = new DateTime() };
            _mockChemicalAdder.Setup(repo => repo.AddChemicalAsync(It.IsAny<ChemicalDtoAdd>())).ReturnsAsync(expectedError);

            // Act
            var result = await _controller.AddChemicalAsync(chemicalDto);

            // Assert
            var conflictResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(conflictResult.Value);
            Assert.Null(response.Data);
            Assert.Equal(ErrorMessage.ConflictPost, response.Error?.Message);
            Assert.Equal(409, response.Error?.Code);
        }

        [Fact]
        public async Task AddChemicalAsync_ShouldReturnServerError_WhenSeverErrorOccurs()
        {
            // Arrange
            var chemicalDto = new ChemicalDtoAdd { ChemicalName = "Name1", Concentration = "Concentration1", Quantity = 1.0, Unit = "Unit1", IsPoliceControlled = true, IsArmyControlled = false, EntryDate = new DateTime(), ExpireDate = new DateTime() };
            _mockChemicalAdder.Setup(repo => repo.AddChemicalAsync(It.IsAny<ChemicalDtoAdd>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.AddChemicalAsync(chemicalDto);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);

            Assert.Equal(500, response.Error?.Code);
            Assert.Null(response.Data);
        }

        [Fact]
        public async Task UpdateChemicalAsync_ShouldReturnBadRequest_WhenIdsDoNotMatch()
        {
            // Arrange
            int ChemicalId = 2;
            var chemical = new ChemicalDtoGetUpdate { ChemicalId = 1, ChemicalName = "Name1", Concentration = "Concentration1", Quantity = 1.0, Unit = "Unit1", IsPoliceControlled = true, IsArmyControlled = false, EntryDate = new DateTime(), ExpireDate = new DateTime() };

            // Act
            var result = await _controller.UpdateChemicalAsync(ChemicalId, chemical);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Null(response.Data);
            Assert.Equal(400, response.Error?.Code);
            Assert.Equal(ErrorMessage.BadRequestID, response.Error?.Message);
        }

        [Fact]
        public async Task UpdateChemicalAsync_ShouldReturnOkResult_WhenUpdateIsSuccessful()
        {
            // Arrange
            int ChemicalId = 1;
            var chemical = new ChemicalDtoGetUpdate { ChemicalId = 1, ChemicalName = "Name1", Concentration = "Concentration1", Quantity = 1.0, Unit = "Unit1", IsPoliceControlled = true, IsArmyControlled = false, EntryDate = new DateTime(), ExpireDate = new DateTime() };
            var expectedError = Error.SetSuccess();
            _mockChemicalUpdatable.Setup(repo => repo.UpdateChemicalAsync(chemical)).ReturnsAsync(expectedError);

            // Act
            var result = await _controller.UpdateChemicalAsync(ChemicalId, chemical);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Null(response.Error);
            Assert.Equal(SuccessMessage.Updated, response.Data);
        }

        [Fact]
        public async Task UpdateChemicalAsync_ShouldReturnNotFound_WhenChemicalDoesNotExist()
        {
            // Arrange
            int ChemicalId = 1;
            var chemical = new ChemicalDtoGetUpdate { ChemicalId = 1, ChemicalName = "Name1", Concentration = "Concentration1", Quantity = 1.0, Unit = "Unit1", IsPoliceControlled = true, IsArmyControlled = false, EntryDate = new DateTime(), ExpireDate = new DateTime() };
            var expectedError = Error.SetError(ErrorMessage.NotFound, 404);
            _mockChemicalUpdatable.Setup(repo => repo.UpdateChemicalAsync(chemical)).ReturnsAsync(expectedError);

            // Act
            var result = await _controller.UpdateChemicalAsync(ChemicalId, chemical);

            // Assert
            var notFoundResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Null(response.Data);
            Assert.Equal(ErrorMessage.NotFound, response.Error?.Message);
            Assert.Equal(404, response.Error?.Code);
        }

        [Fact]
        public async Task UpdateChemicalAsync_ShouldReturnServerError_WhenSeverErrorOccurs()
        {
            // Arrange
            int ChemicalId = 1;
            var chemical = new ChemicalDtoGetUpdate { ChemicalId = 1, ChemicalName = "Name1", Concentration = "Concentration1", Quantity = 1.0, Unit = "Unit1", IsPoliceControlled = true, IsArmyControlled = false, EntryDate = new DateTime(), ExpireDate = new DateTime() };
            _mockChemicalUpdatable.Setup(repo => repo.UpdateChemicalAsync(It.IsAny<ChemicalDtoGetUpdate>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.UpdateChemicalAsync(ChemicalId, chemical);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);

            Assert.Equal(500, response.Error?.Code);
            Assert.Null(response.Data);
        }
    }
}
