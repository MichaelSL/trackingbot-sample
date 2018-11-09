using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UkrTrackingBot.ApiWrapper.DeliveryAuto.Domain.Waybills;

namespace UkrTrackingBot.ApiWrapper.DeliveryAuto.Controllers
{
    [Route("api/[controller]")]
    public class WaybillsController : Controller
    {
        private readonly IWaybillsService waybillsService;
        private readonly ILogger log;

        public WaybillsController(IWaybillsService waybillsService, ILogger<WaybillsController> log)
        {
            this.waybillsService = waybillsService;
            this.log = log;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Track(string number, string culture)
        {
            try
            {
                var data = await waybillsService.TrackWaybill(new Domain.Models.TrackWaybillApiRequest
                {
                    Number = number,
                    Culture = culture
                });
                return Ok(data);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Can't call Delivery API: {0}, {1}", number, culture);
                throw ex;
            }
        }
    }
}
