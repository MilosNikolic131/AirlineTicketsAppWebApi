namespace AirlineTicketsAppWebApi.Models;

public class Flight
{
    public int FlightId { get; set; }

    public FlightDestination FlightFrom { get; set; }

    public FlightDestination FlightTo { get; set; }

    public int NumOfLayovers { get; set; }

    public int NumOfSeats { get; set; }

    public DateTime FlightDate { get; set; }

    public string? FlightStatus { get; set; }
}
