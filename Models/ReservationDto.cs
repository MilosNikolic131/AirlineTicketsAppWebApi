using System.ComponentModel.DataAnnotations;

namespace AirlineTicketsAppWebApi.Models;

public class ReservationDto
{
    [Required]
    public int Flightid { get; set; }
    [Required]
    public int Userid { get; set; }
    [Required]
    public int NumberOfSeats { get; set; }
}
