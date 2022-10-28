using AirlineWeb.Data;
using AirlineWeb.Dtos;
using AirlineWeb.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AirlineWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookSubscriptionController : ControllerBase
    {
        private readonly AirlineDbContext _context;
        private readonly IMapper _mapper;

        public WebhookSubscriptionController(AirlineDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<List<WebhookSubscriptionReadDto>> GetAllWebhookSubscriptions()
        {
            var subscriptions = _context.WebhookSubscriptions.ToList();
            return Ok(_mapper.Map<List<WebhookSubscriptionReadDto>>(subscriptions));
        }

        [HttpGet("{secret}", Name = "GetSubscriptionBySecret")]
        public ActionResult<WebhookSubscriptionReadDto> GetSubscriptionBySecret(string secret)
        {
            var subscription = _context.WebhookSubscriptions.FirstOrDefault(s => s.Secret == secret);
            if (subscription is null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<WebhookSubscriptionReadDto>(subscription));
        }

        [HttpPost]
        public ActionResult<WebhookSubscriptionReadDto> CreateSubscription(WebhookSubscriptionCreateDto webhookSubscriptionDto)
        {
            var subscription = _context.WebhookSubscriptions.FirstOrDefault(s => s.WebhookURI == webhookSubscriptionDto.WebhookURI);
            if (subscription is not null)
            {
                return BadRequest("The subscription already exists");
            }

            subscription = _mapper.Map<WebhookSubscription>(webhookSubscriptionDto);
            subscription.Secret = Guid.NewGuid().ToString();
            subscription.WebhookPublisher = "AeroSur";

            try
            {
                _context.Add(subscription);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var response = _mapper.Map<WebhookSubscriptionReadDto>(subscription);

            return CreatedAtRoute(nameof(GetSubscriptionBySecret), new
            {
                secret = response.Secret
            }, response
            );
        }
    }
}