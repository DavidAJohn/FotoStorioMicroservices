using System.ComponentModel.DataAnnotations;

namespace Store.BlazorWasm.Models;

public class RegisterModel
{
    // Fluent Validation provided by ./Validators/RegistrationValidator

    public string DisplayName { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string ConfirmPassword { get; set; }
}
