using AirlineTicketsAppWebApi.Models;

namespace AirlineTicketsAppWebApi.Repositories;

public interface IReservationValidator
{
    Task<bool> ValidateReservationAsync(ReservationDto reservationDto);
}
