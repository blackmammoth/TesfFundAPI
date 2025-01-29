using Microsoft.AspNetCore.Mvc;
using TesfaFundApp.Services;

namespace TesfaFundApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignsController : ControllerBase
{
    private readonly ICampaignService _campaignService;

    public CampaignsController(ICampaignService campaignService)
    {
        _campaignService = campaignService;
    }

    /// <summary>
    /// Creates a new campaign.
    /// </summary>
    /// <param name="campaign">The campaign data.</param>
    /// <returns>The created campaign.</returns>
    /// <response code="201">Successfully created the campaign.</response>
    /// <response code="400">Bad Request - Invalid campaign data (e.g., missing required fields, invalid UserId).</response>
    /// <response code="500">Internal Server Error - Failed to create campaign due to a server error.</response>
    [HttpPost("create")]
    [ProducesResponseType(typeof(Campaign), StatusCodes.Status201Created)] // Use StatusCodes enum
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateCampaignAsync([FromBody] Campaign campaign)
    {
        if (string.IsNullOrEmpty(campaign.UserId)) //Validation can be moved to model
        {
            return BadRequest(new { message = "UserId is required." });
        }

        if (!Guid.TryParse(campaign.UserId, out _)) //Validation can be moved to model
        {
            return BadRequest("UserId is not a valid UUID.");
        }

        var createdCampaign = await _campaignService.CreateCampaignAsync(campaign, campaign.UserId);

        if (createdCampaign == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create campaign."); // Use StatusCodes enum
        }

        return CreatedAtAction(nameof(GetCampaignByIdAsync), new { id = createdCampaign.Id }, createdCampaign);
    }

    /// <summary>
    /// Gets a campaign by ID.
    /// </summary>
    /// <param name="id">The ID of the campaign.</param>
    /// <returns>The campaign, or 404 if not found.</returns>
    /// <response code="200">Returns the requested campaign.</response>
    /// <response code="404">Not Found - Campaign not found with the specified ID.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Campaign), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCampaignByIdAsync(string id)
    {
        if (!Guid.TryParse(id, out _)) //Validation can be moved to model
        {
            return BadRequest("CampaignId is not a valid UUID.");
        }

        var campaign = await _campaignService.GetCampaignByIdAsync(id);

        if (campaign == null)
        {
            return NotFound();
        }

        return Ok(campaign);
    }

    /// <summary>
    /// Gets all campaigns (with optional filtering).
    /// </summary>
    /// <param name="filterParams">Filter parameters (title, userId, fundraising goal range).</param>
    /// <returns>A list of campaigns.</returns>
    /// <response code="200">Returns a list of campaigns.</response>
    /// <response code="400">Bad Request - Invalid filter parameters (e.g., MinFundraisingGoal > MaxFundraisingGoal).</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Campaign>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllCampaignsAsync([FromQuery] CampaignFilterParams filterParams)
    {
        if (filterParams != null && filterParams.MinFundraisingGoal.HasValue && filterParams.MaxFundraisingGoal.HasValue && filterParams.MinFundraisingGoal > filterParams.MaxFundraisingGoal)
        {
            return BadRequest("Min fundraising goal cannot be greater than max fundraising goal.");
        }

        var campaigns = await _campaignService.GetAllCampaignsAsync(filterParams);
        return Ok(campaigns);
    }

    /// <summary>
    /// Updates a campaign.
    /// </summary>
    /// <param name="id">The ID of the campaign to update.</param>
    /// <param name="updatedCampaign">The updated campaign data.</param>
    /// <returns>No Content (204) if successful.</returns>
    /// <response code="204">No Content - Campaign successfully updated.</response>
    /// <response code="400">Bad Request - Invalid campaign data or ID.</response>
    /// <response code="404">Not Found - Campaign not found.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCampaignAsync(string id, [FromBody] Campaign updatedCampaign)
    {
        if (updatedCampaign == null || string.IsNullOrEmpty(id))
        {
            return BadRequest("Campaign or ID is invalid.");
        }

        var updateSuccessful = await _campaignService.UpdateCampaignAsync(id, updatedCampaign);

        if (!updateSuccessful)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes a campaign.
    /// </summary>
    /// <param name="id">The ID of the campaign to delete.</param>
    /// <returns>No Content (204) if successful.</returns>
    /// <response code="204">No Content - Campaign successfully deleted.</response>
    /// <response code="400">Bad Request - Invalid campaign ID.</response>
    /// <response code="404">Not Found - Campaign not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCampaignAsync(string id)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out _))
        {
            return BadRequest("Invalid campaign ID.");
        }

        var deleteSuccessful = await _campaignService.DeleteCampaignAsync(id);

        if (!deleteSuccessful)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Gets donation progress for a campaign.
    /// </summary>
    /// <param name="id">The ID of the campaign.</param>
    /// <returns>The donation progress.</returns>
    /// <response code="200">Returns the donation progress.</response>
    /// <response code="400">Bad Request - Invalid campaign ID.</response>
    /// <response code="404">Not Found - Campaign not found.</response>
    [HttpGet("{id}/progress")]
    [ProducesResponseType(typeof(DonationProgress), StatusCodes.Status200OK)] // Specify the type
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCampaignDonationProgressAsync(string id)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out _))
        {
            return BadRequest("Invalid campaign ID.");
        }

        var progress = await _campaignService.GetCampaignDonationProgressAsync(id);

        if (progress == null)
        {
            return NotFound();
        }

        return Ok(progress);
    }
}