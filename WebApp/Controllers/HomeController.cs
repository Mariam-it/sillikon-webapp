using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class HomeController(HttpClient httpClient) : Controller
    {
        private readonly HttpClient _httpClient = httpClient;

        public IActionResult Index()
        {
            TempData["StatusMessage"] = "* yes, I agree to the terms and policy.";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(SubscribeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("https://localhost:7049/api/subscribe?key=sRj6J7NcV5fSL2F600m5qW68QcyWOANjkcwoaMGQuKW3HLrkdGFf8H0mLIOhNkHnlMfNW8HZQGkzgBfi86T7waOUjo6gyNUgy6s1CHGU0YMlkPmEqOrFn2mzcUCdbVUQ", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["StatusMessage"] = "You are now subscribed";
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
            return RedirectToAction("Index", "Home", "subscribe");
        }
    }
}
