using System.Security.Claims;
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
    /// Creates a new campaign and validates the UserId.
    /// </summary>
    /// <param name="campaign">The campaign object.</param>
    /// <returns>Returns the created campaign.</returns>
    /// <response code="201">Successfully created the campaign.</response>
    /// <response code="400">Invalid campaign data or UserId mismatch.</response>
    /// <response code="500">Failed to create campaign.</response>
    [HttpPost("create")]
    [ProducesResponseType(typeof(Campaign), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> CreateCampaignAsync([FromBody] Campaign campaign)
    {
        // Validate that the UserId is provided and matches the expected format
        if (string.IsNullOrEmpty(campaign.UserId))
        {
            return BadRequest("UserId is required.");
        }

        // Optionally, ensure the UserId is in a valid GUID format
        if (!Guid.TryParse(campaign.UserId, out var parsedUserId))
        {
            return BadRequest("Invalid UserId format.");
        }

        // Proceed to create the campaign
        var createdCampaign = await _campaignService.CreateCampaignAsync(campaign, campaign.UserId);

        if (createdCampaign == null)
        {
            return StatusCode(500, "Failed to create campaign.");
        }

        return CreatedAtAction(nameof(GetCampaignByIdAsync), new { id = createdCampaign.Id }, createdCampaign);
    }

    /// <summary>
    /// Gets a campaign by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the campaign.</param>
    /// <returns>Returns the requested campaign or a 404 if not found.</returns>
    /// <response code="200">Returns the requested campaign.</response>
    /// <response code="404">Campaign not found with the specified ID.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Campaign), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCampaignByIdAsync(string id)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out _))
        {
            return BadRequest("Campaign ID is missing or invalid");
        }

        var campaign = await _campaignService.GetCampaignByIdAsync(id);

        if (campaign == null)
        {
            return NotFound();
        }

        return Ok(campaign);
    }

    /// <summary>
    /// Retrieves all campaigns with optional filter parameters.
    /// </summary>
    /// <param name="filterParams">The filter parameters to apply to the campaign search. 
    /// The filter can include criteria like the campaign title, user ID, and fundraising goal range.</param>
    /// <returns>A list of campaigns that match the filter criteria, or all campaigns if no filters are provided.</returns>
    /// <response code="200">Returns a list of campaigns that match the filter parameters.</response>
    /// <response code="400">If the filter parameters are invalid (e.g., MinFundraisingGoal > MaxFundraisingGoal).</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Campaign>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetAllCampaignsAsync([FromQuery] CampaignFilterParams filterParams)
    {
        if (filterParams != null && filterParams.MinFundraisingGoal.HasValue && filterParams.MaxFundraisingGoal.HasValue && filterParams.MinFundraisingGoal > filterParams.MaxFundraisingGoal)
        {
            return BadRequest("Min fundraising goal cannot be greater than max fundraising goal.");
        }

        // Call the service method to retrieve the campaigns based on the filter parameters
        var campaigns = await _campaignService.GetAllCampaignsAsync(filterParams);

        // Return the list of campaigns with a 200 OK response
        return Ok(campaigns);
    }

    /// <summary>
    /// Updates the details of an existing campaign.
    /// </summary>
    /// <param name="id">The ID of the campaign to update.</param>
    /// <param name="updatedCampaign">The updated campaign data.</param>
    /// <returns>Returns a 204 No Content response if the update is successful, 
    /// or a 400 Bad Request if the input data is invalid, or 404 Not Found if the campaign with the specified ID does not exist.</returns>
    /// <response code="204">The campaign was successfully updated.</response>
    /// <response code="400">The provided campaign or ID is invalid.</response>
    /// <response code="404">The campaign with the specified ID was not found.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
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
    /// Deletes a campaign by its ID.
    /// </summary>
    /// <param name="id">The ID of the campaign to delete.</param>
    /// <returns>Returns a 204 No Content response if the deletion is successful, 
    /// or a 400 Bad Request if the ID is invalid, or 404 Not Found if the campaign with the specified ID does not exist.</returns>
    /// <response code="204">The campaign was successfully deleted.</response>
    /// <response code="400">The provided campaign ID is invalid.</response>
    /// <response code="404">The campaign with the specified ID was not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)] // No Content
    [ProducesResponseType(400)] // Bad Request
    [ProducesResponseType(404)] // Not Found
    public async Task<IActionResult> DeleteCampaignAsync(string id)
    {
        // Validate the campaign ID, ensure it's not null or empty and is a valid GUID
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out _))
        {
            return BadRequest("Invalid campaign ID."); // Return BadRequest if ID is invalid
        }

        // Call the service to delete the campaign and check if the operation was successful
        var deleteSuccessful = await _campaignService.DeleteCampaignAsync(id);

        if (!deleteSuccessful)
        {
            return NotFound(); // Return NotFound if the campaign with the provided ID does not exist
        }

        // Return a 204 No Content response if the deletion is successful
        return NoContent();
    }

    /// <summary>
    /// Retrieves the donation progress for a specific campaign.
    /// </summary>
    /// <param name="id">The ID of the campaign to fetch donation progress for.</param>
    /// <returns>Returns the donation progress if found, or appropriate error responses for invalid or non-existing campaigns.</returns>
    /// <response code="200">Returns the donation progress for the campaign.</response>
    /// <response code="400">The provided campaign ID is invalid.</response>
    /// <response code="404">The campaign with the specified ID was not found.</response>
    [HttpGet("{id}/progress")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
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
