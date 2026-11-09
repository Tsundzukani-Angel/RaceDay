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
        if (!baseUrl.EndsWith("/")) baseUrl += "/";
        _httpClient.BaseAddress = new Uri(baseUrl);

        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _postOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    }

    // Attach cookie to the individual request, not the HttpClient
    private HttpRequestMessage CreateRequest(HttpMethod method, string endpoint, HttpContent? content = null)
    {
        var request = new HttpRequestMessage(method, endpoint.TrimStart('/'));

        var sessionCookie = _httpContextAccessor.HttpContext?.Request.Cookies[".AspNetCore.Session"];
        if (!string.IsNullOrEmpty(sessionCookie))
        {
            request.Headers.Add("Cookie", $".AspNetCore.Session={sessionCookie}");
        }

        if (content != null)
        {
            request.Content = content;
        }

        return request;
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        var request = CreateRequest(HttpMethod.Get, endpoint);
        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }
        return default;
    }

    public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data)
    {
        var json = JsonSerializer.Serialize(data, _postOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = CreateRequest(HttpMethod.Post, endpoint, content);
        var response = await _httpClient.SendAsync(request);

        // Capture the new session cookie from the API (on login)
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
        var json = JsonSerializer.Serialize(data, _postOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var request = CreateRequest(HttpMethod.Put, endpoint, content);
        return await _httpClient.SendAsync(request);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
    {
        var request = CreateRequest(HttpMethod.Delete, endpoint);
        return await _httpClient.SendAsync(request);
    }

    // --- Auth Helpers ---
    public async Task<UserResponse?> LoginAsync(LoginModel model)
    {
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

    public async Task<HttpResponseMessage> PostFileAsync(string endpoint, MultipartFormDataContent content)
    {
        // Create the request
        var request = new HttpRequestMessage(HttpMethod.Post, endpoint.TrimStart('/'));

        // Attach the session cookie manually
        var sessionCookie = _httpContextAccessor.HttpContext?.Request.Cookies[".AspNetCore.Session"];
        if (!string.IsNullOrEmpty(sessionCookie))
        {
            request.Headers.Add("Cookie", $".AspNetCore.Session={sessionCookie}");
        }

        // Assign the content. HttpClient will automatically add the correct Content-Type (with boundary).
        request.Content = content;

        return await _httpClient.SendAsync(request);
    }
}