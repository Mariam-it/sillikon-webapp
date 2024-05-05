using System.ComponentModel.DataAnnotations;
using WebApp.Filters;

namespace WebApp.ViewModels;

public class SignUpViewModel
{
    [DataType(DataType.Text)]
    [Display(Name = "First name", Prompt = "Enter your first name")]
    [Required(ErrorMessage = "Enter a first name")]
    [MinLength(2, ErrorMessage = "Enter a first name")]
    public string FirstName { get; set; } = null!;

    [DataType(DataType.Text)]
    [Display(Name = "Last name", Prompt = "Enter your last name")]
    [Required(ErrorMessage = "Enter a last name")]
    [MinLength(2, ErrorMessage = "Enter a last name")]
    public string LastName { get; set; } = null!;

    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email Address", Prompt = "Enter your email address")]
    [Required(ErrorMessage = "Enter an valid email address")]
    public string Email { get; set; } = null!;

    [DataType(DataType.Password)]
    [Display(Name = "Password", Prompt = "Enter your password")]
    [Required(ErrorMessage = "Enter a valid password")]
    public string Password { get; set; } = null!;

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password", Prompt = "Confirm your password")]
    [Required(ErrorMessage = "Password must be confirmed")]
    [Compare(nameof(Password), ErrorMessage = "Password must be confirmed")]
    public string ConfirmPassword { get; set; } = null!;

    [CheckboxRequierd(ErrorMessage = "You must accept the terms and conditions to proceed.")]
    [Display(Name = "I  agree to the Terms & Conditions.", Prompt = "Terms and Conditions")]
    public bool TermsAndConditions { get; set; }

}
