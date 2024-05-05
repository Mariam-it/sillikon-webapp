using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels;

public class EnrollmentModel
{
    public string UserId { get; set; } = null!;

    public int CourseId { get; set; }
}
