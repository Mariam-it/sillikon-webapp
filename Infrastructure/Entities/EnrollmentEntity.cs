
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities;

public class EnrollmentEntity
{
    [Key]
    public int Id { get; set; }

    [MaxLength]
    public string UserId { get; set; } = null!;
    public UserEntity User { get; set; } = null!;

    public int CourseId { get; set; }
}
