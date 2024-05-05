using Infrastructure.Models;

namespace WebApp.ViewModels;

public class CourseDetailsViewModel
{
    public CourseModel? Course { get; set; }
    public EnrollmentModel? Enrollment { get; set; }

    public bool IsEnrollment { get; set; } = false;
}
