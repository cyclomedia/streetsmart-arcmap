using FluentAssertions;
using Moq;
using StreetSmart.Common.Interfaces.API;
using StreetSmart.Common.Interfaces.Data;
using StreetSmart.Common.Interfaces.DomElement;
using StreetSmartArcMap.Logic;
using Xunit;
namespace StreetSmartArcMap.Logic.Tests
{
    public class StreetSmartApiWrapper_Tests
    {
        [Fact]
        public void StreetSmartApi_Should_Initialize()
        {
            var sut = StreetSmartApiWrapper.Instance;
            var options = new StreetSmartOptions()
            {
                ApiKey = "1",
                Database = "2",
                EpsgCode = "3",
                Locale = "4",
                Password = "5",
                Username = "6"
            };
            var apiMock = new Mock<IStreetSmartAPI>();
            apiMock.Setup(a => a.Init(It.IsAny<IOptions>()));
            sut.InitApi(options, apiMock.Object);
        }
    }
}
