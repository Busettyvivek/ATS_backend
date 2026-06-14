using System.Text;
using System.Text.Json;
using ATS_backend.Models;

namespace ATS_backend.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

    public GeminiService(
        HttpClient httpClient,
        IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<AiAnalysisResponse> AnalyzeResumeAsync(
            string resumeText,
            string jobDescription)
        {
            //var apiKey = _configuration["Gemini:ApiKey"];
            //var apiKey = _configuration["Gemini:ApiKey"];
            var apiKey =
            _configuration["GEMINI_API_KEY"]
            ?? _configuration["Gemini:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new Exception(
                    "Gemini API Key is missing.");
            }

            var prompt = $@"
```

Compare this resume against the job description.

Return ONLY valid JSON.

Do not use markdown.
Do not use ```json.
Do not add explanations.

Return this exact structure:

{{
""atsScore"": 0,
""missingSkills"": [],
""missingKeywords"": [],
""strengths"": [],
""improvements"": []
}}

Resume:
{resumeText}

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

            var result =
                JsonSerializer.Deserialize<AiAnalysisResponse>(
                    jsonText,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (result == null)
            {
                throw new Exception(
                    "Failed to parse Gemini response.");
            }

            return result;
        }
    }

}
