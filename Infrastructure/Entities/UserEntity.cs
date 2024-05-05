using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Entities;

public class UserEntity : IdentityUser
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? ProfileImage { get; set; } = "avatar.png";
    public string? Bio { get; set; }

    public int? AddressId { get; set; }
    public AddressEntity? Address { get; set; }
    public bool IsExternalAccount { get; set; } = false;

    public List<SaveCourseEntity>? SaveCourses { get; set; }
    public List<EnrollmentEntity>? Enrollments { get; set; }
}
