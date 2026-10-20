using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RaceDay.MVC.Models;

namespace RaceDay.MVC.Services;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiClient(HttpClient httpClient, IConfiguration config, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;

        var baseUrl = config["ApiSettings:BaseUrl"];
        _httpClient.BaseAddress = new Uri(baseUrl!);

        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    private void AttachSessionCookie()
    {
        var sessionCookie = _httpContextAccessor.HttpContext?.Request.Cookies[".AspNetCore.Session"];
        if (!string.IsNullOrEmpty(sessionCookie))
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Cookie", $".AspNetCore.Session={sessionCookie}");
        }
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        AttachSessionCookie();
        var response = await _httpClient.GetAsync(endpoint);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }
        return default;
    }

    public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data)
    {
        AttachSessionCookie();
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(endpoint, content);

        // Capture session cookie from API and pass it to the browser
        if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            foreach (var cookie in cookies)
            {
                if (cookie.StartsWith(".AspNetCore.Session="))
                {
                    var cookieValue = cookie.Split(';')[0].Replace(".AspNetCore.Session=", "");
                    _httpContextAccessor.HttpContext?.Response.Cookies.Append(".AspNetCore.Session", cookieValue, new CookieOptions { HttpOnly = true });
                }
            }
        }
        return response;
    }

    // Helper methods for authentication
    public async Task<UserResponse?> LoginAsync(LoginModel model)
    {
        var response = await PostAsync("/auth/login", model);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UserResponse>(json, _jsonOptions);
        }
        return null;
    }

    public async Task<UserResponse?> GetCurrentUserAsync()
    {
        return await GetAsync<UserResponse>("/auth/me");
    }
}