using System.ComponentModel.DataAnnotations;

namespace AirlineTicketsAppWebApi.Models;

public class FlightDto
{

    [Required]
    public FlightDestination FlightFrom { get; set; }

    [Required]
    public FlightDestination FlightTo { get; set;}

    [Required]
    public int NumOfLayovers { get; set; }

    [Required]
    public int NumOfSeats { get; set; }

    [Required]
    public DateTime FlightDate { get; set; }

}
