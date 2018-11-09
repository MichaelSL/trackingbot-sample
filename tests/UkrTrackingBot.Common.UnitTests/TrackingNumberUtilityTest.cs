using System;
using Xunit;

namespace UkrTrackingBot.Common.UnitTests
{
    public class TrackingNumberUtilityTest
    {
        #region BasicChecking
        [Theory]
        [InlineData("1234567890")]
        [InlineData("12345678901")]
        [InlineData("12345678901234")]
        public void TestRightNumbers(string number)
        {
            var carrier = TrackingNumberUtility.GetCarrierByTrackingNumber(number);
            Assert.NotNull(carrier);
        }

        [Theory]
        [InlineData("12345678901")]
        [InlineData("12345678901234")]
        public void TestNovaPoshtaNumbers(string number)
        {
            var carrier = TrackingNumberUtility.GetCarrierByTrackingNumber(number);
            Assert.NotNull(carrier);
        }

        [Theory]
        [InlineData("1234567890")]
        public void TestDeliveryNumbers(string number)
        {
            var carrier = TrackingNumberUtility.GetCarrierByTrackingNumber(number);
            Assert.NotNull(carrier);
        }
        #endregion

        #region Carrier
        [Theory]
        [InlineData("1234567890")]
        public void TestDeliverySpecificNumbers(string number)
        {
            var carrier = TrackingNumberUtility.GetCarrierByTrackingNumber(number);
            Assert.Equal(Carriers.Delivery, carrier);
        }

        [Theory]
        [InlineData("12345678901")]
        [InlineData("12345678901234")]
        public void TestNovaPoshtaSpecificNumbers(string number)
        {
            var carrier = TrackingNumberUtility.GetCarrierByTrackingNumber(number);
            Assert.Equal(Carriers.NovaPoshta, carrier);
        }
        #endregion

        [Theory]
        [InlineData("A123456789")]
        [InlineData("012345B789")]
        [InlineData("012345678C")]
        public void TestLetterInside(string number)
        {
            var carrier = TrackingNumberUtility.GetCarrierByTrackingNumber(number);
            Assert.Null(carrier);
        }
    }
}
