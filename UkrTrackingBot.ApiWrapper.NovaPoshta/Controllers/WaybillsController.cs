using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UkrTrackingBot.ApiWrapper.NovaPoshta.Domain.Waybills;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UkrTrackingBot.ApiWrapper.NovaPoshta.Controllers
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
        public async Task<IActionResult> Track(string number, string phone)
        {
            try
            {
                var data = await waybillsService.TrackWaybill(number, phone);
                return Ok(data);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Can't call Nova poshta API: {0}, {1}", number, phone);
                throw ex;
            }
        }
    }
}
