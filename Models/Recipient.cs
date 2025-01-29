namespace TesfaFundApp;

public class Recipient
{
    public String? Id { get; set; }
    public String? FirstName { get; set; }
    public String? MiddleName { get; set; }
    public String? LastName { get; set; }
    public String? Email { get; set; }
    public String? PasswordHash { get; set; }
}

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class RecipientFilterParams
{
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
}
