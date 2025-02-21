using AirlineTicketsAppWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.OpenApi.Models;

namespace AirlineTicketsAppWebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class FlightController : ControllerBase
{
    private readonly string connectionString;

    public FlightController(IConfiguration configuration)
    {
        connectionString = configuration["ConnectionStrings:SqlServerDb"] ?? "";
    }

    [HttpPost]
    public IActionResult CreateFlight(FlightDto flightDto)
    {
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "Insert into flight " +
                    "(flightfrom, flightto, numoflayovers, numofseats, flightdate) values " +
                    "(@flightfrom, @flightto, @numoflayovers, @numofseats, @flightdate)";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@flightfrom", flightDto.FlightFrom);
                    command.Parameters.AddWithValue("@flightto", flightDto.FlightTo);
                    command.Parameters.AddWithValue("@numoflayovers", flightDto.NumOfLayovers);
                    command.Parameters.AddWithValue("@numofseats", flightDto.NumOfSeats);
                    command.Parameters.AddWithValue("@flightdate", flightDto.FlightDate);

                    command.ExecuteNonQuery();
                }
            
            }
        }
        catch (Exception ex) 
        {
            ModelState.AddModelError("Flight", $"Exception thrown {ex.ToString()}");
            return BadRequest(ModelState);
        }
        return Ok();
    }

    [HttpGet]
    public IActionResult GetFligths()
    {
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

}
