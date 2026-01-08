using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using WeLovePdf.Server.Contracts;

namespace WeLovePdf.Server.Controllers
{
    [ApiController]
    [Route("api/pdf/split")]
    public class SplitController : ControllerBase
    {
        private readonly IPdfSplitService _splitService;

        public SplitController(IPdfSplitService splitService)
        {
            _splitService = splitService;
        }

        [HttpPost]
        public async Task<IActionResult> Split(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("PDF file is required");

            byte[] pdfBytes;
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                pdfBytes = ms.ToArray();
            }

            var pages = await _splitService.SplitAsync(pdfBytes);

            // ZIP result
            using var zipStream = new MemoryStream();
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                for (int i = 0; i < pages.Count; i++)
                {
                    var entry = archive.CreateEntry($"page_{i + 1}.pdf");
                    using var entryStream = entry.Open();
                    await entryStream.WriteAsync(pages[i]);
                }
            }

            return File(zipStream.ToArray(), "application/zip", "split-pages.zip");
        }
    }
}
