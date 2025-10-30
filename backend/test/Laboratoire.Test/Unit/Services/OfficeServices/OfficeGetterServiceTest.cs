using System.Threading.Tasks;
using Laboratoire.Application.Services.OfficeServices;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Unit.Services.OfficeServices;

public class OfficeGetterServiceTest
{
    private readonly Mock<IOfficeRepository> _repositoryMock;
    private readonly Mock<ILogger<OfficeGetterService>> _loggerMock;
    private readonly OfficeGetterService _service;

    public OfficeGetterServiceTest()
    {
        _repositoryMock = new Mock<IOfficeRepository>();
        _loggerMock = new Mock<ILogger<OfficeGetterService>>();

        _service = new OfficeGetterService(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllOfficesAsync_ShouldReturnAllOffices_WhenOfficeExists()
    {
        // Arrange
        var expectedOffices = new List<Office>()
        {
            new(){OfficeId=new Guid(),OfficeEmail="test01@email.com",OfficeName="Test01",City="Test"},
            new(){OfficeId=new Guid(),OfficeEmail="test02@email.com",OfficeName="Test02",City="Test"},
            new(){OfficeId=new Guid(),OfficeEmail="test03@email.com",OfficeName="Test03",City="Test"}
        };

        _repositoryMock.Setup(repo => repo.GetAllOfficesAsync()).ReturnsAsync(expectedOffices);

        // Act
        var result = await _service.GetAllOfficesAsync();

        // Assert
        Assert.NotEmpty(result);
        Assert.Collection
            (
                result,
                item => Assert.Equal(expectedOffices[0].OfficeId, item.OfficeId),
                item => Assert.Equal(expectedOffices[1].OfficeId, item.OfficeId),
                item => Assert.Equal(expectedOffices[2].OfficeId, item.OfficeId)
            );
        _repositoryMock.Verify(repo => repo.GetAllOfficesAsync(), Times.Once);
    }


    [Fact]
    public async Task GetAllOfficesAsync_ShouldReturnEmptyList_WhenNoOfficeExists()
    {
        // Arrange
        var expectedOffices = Enumerable.Empty<Office>();

        _repositoryMock.Setup(repo => repo.GetAllOfficesAsync()).ReturnsAsync(expectedOffices);

        // Act
        var result = await _service.GetAllOfficesAsync();

        // Assert
        Assert.Empty(result);
        _repositoryMock.Verify(repo => repo.GetAllOfficesAsync(), Times.Once);
    }
}
