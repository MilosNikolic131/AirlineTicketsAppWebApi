using AirlineTicketsAppWebApi.Models;

namespace AirlineTicketsAppWebApi.Repositories;

public interface IFlightRepository
{
    Task<int> CreateFlightAsync(FlightDto flightDto);
    Task<FlightDto?> GetFlightByIdAsync(int id);

    Task<IEnumerable<Flight>> GetFlightsFromToAsync(FlightDestination from, FlightDestination to);

    Task<IEnumerable<Flight>> GetAllFlightsAsync();

    Task<bool> CancelFlightAsync(int flightid);
}
