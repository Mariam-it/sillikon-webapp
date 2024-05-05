using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public class ErrorController : Controller
{
    [Route("/StatusCodeError/{statusCode}")]
    public IActionResult Error404(int statusCode)
    {
        if (statusCode == 404)
        {
            ViewBag.ErrorMessage = "404 Page Not Found Exeption";
        }
        return View("Error404");
    }

}
