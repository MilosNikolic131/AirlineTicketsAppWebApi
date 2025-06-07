using AirlineTicketsAppWebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace AirlineTicketsAppWebApi.Repositories;

public interface IReservationRepository
{
    Task ReserveFlightAsync(ReservationDto reservationDto);

    Task<IActionResult> ApproveReservationAsync(int FlightId, int UserId);

    Task<IEnumerable<Reservation>> GetReservationsByIdAsync(int UserId);
}
