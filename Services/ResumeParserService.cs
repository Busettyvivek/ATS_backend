namespace ATS_backend.Services
{
    public class ResumeParserService
    {
        private readonly List<string> _skills = new()
        {
            "C#",
            ".NET",
            "ASP.NET",
            "ASP.NET MVC",
            "ASP.NET Core",
            "SQL",
            "MySQL",
            "React",
            "JavaScript",
            "HTML",
            "CSS",
            "Azure",
            "Azure Data Factory",
            "PySpark",
            "Python",
            "Git",
            "Entity Framework"
        };

        public List<string> ExtractSkills(string resumeText)
        {
            return _skills
                .Where(skill =>
                    resumeText.Contains(skill,
                    StringComparison.OrdinalIgnoreCase))
                .Distinct()
                .ToList();
        }
    }
}