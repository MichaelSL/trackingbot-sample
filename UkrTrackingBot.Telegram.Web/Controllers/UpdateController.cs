using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotanClient;
using BotanClient.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UkrTrackingBot.Bot.Common.Services;

namespace UkrTrackingBot.Telegram.Web.Controllers
{
    [Route("api/[controller]")]
    public class UpdateController : Controller
    {
        private readonly ITrackingBotService trackingBotService;
        private readonly ILogger logger;

        public UpdateController(ITrackingBotService trackingBotService, ILogger<UpdateController> logger)
        {
            this.trackingBotService = trackingBotService;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            if (update == null)
            {
                logger.LogWarning(new ArgumentNullException(nameof(update)), "Received empty update");
                return BadRequest();
            }

            var message = update.Message;
            if (message == null)
            {
                logger.LogWarning(new ArgumentNullException("Message"), "Received a malformed update: Message == null");
                return BadRequest();
            }
            switch (message.Type)
            {
                case MessageType.TextMessage:
                    try
                    {
                        await trackingBotService.ProcessTextMessage(message);
                        logger.LogTrace("Message processed: {message}", message);
                        return Ok();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Incoming Message: {message}", message);
                        throw;
                    }
                case MessageType.ServiceMessage:
                    logger.LogDebug($"Received service message: {JsonConvert.SerializeObject(message)}");
                    break;
                case MessageType.PhotoMessage:
                    logger.LogDebug("Received photo message: {message}", message);
                    break;
                default:
                    logger.LogInformation($"Received unsupported message type: {message.Type}");
                    break;
            }


            return BadRequest();
        }
    }
}
