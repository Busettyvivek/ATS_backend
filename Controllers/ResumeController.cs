using ATS_backend.Models;
using ATS_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace ATS_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResumeController : ControllerBase
    {
        private readonly PdfService _pdfService;
        private readonly ResumeParserService _resumeParserService;
        private readonly JobDescriptionParserService _jobDescriptionParserService;
        private readonly AtsAnalysisService _atsAnalysisService;
        private readonly GeminiService _geminiService;
        private readonly ResumeRewriteService _resumeRewriteService;
        public ResumeController(
            PdfService pdfService,
            ResumeParserService resumeParserService,
            JobDescriptionParserService jobDescriptionParserService,
            AtsAnalysisService atsAnalysisService,
            GeminiService geminiService,
            ResumeRewriteService resumeRewriteService)
        {
            _pdfService = pdfService;
            _resumeParserService = resumeParserService;
            _jobDescriptionParserService = jobDescriptionParserService;
            _atsAnalysisService = atsAnalysisService;
            _geminiService = geminiService;
            _resumeRewriteService = resumeRewriteService;
        }
        //[HttpPost("analyze-ai")]
        //public async Task<IActionResult> AnalyzeAi(
        //[FromForm] ResumeAnalysisRequest request)
        //        {
        //            if (request.Resume == null)
        //            {
        //                return BadRequest("Resume file is required.");
        //            }
        //var resumeText =
        //    _pdfService.ExtractText(request.Resume);

        //            var result =
        //                await _geminiService.AnalyzeResumeAsync(
        //                    resumeText,
        //                    request.JobDescription ?? string.Empty);

        //            return Ok(result);

        //}

        [HttpPost("analyze-ai")]
        public async Task<IActionResult> AnalyzeAi(
    [FromForm] ResumeAnalysisRequest request)
        {
            try
            {
                // Empty file validation
                if (request.Resume == null)
                {
                    return BadRequest(new
                    {
                        message = "Resume file is required."
                    });
                }

                // File size validation (5 MB)
                if (request.Resume.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new
                    {
                        message = "Maximum allowed file size is 5 MB."
                    });
                }

                // PDF validation
                var extension =
                    Path.GetExtension(request.Resume.FileName)
                        .ToLowerInvariant();

                if (extension != ".pdf")
                {
                    return BadRequest(new
                    {
                        message = "Only PDF files are allowed."
                    });
                }

                // Job description validation
                if (string.IsNullOrWhiteSpace(
                        request.JobDescription))
                {
                    return BadRequest(new
                    {
                        message = "Job Description is required."
                    });
                }

                var resumeText =
                    _pdfService.ExtractText(request.Resume);

                var result =
                    await _geminiService.AnalyzeResumeAsync(
                        resumeText,
                        request.JobDescription);

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return StatusCode(500, new
                {
                    message =
                        "An error occurred while analyzing the resume."
                });
            }
        }

        [HttpPost("rewrite")]
        public async Task<IActionResult> Rewrite(
         [FromBody] ResumeRewriteRequest request)
        {
            var result =
                await _resumeRewriteService.RewriteAsync(
                    request.ResumeBullets,
                    request.JobDescription);

            return Ok(result);
        }

    //    [HttpPost("analyze")]
    //    public IActionResult Analyze([FromForm] ResumeAnalysisRequest request)
    //    {
    //        if (request.Resume == null)
    //        {
    //            return BadRequest("Resume file is required.");
    //        }

    //        try
    //        {
    //            // Extract text from resume PDF
    //            var resumeText = _pdfService.ExtractText(request.Resume);

    //            // Extract skills from resume
    //            var resumeSkills =
    //                _resumeParserService.ExtractSkills(resumeText);

    //            // Extract skills from Job Description
    //            var jdSkills =
    //                _jobDescriptionParserService.ExtractSkills(
    //                    request.JobDescription ?? string.Empty);

    //            var analysis = _atsAnalysisService.Analyze(
    //                    resumeSkills,
    //                    jdSkills);

    //            return Ok(analysis);
    //        }
    //        catch (Exception ex)
    //        {
    //            return StatusCode(500, new
    //            {
    //                Message = "An error occurred while processing the resume.",
    //                Error = ex.Message
    //            });
    //        }
    //    }
    }
}