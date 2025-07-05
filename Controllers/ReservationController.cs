using AirlineTicketsAppWebApi.Models;
using AirlineTicketsAppWebApi.Repositories;
using AirlineTicketsAppWebApi.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AirlineTicketsAppWebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ReservationController : ControllerBase
{
    private readonly string connectionString;
    private readonly IReservationRepository _reservationRepository;
    private readonly ILogger<ReservationController> _logger;
   

    public ReservationController(IConfiguration configuration,
                                 IReservationRepository reservationRepository,
                                 ILogger<ReservationController> logger
                                 )
    {
        connectionString = configuration["ConnectionStrings:SqlServerDb"] ?? "";
        _reservationRepository = reservationRepository;
        _logger = logger;
    }

    [SubAuthorize("USER")]
    [HttpPost]
    public async Task<IActionResult> ReserveFlight(ReservationDto reservationDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _reservationRepository.ReserveFlightAsync(reservationDto);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating reservation");
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    [SubAuthorize("AGENT")]
    [HttpPut("{flightid}/{userid}")]
    public async Task<IActionResult> ApproveReservation(int flightid, int userid)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _reservationRepository.ApproveReservationAsync(flightid, userid);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving reservation");
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    [SubAuthorize("USER")]
    [HttpGet("{userid}")]
    public async Task<IActionResult> GetReservationsById(int userid)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _reservationRepository.GetReservationsByIdAsync(userid);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reservations from userid");
            return StatusCode(500, "An error occurred while processing your request");
        }
    }
    
}
