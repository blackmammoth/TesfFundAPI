using Microsoft.AspNetCore.Mvc;
using TesfaFundApp.Services;
using TesfaFundApp.Models;

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
    /// <param name="campaign">The campaign data from the incoming request.</param>
    /// <returns>The created campaign, or a BadRequest result with error details.</returns> // Improved return description
    /// <response code="201">Successfully created the campaign. Returns the created campaign object.</response> // Clarified response
    /// <response code="400">Bad Request - Invalid campaign data. Check the response body for error details.</response> // More specific
    /// <response code="500">Internal Server Error - A server error occurred while creating the campaign.</response>
    [HttpPost("create")]
    [ProducesResponseType(typeof(Campaign), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)] // Use ProblemDetails
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)] // Use ProblemDetails
    public async Task<IActionResult> CreateCampaignAsync([FromBody] Campaign campaign)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (errorMessage, createdCampaign) = await _campaignService.CreateCampaignAsync(campaign);

        if (errorMessage != null)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid Campaign Data",
                Detail = errorMessage // Specific error message from the service
            });
        }

        return CreatedAtAction(nameof(GetCampaignByIdAsync), new { id = createdCampaign!.Id }, createdCampaign);
    }

    /// <summary>
    /// Gets a campaign by ID.
    /// </summary>
    /// <param name="id">The ID of the campaign.</param>
    /// <returns>The campaign, or a 404 Not Found result if not found.</returns>
    /// <response code="200">Returns the requested campaign.</response>
    /// <response code="400">Bad Request - The provided ID is not a valid UUID.</response>
    /// <response code="404">Not Found - Campaign not found with the specified ID.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Campaign), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCampaignByIdAsync(string id)
    {
        if (!Guid.TryParse(id, out _))
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid Campaign ID",
                Detail = $"'{id}' is not a valid UUID."
            });
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
    /// <param name="filterParams">Filter parameters (title, recipientId, fundraising goal range).</param>
    /// <returns>A list of campaigns.</returns>
    /// <response code="200">Returns a list of campaigns.</response>
    /// <response code="400">Bad Request - Invalid filter parameters. Check the response body for details.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Campaign>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllCampaignsAsync([FromQuery] CampaignFilterParams? filterParams)
    {
        if (filterParams != null && !ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (filterParams != null && filterParams.MinFundraisingGoal > filterParams.MaxFundraisingGoal)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid Fundraising Goal Range",
                Detail = "Minimum fundraising goal cannot be greater than maximum fundraising goal."
            });
        }

        var campaigns = await _campaignService.GetAllCampaignsAsync(filterParams);
        return Ok(campaigns);
    }

    /// <summary>
    /// Updates a campaign.
    /// </summary>
    /// <param name="id">The ID of the campaign to update.</param>
    /// <param name="updatedCampaign">The updated campaign data.</param>
    /// <returns>No Content (204) if successful, or an appropriate error response.</returns>
    /// <response code="204">No Content - Campaign successfully updated.</response>
    /// <response code="400">Bad Request - Invalid campaign data. Check the response body for details.</response>
    /// <response code="404">Not Found - Campaign not found with the specified ID.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCampaignAsync(string id, [FromBody] Campaign updatedCampaign)
    {
        if (!Guid.TryParse(id, out _))
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid Campaign ID",
                Detail = "Invalid campaign ID."
            });
        }

        updatedCampaign.Id = id;

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (errorMessage, _) = await _campaignService.UpdateCampaignAsync(id, updatedCampaign);

        if (errorMessage != null)
        {
            if (errorMessage == "Campaign not found or not modified.")
            {
                return NotFound();
            }

            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Campaign Update Failed",
                Detail = errorMessage
            });
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes a campaign.
    /// </summary>
    /// <param name="id">The ID of the campaign to delete.</param>
    /// <returns>No Content (204) if successful, or an appropriate error response.</returns>
    /// <response code="204">No Content - Campaign successfully deleted.</response>
    /// <response code="400">Bad Request - Invalid campaign ID.</response>
    /// <response code="404">Not Found - Campaign not found with the specified ID.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCampaignAsync(string id)
    {
        if (!Guid.TryParse(id, out _))
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid Campaign ID",
                Detail = "Invalid campaign ID."
            });
        }

        if (!await _campaignService.DeleteCampaignAsync(id))
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Gets donation progress for a campaign.
    /// </summary>
    /// <param name="id">The ID of the campaign.</param>
    /// <returns>The donation progress, or a 404 Not Found result if not found.</returns>
    /// <response code="200">Returns the donation progress.</response>
    /// <response code="400">Bad Request - Invalid campaign ID.</response>
    /// <response code="404">Not Found - Campaign not found with the specified ID.</response>
    [HttpGet("{id}/progress")]
    [ProducesResponseType(typeof(DonationProgress), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCampaignDonationProgressAsync(string id)
    {
        if (!Guid.TryParse(id, out _))
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid Campaign ID",
                Detail = "Invalid campaign ID."
            });
        }

        var progress = await _campaignService.GetCampaignDonationProgressAsync(id);

        if (progress == null)
        {
            return NotFound();
        }

        return Ok(progress);
    }
}