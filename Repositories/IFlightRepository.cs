using AirlineTicketsAppWebApi.Models;

namespace AirlineTicketsAppWebApi.Repositories;

public interface IFlightRepository
{
    Task<int> CreateFlightAsync(FlightDto flightDto);
    Task<FlightDto?> GetFlightByIdAsync(int id);
}
