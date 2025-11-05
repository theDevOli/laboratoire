using Laboratoire.Application.Services.PartnerServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Unit.Services.PartnerServices;

public class PartnerGetterServiceTest
{
    private readonly Mock<IPartnerRepository> _repositoryMock;
    private readonly Mock<ILogger<PartnerGetterService>> _loggerMock;
    private readonly PartnerGetterService _service;

    public PartnerGetterServiceTest()
    {
        _repositoryMock = new Mock<IPartnerRepository>();
        _loggerMock = new Mock<ILogger<PartnerGetterService>>();
        _service = new PartnerGetterService(
            _repositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task GetAllPartnersAsync_ShouldReturnEmptyList_WhenNoPartnersExist()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllPartnersAsync())
                       .ReturnsAsync(new List<Partner>());

        // Act
        var result = await _service.GetAllPartnersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _repositoryMock.Verify(r => r.GetAllPartnersAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllPartnersAsync_ShouldReturnPartners_WhenRepositoryHasData()
    {
        // Arrange
        var partners = new List<Partner>
            {
                new Partner { PartnerId = Guid.NewGuid(), PartnerName = "Partner A", OfficeId=Guid.NewGuid() },
                new Partner { PartnerId = Guid.NewGuid(), PartnerName = "Partner B", OfficeId=Guid.NewGuid() }
            };

        _repositoryMock.Setup(r => r.GetAllPartnersAsync())
                       .ReturnsAsync(partners);

        // Act
        var result = await _service.GetAllPartnersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Collection(
            result,
            item => Assert.Equal(item.PartnerName, partners[0].PartnerName),
            item => Assert.Equal(item.PartnerName, partners[1].PartnerName)
       );
       
        _repositoryMock.Verify(r => r.GetAllPartnersAsync(), Times.Once);
    }
}
