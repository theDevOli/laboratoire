using Laboratoire.Application.DTO;
using Laboratoire.Application.Services.ClientServices;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.ClientServices
{
    public class ClientUpdatableServiceTest
    {
        private readonly Mock<IClientRepository> _clientRepositoryMock;
        private readonly Mock<IUserGetterByUsernameService> _userGetterMock;
        private readonly Mock<IUserRenameService> _userRenameMock;
        private readonly Mock<ILogger<ClientUpdatableService>> _loggerMock;
        private readonly ClientUpdatableService _service;

        public ClientUpdatableServiceTest()
        {
            _clientRepositoryMock = new Mock<IClientRepository>();
            _userGetterMock = new Mock<IUserGetterByUsernameService>();
            _userRenameMock = new Mock<IUserRenameService>();
            _loggerMock = new Mock<ILogger<ClientUpdatableService>>();
            _service = new ClientUpdatableService(
                _clientRepositoryMock.Object,
                _userGetterMock.Object,
                _userRenameMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task UpdateClientAsync_ShouldReturnNotFound_WhenClientDoesNotExist()
        {
            var client = new Client { ClientId = Guid.NewGuid(), ClientTaxId = "123" };

            _clientRepositoryMock.Setup(r => r.GetByClientIdAsync(client.ClientId))
                                 .ReturnsAsync((Client?)null);

            var result = await _service.UpdateClientAsync(client);

            Assert.True(result.IsNotSuccess());
            Assert.Equal(404, result.StatusCode);
            Assert.Equal(ErrorMessage.NotFound, result.Message);
            _clientRepositoryMock.Verify(r => r.GetByClientIdAsync(client.ClientId), Times.Once);
            _userGetterMock.Verify(r => r.GetUserByUsernameAsync(It.IsAny<string>()), Times.Never);
            _userRenameMock.Verify(r => r.UserRenameAsync(It.IsAny<UserDtoRename>()), Times.Never);
            _clientRepositoryMock.Verify(r => r.UpdateClientAsync(It.IsAny<Client>()), Times.Never);
        }

        [Fact]
        public async Task UpdateClientAsync_ShouldReturnDbError_WhenTaxIdChangedAndUserNameIsNotChanged()
        {
            var client = new Client { ClientId = Guid.NewGuid(), ClientTaxId = "456" };
            var clientDb = new Client { ClientId = client.ClientId, ClientTaxId = "123" };
            var user = new Domain.Entity.User { Username = "123" };

            _clientRepositoryMock.Setup(r => r.GetByClientIdAsync(client.ClientId))
                                 .ReturnsAsync(clientDb);
            _userGetterMock.Setup(s => s.GetUserByUsernameAsync(clientDb.ClientTaxId))
                           .ReturnsAsync(user);
            _userRenameMock.Setup(s => s.UserRenameAsync(It.IsAny<UserDtoRename>()))
                           .ReturnsAsync(Error.SetError(ErrorMessage.DbError, 500));
            _clientRepositoryMock.Setup(r => r.UpdateClientAsync(client))
                                 .ReturnsAsync(true);

            var result = await _service.UpdateClientAsync(client);

            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            Assert.Equal(ErrorMessage.DbError, result.Message);
            _clientRepositoryMock.Verify(r => r.GetByClientIdAsync(client.ClientId), Times.Once);
            _userGetterMock.Verify(r => r.GetUserByUsernameAsync(It.IsAny<string>()), Times.Once);
            _userRenameMock.Verify(r => r.UserRenameAsync(It.IsAny<UserDtoRename>()), Times.Once);
            _clientRepositoryMock.Verify(r => r.UpdateClientAsync(It.IsAny<Client>()), Times.Never);
        }


        [Fact]
        public async Task UpdateClientAsync_ShouldUpdateClient_WhenTaxIdNotChanged()
        {
            var client = new Client { ClientId = Guid.NewGuid(), ClientTaxId = "123" };
            var clientDb = new Client { ClientId = client.ClientId, ClientTaxId = "123" };

            _clientRepositoryMock.Setup(r => r.GetByClientIdAsync(client.ClientId))
                                 .ReturnsAsync(clientDb);
            _clientRepositoryMock.Setup(r => r.UpdateClientAsync(client))
                                 .ReturnsAsync(true);

            var result = await _service.UpdateClientAsync(client);

            Assert.False(result.IsNotSuccess());
            Assert.Equal(0, result.StatusCode);
            Assert.Null(result.Message);
            _clientRepositoryMock.Verify(r => r.GetByClientIdAsync(client.ClientId), Times.Once);
            _userGetterMock.Verify(r => r.GetUserByUsernameAsync(It.IsAny<string>()), Times.Never);
            _userRenameMock.Verify(r => r.UserRenameAsync(It.IsAny<UserDtoRename>()), Times.Never);
            _clientRepositoryMock.Verify(r => r.UpdateClientAsync(It.IsAny<Client>()), Times.Once);
        }

        [Fact]
        public async Task UpdateClientAsync_ShouldRenameUserAndUpdateClient_WhenTaxIdChanged()
        {
            var client = new Client { ClientId = Guid.NewGuid(), ClientTaxId = "456" };
            var clientDb = new Client { ClientId = client.ClientId, ClientTaxId = "123" };
            var user = new User { Username = "123" };

            _clientRepositoryMock.Setup(r => r.GetByClientIdAsync(client.ClientId))
                                 .ReturnsAsync(clientDb);
            _userGetterMock.Setup(s => s.GetUserByUsernameAsync(clientDb.ClientTaxId))
                           .ReturnsAsync(user);
            _userRenameMock.Setup(s => s.UserRenameAsync(It.IsAny<UserDtoRename>()))
                           .ReturnsAsync(Error.SetSuccess());
            _clientRepositoryMock.Setup(r => r.UpdateClientAsync(client))
                                 .ReturnsAsync(true);

            var result = await _service.UpdateClientAsync(client);

            Assert.False(result.IsNotSuccess());
            Assert.Equal(0, result.StatusCode);
            Assert.Null(result.Message);
            _clientRepositoryMock.Verify(r => r.GetByClientIdAsync(client.ClientId), Times.Once);
            _userGetterMock.Verify(r => r.GetUserByUsernameAsync(It.IsAny<string>()), Times.Once);
            _userRenameMock.Verify(r => r.UserRenameAsync(It.IsAny<UserDtoRename>()), Times.Once);
            _clientRepositoryMock.Verify(r => r.UpdateClientAsync(It.IsAny<Client>()), Times.Once);
        }

        [Fact]
        public async Task UpdateClientAsync_ShouldReturnDbError_WhenUpdateFailsTaxIdChanged()
        {
            var client = new Client { ClientId = Guid.NewGuid(), ClientTaxId = "123" };
            var clientDb = new Client { ClientId = client.ClientId, ClientTaxId = "124" };
            var user = new User { Username = "123" };

            _clientRepositoryMock.Setup(r => r.GetByClientIdAsync(client.ClientId))
                                 .ReturnsAsync(clientDb);
            _userGetterMock.Setup(s => s.GetUserByUsernameAsync(clientDb.ClientTaxId))
                                .ReturnsAsync(user);
            _userRenameMock.Setup(s => s.UserRenameAsync(It.IsAny<UserDtoRename>()))
                                .ReturnsAsync(Error.SetSuccess());
            _clientRepositoryMock.Setup(r => r.UpdateClientAsync(client))
                                 .ReturnsAsync(false);

            var result = await _service.UpdateClientAsync(client);

            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            _clientRepositoryMock.Verify(r => r.GetByClientIdAsync(client.ClientId), Times.Once);
            _userGetterMock.Verify(r => r.GetUserByUsernameAsync(It.IsAny<string>()), Times.Once);
            _userRenameMock.Verify(r => r.UserRenameAsync(It.IsAny<UserDtoRename>()), Times.Once);
            _clientRepositoryMock.Verify(r => r.UpdateClientAsync(It.IsAny<Client>()), Times.Once);
        }

        [Fact]
        public async Task UpdateClientAsync_ShouldReturnDbError_WhenUpdateFailsTaxIdNotChanged()
        {
            var client = new Client { ClientId = Guid.NewGuid(), ClientTaxId = "123" };
            var clientDb = new Client { ClientId = client.ClientId, ClientTaxId = "123" };

            _clientRepositoryMock.Setup(r => r.GetByClientIdAsync(client.ClientId))
                                 .ReturnsAsync(clientDb);
            _clientRepositoryMock.Setup(r => r.UpdateClientAsync(client))
                                 .ReturnsAsync(false);

            var result = await _service.UpdateClientAsync(client);

            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            _clientRepositoryMock.Verify(r => r.GetByClientIdAsync(client.ClientId), Times.Once);
            _userGetterMock.Verify(r => r.GetUserByUsernameAsync(It.IsAny<string>()), Times.Never);
            _userRenameMock.Verify(r => r.UserRenameAsync(It.IsAny<UserDtoRename>()), Times.Never);
            _clientRepositoryMock.Verify(r => r.UpdateClientAsync(It.IsAny<Client>()), Times.Once);
        }
    }
}
