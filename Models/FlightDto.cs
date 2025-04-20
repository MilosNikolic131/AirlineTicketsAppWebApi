using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AirlineTicketsAppWebApi.Models;

public class FlightDto
{
    [JsonIgnore]
    public int FlightId { get; set; }

    [Required]
    [Range(0, 3)]
    public FlightDestination FlightFrom { get; set; }

    [Required]
    [Range(0, 3)]
    public FlightDestination FlightTo { get; set;}

    [Required]
    [Range(1, 4)]
    public int NumOfLayovers { get; set; }

    [Required]
    [Range(1, 250)]
    public int NumOfSeats { get; set; }

    [Required]
    public DateTime FlightDate { get; set; }

}
