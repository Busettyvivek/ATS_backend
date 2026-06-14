using System.Text;
using System.Text.Json;
using ATS_backend.Models;

namespace ATS_backend.Services
{
    public class ResumeRewriteService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ResumeRewriteService(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<ResumeRewriteResponse> RewriteAsync(
            List<string> resumeBullets,
            string jobDescription)
        {
            var apiKey = _configuration["Gemini:ApiKey"];

            var bulletsText = string.Join("\n", resumeBullets);

            var prompt = $@"
You are an expert resume writer.

Rewrite all resume bullets to better match the job description.

Rules:
1. Keep it truthful.
2. Do not invent experience.
3. Use ATS-friendly language.
4. Use strong action verbs.
5. Return ONLY valid JSON.
6. Return exactly one rewritten bullet for each input bullet.

Return this JSON format:

{{
    ""optimizedBullets"": []
}}

Resume Bullets:
{bulletsText}

Job Description:
{jobDescription}
";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = prompt
                            }
                        }
                    }
                }
            };

            var requestJson =
                JsonSerializer.Serialize(requestBody);

            var content =
                new StringContent(
                    requestJson,
                    Encoding.UTF8,
                    "application/json");

            var response =
                await _httpClient.PostAsync(
                    $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}",
                    content);

            response.EnsureSuccessStatusCode();

            var responseText =
                await response.Content.ReadAsStringAsync();

            var geminiResponse =
                JsonSerializer.Deserialize<GeminiApiResponse>(
                    responseText,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            var jsonText =
                geminiResponse?
                    .Candidates
                    .FirstOrDefault()?
                    .Content
                    .Parts
                    .FirstOrDefault()?
                    .Text;

            if (string.IsNullOrWhiteSpace(jsonText))
            {
                throw new Exception(
                    "Gemini returned an empty response.");
            }

            jsonText = jsonText
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();

            using var document =
                JsonDocument.Parse(jsonText);

            var optimizedBullets =
                document.RootElement
                    .GetProperty("optimizedBullets")
                    .EnumerateArray()
                    .Select(x => x.GetString() ?? string.Empty)
                    .ToList();

            return new ResumeRewriteResponse
            {
                OptimizedBullets = optimizedBullets
            };
        }
    }
}