using Laboratoire.Application.DTO;
using Laboratoire.Application.Services.ClientServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Application.ServicesContracts;
using Microsoft.Extensions.Logging;
using Moq;
using Laboratoire.Application.Utils;

namespace Laboratoire.Test.Services.ClientServices;

public class ClientAdderServiceTest
{
    private readonly Mock<IClientRepository> _clientRepositoryMock = new();
    private readonly Mock<IUserAdderService> _userAdderServiceMock = new();
    private readonly Mock<IUserDeletionService> _userDeletionServiceMock = new();
    private readonly Mock<ILogger<ClientAdderService>> _loggerMock = new();
    private readonly ClientAdderService _service;

    public ClientAdderServiceTest()
    {
        _service = new ClientAdderService(
            _clientRepositoryMock.Object,
            _userAdderServiceMock.Object,
            _userDeletionServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task AddClientAsync_ShouldReturnConflict_WhenClientAlreadyExists()
    {
        // Arrange
        var dto = new ClientDtoAdd { ClientTaxId = "123" };
        _clientRepositoryMock.Setup(r => r.DoesClientExistByTaxIdAsync(It.IsAny<Client>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.AddClientAsync(dto);

        // Assert
        Assert.True(result.IsNotSuccess());
        Assert.Equal(409, result.StatusCode);
        Assert.Equal(ErrorMessage.ConflictPost, result.Message);
        _clientRepositoryMock.Verify(r => r.DoesClientExistByTaxIdAsync(It.IsAny<Client>()), Times.Once);
        _userAdderServiceMock.Verify(r => r.AddUserAsync(It.IsAny<UserDtoAdd>()), Times.Never);
        _clientRepositoryMock.Verify(r => r.AddClientAsync(It.IsAny<Client>(), It.IsAny<Guid?>()), Times.Never);
        _userDeletionServiceMock.Verify(r => r.DeletionUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task AddClientAsync_ShouldReturnNotFound_WhenUserCreationFails()
    {
        // Arrange
        var dto = new ClientDtoAdd { ClientTaxId = "123" };
        _clientRepositoryMock.Setup(r => r.DoesClientExistByTaxIdAsync(It.IsAny<Client>()))
            .ReturnsAsync(false);

        _userAdderServiceMock.Setup(u => u.AddUserAsync(It.IsAny<UserDtoAdd>())).ReturnsAsync((Guid?)null);


        // Act
        var result = await _service.AddClientAsync(dto);

        // Assert
        Assert.True(result.IsNotSuccess());
        Assert.Equal(500, result.StatusCode);
        Assert.Equal(ErrorMessage.DbError, result.Message);
        _clientRepositoryMock.Verify(r => r.DoesClientExistByTaxIdAsync(It.IsAny<Client>()), Times.Once);
        _userAdderServiceMock.Verify(r => r.AddUserAsync(It.IsAny<UserDtoAdd>()), Times.Once);
        _clientRepositoryMock.Verify(r => r.AddClientAsync(It.IsAny<Client>(), It.IsAny<Guid?>()), Times.Never);
        _userDeletionServiceMock.Verify(r => r.DeletionUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task AddClientAsync_ShouldReturnDbError_WhenClientInsertFails()
    {
        // Arrange
        var dto = new ClientDtoAdd { ClientTaxId = "123" };
        _clientRepositoryMock.Setup(r => r.DoesClientExistByTaxIdAsync(It.IsAny<Client>()))
            .ReturnsAsync(false);

        _userAdderServiceMock.Setup(u => u.AddUserAsync(It.IsAny<UserDtoAdd>()))
            .ReturnsAsync(It.IsAny<Guid>());

        _clientRepositoryMock.Setup(r => r.AddClientAsync(It.IsAny<Client>(), It.IsAny<Guid>()))
            .ReturnsAsync(false);

        // Act
        var result = await _service.AddClientAsync(dto);

        // Assert
        Assert.True(result.IsNotSuccess());
        Assert.Equal(500, result.StatusCode);
        Assert.Equal(ErrorMessage.DbError, result.Message);
        _clientRepositoryMock.Verify(r => r.DoesClientExistByTaxIdAsync(It.IsAny<Client>()), Times.Once);
        _userAdderServiceMock.Verify(r => r.AddUserAsync(It.IsAny<UserDtoAdd>()), Times.Once);
        _clientRepositoryMock.Verify(r => r.AddClientAsync(It.IsAny<Client>(), It.IsAny<Guid?>()), Times.Once);
        _userDeletionServiceMock.Verify(r => r.DeletionUserAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task AddClientAsync_ShouldReturnSuccess_WhenAllStepsSucceed()
    {
        // Arrange
        var dto = new ClientDtoAdd { ClientTaxId = "123" };
        _clientRepositoryMock.Setup(r => r.DoesClientExistByTaxIdAsync(It.IsAny<Client>()))
            .ReturnsAsync(false);

        _userAdderServiceMock.Setup(u => u.AddUserAsync(It.IsAny<UserDtoAdd>()))
            .ReturnsAsync(It.IsAny<Guid>());

        _clientRepositoryMock.Setup(r => r.AddClientAsync(It.IsAny<Client>(), It.IsAny<Guid>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.AddClientAsync(dto);

        // Assert
        Assert.Equal(0, result.StatusCode);
        Assert.False(result.IsNotSuccess());
        Assert.Null(result.Message);
        _clientRepositoryMock.Verify(r => r.DoesClientExistByTaxIdAsync(It.IsAny<Client>()), Times.Once);
        _userAdderServiceMock.Verify(r => r.AddUserAsync(It.IsAny<UserDtoAdd>()), Times.Once);
        _clientRepositoryMock.Verify(r => r.AddClientAsync(It.IsAny<Client>(), It.IsAny<Guid?>()), Times.Once);
        _userDeletionServiceMock.Verify(r => r.DeletionUserAsync(It.IsAny<User>()), Times.Never);
    }
}
