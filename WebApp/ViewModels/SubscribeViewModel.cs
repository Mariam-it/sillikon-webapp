using System.ComponentModel.DataAnnotations;
using WebApp.Filters;

namespace WebApp.ViewModels;

public class SubscribeViewModel
{
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email Address", Prompt = "Enter your email address")]
    [Required(ErrorMessage = "Enter an valid email address")]
    public string Email { get; set; } = null!;

    public bool DailNewsletter { get; set; }
    public bool AdvertisingUpdate { get; set; }
    public bool WeekingReview { get; set; }
    public bool EventUpdates { get; set; }
    public bool StartupWeekly { get; set; }
    public bool Podcasts { get; set; }
}
