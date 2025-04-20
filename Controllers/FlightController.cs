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
    private readonly string connectionString;
    private readonly IFlightRepository _flightRepository;
    private readonly ILogger<FlightController> _logger;


    public FlightController(IConfiguration configuration,
                            IFlightRepository flightRepository,
                            ILogger<FlightController> logger)
    {
        connectionString = configuration["ConnectionStrings:SqlServerDb"] ?? "";
        _flightRepository = flightRepository;
        _logger = logger;
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
            int newFlightId = await _flightRepository.CreateFlightAsync(flightDto);

            flightDto.FlightId = newFlightId;

            return CreatedAtAction(
                actionName: nameof(GetFlightById),
                routeValues: new { id = newFlightId },
                value: flightDto);
        }
        catch (DbException ex)
        {
            _logger.LogError(ex, "Database error creating flight {FlightFrom}→{FlightTo}",
                flightDto.FlightFrom, flightDto.FlightTo);
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
    public IActionResult GetFligths()
    {
        var subClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        List<Flight> flights = new List<Flight>();

        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                flights = HelperController.getAllFlightsList(connection).Where(flight => flight.NumOfSeats > 0).ToList();


            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("Flight", $"Exception thrown {ex.ToString()}");
            return BadRequest(ModelState);
        }

        return Ok(flights);
    }

    [SubAuthorize("USER")]
    [HttpGet("{flightFrom}/{flightTo}")]
    public IActionResult GetFlights(FlightDestination flightFrom, FlightDestination flightTo)
    {
        List<Flight> flights = new List<Flight>();

        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = $"Select * from flight where flightfrom = @flightfrom and flightto = @flightto";
                
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@flightfrom", flightFrom);
                    command.Parameters.AddWithValue("@flightto", flightTo);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Flight flight = new Flight();

                            flight.FlightId = reader.GetInt32(0);

                            flight.FlightFrom = (FlightDestination)Int32.Parse(reader.GetString(1));
                            flight.FlightTo = (FlightDestination)Int32.Parse(reader.GetString(2));
                            flight.NumOfLayovers = reader.GetInt32(3);
                            flight.NumOfSeats = reader.GetInt32(4);
                            flight.FlightDate = reader.GetDateTime(5);
                            flight.FlightStatus = reader.GetString(6);
                            flights.Add(flight);
                        }
                    }

                }
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("Flight", $"Exception thrown {ex.ToString()}");
            return BadRequest(ModelState);
        }

        return Ok(flights);
    }

    [SubAuthorize("ADMIN")]
    [HttpPut("{flightid}")]
    public IActionResult CancelFlight(int flightid)
    {
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = $"Update flight SET flightstatus = 'canceled' WHERE flightid = @flightid";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@flightid", flightid);

                    command.ExecuteNonQuery();
                }
            }
        }
        catch(Exception ex)
        {
            ModelState.AddModelError("Flight", $"Exception thrown {ex.ToString()}");
            return BadRequest(ModelState);
        }

        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFlightById(int id)
    {
        var flight = await _flightRepository.GetFlightByIdAsync(id);
        return flight != null ? Ok(flight) : NotFound();
    }

}
