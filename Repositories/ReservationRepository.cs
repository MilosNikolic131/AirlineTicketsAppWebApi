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
    private readonly IReservationValidator _reservationValidator;

    public ReservationRepository(IConfiguration config, IReservationValidator reservationValidator)
    {
        _connectionString = config.GetConnectionString("SqlServerDb")
        ?? throw new ArgumentNullException("Missing SQL connection string");
        _reservationValidator = reservationValidator;
    }

    public async Task ReserveFlightAsync(ReservationDto reservationDto)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var transaction = await connection.BeginTransactionAsync() as SqlTransaction;

        try
        {
            if (!await _reservationValidator.ValidateReservationAsync(reservationDto))
            {
                throw new Exception("Validation failed");
            }

            Task[] tasks;

            Task.WaitAll(tasks = [CreateReservationRecordAsync(reservationDto, connection, transaction),
                                  UpdateAvailableSeatsAsync(reservationDto, connection, transaction)]);

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
        //const string sql = "Insert into reservation " +
        //                    "(flightid, userid, numberOfSeats) values " +
        //                    "(@flightid, @userid, @numberOfSeats)";

        //return await connection.QuerySingleAsync<(int, int)>(sql, reservationDto);
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

    private async Task CreateReservationRecordAsync(ReservationDto reservationDto, SqlConnection connection, SqlTransaction transaction)
    {
        const string sql = @"Insert into reservation " +
                            "(flightid, userid, numberOfSeats) values " +
                            "(@flightid, @userid, @numberOfSeats)";

        await connection.ExecuteAsync(sql, new
        {
            reservationDto.Flightid,
            reservationDto.Userid,
            reservationDto.NumberOfSeats,
        }, transaction);
    }

    private async Task UpdateAvailableSeatsAsync(ReservationDto reservationDto, SqlConnection connection, SqlTransaction transaction)
    {
        const string sql = @"UPDATE flight SET numofseats = numofseats - @numberOfSeats WHERE flightid = @flightid";

        var affectedRows = await connection.ExecuteAsync(sql, new
        {
            reservationDto.NumberOfSeats,
            reservationDto.Flightid
        }, transaction);

        if (affectedRows != 1)
        {
            throw new Exception("Failed to update flight seats");
        }
    }

}
