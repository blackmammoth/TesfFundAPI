
using Microsoft.AspNetCore.Mvc;
using TesfaFundApp.Services;
using TesfaFundApp.Models;
using MongoDB.Bson;

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
    [ProducesResponseType(typeof(Recipient), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateRecipientAsync([FromBody] Recipient recipient)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (errorMessage, createdRecipient) = await _recipientService.CreateRecipientAsync(recipient);

        if (errorMessage != null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Recipient Creation Failed",
                Detail = errorMessage
            });
        }
        // DEBUG
        Console.WriteLine($"Created Recipient: {createdRecipient.ToJson()}");
        Console.WriteLine($"Route Values: id = {createdRecipient.Id}");

        return CreatedAtRoute(nameof(GetRecipientByIdAsync), new { id = createdRecipient!.Id }, createdRecipient);
    }

    /// <summary>
    /// Retrieves a recipient by ID.
    /// </summary>
    /// <param name="id">The ID of the recipient to fetch.</param>
    /// <returns>Returns the recipient details.</returns>
    /// <response code="200">Returns the recipient details.</response>
    /// <response code="400">Bad Request - Invalid recipient ID provided.</response>
    /// <response code="404">Recipient not found.</response>
    [HttpGet("{id}", Name = "GetRecipientByIdAsync")]
    [ProducesResponseType(typeof(Recipient), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRecipientByIdAsync(string id)
    {
        if (!Guid.TryParse(id, out _))
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid Recipient ID",
                Detail = $"'{id}' is not a valid Recipient ID."
            });
        }

        var recipient = await _recipientService.GetRecipientByIdAsync(id);

        if (recipient == null)
        {
            return NotFound();
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
    /// <response code="400">Bad Request - Invalid recipient data or ID.</response>
    /// <response code="404">Recipient not found.</response>
    /// <response code="500">Internal Server Error - An unexpected error occurred.</response> 
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateRecipientAsync(string id, [FromBody] Recipient recipient)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!Guid.TryParse(id, out _))
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid Recipient ID",
                Detail = $"'{id}' is not a valid Recipient ID."
            });
        }

        var (errorMessage, _) = await _recipientService.UpdateRecipientAsync(id, recipient);

        if (errorMessage == "Recipient not found or not updated.")
        {
            return NotFound();
        }
        else if (errorMessage != null)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Recipient Update Failed",
                Detail = errorMessage
            });
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes a recipient.
    /// </summary>
    /// <param name="id">The ID of the recipient to delete.</param>
    /// <returns>Returns a 204 No Content response if the delete is successful.</returns>
    /// <response code="204">Recipient deleted successfully.</response>
    /// <response code="400">Bad Request - Invalid recipient ID or recipient has associated campaigns.</response>
    /// <response code="404">Recipient not found.</response>
    /// <response code="500">Internal Server Error - An unexpected error occurred.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteRecipientAsync(string id)
    {
        if (!Guid.TryParse(id, out _))
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid Recipient ID",
                Detail = $"'{id}' is not a valid Recipient ID."
            });
        }

        var errorMessage = await _recipientService.DeleteRecipientAsync(id);

        if (errorMessage == "Recipient not found or not updated.")
        {
            return NotFound();
        }
        else if (errorMessage != null && errorMessage.Contains("has associated campaigns"))
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Recipient Deletion Failed",
                Detail = errorMessage
            });
        }
        else if (errorMessage != null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Recipient Deletion Failed",
                Detail = errorMessage
            });
        }

        return NoContent();
    }

    /// <summary>
    /// Retrieves all recipients with optional search and filter parameters.
    /// </summary>
    /// <param name="filters">Optional filter parameters for searching recipients.</param>
    /// <returns>Returns a list of recipients based on the filter criteria.</returns>
    /// <response code="200">Returns a list of recipients.</response>
    /// <response code="400">Bad Request - Invalid filter parameters.</response>
    /// <response code="500">Internal Server Error - An unexpected error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Recipient>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllRecipientsAsync([FromQuery] RecipientFilterParams? filters)
    {
        if (filters != null && !ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var recipients = await _recipientService.GetAllRecipientsAsync(filters);

        return Ok(recipients ?? new List<Recipient>());
    }
}