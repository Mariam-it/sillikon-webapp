using Infrastructure.Contexts;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using WebApp.ViewModels;

namespace WebApp.Controllers;

public class AuthController(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager, WebAppContext context, HttpClient httpClient, IConfiguration configuration) : Controller
{
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly SignInManager<UserEntity> _signInManager = signInManager;
    private readonly WebAppContext _context = context;
    private readonly HttpClient _httpClient = httpClient;
    private readonly IConfiguration _configuration = configuration;

    #region Account | Sign Up
    [Route("/signup")]
    public IActionResult SignUp()
    {
        return View();
    }
    [HttpPost]
    [Route("/signup")]
    public async Task<IActionResult> SignUp(SignUpViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (!await _context.Users.AnyAsync(x => x.Email == model.Email))
            {
                var userEntity = new UserEntity
                {
                    Email = model.Email,
                    UserName = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                };
                if ((await _userManager.CreateAsync(userEntity, model.Password)).Succeeded)
                {
                    if ((await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false)).Succeeded)
                        return LocalRedirect("/");
                    else
                        return LocalRedirect("/signin");
                }
                else
                {
                    ViewData["StatusMessage"] = "Somthing went wrong. Try again later or contact customer service";
                }
            }
            else
            {
                ViewData["StatusMessage"] = "User with the same email already exists";
            }

        }
        return View(model);
    }
    #endregion

    #region Account | Sign In
    [Route("/signin")]
    public IActionResult SignIn(string returnUrl)
    {
        ViewData["ReturnUrl"] = returnUrl ?? "/";
        return View();
    }
    [HttpPost]
    [Route("/signin")]
    public async Task<IActionResult> SignIn(SignInViewModel model, string returnUrl)
    {
        if (ModelState.IsValid)
        {
            if ((await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.IsPresistent, false)).Succeeded)
            {
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"https://localhost:7049/api/Auth/token?key={_configuration["ApiKeys:Key"]}", content);
                if (response.IsSuccessStatusCode)
                {
                    var token = await response.Content.ReadAsStringAsync();
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        Expires = DateTime.Now.AddDays(1)
                    };
                    Response.Cookies.Append("AccessToken", token, cookieOptions);
                }

                return LocalRedirect(returnUrl);
            }

        }
        ViewData["ReturnUrl"] = returnUrl;
        ViewData["StatusMessage"] = "Incorrect email or password";
        return View();
    }

    #endregion

    #region Account | Sign Out

    [Route("/signout")]
    public new async Task<IActionResult> SignOut()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    #endregion

    #region External Account | Facebook
    [HttpGet]
    public IActionResult Facebook()
    {
        var authProps = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", Url.Action("FacebookCallback"));
        return new ChallengeResult("Facebook", authProps);
    }

    [HttpGet]
    public async Task<IActionResult> FacebookCallback()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info != null)
        {
            var userEntity = new UserEntity
            {
                FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName)!,
                LastName = info.Principal.FindFirstValue(ClaimTypes.Surname)!,
                Email = info.Principal.FindFirstValue(ClaimTypes.Email)!,
                UserName = info.Principal.FindFirstValue(ClaimTypes.Email)!,
                IsExternalAccount = true,
            };

            var user = await _userManager.FindByEmailAsync(userEntity.Email);
            if (user == null)
            {
                var result = await _userManager.CreateAsync(userEntity);
                if (result.Succeeded)
                    user = await _userManager.FindByEmailAsync(userEntity.Email);
            }

            if (user != null)
            {
                if (user.FirstName != userEntity.FirstName || user.LastName != userEntity.LastName || user.Email != userEntity.Email)
                {
                    user.FirstName = userEntity.FirstName;
                    user.LastName = userEntity.LastName;
                    user.Email = userEntity.Email;

                    await _userManager.UpdateAsync(user);
                }
                await _signInManager.SignInAsync(user, isPersistent: false);

                if (HttpContext.User != null)
                    return RedirectToAction("Details", "Account");
            }
        }
        ModelState.AddModelError("InvalidFacebookAuthentication", "Failed to authenticate with Facebook.");
        ViewData["StatusMessage"] = "Failed to authenticate with Facebook";
        return RedirectToAction("SignIn", "Auth");
    }
    #endregion

    #region External Account | Google

    // Google inloggning
    [HttpGet]
    public IActionResult Google()
    {
        var authProps = _signInManager.ConfigureExternalAuthenticationProperties("Google", Url.Action("GoogleCallback"));
        return new ChallengeResult("Google", authProps);
    }

    [HttpGet]
    public async Task<IActionResult> GoogleCallback()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info != null)
        {
            var userEntity = new UserEntity
            {
                FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName)!,
                LastName = info.Principal.FindFirstValue(ClaimTypes.Name)!,
                Email = info.Principal.FindFirstValue(ClaimTypes.Email)!,
                UserName = info.Principal.FindFirstValue(ClaimTypes.Email)!,
                IsExternalAccount = true,
            };

            var user = await _userManager.FindByEmailAsync(userEntity.Email);
            if (user == null)
            {
                var result = await _userManager.CreateAsync(userEntity);
                if (result.Succeeded)
                    user = await _userManager.FindByEmailAsync(userEntity.Email);
            }

            if (user != null)
            {
                if (user.FirstName != userEntity.FirstName || user.LastName != userEntity.LastName || user.Email != userEntity.Email)
                {
                    user.FirstName = userEntity.FirstName;
                    user.LastName = userEntity.LastName;
                    user.Email = userEntity.Email;

                    await _userManager.UpdateAsync(user);
                }
                await _signInManager.SignInAsync(user, isPersistent: false);

                if (HttpContext.User != null)
                    return RedirectToAction("Details", "Account");
            }
        }
        ModelState.AddModelError("InvalidGoogleAuthentication", "Failed to authenticate with Google.");
        ViewData["StatusMessage"] = "Failed to authenticate with Google";
        return RedirectToAction("SignIn", "Auth");
    }


    #endregion
}
