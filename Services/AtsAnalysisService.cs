namespace ATS_backend.Services
{
    public class AtsAnalysisService
    {
        public object Analyze(
            List<string> resumeSkills,
            List<string> jdSkills)
        {
            var matchedSkills = resumeSkills
                .Intersect(jdSkills, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var missingSkills = jdSkills
                .Except(resumeSkills, StringComparer.OrdinalIgnoreCase)
                .ToList();

            double matchPercentage = 0;

            if (jdSkills.Any())
            {
                matchPercentage =
                    (double)matchedSkills.Count /
                    jdSkills.Count * 100;
            }

            return new
            {
                MatchedSkills = matchedSkills,
                MissingSkills = missingSkills,
                MatchPercentage = Math.Round(matchPercentage, 2)
            };
        }
    }
}