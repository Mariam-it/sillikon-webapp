using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels;

public class SignInViewModel
{
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email Address", Prompt = "Enter your email address")]
    [Required(ErrorMessage = "Enter an valid email address")]
    public string Email { get; set; } = null!;

    [DataType(DataType.Password)]
    [Display(Name = "Password", Prompt = "Enter your password")]
    [Required(ErrorMessage = "Enter a valid password")]
    public string Password { get; set; } = null!;
    public bool IsPresistent { get; set; }
}
