using System;
using System.Collections.Generic;
using System.Text;

namespace UkrTrackingBot.ApiWrapper.DeliveryAuto.Domain.Models
{
    public class TrackWaybillApiRequest: LocalizableApiRequest
    {
        public string Number { get; set; }
    }
}
