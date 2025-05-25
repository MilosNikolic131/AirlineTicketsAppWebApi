using AirlineTicketsAppWebApi.Controllers;
using AirlineTicketsAppWebApi.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Data.SqlClient;

namespace AirlineTicketsAppWebApi.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly string _connectionString;

    public ReservationRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("SqlServerDb")
        ?? throw new ArgumentNullException("Missing SQL connection string");
    }

    public async Task<(int FlightId, int UserId)> ReserveFlightAsync(ReservationDto reservationDto)
    {
        await using var connection = new SqlConnection(_connectionString);

        if (HelperController.validateReservation(reservationDto, connection))
        {
            throw new Exception("Validation failed");
        }
        const string sql = "Insert into reservation " +
                            "(flightid, userid, numberOfSeats) values " +
                            "(@flightid, @userid, @numberOfSeats)";

        return await connection.QuerySingleAsync<(int, int)>(sql, reservationDto);
    }

    public async Task<IActionResult> ApproveReservationAsync(int FlightId, int UserId)
    {
        await using var connection = new SqlConnection(_connectionString);

        const string sql = $"Update reservation SET reservationstatus = @reservationstatus " +
                    $"WHERE flightid = @flightid AND userid = @userid";

        return await connection.QuerySingleAsync<IActionResult>(sql, new { FlightId, UserId });
    }

    public async Task<IEnumerable<Reservation>> GetReservationsByIdAsync(int UserId)
    {
        await using var connection = new SqlConnection(_connectionString);

        const string sql = $"Select * from reservation where userid = @userid";

        return await connection.QueryAsync<Reservation>(sql, new { UserId });
    }

}
