using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels;

public class ContactViewModel
{
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email Address", Prompt = "Enter your email address")]
    [Required(ErrorMessage = "Enter an valid email address")]
    public string Email { get; set; } = null!;

    [DataType(DataType.Text)]
    [Display(Name = "Name", Prompt = "Enter your name")]
    [Required(ErrorMessage = "Enter a name")]
    [MinLength(2, ErrorMessage = "Enter a name")]
    public string Name { get; set; } = null!;

    [DataType(DataType.MultilineText)]
    [Display(Name = "Message", Prompt = "Enter your message")]
    [Required(ErrorMessage = "Enter a message with at least 10 characters")]
    [MinLength(10, ErrorMessage = "Enter a message with at least 10 characters")]
    public string Message { get; set; } = null!;
}
