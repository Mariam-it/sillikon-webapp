
using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public class WebAppContext(DbContextOptions<WebAppContext> options) : IdentityDbContext<UserEntity>(options)
{
    public DbSet<AddressEntity> Addresses { get; set; }
    public DbSet<SaveCourseEntity> SaveCourses { get; set; }
    public DbSet<EnrollmentEntity> Enrollments { get; set; }
}
