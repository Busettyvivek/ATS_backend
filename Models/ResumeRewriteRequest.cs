

namespace ATS_backend.Models
{
    public class ResumeRewriteRequest
    {
        public List<string> ResumeBullets { get; set; } = new();

        public string JobDescription { get; set; } = string.Empty;
    }
}