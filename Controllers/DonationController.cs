using Microsoft.AspNetCore.Mvc;
using TesfaFundApp.Services;
using TesfaFundApp.Models;

namespace TesfaFundApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DonationController : ControllerBase
{
    private readonly IDonationService _donationService;
    private readonly ICampaignService _campaignService;

    public DonationController(IDonationService donationService, ICampaignService campaignService)
    {
        _donationService = donationService;
        _campaignService = campaignService;
    }

    /// <summary>
    /// Get donation details by ID.
    /// </summary>
    /// <param name="id">The unique identifier of the donation.</param>
    /// <returns>The details of the donation, or a 404 Not Found result if not found.</returns>
    /// <response code="200">Donation details retrieved successfully.</response>
    /// <response code="400">Bad Request - Invalid donation ID provided.</response>
    /// <response code="404">Not Found - Donation not found with the specified ID.</response>
    [HttpGet("{id}", Name = "GetDonationByIdAsync")]
    [ProducesResponseType(typeof(Donation), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDonationByIdAsync(string id)
    {
        if (!Guid.TryParse(id, out _))
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid Donation ID",
                Detail = "Invalid donation ID provided."
            });
        }

        var donation = await _donationService.GetDonationByIdAsync(id);

        if (donation == null)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Donation Not Found",
                Detail = "Donation not found with the specified ID."
            });
        }

        return Ok(donation);
    }

    /// <summary>
    /// Make a donation to a campaign. Donation timestamp is set by the server.
    /// </summary>
    /// <param name="donation">The donation details.</param>
    /// <returns>The created donation details or an error response.</returns>
    /// <response code="201">Donation successfully created. Returns the created donation object.</response>
    /// <response code="400">Bad Request - Invalid donation data or donation processing failed. Check the response body for details.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Donation), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateDonationAsync([FromBody] Donation donation)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (errorMessage, createdDonation) = await _donationService.MakeDonationAsync(donation);

        if (errorMessage != null)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Donation Processing Failed",
                Detail = errorMessage
            });
        }

        return CreatedAtRoute(nameof(GetDonationByIdAsync), new { id = createdDonation!.Id }, createdDonation);
    }

    /// <summary>
    /// Get a list of donations with optional search and filter parameters.
    /// </summary>
    /// <param name="filters">The filter parameters for searching donations.</param>
    /// <returns>A list of donations that match the specified filter criteria.</returns>
    /// <response code="200">A list of donations successfully retrieved.</response>
    /// <response code="400">Bad Request - Invalid filter parameters. Check the response body for details.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Donation>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDonationsAsync([FromQuery] DonationFilterParams? filters)
    {
        if (filters != null && !ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (filters != null && filters.MinAmount > filters.MaxAmount)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid Amount Range",
                Detail = "Minimum amount cannot be greater than maximum amount."
            });
        }

        if (filters != null && filters.StartDate > filters.EndDate)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid Date Range",
                Detail = "Start Date cannot be greater than End Date."
            });
        }

        var donations = await _donationService.GetFilteredDonationsAsync(filters);
        return Ok(donations);
    }
}