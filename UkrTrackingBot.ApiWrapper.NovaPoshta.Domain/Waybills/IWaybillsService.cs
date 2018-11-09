using UkrTrackingBot.ApiWrapper.NovaPoshta.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UkrTrackingBot.ApiWrapper.NovaPoshta.Domain.Waybills
{
    public interface IWaybillsService
    {
        Task<ApiResponse> TrackWaybill(string documentNumber, string phone = null);
    }
}
