using AutoFixture;
using Azure;
using Azure.Data.Tables;
using DisneyParkInsights;
using DisneyParkInsights.TableEntities;
using DisneyWorldWaitTracker.Data;
using FluentAssertions;
using FluentAssertions.ArgumentMatchers.Moq;
using FluentAssertions.Equivalency;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests
{
    public class AttractionInfoAzureTableStorageServiceTests
    {
        private AttractionInfoAzureTableStorageService _uut;
        private Mock<ITableClientFactory> _mockTableClientFactory;
        private Fixture _fixture;
        private Mock<TableClient> _mockTableClient;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _mockTableClientFactory = new Mock<ITableClientFactory>();
            _mockTableClient = new Mock<TableClient>();

            _uut = new AttractionInfoAzureTableStorageService(_mockTableClientFactory.Object, Mock.Of<ILogger<AttractionInfoAzureTableStorageService>>());
        }

        [Test]
        public async Task StoreAttractionInfo_WhenValidData_ThenStoresInfo()
        {
            // Arrange
            var parkConfig = new ParkConfig("Disneyland", TimeZoneInfo.Utc);
            var attractionData = _fixture.Create<AttractionData>();

            var expectedAttractionInfoEntity = attractionData.ToAttractionInfoEntity();
            var expectedAttractionWaitTimeEntity = attractionData.ToAttractionWaitTimeEntity(parkConfig);

            _mockTableClient.Setup(x => x.GetEntityIfExistsAsync<AttractionInfoEntity>(attractionData.Id, attractionData.Id, null, default))
                            .ReturnsAsync(Mock.Of<NullableResponse<AttractionInfoEntity>>());
            _mockTableClient.Setup(x => x.UpsertEntityAsync(It.IsAny<AttractionInfoEntity>(), TableUpdateMode.Merge, default))
                            .ReturnsAsync(Mock.Of<Response>());
            _mockTableClient.Setup(x => x.UpsertEntityAsync(It.IsAny<AttractionWaitTimeEntity>(), TableUpdateMode.Merge, default))
                            .ReturnsAsync(Mock.Of<Response>());

            _mockTableClientFactory.Setup(x => x.GetCloudTable(It.IsAny<string>()))
                                   .ReturnsAsync(_mockTableClient.Object);

            // Act
            var action = () => _uut.StoreAttractionInfo(parkConfig, attractionData);

            // Assert
            var e = new EquivalencyAssertionOptions<AttractionWaitTimeEntity>();

            await action.Should().NotThrowAsync();
            _mockTableClient.Verify(x => x.UpsertEntityAsync(Its.EquivalentTo(expectedAttractionInfoEntity), TableUpdateMode.Merge, default));
            _mockTableClient.Verify(x => x.UpsertEntityAsync(Its.EquivalentTo(expectedAttractionWaitTimeEntity, e => e.Excluding(x => x.RetrievalTime).Excluding(x => x.RowKey)), TableUpdateMode.Merge, default));
        }

        [Test]
        public async Task StoreAttractionInfo_WhenParkConfigNull_ThrowNullArgumentException()
        {
            // Arrange
            var attractionData = _fixture.Create<AttractionData>();
            ParkConfig parkConfig;

            // Act
            var action = () => _uut.StoreAttractionInfo(parkConfig, attractionData);

            // Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        public async Task StoreAttractionInfo_WhenAttractionDataNull_ThrowNullArgumentException()
        {
            // Arrange
            AttractionData? attractionData = null;
            var parkConfig = new ParkConfig("Disneyland", TimeZoneInfo.Utc);

            // Act
            var action = () => _uut.StoreAttractionInfo(parkConfig, attractionData);

            // Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }
    }
}