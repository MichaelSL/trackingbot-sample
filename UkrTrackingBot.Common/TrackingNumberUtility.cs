using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace UkrTrackingBot.Common
{
    public sealed class TrackingNumberUtility
    {
        static readonly Regex numericStringRegex = new Regex("\\D", RegexOptions.Compiled);

        public static Carriers? GetCarrierByTrackingNumber(string trackingNumber)
        {
            if (!numericStringRegex.IsMatch(trackingNumber))
            {
                switch (trackingNumber.Length)
                {
                    case 11:
                    case 14:
                        return Carriers.NovaPoshta;
                    case 10:
                        return Carriers.Delivery;
                }
            }

            return null;
        }
    }
}
