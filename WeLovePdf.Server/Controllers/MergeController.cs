using Microsoft.AspNetCore.Mvc;
using WeLovePdf.Server.Contracts;

namespace WeLovePdf.Server.Controllers
{
    [ApiController]
    [Route("api/pdf/merge")]
    public class MergeController : ControllerBase
    {
        private readonly IPdfMergeService _mergeService;

        public MergeController(IPdfMergeService mergeService)
        {
            _mergeService = mergeService;
        }

        [HttpPost]
        public async Task<IActionResult> Merge(List<IFormFile> files)
        {
            if (files.Count < 2)
                return BadRequest("At least two PDF files required");

            var pdfBytes = new List<byte[]>();

            foreach (var file in files)
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                pdfBytes.Add(ms.ToArray());
            }

            var result = await _mergeService.MergeAsync(pdfBytes);

            return File(result, "application/pdf", "merged.pdf");
        }
    }
}
