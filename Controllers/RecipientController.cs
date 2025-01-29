
using Microsoft.AspNetCore.Mvc;
using TesfaFundApp.Services;

namespace TesfaFundApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipientController : ControllerBase
{

    private readonly IRecipientService _recipientService;

    public RecipientController(IRecipientService recipientService)
    {
        _recipientService = recipientService;
    }

    /// <summary>
    /// Creates a new recipient.
    /// </summary>
    /// <param name="recipient">The recipient object to create.</param>
    /// <returns>Returns the created recipient.</returns>
    /// <response code="201">The recipient was successfully created.</response>
    /// <response code="400">Invalid recipient data.</response>
    /// <response code="500">Failed to create recipient.</response>
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> CreateRecipientAsync([FromBody] Recipient recipient)
    {
        if (recipient == null)
        {
            return BadRequest("Recipient data is invalid.");
        }

        var createdRecipient = await _recipientService.CreateRecipientAsync(recipient);
        if (createdRecipient == null)
        {
            return StatusCode(500, "Failed to create recipient.");
        }

        return CreatedAtAction(nameof(GetRecipientByIdAsync), new { id = createdRecipient.Id }, createdRecipient);
    }

    /// <summary>
    /// Retrieves a recipient by ID.
    /// </summary>
    /// <param name="id">The ID of the recipient to fetch.</param>
    /// <returns>Returns the recipient details.</returns>
    /// <response code="200">Returns the recipient details.</response>
    /// <response code="404">Recipient not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetRecipientByIdAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest("Recipient ID is required.");
        }

        var recipient = await _recipientService.GetRecipientByIdAsync(id);
        if (recipient == null)
        {
            return NotFound($"Recipient with ID {id} not found.");
        }

        return Ok(recipient);
    }

    /// <summary>
    /// Updates an existing recipient.
    /// </summary>
    /// <param name="id">The ID of the recipient to update.</param>
    /// <param name="recipient">The updated recipient object.</param>
    /// <returns>Returns a 204 No Content response if the update is successful.</returns>
    /// <response code="204">Recipient updated successfully.</response>
    /// <response code="400">Invalid recipient data.</response>
    /// <response code="404">Recipient not found.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateRecipientAsync(string id, [FromBody] Recipient recipient)
    {
        if (string.IsNullOrEmpty(id) || recipient == null)
        {
            return BadRequest("Invalid recipient data.");
        }

        var updated = await _recipientService.UpdateRecipientAsync(id, recipient);
        if (updated)
        {
            return NoContent();
        }

        return NotFound($"Recipient with ID {id} not found.");
    }

    /// <summary>
    /// Deletes a recipient.
    /// </summary>
    /// <param name="id">The ID of the recipient to delete.</param>
    /// <returns>Returns a 204 No Content response if the delete is successful.</returns>
    /// <response code="204">Recipient deleted successfully.</response>
    /// <response code="404">Recipient not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteRecipientAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest("Recipient ID is required.");
        }

        var result = await _recipientService.DeleteRecipientAsync(id);
        if (result)
        {
            return NoContent();
        }

        return NotFound($"Recipient with ID {id} not found.");
    }

    /// <summary>
    /// Retrieves all recipients with optional search and filter parameters.
    /// </summary>
    /// <param name="filterParams">Optional filter parameters for searching recipients.</param>
    /// <returns>Returns a list of recipients based on the filter criteria.</returns>
    /// <response code="200">Returns a list of recipients.</response>
    /// <response code="204">No recipients found.</response>
    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(204)]
    public async Task<IActionResult> GetAllRecipientsAsync([FromQuery] RecipientFilterParams filterParams)
    {
        var recipients = await _recipientService.GetAllRecipientsAsync(filterParams);
        if (recipients == null || !recipients.Any())
        {
            return NoContent();
        }
        return Ok(recipients);
    }
}