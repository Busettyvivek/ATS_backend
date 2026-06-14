namespace ATS_backend.Services
{
    public class JobDescriptionParserService
    {
        private readonly List<string> _skills = new()
        {
            "C#",
            ".NET",
            "ASP.NET",
            "ASP.NET Core",
            "SQL",
            "MySQL",
            "React",
            "JavaScript",
            "HTML",
            "CSS",
            "Azure",
            "Python",
            "Git",
            "Entity Framework"
        };

        public List<string> ExtractSkills(string jobDescription)
        {
            return _skills
                .Where(skill =>
                    jobDescription.Contains(
                        skill,
                        StringComparison.OrdinalIgnoreCase))
                .Distinct()
                .ToList();
        }
    }
}