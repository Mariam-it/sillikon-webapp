using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.ViewModels;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Infrastructure.Contexts;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;

namespace WebApp.Controllers;

[Authorize]
public class CoursesController(CategoryService categoryService, CourseService courseService, HttpClient httpClient, UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager, WebAppContext context) : Controller
{
    private readonly CategoryService _categoryService = categoryService;
    private readonly CourseService _courseService = courseService;
    private readonly HttpClient _httpClient = httpClient;
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly SignInManager<UserEntity> _signInManager = signInManager;
    private readonly WebAppContext _context = context;


    public async Task<IActionResult> Index(string category = "", string searchQuery = "", int pageNumber = 1, int pageSize = 6)
    {
        if (HttpContext.Request.Cookies.TryGetValue("AccessToken", out var token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var courseResult = await _courseService.GetCoursesAsync(category, searchQuery, pageNumber, pageSize);

            var viewModel = new CourseIndexViewModel
            {
                Categories = await _categoryService.GetCategoriesAsync(),
                Courses = courseResult.Courses,
                Pagination = new Pagination
                {
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = courseResult.TotalPages,
                    TotalItems = courseResult.TotalItems
                }
            };

            return View(viewModel);

        }

        return View();
    }
    public async Task<IActionResult> Details(int id)
    {
        if (HttpContext.Request.Cookies.TryGetValue("AccessToken", out var token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var course = await _courseService.GetCourseByIdAsync(id);

            if (course != null)
            {
                // Hämta användarens ID från identitetskontexten
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Hämta den specifika Enrollment för användaren och kursen
                var enrollment = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.CourseId == id);
                bool isEnromment;
                if (enrollment != null)
                {
                    isEnromment = true;
                }
                else
                {
                    isEnromment = false;
                }

                // Skapa ett vymodell-objekt för att skicka både kursen och användarens Enrollment till vyn
                var viewModel = new CourseDetailsViewModel
                {
                    Course = course,
                    Enrollment = new EnrollmentModel
                    {
                        CourseId = course.Id,
                        UserId = userId!,

                    },
                    IsEnrollment = isEnromment
                };

                return View(viewModel);
            }
            else
            {
                return NotFound();
            }
        }

        return View();
    }


    public async Task<IActionResult> SavedCourses()
    {
        if (HttpContext.Request.Cookies.TryGetValue("AccessToken", out var token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var savedCourses = await _context.Users.Where(u => u.Id == userId).Include(c => c.SaveCourses).ToListAsync();

            var courses = new List<CourseModel>();
            foreach (var savedCourse in savedCourses)
            {
                foreach (var courseId in savedCourse.SaveCourses!)
                {
                    var courseModel = await _courseService.GetCourseByIdAsync(courseId.CourseId);
                    if (courseModel != null)
                    {
                        var course = new CourseModel
                        {
                            Id = courseModel.Id,
                            Image = courseModel.Image,
                            Title = courseModel.Title,
                            Author = courseModel.Author,
                            Price = courseModel.Price,
                            DiscountPrice = courseModel.DiscountPrice,
                            Hours = courseModel.Hours,
                            LikesProcent = courseModel.LikesProcent,
                            LikesInNumber = courseModel.LikesInNumber

                        };
                        courses.Add(course);
                    }
                }

            }

            return RedirectToAction("Details", "Account", "saved-courses");
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SaveCourse(int courseId)
    {
        // Hämta användarens ID från identitetskontexten
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Kontrollera om kursen redan är sparad för användaren
        var existingSavedCourse = await _context.SaveCourses
            .FirstOrDefaultAsync(sc => sc.UserId == userId && sc.CourseId == courseId);

        if (existingSavedCourse != null)
        {
            // Kursen är redan sparad, skicka ett popup-meddelande
            TempData["CourseSavedMessage"] = "Course already saved";
            return RedirectToAction("Index");
        }

        // Skapa en instans av SavedCourseEntity
        var savedCourse = new SaveCourseEntity
        {
            UserId = userId!,
            CourseId = courseId
        };

        // Lägg till den sparade kursen i databasen
        _context.SaveCourses.Add(savedCourse);
        await _context.SaveChangesAsync();

        // Återvänd till den sida där användaren kom ifrån
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> RemoveCourse(int courseId)
    {
        // Hämta användarens ID från identitetskontexten
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Hitta den sparade kursen för den aktuella användaren
        var savedCourse = await _context.SaveCourses
            .FirstOrDefaultAsync(sc => sc.UserId == userId && sc.CourseId == courseId);

        if (savedCourse == null)
        {
            // Om kursen inte finns, returnera en felmeddelande
            return RedirectToAction("Details", "Account");
        }

        // Ta bort den sparade kursen från databasen
        _context.SaveCourses.Remove(savedCourse);
        await _context.SaveChangesAsync();

        // Skicka ett meddelande om att kursen har tagits bort
        TempData["CourseRemovedMessage"] = "Course removed successfully";

        // Återvänd till den sida där användaren kom ifrån
        return RedirectToAction("Details", "Account");
    }

    [HttpPost]
    public async Task<IActionResult> RemoveAllCourses()
    {
        // Hämta användarens ID från identitetskontexten
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Hämta alla sparade kurser för den aktuella användaren
        var savedCourses = await _context.SaveCourses
            .Where(sc => sc.UserId == userId)
            .ToListAsync();

        if (savedCourses == null || savedCourses.Count == 0)
        {
            return RedirectToAction("Details", "Account");
        }

        // Ta bort alla sparade kurser från databasen
        _context.SaveCourses.RemoveRange(savedCourses);
        await _context.SaveChangesAsync();

        // Skicka ett meddelande om att alla kurser har tagits bort
        TempData["AllCoursesRemovedMessage"] = "All saved courses removed successfully";

        // Återvänd till den sida där användaren kom ifrån
        return RedirectToAction("Details", "Account");
    }


    [HttpPost]
    public async Task<IActionResult> JoinCourse(int courseId)
    {
        // Hämta användarens ID från identitetskontexten
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Kontrollera om användaren redan är med i kursen
        var existingEnrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

        if (existingEnrollment != null)
        {
            // Användaren är redan med i kursen, skicka ett meddelande
            TempData["AlreadyEnrolledMessage"] = "You are already enrolled in this course";
            return RedirectToAction("Index");
        }

        // Skapa en ny instans av EnrollmentEntity för att gå med i kursen
        var enrollment = new EnrollmentEntity
        {
            UserId = userId!,
            CourseId = courseId
        };

        // Lägg till användarens anmälan till kursen i databasen
        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();

        // Skicka ett meddelande om att användaren har gått med i kursen
        TempData["EnrollmentSuccessMessage"] = "You have successfully joined the course";

        // Återvänd till den sida där användaren kom ifrån
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> LeaveCourse(int courseId)
    {
        // Hämta användarens ID från identitetskontexten
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Hitta användarens deltagande i den angivna kursen
        var enrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

        if (enrollment == null)
        {
            return RedirectToAction("Index");
        }

        // Ta bort användarens deltagande från kursen
        _context.Enrollments.Remove(enrollment);
        await _context.SaveChangesAsync();

        // Skicka ett meddelande om att användaren har lämnat kursen
        TempData["LeaveCourseMessage"] = "You have left the course";

        return RedirectToAction("Index");
    }
}