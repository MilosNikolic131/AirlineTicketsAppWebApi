using AirlineTicketsAppWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AirlineTicketsAppWebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ReservationController : ControllerBase
{
    private readonly string connectionString;

    public ReservationController(IConfiguration configuration)
    {
        connectionString = configuration["ConnectionStrings:SqlServerDb"] ?? "";
    }

    [HttpPost]
    public IActionResult ReserveFlight(ReservationDto reservationDto)
    {
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                if (HelperController.validateReservation(reservationDto, connection))
                {
                    throw new Exception("Validation failed");
                }

                string sql = "Insert into reservation " +
                    "(flightid, userid, numberOfSeats) values " +
                    "(@flightid, @userid, @numberOfSeats)";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@flightid", reservationDto.Flightid);
                    command.Parameters.AddWithValue("@userid", reservationDto.Userid);
                    command.Parameters.AddWithValue("@numberOfSeats", reservationDto.NumberOfSeats);

                    command.ExecuteNonQuery();
                }

                sql = "UPDATE flight SET numofseats = numofseats - @numberOfSeats WHERE flightid = @flightid";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@numberOfSeats", reservationDto.NumberOfSeats);
                    command.Parameters.AddWithValue("@flightid", reservationDto.Flightid);

                    command.ExecuteNonQuery();
                }

            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("Reservation", $"Exception thrown {ex.ToString()}");
            return BadRequest(ModelState);
        }
        return Ok();

    }

    [HttpPut("{flightid}/{userid}")]
    public IActionResult ApproveReservation(int flightid, int userid)
    {
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = $"Update reservation SET reservationstatus = '@reservationstatus' WHERE flightid = @flightid AND userid = @userid";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("reservationstatus", ReservationStatus.APPROVED);
                    command.Parameters.AddWithValue("@flightid", flightid);
                    command.Parameters.AddWithValue("@userid", userid);

                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("Reservation", $"Exception thrown {ex.ToString()}");
            return BadRequest(ModelState);
        }

        return Ok();
    }

    [HttpGet("{userid}")]
    public IActionResult GetReservationsById(int userid)
    {
        List<Reservation> reservations = new List<Reservation>();

        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = $"Select * from reservation where userid = @userid";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@userid", userid);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Reservation reservation = new Reservation();

                            reservation.Flightid = reader.GetInt32(0);
                            reservation.Userid = reader.GetInt32(1);
                            reservation.NumberOfSeats = reader.GetInt32(2);
                            reservation.ReservationStatus = (ReservationStatus) Int32.Parse(reader.GetString(3));
                            reservations.Add(reservation);
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

        return Ok(reservations);
    }
}
