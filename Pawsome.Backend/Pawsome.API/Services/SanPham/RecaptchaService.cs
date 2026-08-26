using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Pawsome.API.Services.SanPham;

public class RecaptchaService : IRecaptchaService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;

    public RecaptchaService(IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
    }

    public async Task<bool> VerifyAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        var secretKey = _config["Recaptcha:SecretKey"];
        if (string.IsNullOrWhiteSpace(secretKey))
            return false;

        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsync(
            "https://www.google.com/recaptcha/api/siteverify",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["secret"] = secretKey,
                ["response"] = token
            }));

        if (!response.IsSuccessStatusCode)
            return false;

        var body = await response.Content.ReadFromJsonAsync<RecaptchaVerifyResponse>();
        return body?.Success ?? false;
    }

    private class RecaptchaVerifyResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }
}
