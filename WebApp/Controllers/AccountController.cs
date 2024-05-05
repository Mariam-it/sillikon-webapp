using Infrastructure.Contexts;
using Infrastructure.Entities;
using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Claims;
using WebApp.ViewModels;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace WebApp.Controllers;

[Authorize]
public class AccountController(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager, WebAppContext context, HttpClient httpClient, CourseService courseService) : Controller
{
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly SignInManager<UserEntity> _signInManager = signInManager;
    private readonly WebAppContext _context = context;
    private readonly HttpClient _httpClient = httpClient;
    private readonly CourseService _courseService = courseService;

    public async Task<IActionResult> Details()
    {
        var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var user = await _context.Users.Include(i => i.Address).Include(s => s.SaveCourses).FirstOrDefaultAsync(x => x.Id == nameIdentifier);


        var viewModel = new AccountDetailsViewModel
        {
            BasicInfo = new AccountBasicInfo
            {
                FirstName = user!.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
                Bio = user.Bio,
                ProfileImage = user.ProfileImage,
                IsExternalAccount = user.IsExternalAccount,

            },
            AddressInfo = new AccountAddressInfo
            {
                AddressLine_1 = user.Address?.AddressLine_1!,
                AddressLine_2 = user.Address?.AddressLine_2!,
                PostalCode = user.Address?.PostalCode!,
                City = user.Address?.City!,

            }
        };
        if (user.SaveCourses != null)
        {
            if (HttpContext.Request.Cookies.TryGetValue("AccessToken", out var token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var savedCourses = await _context.Users.Where(u => u.Id == userId).Include(c => c.SaveCourses).ToListAsync();

                // Hämta detaljer för varje sparad kurs från webb-API:en
                var courses = new List<CourseModel>(); // Listan där du lagrar alla detaljerade kurser
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
                viewModel.Course = courses;
            }
        }

        return View(viewModel);
    }
    [HttpPost]
    public async Task<IActionResult> UpdateBasicInfo(AccountDetailsViewModel model)
    {
        if (TryValidateModel(model.BasicInfo!))
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                user.FirstName = model.BasicInfo!.FirstName;
                user.LastName = model.BasicInfo!.LastName;
                user.Email = model.BasicInfo!.Email;
                user.PhoneNumber = model.BasicInfo!.PhoneNumber;
                user.Bio = model.BasicInfo!.Bio;
                user.UserName = model.BasicInfo!.Email;
                user.ProfileImage = model.BasicInfo!.ProfileImage;
                user.IsExternalAccount = model.BasicInfo!.IsExternalAccount;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["StatusMessage"] = "Updated basic information successfully.";
                }
                else
                {
                    TempData["StatusMessage"] = "Unable to update basic information.";
                }
            }
        }
        else
        {
            TempData["StatusMessage"] = "Unable to save basic information.";
        }

        return RedirectToAction("Details");
    }
    [HttpPost]
    public async Task<IActionResult> UpdateAddressInfo(AccountDetailsViewModel model)
    {
        if (TryValidateModel(model.AddressInfo!))
        {
            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await _context.Users.Include(i => i.Address).FirstOrDefaultAsync(x => x.Id == nameIdentifier);
            if (user != null)
            {
                try
                {
                    if (user.Address != null)
                    {
                        user.Address.AddressLine_1 = model.AddressInfo!.AddressLine_1;
                        user.Address.AddressLine_2 = model.AddressInfo!.AddressLine_2;
                        user.Address.PostalCode = model.AddressInfo!.PostalCode;
                        user.Address.City = model.AddressInfo!.City;
                    }
                    else
                    {
                        user.Address = new AddressEntity
                        {
                            AddressLine_1 = model.AddressInfo!.AddressLine_1,
                            AddressLine_2 = model.AddressInfo!.AddressLine_2,
                            PostalCode = model.AddressInfo!.PostalCode,
                            City = model.AddressInfo!.City,
                        };
                    }
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    TempData["StatusMessage"] = "Updated address information successfully.";
                }
                catch (Exception ex)
                {
                    TempData["StatusMessage"] = "Unable to update address information.";
                }
            }
        }
        else
        {
            TempData["StatusMessage"] = "Unable to save address information.";
        }

        return RedirectToAction("Details");
    }

    [HttpPost]
    public async Task<IActionResult> UploadProfileImage(IFormFile file)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user != null && file != null && file.Length != 0)
        {
            var fileName = $"p_{user.Id}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/uploads/profiles", fileName);

            using var fs = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(fs);

            user.ProfileImage = fileName;
            await _userManager.UpdateAsync(user);
        }
        else
        {
            TempData["StatusMessage"] = "Unable to upload profile image.";
        }
        return RedirectToAction("Details", "Account");
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(AccountDetailsViewModel model)
    {
        if (!TryValidateModel(model.ChangePassword!))
        {
            return View("Details");
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.ChangePassword!.OldPassword, model.ChangePassword!.NewPassword);
        if (!changePasswordResult.Succeeded)
        {
            foreach (var error in changePasswordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View("Details", model);
        }

        await _signInManager.RefreshSignInAsync(user);
        TempData["StatusMessage"] = "Password changed successfully.";

        return RedirectToAction("Details");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAccount(DeleteAccount deleteAccount)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    TempData["StatusMessage"] = "Your account has been deleted.";
                    return RedirectToAction("SignOut", "Auth");
                }
                else
                {
                    TempData["StatusMessage"] = "Unable to delete your account.";
                    return RedirectToAction("Details", "Account", "#delete-user-account");
                }
            }

            return RedirectToAction("Index", "Home");
        }
        return RedirectToAction("Details");

    }



}