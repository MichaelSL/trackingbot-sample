using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UkrTrackingBot.ApiWrapper.DeliveryAuto.Domain.Models;

namespace UkrTrackingBot.ApiWrapper.DeliveryAuto.Domain.Waybills
{
    public interface IWaybillsService
    {
        Task<ApiResponse> TrackWaybill(TrackWaybillApiRequest request);
    }
}
