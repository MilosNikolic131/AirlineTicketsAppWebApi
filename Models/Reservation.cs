namespace AirlineTicketsAppWebApi.Models;

public class Reservation
{
    public int Flightid { get; set; }

    public int Userid { get; set; }

    public int NumberOfSeats { get; set; }

    public ReservationStatus ReservationStatus { get; set;}
}
