
namespace ATS_backend.Models
{
    public class ResumeAnalysisRequest
    {
        public IFormFile Resume { get; set; }

        public string JobDescription { get; set; }
    }
}