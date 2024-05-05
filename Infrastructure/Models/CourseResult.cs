namespace Infrastructure.Models;

public class CourseResult
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public bool Succeeded { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public IEnumerable<Course>? Courses { get; set; }
}
