using AirlineTicketsAppWebApi.Models;
using AirlineTicketsAppWebApi.Repositories;
using AirlineTicketsAppWebApi.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;


namespace AirlineTicketsAppWebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class FlightController : ControllerBase
{
    private readonly string                     connectionString;
    private readonly IFlightRepository          _flightRepository;
    private readonly ILogger<FlightController>  _logger;


    public FlightController(IConfiguration              configuration,
                            IFlightRepository           flightRepository,
                            ILogger<FlightController>   logger)
    {
        connectionString    = configuration["ConnectionStrings:SqlServerDb"] ?? "";
        _flightRepository   = flightRepository;
        _logger             = logger;
    }

    [SubAuthorize("AGENT")]
    [HttpPost]
    public async Task<IActionResult> CreateFlight([FromBody] FlightDto flightDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            int newFlightId     = await _flightRepository.CreateFlightAsync(flightDto);
            flightDto.FlightId  = newFlightId;

            return CreatedAtAction(
                actionName: nameof(GetFlightById),
                routeValues: new { id = newFlightId },
                value: flightDto);
        }
        catch (DbException ex)
        {
            _logger.LogError(ex, "Database error creating flight");
            return StatusCode(500, "Failed to save flight");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating flight");
            return StatusCode(500, "An internal error occurred");
        }
    }

    [SubAuthorize("AGENT", "USER", "ADMIN")]
    [HttpGet]
    public async Task<IActionResult> GetFligths()
    {
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var flights = await _flightRepository.GetAllFlightsAsync();
                return Ok(flights.ToList().Where(flight => flight.NumOfSeats > 0).ToList());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetching flights");
            return StatusCode(500, "An unexpected error occurred");
        }
    }

    [SubAuthorize("USER")]
    [HttpGet("{flightFrom}/{flightTo}")]
    public async Task<IActionResult> GetFlights(
    FlightDestination flightFrom,
    FlightDestination flightTo)
    {
        try
        {
            var flights = await _flightRepository.GetFlightsFromToAsync(flightFrom, flightTo);
            return Ok(flights);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetching flights");
            return StatusCode(500, "An unexpected error occurred");
        }
    }

    [SubAuthorize("ADMIN")]
    [HttpPut("cancel/{flightid}")]
    public async Task<IActionResult> CancelFlight(int flightid)
    {
        try
        {
            bool success = await _flightRepository.CancelFlightAsync(flightid);
            return success ? NoContent() : NotFound();

        }
        catch (DbException ex)
        {
            _logger.LogError(ex, "Error canceling flight {FlightId}", flightid);
            return StatusCode(500, "Failed to cancel flight");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("Flight", $"Exception thrown {ex.ToString()}");
            return BadRequest(ModelState);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFlightById(int id)
    {
        var flight = await _flightRepository.GetFlightByIdAsync(id);
        return flight != null ? Ok(flight) : NotFound();
    }

}
