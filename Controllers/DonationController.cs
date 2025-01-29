using Microsoft.AspNetCore.Mvc;
using TesfaFundApp.Services;
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
    /// <returns>The details of the donation, or a 404 if not found.</returns>
    /// <response code="200">Donation details retrieved successfully.</response>
    /// <response code="400">Invalid donation ID provided.</response>
    /// <response code="404">Donation not found.</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDonationByIdAsync(string id)
    {
        // Validate the donation ID
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out _))
        {
            return BadRequest("Invalid donation ID.");
        }

        // Fetch the donation using the donation service
        var donation = await _donationService.GetDonationByIdAsync(id);

        // Return the donation if found, otherwise return NotFound
        if (donation == null)
        {
            return NotFound();
        }

        return Ok(donation);
    }

    /// <summary>
    /// Make a donation to a campaign.
    /// </summary>
    /// <param name="donation">The donation details.</param>
    /// <returns>The created donation details or an error response.</returns>
    /// <response code="200">Donation successfully created.</response>
    /// <response code="400">Invalid donation data, such as missing or invalid fields.</response>
    /// <response code="404">Campaign not found with the provided ID.</response>
    /// <response code="500">Failed to process the donation.</response>
    [HttpPost]
    public async Task<IActionResult> CreateDonationAsync([FromBody] Donation donation)
    {
        // Validate the donation data
        if (donation == null || donation.Amount <= 0 || string.IsNullOrEmpty(donation.CampaignId) || !Guid.TryParse(donation.CampaignId, out _))
        {
            return BadRequest("Invalid donation data.");
        }

        // Check if the campaign exists
        var campaign = await _campaignService.GetCampaignByIdAsync(donation.CampaignId);
        if (campaign == null)
        {
            return NotFound($"Campaign with ID {donation.CampaignId} not found.");
        }

        // Process the donation
        var createdDonation = await _donationService.MakeDonationAsync(donation);
        if (createdDonation == null)
        {
            return StatusCode(500, "Failed to process donation.");
        }

        return Ok(createdDonation);
    }

    /// <summary>
    /// Get a list of donations with optional search and filter parameters.
    /// </summary>
    /// <param name="filters">The filter parameters for searching donations.</param>
    /// <returns>A list of donations that match the specified filter criteria.</returns>
    /// <response code="200">A list of donations successfully retrieved.</response>
    /// <response code="400">Invalid filter parameters provided.</response>
    [HttpGet]
    public async Task<IActionResult> GetDonationsAsync([FromQuery] DonationFilterParams filters)
    {
        if (filters == null)
        {
            return BadRequest("Invalid filter parameters.");
        }

        // Retrieve filtered donations using the donation service
        var donations = await _donationService.GetFilteredDonationsAsync(filters);
        return Ok(donations);
    }

}