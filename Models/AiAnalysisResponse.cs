namespace ATS_backend.Models
{
    public class AiAnalysisResponse
    {
        public int AtsScore { get; set; }

        public List<string> MissingSkills { get; set; } = new();

        public List<string> MissingKeywords { get; set; } = new();

        public List<string> Strengths { get; set; } = new();

        public List<string> Improvements { get; set; } = new();
    }
}