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

    public async Task<Flight?> GetFlightByIdAsync(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = "SELECT * FROM Flights WHERE flightid = @Id";
        return await connection.QuerySingleOrDefaultAsync<Flight>(sql, new { Id = id });
    }

    public async Task<IEnumerable<Flight>> GetFlightsFromToAsync(FlightDestination flightFrom, FlightDestination flightTo)
    {
        await using var connection = new SqlConnection(_connectionString);

        const string sql = @"Select * from flight where flightfrom = @flightfrom and flightto = @flightto";

        return await connection.QueryAsync<Flight>(sql, new { flightFrom, flightTo });
    }

    public async Task<IEnumerable<Flight>> GetAllFlightsAsync()
    {
        await using var connection = new SqlConnection(_connectionString);

        const string sql = "Select * from flight";

        return await connection.QueryAsync<Flight>(sql);
    }

    public async Task<bool> CancelFlightAsync(int flightid)
    {
        await using var connection = new SqlConnection(_connectionString);

        const string sql = "Update flight SET flightstatus = 'canceled' WHERE flightid = @flightid";

        int affectedRows = await connection.ExecuteAsync(sql, new { flightid });
        return affectedRows > 0;
    }
}
