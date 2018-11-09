using UkrTrackingBot.ApiWrapper.NovaPoshta.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UkrTrackingBot.ApiWrapper.NovaPoshta.Domain
{
    public interface IDataProvider
    {
        Task<ApiResponse> PostData(string url, ApiRequest request);
    }
}
