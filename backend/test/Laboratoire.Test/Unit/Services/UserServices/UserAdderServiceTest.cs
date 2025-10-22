using Laboratoire.Application.DTO;
using Laboratoire.Application.Services.UserServices;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Application.ServicesContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Unit.Services.UserServices;

public class UserAdderServiceTest
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IAuthRegistrationService> _authRegMock;
    private readonly Mock<ILogger<UserAdderService>> _loggerMock;
    private readonly UserAdderService _service;

    public UserAdderServiceTest()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _authRegMock = new Mock<IAuthRegistrationService>();
        _loggerMock = new Mock<ILogger<UserAdderService>>();
        _service = new UserAdderService(_userRepoMock.Object, _authRegMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task AddUserAsync_ShouldUserId_WhenUserAndPartnerInsertionSucceeds()
    {
        // Arrange
        var partner = new Partner() { PartnerId = Guid.NewGuid() };
        var dto = new UserDtoAdd { Username = "originalUser", RoleId = 4, Name = "Test", Client = null, Partner = partner };
        var userId = Guid.NewGuid();
        _userRepoMock.Setup(r => r.SetUserNameAsync(dto.Username))
                     .ReturnsAsync("modifiedUser01");
        _userRepoMock.Setup(r => r.AddUserAndPartnerAsync(It.IsAny<User>(), It.IsAny<Partner>()))
                     .ReturnsAsync(userId);
        _authRegMock.Setup(a => a.RegisterUserAsync(It.IsAny<UserRegistration>()))
                    .ReturnsAsync(Error.SetSuccess());

        // Act
        var result = await _service.AddUserAsync(dto);

        // Assert
        Assert.Equal(userId, result);
        _userRepoMock.Verify(r => r.SetUserNameAsync(It.IsAny<string?>()), Times.Once);
        _userRepoMock.Verify(r => r.AddUserAndPartnerAsync(It.IsAny<User>(), It.IsAny<Partner>()), Times.Once);
        _userRepoMock.Verify(r => r.AddUserAndClientAsync(It.IsAny<User>(), It.IsAny<Client>()), Times.Never);
        _userRepoMock.Verify(r => r.AddUserAndEmployeeAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        _authRegMock.Verify(a => a.RegisterUserAsync(It.IsAny<UserRegistration>()), Times.Once);
    }
    [Fact]
    public async Task AddUserAsync_ShouldReturnNull_WhenUserAndPartnerInsertionFails()
    {
        // Arrange
        var partner = new Partner() { PartnerId = Guid.NewGuid() };
        var dto = new UserDtoAdd { Username = "originalUser", RoleId = 4, Name = "Test", Client = null, Partner = partner };
        var userId = Guid.NewGuid();
        _userRepoMock.Setup(r => r.SetUserNameAsync(dto.Username))
                     .ReturnsAsync("modifiedUser01");
        _userRepoMock.Setup(r => r.AddUserAndPartnerAsync(It.IsAny<User>(), It.IsAny<Partner>()))
                     .ReturnsAsync((Guid?)null);

        // Act
        var result = await _service.AddUserAsync(dto);

        // Assert
        Assert.Null(result);
        _userRepoMock.Verify(r => r.SetUserNameAsync(It.IsAny<string?>()), Times.Once);
        _userRepoMock.Verify(r => r.AddUserAndPartnerAsync(It.IsAny<User>(), It.IsAny<Partner>()), Times.Once);
        _userRepoMock.Verify(r => r.AddUserAndClientAsync(It.IsAny<User>(), It.IsAny<Client>()), Times.Never);
        _userRepoMock.Verify(r => r.AddUserAndEmployeeAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        _authRegMock.Verify(a => a.RegisterUserAsync(It.IsAny<UserRegistration>()), Times.Never);
    }

    [Fact]
    public async Task AddUserAsync_ShouldReturnUserId_WhenUserAndClientInsertionSucceeds()
    {
        // Arrange
        var client = new Client() { ClientId = Guid.NewGuid() };
        var dto = new UserDtoAdd { Username = "testUser", RoleId = 5, Name = "Test", Client = client, Partner = null };
        var userId = Guid.NewGuid();
        _userRepoMock.Setup(r => r.AddUserAndClientAsync(It.IsAny<User>(), It.IsAny<Client>()))
                     .ReturnsAsync(userId);
        _authRegMock.Setup(a => a.RegisterUserAsync(It.IsAny<UserRegistration>()))
                    .ReturnsAsync(Error.SetSuccess());

        // Act
        var result = await _service.AddUserAsync(dto);

        // Assert
        Assert.Equal(userId, result);
        _userRepoMock.Verify(r => r.SetUserNameAsync(It.IsAny<string?>()), Times.Never);
        _userRepoMock.Verify(r => r.AddUserAndPartnerAsync(It.IsAny<User>(), It.IsAny<Partner>()), Times.Never);
        _userRepoMock.Verify(r => r.AddUserAndClientAsync(It.IsAny<User>(), It.IsAny<Client>()), Times.Once);
        _userRepoMock.Verify(r => r.AddUserAndEmployeeAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        _authRegMock.Verify(a => a.RegisterUserAsync(It.IsAny<UserRegistration>()), Times.Once);
    }

    [Fact]
    public async Task AddUserAsync_ShouldReturnNull_WhenUserAndClientInsertionFails()
    {
        // Arrange
        var client = new Client() { ClientId = Guid.NewGuid() };
        var dto = new UserDtoAdd { Username = "testUser", RoleId = 5, Name = "Test", Client = client, Partner = null };
        _userRepoMock.Setup(r => r.AddUserAndClientAsync(It.IsAny<User>(), It.IsAny<Client>()))
                     .ReturnsAsync((Guid?)null);
        // Act
        var result = await _service.AddUserAsync(dto);

        // Assert
        Assert.Null(result);

        _userRepoMock.Verify(r => r.SetUserNameAsync(It.IsAny<string?>()), Times.Never);
        _userRepoMock.Verify(r => r.AddUserAndPartnerAsync(It.IsAny<User>(), It.IsAny<Partner>()), Times.Never);
        _userRepoMock.Verify(r => r.AddUserAndClientAsync(It.IsAny<User>(), It.IsAny<Client>()), Times.Once);
        _userRepoMock.Verify(r => r.AddUserAndEmployeeAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        _authRegMock.Verify(a => a.RegisterUserAsync(It.IsAny<UserRegistration>()), Times.Never);
    }

    [Fact]
    public async Task AddUserAsync_ShouldReturnUserId_WhenUserInsertionSucceeds()
    {
        // Arrange
        var dto = new UserDtoAdd { Username = "testUser", RoleId = 1, Name = "Test", Client = null, Partner = null };
        var userId = Guid.NewGuid();
        _userRepoMock.Setup(r => r.AddUserAndEmployeeAsync(It.IsAny<User>(), It.IsAny<string>()))
                     .ReturnsAsync(userId);
        _authRegMock.Setup(a => a.RegisterUserAsync(It.IsAny<UserRegistration>()))
                    .ReturnsAsync(Error.SetSuccess());

        // Act
        var result = await _service.AddUserAsync(dto);

        // Assert
        Assert.Equal(userId, result);
        _userRepoMock.Verify(r => r.SetUserNameAsync(It.IsAny<string?>()), Times.Never);
        _userRepoMock.Verify(r => r.AddUserAndPartnerAsync(It.IsAny<User>(), It.IsAny<Partner>()), Times.Never);
        _userRepoMock.Verify(r => r.AddUserAndClientAsync(It.IsAny<User>(), It.IsAny<Client>()), Times.Never);
        _userRepoMock.Verify(r => r.AddUserAndEmployeeAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        _authRegMock.Verify(a => a.RegisterUserAsync(It.IsAny<UserRegistration>()), Times.Once);
    }

    [Fact]
    public async Task AddUserAsync_ShouldReturnNull_WhenUserInsertionFails()
    {
        // Arrange
        var dto = new UserDtoAdd { Username = "testUser", RoleId = 1, Name = "Test", Client = null, Partner = null };
        _userRepoMock.Setup(r => r.AddUserAndEmployeeAsync(It.IsAny<User>(),It.IsAny<string>()))
                     .ReturnsAsync((Guid?)null);
        // Act
        var result = await _service.AddUserAsync(dto);

        // Assert
        Assert.Null(result);
        _userRepoMock.Verify(r => r.SetUserNameAsync(It.IsAny<string?>()), Times.Never);
        _userRepoMock.Verify(r => r.AddUserAndPartnerAsync(It.IsAny<User>(), It.IsAny<Partner>()), Times.Never);
        _userRepoMock.Verify(r => r.AddUserAndClientAsync(It.IsAny<User>(), It.IsAny<Client>()), Times.Never);
        _userRepoMock.Verify(r => r.AddUserAndEmployeeAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        _authRegMock.Verify(a => a.RegisterUserAsync(It.IsAny<UserRegistration>()), Times.Never);
    }
}