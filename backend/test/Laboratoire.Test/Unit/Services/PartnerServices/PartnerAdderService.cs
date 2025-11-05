using Laboratoire.Application.DTO;
using Laboratoire.Application.Services.PartnerServices;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Unit.Services.PartnerServices;

public class PartnerAdderServiceTest
{
    private readonly Mock<IPartnerRepository> _repositoryMock;
    private readonly Mock<IUserAdderService> _userAdderMock;
    private readonly Mock<IUserDeletionService> _userDeletionMock;
    private readonly Mock<ILogger<PartnerAdderService>> _loggerMock;
    private readonly PartnerAdderService _service;

    public PartnerAdderServiceTest()
    {
        _repositoryMock = new Mock<IPartnerRepository>();
        _userAdderMock = new Mock<IUserAdderService>();
        _userDeletionMock = new Mock<IUserDeletionService>();
        _loggerMock = new Mock<ILogger<PartnerAdderService>>();
        _service = new PartnerAdderService(
            _repositoryMock.Object,
            _userAdderMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task AddPartnerAsync_ShouldReturnConflict_WhenPartnerAlreadyExists()
    {
        // Arrange
        var partnerDto = new PartnerDtoAdd { OfficeId=Guid.NewGuid(), PartnerName = "Conflict Partner" };

        _repositoryMock.Setup(r => r.DoesPartnerExistByOfficeAndNameAsync(It.IsAny<Partner>()))
                       .ReturnsAsync(true);

        // Act
        var result = await _service.AddPartnerAsync(partnerDto);

        // Assert
        Assert.True(result.IsNotSuccess());
        Assert.Equal(409, result.StatusCode);
        Assert.Equal(ErrorMessage.ConflictPost, result.Message);
        _repositoryMock.Verify(r => r.AddPartnerAsync(It.IsAny<Partner>(), It.IsAny<Guid>()), Times.Never);
        _userAdderMock.Verify(u => u.AddUserAsync(It.IsAny<UserDtoAdd>()), Times.Never);
        _userDeletionMock.Verify(u => u.DeletionUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task AddPartnerAsync_ShouldReturnDbError_WhenUserInsertionFromPartnerFails()
    {
        // Arrange
        var partnerDto = new PartnerDtoAdd { OfficeId=Guid.NewGuid(), PartnerName = "Rollback Partner" };

        _repositoryMock.Setup(r => r.DoesPartnerExistByOfficeAndNameAsync(It.IsAny<Partner>()))
                       .ReturnsAsync(false);
        _userAdderMock.Setup(u => u.AddUserAsync(It.IsAny<UserDtoAdd>()))
                      .ReturnsAsync((Guid?)null);
        // Act
        var result = await _service.AddPartnerAsync(partnerDto);

        // Assert
        Assert.True(result.IsNotSuccess());
        Assert.Equal(500, result.StatusCode);
        Assert.Equal(ErrorMessage.DbError, result.Message);
        _repositoryMock.Verify(r => r.DoesPartnerExistByOfficeAndNameAsync(It.IsAny<Partner>()), Times.Once);
        _userAdderMock.Verify(u => u.AddUserAsync(It.IsAny<UserDtoAdd>()), Times.Once);
    }

    [Fact]
    public async Task AddPartnerAsync_ShouldReturnSuccess_WhenPartnerAndUserAddedSuccessfully()
    {
        // Arrange
        var fakeUserId = Guid.NewGuid();
        var fakeOfficeId = Guid.NewGuid();
        var partnerDto = new PartnerDtoAdd { OfficeId=fakeOfficeId, PartnerName = "Success Partner" };

        _repositoryMock.Setup(r => r.DoesPartnerExistByOfficeAndNameAsync(It.IsAny<Partner>()))
                       .ReturnsAsync(false);
        _userAdderMock.Setup(u => u.AddUserAsync(It.IsAny<UserDtoAdd>()))
                      .ReturnsAsync(fakeUserId);
        // Act
        var result = await _service.AddPartnerAsync(partnerDto);

        // Assert
        Assert.False(result.IsNotSuccess());
        Assert.Equal(0, result.StatusCode);
        Assert.Null(result.Message);
        _repositoryMock.Verify(r => r.DoesPartnerExistByOfficeAndNameAsync(It.IsAny<Partner>()), Times.Once);
        _userAdderMock.Verify(u => u.AddUserAsync(It.IsAny<UserDtoAdd>()), Times.Once);
    }
}