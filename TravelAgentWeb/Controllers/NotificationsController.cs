using Microsoft.AspNetCore.Mvc;
using TravelAgentWeb.Data;
using TravelAgentWeb.Dtos;

namespace TravelAgentWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly TravelAgentDbContext _context;

        public NotificationsController(TravelAgentDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public ActionResult FlightChanged(FlightDetailUpdateDto flightDetailUpdateDto)
        {
            Console.WriteLine($"Webhook Received! from {flightDetailUpdateDto.Publisher}");

            var secretModel = _context.WebhookSecrets.FirstOrDefault(s => s.Publisher == flightDetailUpdateDto.Publisher
            && s.Secret == flightDetailUpdateDto.Secret);

            if (secretModel is null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid Secret - Ingore Webhook");
                Console.ResetColor();
                return Ok();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Valid Webhook");
            Console.WriteLine($"Old Price {flightDetailUpdateDto.OldPrice}, New Price {flightDetailUpdateDto.NewPrice}");
            Console.ResetColor();
            return Ok();
        }
    }
}