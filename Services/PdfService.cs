using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using System.Text;
namespace ATS_backend.Services
{
    public class PdfService
    {
        public string ExtractText(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var document = PdfDocument.Open(stream);

            var text = "";

            foreach (Page page in document.GetPages())
            {
                text += page.Text + Environment.NewLine;
            }

            return text;
        }
    }
}