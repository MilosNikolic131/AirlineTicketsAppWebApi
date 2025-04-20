using AirlineTicketsAppWebApi.Models;
using System.Data;
//using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;

namespace AirlineTicketsAppWebApi.Repositories;

public class FlightRepository : IFlightRepository
{
    private readonly string _connectionString;

    public FlightRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("SqlServerDb")
            ?? throw new ArgumentNullException("Missing SQL connection string");
    }

    public async Task<int> CreateFlightAsync(FlightDto flightDto)
    {
        await using var connection = new SqlConnection(_connectionString);

        const string sql = @"
        INSERT INTO Flight
        (FlightFrom, FlightTo, NumOfLayovers, NumOfSeats, FlightDate)
        VALUES
        (@FlightFrom, @FlightTo, @NumOfLayovers, @NumOfSeats, @FlightDate);
        SELECT CAST (SCOPE_IDENTITY() AS INT);";

        return await connection.QuerySingleAsync<int>(sql, flightDto);
    }

    public async Task<FlightDto?> GetFlightByIdAsync(int id)
    {
        const string sql = "SELECT * FROM Flights WHERE flightid = @Id";
        await using var connection = new SqlConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<FlightDto>(sql, new { Id = id });
    }
}
