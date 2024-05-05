using Infrastructure.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Infrastructure.Services;

public class CourseService(HttpClient httpClient, IConfiguration configuration)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IConfiguration _configuration = configuration;

    public async Task<CourseResult> GetCoursesAsync(string category = "", string searchQuery = "", int pageNumber = 1, int pageSize = 10)
    {

        var response = await _httpClient.GetAsync($"{_configuration["ApiUris:Courses"]}?category={Uri.UnescapeDataString(category)}&searchQuery={Uri.UnescapeDataString(searchQuery)}&pageNumber={pageNumber}&pageSize={pageSize}&key={_configuration["ApiKeys:Key"]}");



        if (response.IsSuccessStatusCode)
        {
            var result = JsonConvert.DeserializeObject<CourseResult>(await response.Content.ReadAsStringAsync());
            if (result != null && result.Succeeded)
                return result;
        }
        return null!;
    }

    public async Task<CourseModel> GetCourseByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"{_configuration["ApiUris:Courses"]}/{id}?key={_configuration["ApiKeys:Key"]}");

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine(json);
            var result = JsonConvert.DeserializeObject<CourseModel>(json);

            if (result != null)
                return result;
        }
        return null!;
    }

}
