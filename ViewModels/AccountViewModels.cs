using System.ComponentModel.DataAnnotations;

namespace CandidateRegistrationPortal.ViewModels;

public class RegisterViewModel
{
    [Required, StringLength(30, MinimumLength = 3)]
    [RegularExpression("^[A-Za-z0-9_.-]+$", ErrorMessage = "Use letters, numbers, dot, underscore or hyphen.")]
    public string UserName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    [RegularExpression("^[A-Za-z .'-]+$", ErrorMessage = "Name can contain alphabetic characters and permitted special characters only.")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^[0-9]{10}$", ErrorMessage = "Mobile number must contain exactly 10 digits.")]
    public string Mobile { get; set; } = string.Empty;

    [Required, DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [Required]
    public string Captcha { get; set; } = string.Empty;
}

public class LoginViewModel
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required, StringLength(6, MinimumLength = 6)]
    [RegularExpression("^[0-9]{6}$")]
    public string OTP { get; set; } = string.Empty;

    [Required]
    public string Captcha { get; set; } = string.Empty;
}
