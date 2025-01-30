using System.ComponentModel.DataAnnotations;

namespace TesfaFundApp.Models;

/// <summary>
/// Validates if a string value is a valid GUID.  Null values are considered valid.
/// </summary>
public class ValidGuidAttribute : ValidationAttribute
{
    /// <summary>
    /// Determines whether the specified value is a valid GUID.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <returns><c>true</c> if the value is a valid GUID or null; otherwise, <c>false</c>.</returns>
    public override bool IsValid(object value)
    {
        // Null is valid to accmodate empty ids on creation requests.
        if (value == null) return true; 

        return value is string guidString && Guid.TryParse(guidString, out _);
    }

    /// <summary>
    /// Formats the error message.
    /// </summary>
    /// <param name="name">The name of the property being validated.</param>
    /// <returns>The formatted error message.</returns>
    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be a valid GUID.";
    }
}