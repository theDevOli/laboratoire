using Microsoft.AspNetCore.Mvc;
using Moq;

using Laboratoire.Domain.Entity;
using Laboratoire.Application.Utils;
using Laboratoire.UI.Controllers;
using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;

namespace Laboratoire.Tests.Controllers
{
    public class CashFlowControllerTests
    {
        private readonly Mock<ICashFlowGetterService> _mockGetterService;
        private readonly Mock<ICashFlowGetterByIdService> _mockGetterByIdService;
        private readonly Mock<ICashFlowGetterByYearAndMonthService> _mockByYearAndMonthService;
        private readonly Mock<ICashFlowUpdatableService> _mockUpdatableService;
        private readonly Mock<ICashFlowAdderService> _mockAdderService;

        private readonly CashFlowController _controller;

        public CashFlowControllerTests()
        {
            _mockGetterService = new Mock<ICashFlowGetterService>();
            _mockGetterByIdService = new Mock<ICashFlowGetterByIdService>();
            _mockByYearAndMonthService = new Mock<ICashFlowGetterByYearAndMonthService>();
            _mockUpdatableService = new Mock<ICashFlowUpdatableService>();
            _mockAdderService = new Mock<ICashFlowAdderService>();
            _controller = new CashFlowController
            (_mockGetterService.Object, _mockGetterByIdService.Object, _mockByYearAndMonthService.Object, _mockUpdatableService.Object, _mockAdderService.Object);
        }

        [Fact]
        public async Task GetAllCashFlowAsync_ShouldReturnOkResult_WithListOfCashFlow()
        {
            // Arrange
            var cashFlows = new List<CashFlow>
            {
                new CashFlow { CashFlowId = 1, TransactionId = 1, Description = "1", PartnerId = null,  TotalPaid = 100.00m,PaymentDate=new DateTime() },
                new CashFlow { CashFlowId = 2, TransactionId = 2, Description = "2", PartnerId = new Guid(),  TotalPaid = 200.00m,PaymentDate=new DateTime()  }
            };
            _mockGetterService.Setup(repo => repo.GetAllCashFlowAsync()).ReturnsAsync(cashFlows);

            // Act
            var result = await _controller.GetAllCashFlowAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<CashFlow>>>(okResult.Value);

            Assert.Equal(cashFlows.Count, response?.Data?.Count());
        }

        [Fact]
        public async Task GetAllCashFlowAsync_ShouldReturnServerError_WhenSeverErrorOccurs()
        {
            // Arrange
            _mockGetterService.Setup(repo => repo.GetAllCashFlowAsync()).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetAllCashFlowAsync();

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);

            Assert.Equal(500, response.Error?.Code);
            Assert.Null(response.Data);
        }

        [Fact]
        public async Task GetAllCashFlowByYearAndMonth_ShouldReturnOkResult_WithListOfCashFlow()
        {
            // Arrange
            var cashFlows = new List<CashFlow>
            {
                new CashFlow { CashFlowId = 1, TransactionId = 1, Description = "1", PartnerId = null,  TotalPaid = 100.00m,PaymentDate=new DateTime(2024,2,15)},
                new CashFlow { CashFlowId = 2, TransactionId = 2, Description = "2", PartnerId = new Guid(),  TotalPaid = 200.00m,PaymentDate=new DateTime(2024,2,15)  },
                new CashFlow { CashFlowId = 3, TransactionId = 1, Description = "1", PartnerId = null,  TotalPaid = 100.00m,PaymentDate=new DateTime(2024,2,15) },
                // new CashFlow { CashFlowId = 4, TransactionId = 2, Description = "2", PartnerId = new Guid(),  TotalPaid = 200.00m,PaymentDate=new DateTime(2025,2,15)  },
                // new CashFlow { CashFlowId = 5, TransactionId = 1, Description = "1", PartnerId = null,  TotalPaid = 100.00m,PaymentDate=new DateTime(2025,2,15) },
                // new CashFlow { CashFlowId = 6, TransactionId = 2, Description = "2", PartnerId = new Guid(),  TotalPaid = 200.00m,PaymentDate=new DateTime(2025,2,15)  },
                // new CashFlow { CashFlowId = 7, TransactionId = 1, Description = "1", PartnerId = null,  TotalPaid = 100.00m,PaymentDate=new DateTime(2025,2,15) },
                // new CashFlow { CashFlowId = 8, TransactionId = 2, Description = "2", PartnerId = new Guid(),  TotalPaid = 200.00m,PaymentDate=new DateTime(2025,2,15)  },
                // new CashFlow { CashFlowId = 9, TransactionId = 1, Description = "1", PartnerId = null,  TotalPaid = 100.00m,PaymentDate=new DateTime(2025,2,15) },
                // new CashFlow { CashFlowId = 10, TransactionId = 2, Description = "2", PartnerId = new Guid(),  TotalPaid = 200.00m,PaymentDate=new DateTime(2025,2,15)  },
            };
            _mockByYearAndMonthService.Setup(repo => repo.GetCashFlowByYearAndMonthAsync(2024, 2)).ReturnsAsync(cashFlows);

            // Assert
            var result = await _controller.GetCashFlowByYearAndMonthAsync(2025, 2, "in", 1,null);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<CashFlow>>>(okResult.Value);

            Assert.Equal(cashFlows.Count, response?.Data?.Count());
        }

        [Fact]
        public async Task GetCashFlowByIdAsync_ShouldReturnOk_WhenCashFlowExists()
        {
            // Arrange
            var cashFlow = new CashFlow { CashFlowId = 1, TransactionId = 1, Description = "1", PartnerId = null, TotalPaid = 100.00m, PaymentDate = new DateTime() };
            _mockGetterByIdService.Setup(repo => repo.GetCashFlowByIdAsync(It.IsAny<int>())).ReturnsAsync(cashFlow);

            // Act
            var result = await _controller.GetCashFlowByIdAsync(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<CashFlow>>(okResult.Value);
            Assert.Null(response.Error);
            Assert.Equal(cashFlow.CashFlowId, response.Data?.CashFlowId);
        }

        [Fact]
        public async Task GetCashFlowByIdAsync_ShouldReturnNotFound_WhenCashFlowDoesNotExist()
        {
            // Arrange
            int cashFlowId = 1;
            CashFlow? cashFlow = null;
            _mockGetterByIdService.Setup(repo => repo.GetCashFlowByIdAsync(cashFlowId)).ReturnsAsync(cashFlow);

            // Act
            var result = await _controller.GetCashFlowByIdAsync(cashFlowId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Null(response.Data);
            Assert.Equal(ErrorMessage.NotFound, response?.Error?.Message);
            Assert.Equal(404, response?.Error?.Code);
        }

        [Fact]
        public async Task GetCashFlowByIdAsync_ShouldReturnServerError_WhenSeverErrorOccurs()
        {
            // Arrange
            _mockGetterByIdService.Setup(repo => repo.GetCashFlowByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetCashFlowByIdAsync(1);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);

            Assert.Equal(500, response.Error?.Code);
            Assert.Null(response.Data);
        }

        [Fact]
        public async Task UpdateCashFlowAsync_ShouldReturnBadRequest_WhenIdsDoNotMatch()
        {
            // Arrange
            int cashFlowId = 1;
            var cashFlow = new CashFlow { CashFlowId = 2, TransactionId = 1, Description = "1", PartnerId = null, TotalPaid = 100.00m, PaymentDate = new DateTime() };

            // Act
            var result = await _controller.UpdateCashFlowAsync(cashFlowId, cashFlow);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Null(response.Data);
            Assert.Equal(ErrorMessage.BadRequestID, response?.Error?.Message);
            Assert.Equal(400, response?.Error?.Code);
        }

        [Fact]
        public async Task UpdateCashFlowAsync_ShouldReturnOkResult_WhenUpdateIsSuccessful()
        {
            // Arrange
            int cashFlowId = 1;
            var cashFlow = new CashFlow { CashFlowId = 1, TransactionId = 1, Description = "1", PartnerId = null, TotalPaid = 100.00m, PaymentDate = new DateTime() };
            _mockUpdatableService.Setup(repo => repo.UpdateCashFlowAsync(cashFlow)).ReturnsAsync(Error.SetSuccess());

            // Act
            var result = await _controller.UpdateCashFlowAsync(cashFlowId, cashFlow);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal(SuccessMessage.Updated, response.Data);
            Assert.Null(response.Error);
        }

        [Fact]
        public async Task UpdateCashFlowAsync_ShouldReturnServerError_WhenSeverErrorOccurs()
        {
            // Arrange
            var cashFlow = new CashFlow { CashFlowId = 1, TransactionId = 1, Description = "1", PartnerId = null, TotalPaid = 100.00m, PaymentDate = new DateTime() }; int cashFlowId = 1;
            _mockUpdatableService.Setup(repo => repo.UpdateCashFlowAsync(It.IsAny<CashFlow>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.UpdateCashFlowAsync(cashFlowId, cashFlow);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);

            Assert.Equal(500, response.Error?.Code);
            Assert.Null(response.Data);
        }


    }
}
