using Microsoft.AspNetCore.Mvc;

namespace st10440926_poeparttwo.Controllers
{
    public class FileController : Controller
    {
        private readonly string _uploadRoot = Path.Combine("upload", "original");

        // VIEW FILE IN BROWSER
        public IActionResult ViewFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return NotFound();

            var path = Path.Combine(_uploadRoot, fileName);

            if (!System.IO.File.Exists(path))
                return NotFound("File not found.");

            var fileBytes = System.IO.File.ReadAllBytes(path);
            var contentType = "application/octet-stream";

            return File(fileBytes, contentType);
        }

        // DOWNLOAD FILE
        public IActionResult DownloadFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return NotFound();

            var path = Path.Combine(_uploadRoot, fileName);

            if (!System.IO.File.Exists(path))
                return NotFound("File not found.");

            var fileBytes = System.IO.File.ReadAllBytes(path);
            var contentType = "application/octet-stream";

            return File(fileBytes, contentType, fileName);
        }
    }
}
