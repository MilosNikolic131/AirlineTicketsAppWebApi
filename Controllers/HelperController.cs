using AirlineTicketsAppWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AirlineTicketsAppWebApi.Controllers;
public class HelperController : Controller
{
    public static List<Flight> getAllFlightsList(SqlConnection connection)
    {
        List<Flight> flights = new List<Flight>();

        string sql = "Select * from flight";
        using (var command = new SqlCommand(sql, connection))
        {
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

        return flights;
    }

    public static List<User> getAllUsersList(SqlConnection connection)
    {
        List<User> users = new List<User>();

        string sql = "Select * from UserTable";
        using (var command = new SqlCommand(sql, connection))
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    User user = new User();

                    user.UserId = reader.GetInt32(0);
                    user.Name = reader.GetString(1);
                    user.Type = (UserType)Int32.Parse(reader.GetString(2));
                    user.Username = reader.GetString(3);
                    user.Password = reader.GetString(4);
                    users.Add(user);
                }
            }

        }

        return users;
    }

    public static bool validateReservation(ReservationDto reservationDto, SqlConnection sqlConnection)
    {
        if (reservationDto == null)
        {
            return false;
        }

        List<Flight>    flights = getAllFlightsList(sqlConnection);
        List<User>      users   = getAllUsersList(sqlConnection);

        Flight foundFlight = flights.Where(flight => flight.FlightId == reservationDto.Flightid).ToList()[0];
        User foundUser = users.Where(user => user.UserId == reservationDto.Userid).ToList()[0];

        if (foundFlight == null 
            || foundFlight.FlightStatus == "canceled" 
            || foundFlight.FlightDate > DateTime.Today.AddDays(3)
            || reservationDto.NumberOfSeats > foundFlight.NumOfSeats
            )
        {
            return false;
        }

        if (foundUser == null || foundUser.Type != UserType.USER)
        {
            return false;
        }

        return true;
    }
}
