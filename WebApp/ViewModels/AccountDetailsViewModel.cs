using Infrastructure.Entities;
using Infrastructure.Models;
using System.ComponentModel.DataAnnotations;
using WebApp.Filters;

namespace WebApp.ViewModels;

public class AccountDetailsViewModel
{
    public AccountBasicInfo? BasicInfo { get; set; }
    public AccountAddressInfo? AddressInfo { get; set; }

    public ChangePasswordViewModel? ChangePassword { get; set; }

    public List<CourseModel>? Course { get; set; }

    public DeleteAccount? DeleteAccount { get; set; }
}
public class SaveCourseViewModel
{

    public string UserId { get; set; } = null!;
    public UserEntity User { get; set; } = null!;

    public int CourseId { get; set; }
}
public class AccountBasicInfo
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
    public string? ProfileImage { get; set; } = "avatar.png";

    [DataType(DataType.Text)]
    [Display(Name = "Phone", Prompt = "Enter your phone")]
    public string? PhoneNumber { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Bio", Prompt = "Add a short bio...")]
    public string? Bio { get; set; }
    public bool IsExternalAccount { get; set; }

}
public class AccountAddressInfo
{
    [DataType(DataType.Text)]
    [Display(Name = "Address", Prompt = "Enter your address")]
    [Required(ErrorMessage = "Enter an valid address")]
    [MinLength(2, ErrorMessage = "Enter an valid address")]
    public string AddressLine_1 { get; set; } = null!;

    [DataType(DataType.Text)]
    [Display(Name = "Address", Prompt = "Enter your second address")]
    public string? AddressLine_2 { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Postal code", Prompt = "Enter your postal code")]
    [Required(ErrorMessage = "Enter an valid postal code")]
    [MinLength(2, ErrorMessage = "Enter an valid postal code")]
    public string PostalCode { get; set; } = null!;

    [DataType(DataType.Text)]
    [Display(Name = "City", Prompt = "Enter your city")]
    [Required(ErrorMessage = "Enter an valid city")]
    [MinLength(2, ErrorMessage = "Enter an valid city")]
    public string City { get; set; } = null!;


}

public class ChangePasswordViewModel
{
    [DataType(DataType.Password)]
    [Display(Name = "Current password", Prompt = "Enter your old password")]
    public string OldPassword { get; set; } = null!;

    [DataType(DataType.Password)]
    [Display(Name = "New Password", Prompt = "Enter your new password")]
    [Required(ErrorMessage = "Enter a valid new password")]
    public string NewPassword { get; set; } = null!;

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password", Prompt = "Confirm your new password")]
    [Required(ErrorMessage = "The new password must be confirmed")]
    [Compare(nameof(NewPassword), ErrorMessage = "The new password must be confirmed")]
    public string ConfirmNewPassword { get; set; } = null!;
}

public class DeleteAccount
{
    [CheckboxRequierd(ErrorMessage = "You must check this box to proceed with account deletion.")]
    [Display(Name = "Yes, I want to delete my account.", Prompt = "Check this box to confirm account deletion.")]
    public bool Delete { get; set; }

}
