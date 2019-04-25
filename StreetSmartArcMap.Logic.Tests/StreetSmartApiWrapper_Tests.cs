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
            var options = new Mock<IStreetSmartOptions>();
            options.SetupGet(o => o.ApiKey).Returns("1");

            options.SetupGet(o => o.AddressDatabase).Returns("2");
            options.SetupGet(o => o.ApiSRS).Returns("3");
            options.SetupGet(o => o.AddressLocale).Returns("4");
            options.SetupGet(o => o.ApiPassword).Returns("5");
            options.SetupGet(o => o.ApiUsername).Returns("6");

            var apiMock = new Mock<IStreetSmartAPI>();
            apiMock.Setup(a => a.Init(It.IsAny<IOptions>()));
            sut.InitApi(options.Object, apiMock.Object);
        }
    }
}
