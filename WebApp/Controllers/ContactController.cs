using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using WebApp.ViewModels;

namespace WebApp.Controllers;

public class ContactController(HttpClient httpClient) : Controller
{
    private readonly HttpClient _httpClient = httpClient;
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Contact(ContactViewModel contactViewModel)
    {
        if (ModelState.IsValid)
        {
            var content = new StringContent(JsonConvert.SerializeObject(contactViewModel), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://localhost:7049/api/Contact?key=sRj6J7NcV5fSL2F600m5qW68QcyWOANjkcwoaMGQuKW3HLrkdGFf8H0mLIOhNkHnlMfNW8HZQGkzgBfi86T7waOUjo6gyNUgy6s1CHGU0YMlkPmEqOrFn2mzcUCdbVUQ", content);
            if (response.IsSuccessStatusCode)
            {
                TempData["StatusMessage"] = "your message has been sent successfully";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                TempData["StatusMessage"] = "You are already subscribed";
            }
        }
        else
        {
            TempData["StatusMessage"] = "Invalid email address";
        }
        return RedirectToAction("Index", "Contact");
    }
}
