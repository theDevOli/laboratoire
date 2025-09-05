using Laboratoire.Application.Services.PartnerServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.PartnerServices;

public class PartnerGetterByIdServiceTest
{
    private readonly Mock<IPartnerRepository> _partnerRepositoryMock;
    private readonly Mock<ILogger<PartnerGetterByIdService>> _loggerMock;
    private readonly PartnerGetterByIdService _service;

    public PartnerGetterByIdServiceTest()
    {
        _partnerRepositoryMock = new Mock<IPartnerRepository>();
        _loggerMock = new Mock<ILogger<PartnerGetterByIdService>>();
        _service = new PartnerGetterByIdService(_partnerRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetPartnerByIdAsync_ShouldReturnNull_WhenIdIsNull()
    {
        // Act
        var result = await _service.GetPartnerByIdAsync(null);

        // Assert
        Assert.Null(result);
        _partnerRepositoryMock.Verify(r => r.GetPartnerByIdAsync(It.IsAny<Guid?>()), Times.Never);
    }

    [Fact]
    public async Task GetPartnerByIdAsync_ShouldReturnPartner_WhenPartnerExists()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var expectedPartner = new Partner { PartnerId = partnerId, PartnerName = "Test Partner" };

        _partnerRepositoryMock
            .Setup(r => r.GetPartnerByIdAsync(partnerId))
            .ReturnsAsync(expectedPartner);

        // Act
        var result = await _service.GetPartnerByIdAsync(partnerId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedPartner.PartnerId, result!.PartnerId);
        Assert.Equal(expectedPartner.PartnerName, result!.PartnerName);
        _partnerRepositoryMock.Verify(r => r.GetPartnerByIdAsync(partnerId), Times.Once);
    }

    [Fact]
    public async Task GetPartnerByIdAsync_ShouldReturnNull_WhenPartnerDoesNotExist()
    {
        // Arrange
        var partnerId = Guid.NewGuid();

        _partnerRepositoryMock
            .Setup(r => r.GetPartnerByIdAsync(partnerId))
            .ReturnsAsync((Partner?)null);

        // Act
        var result = await _service.GetPartnerByIdAsync(partnerId);

        // Assert
        Assert.Null(result);
        _partnerRepositoryMock.Verify(r => r.GetPartnerByIdAsync(partnerId), Times.Once);
    }
}
