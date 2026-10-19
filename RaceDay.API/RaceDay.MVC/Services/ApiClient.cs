using System.Text;
using System.Text.Json;

namespace RaceDay.MVC.Services;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiClient(HttpClient httpClient, IConfiguration config, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;

        var baseUrl = config["ApiSettings:BaseUrl"];
        _httpClient.BaseAddress = new Uri(baseUrl!);
    }

    // Helper method to grab the session cookie from the browser and send it to the API
    private void AttachSessionCookie()
    {
        var sessionCookie = _httpContextAccessor.HttpContext?.Request.Cookies[".AspNetCore.Session"];
        if (!string.IsNullOrEmpty(sessionCookie))
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Cookie", $".AspNetCore.Session={sessionCookie}");
        }
    }

    // GET request
    public async Task<T?> GetAsync<T>(string endpoint)
    {
        AttachSessionCookie();
        var response = await _httpClient.GetAsync(endpoint);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        return default;
    }

    // POST request
    public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data)
    {
        AttachSessionCookie();
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(endpoint, content);

        // Capture the API's session cookie and pass it to the browser
        if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            foreach (var cookie in cookies)
            {
                if (cookie.StartsWith(".AspNetCore.Session="))
                {
                    var cookieValue = cookie.Split(';')[0].Replace(".AspNetCore.Session=", "");
                    _httpContextAccessor.HttpContext?.Response.Cookies.Append(".AspNetCore.Session", cookieValue);
                }
            }
        }
        return response;
    }

    // PUT request
    public async Task<HttpResponseMessage> PutAsync<T>(string endpoint, T data)
    {
        AttachSessionCookie();
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await _httpClient.PutAsync(endpoint, content);
    }

    // DELETE request
    public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
    {
        AttachSessionCookie();
        return await _httpClient.DeleteAsync(endpoint);
    }
}
