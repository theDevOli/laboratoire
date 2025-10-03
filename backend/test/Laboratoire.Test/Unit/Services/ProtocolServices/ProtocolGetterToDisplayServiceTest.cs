using Laboratoire.Application.DTO;
using Laboratoire.Application.Mapper;
using Laboratoire.Application.Services.ProtocolServices;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.ProtocolServices
{
    public class ProtocolGetterToDisplayServiceTest
    {
        private readonly Mock<IProtocolRepository> _protocolRepoMock;
        private readonly Mock<ICropsNormalizationGetterService> _cropsNormalizationMock;
        private readonly Mock<ICropGetterService> _cropGetterMock;
        private readonly Mock<ILogger<ProtocolGetterToDisplayService>> _loggerMock;
        private readonly ProtocolGetterToDisplayService _service;

        public ProtocolGetterToDisplayServiceTest()
        {
            _protocolRepoMock = new Mock<IProtocolRepository>();
            _cropsNormalizationMock = new Mock<ICropsNormalizationGetterService>();
            _cropGetterMock = new Mock<ICropGetterService>();
            _loggerMock = new Mock<ILogger<ProtocolGetterToDisplayService>>();

            _service = new ProtocolGetterToDisplayService(
                _protocolRepoMock.Object,
                _cropsNormalizationMock.Object,
                _cropGetterMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetDisplayProtocolsAsync_ShouldReturnAll_WhenNoFilter()
        {
            // Arrange
            int year = 2025;
            Guid? id = null;
            bool? isPartner = null;

            var protocols = new List<ProtocolDtoDisplayDb> { new ProtocolDtoDisplayDb { ProtocolId = "0001/2025" } };
            var cropsNormalizations = new List<CropsNormalization>()
            {
                new CropsNormalization { CropId=1,ProtocolId="0001/2025"},
                new CropsNormalization { CropId=2,ProtocolId="0001/2025"},
            };
            var crops = new List<Crop>()
            {
                new Crop { CropId=1},
                new Crop { CropId=2},
            };

            _protocolRepoMock.Setup(r => r.GetDisplayProtocolsAsync<ProtocolDtoDisplayDb>(year, true))
                             .ReturnsAsync(protocols);
            _cropsNormalizationMock.Setup(c => c.GetAllCropsAsync())
                                   .ReturnsAsync(cropsNormalizations);
            _cropGetterMock.Setup(c => c.GetAllCropsAsync())
                           .ReturnsAsync(crops);

            // Act
            var result = await _service.GetDisplayProtocolsAsync(year, id, isPartner);

            // Assert
            Assert.NotNull(result);
            Assert.Collection(result, item => Assert.Equal(item.ProtocolId, protocols[0].ProtocolId));
            _protocolRepoMock.Verify(r => r.GetDisplayProtocolsAsync<ProtocolDtoDisplayDb>(year, true), Times.Once);
            _cropsNormalizationMock.Verify(c => c.GetAllCropsAsync(), Times.Once);
            _cropGetterMock.Verify(c => c.GetAllCropsAsync(), Times.Once);

        }

        [Fact]
        public async Task GetDisplayProtocolsAsync_ShouldFilterByPartnerId_WhenIsPartnerTrue()
        {
            // Arrange
            int year = 2025;
            Guid partnerId = Guid.NewGuid();
            bool? isPartner = true;

            var protocols = new List<ProtocolDtoDisplayDb>
            {
                new ProtocolDtoDisplayDb { ProtocolId = "0001/2025" ,PartnerId=partnerId},
                new ProtocolDtoDisplayDb { ProtocolId = "0002/2025" ,PartnerId=partnerId},
                new ProtocolDtoDisplayDb { ProtocolId = "0003/2025" ,PartnerId=partnerId},
                new ProtocolDtoDisplayDb { ProtocolId = "0004/2025" ,PartnerId=null},
                new ProtocolDtoDisplayDb { ProtocolId = "0005/2025" ,PartnerId=null},
            };
            var cropsNormalizations = new List<CropsNormalization>()
            {
                new CropsNormalization { CropId=1,ProtocolId="0001/2025"},
                new CropsNormalization { CropId=2,ProtocolId="0001/2025"},
                new CropsNormalization { CropId=2,ProtocolId="0002/2025"},
                new CropsNormalization { CropId=2,ProtocolId="0003/2025"},
                new CropsNormalization { CropId=2,ProtocolId="0004/2025"},
                new CropsNormalization { CropId=2,ProtocolId="0005/2025"},
            };
            var crops = new List<Crop>()
            {
                new Crop { CropId=1},
                new Crop { CropId=2},
            };

            _protocolRepoMock.Setup(r => r.GetDisplayProtocolsAsync<ProtocolDtoDisplayDb>(year, false))
                             .ReturnsAsync(protocols);
            _cropsNormalizationMock.Setup(c => c.GetAllCropsAsync())
                                   .ReturnsAsync(cropsNormalizations);
            _cropGetterMock.Setup(c => c.GetAllCropsAsync())
                           .ReturnsAsync(crops);


            // Act
            var result = await _service.GetDisplayProtocolsAsync(year, partnerId, isPartner);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            Assert.Collection
            (
                result,
                item => Assert.Equal(item.ProtocolId, protocols[0].ProtocolId),
                item => Assert.Equal(item.ProtocolId, protocols[1].ProtocolId),
                item => Assert.Equal(item.ProtocolId, protocols[2].ProtocolId)
            );
            _protocolRepoMock.Verify(r => r.GetDisplayProtocolsAsync<ProtocolDtoDisplayDb>(It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
            _cropsNormalizationMock.Verify(c => c.GetAllCropsAsync(), Times.Once);
            _cropGetterMock.Verify(c => c.GetAllCropsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetDisplayProtocolsAsync_ShouldFilterByClientId_WhenIsPartnerFalse()
        {
            // Arrange
            int year = 2025;
            Guid clientId = Guid.NewGuid();
            bool? isPartner = false;

            var protocols = new List<ProtocolDtoDisplayDb>
            {
                new ProtocolDtoDisplayDb { ProtocolId = "0001/2025" ,ClientId=clientId},
                new ProtocolDtoDisplayDb { ProtocolId = "0002/2025" ,ClientId=clientId},
                new ProtocolDtoDisplayDb { ProtocolId = "0003/2025" ,ClientId=clientId},
                new ProtocolDtoDisplayDb { ProtocolId = "0004/2025" ,ClientId=Guid.NewGuid()},
                new ProtocolDtoDisplayDb { ProtocolId = "0005/2025" ,ClientId=Guid.NewGuid()},
            };
            var cropsNormalizations = new List<CropsNormalization>()
            {
                new CropsNormalization { CropId=1,ProtocolId="0001/2025"},
                new CropsNormalization { CropId=2,ProtocolId="0001/2025"},
                new CropsNormalization { CropId=2,ProtocolId="0002/2025"},
                new CropsNormalization { CropId=2,ProtocolId="0003/2025"},
                new CropsNormalization { CropId=2,ProtocolId="0004/2025"},
                new CropsNormalization { CropId=2,ProtocolId="0005/2025"},
            };
            var crops = new List<Crop>()
            {
                new Crop { CropId=1},
                new Crop { CropId=2},
            };

            _protocolRepoMock.Setup(r => r.GetDisplayProtocolsAsync<ProtocolDtoDisplayDb>(year, true))
                             .ReturnsAsync(protocols);
            _cropsNormalizationMock.Setup(c => c.GetAllCropsAsync())
                                   .ReturnsAsync(cropsNormalizations);
            _cropGetterMock.Setup(c => c.GetAllCropsAsync())
                           .ReturnsAsync(crops);

            // Act
            var result = await _service.GetDisplayProtocolsAsync(year, clientId, isPartner);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            Assert.Collection
            (
                result,
                item => Assert.Equal(item.ProtocolId, protocols[0].ProtocolId),
                item => Assert.Equal(item.ProtocolId, protocols[1].ProtocolId),
                item => Assert.Equal(item.ProtocolId, protocols[2].ProtocolId)
            );
            _protocolRepoMock.Verify(r => r.GetDisplayProtocolsAsync<ProtocolDtoDisplayDb>(It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
            _cropsNormalizationMock.Verify(c => c.GetAllCropsAsync(), Times.Once);
            _cropGetterMock.Verify(c => c.GetAllCropsAsync(), Times.Once);
        }
    }
}
