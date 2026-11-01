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
    private readonly JsonSerializerOptions _postOptions;

    public ApiClient(HttpClient httpClient, IConfiguration config, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;

        var baseUrl = config["ApiSettings:BaseUrl"] ?? "https://localhost:7112/";

        // Ensures it ends with a slash
        if (!baseUrl.EndsWith("/")) baseUrl += "/";

        _httpClient.BaseAddress = new Uri(baseUrl);

        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _postOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
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
        // Removes leading slash so it appends to BaseAddress correctly
        var response = await _httpClient.GetAsync(endpoint.TrimStart('/'));
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

        var json = JsonSerializer.Serialize(data, _postOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Remove leading slash so it appends to BaseAddress correctly
        var response = await _httpClient.PostAsync(endpoint.TrimStart('/'), content);

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

    public async Task<HttpResponseMessage> PutAsync<T>(string endpoint, T data)
    {
        AttachSessionCookie();
        var json = JsonSerializer.Serialize(data, _postOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await _httpClient.PutAsync(endpoint.TrimStart('/'), content);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
    {
        AttachSessionCookie();
        return await _httpClient.DeleteAsync(endpoint.TrimStart('/'));
    }

    // --- Auth Helpers ---

    public async Task<UserResponse?> LoginAsync(LoginModel model)
    {
        // Notice there is no leading slash here
        var response = await PostAsync("api/auth/login", model);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UserResponse>(json, _jsonOptions);
        }
        return null;
    }

    public async Task<UserResponse?> GetCurrentUserAsync()
    {
        return await GetAsync<UserResponse>("api/auth/me");
    }
}