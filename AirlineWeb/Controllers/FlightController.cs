using AirlineWeb.Data;
using AirlineWeb.Dtos;
using AirlineWeb.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AirlineWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightController : ControllerBase
    {
        private readonly AirlineDbContext _context;
        private readonly IMapper _mapper;

        public FlightController(AirlineDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("{flightCode}", Name = "GetFlightDetailsByCode")]
        public ActionResult<FlightDetailReadDto> GetFlightDetailsByCode(string flightCode)
        {
            var flight = _context.FlightDetails.FirstOrDefault(f => f.FlightCode == flightCode);
            if (flight is null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<FlightDetailReadDto>(flight));
        }

        public ActionResult<List<FlightDetailReadDto>> GetFlightDetails()
        {
            var flightDetails = _context.FlightDetails.ToList();
            return Ok(_mapper.Map<List<FlightDetailReadDto>>(flightDetails));
        }

        [HttpPost]
        public ActionResult<FlightDetailReadDto> CreateFlight(FlightDetailCreateDto flightDetailCreateDto)
        {
            var flight = _context.FlightDetails.FirstOrDefault(f => f.FlightCode == flightDetailCreateDto.FlightCode);
            if (flight is not null)
            {
                return BadRequest("The Flight already exists.");
            }

            var flightModel = _mapper.Map<FlightDetail>(flightDetailCreateDto);
            try
            {
                _context.FlightDetails.Add(flightModel);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var result = _mapper.Map<FlightDetailReadDto>(flightModel);
            return CreatedAtRoute(nameof(GetFlightDetailsByCode), new
            {
                flightCode = result.FlightCode
            }, result);
        }

        [HttpPut("{id}")]
        public ActionResult<FlightDetailReadDto> UpdateFlight(int id, FlightDetailUpdateDto flightDetailUpdateDto)
        {
            var flight = _context.FlightDetails.Find(id);
            if (flight is null)
            {
                return NotFound();
            }

            _mapper.Map(flightDetailUpdateDto, flight);
            _context.SaveChanges();

            return Ok(flight);
        }
    }
}