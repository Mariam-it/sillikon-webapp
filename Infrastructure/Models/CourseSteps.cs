
namespace Infrastructure.Models;

public class CourseSteps
{
    public int Id { get; set; }
    public string StepTitle { get; set; } = null!;
    public string StepDescription { get; set; } = null!;
    public string StepNumber { get; set; } = null!;
}
