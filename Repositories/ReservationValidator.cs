using AirlineTicketsAppWebApi.Models;

namespace AirlineTicketsAppWebApi.Repositories;

public class ReservationValidator : IReservationValidator
{
    private readonly IFlightRepository _flightRepository;
    private readonly IUserRepository _userRepository;

    public ReservationValidator(IFlightRepository flightRepository, IUserRepository userRepository)
    {
        _flightRepository = flightRepository;
        _userRepository = userRepository;
    }

    public async Task<bool> ValidateReservationAsync(ReservationDto reservationDto)
    {
        if (reservationDto == null)
            return false;

        // Get flight and user asynchronously (avoids loading all records)
        var flight = await _flightRepository.GetFlightByIdAsync(reservationDto.Flightid);
        var user = await _userRepository.GetUserByIdAsync(reservationDto.Userid);

        if (flight == null
            || flight.FlightStatus == "canceled"
            || flight.FlightDate > DateTime.Today.AddDays(3)
            || reservationDto.NumberOfSeats > flight.NumOfSeats)
        {
            return false;
        }

        if (user == null || user.Type != UserType.USER)
        {
            return false;
        }

        return true;
    }
}