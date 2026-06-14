namespace ATS_backend.Models
{
    public class GeminiApiResponse
    {
        public List<Candidate> Candidates { get; set; } = new();
    }

    public class Candidate
    {
        public Content Content { get; set; } = new();
    }

    public class Content
    {
        public List<Part> Parts { get; set; } = new();
    }

    public class Part
    {
        public string Text { get; set; } = string.Empty;
    }
}