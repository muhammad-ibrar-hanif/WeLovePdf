using System.Reflection.PortableExecutable;
using WeLovePdf.Server.Contracts;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;

namespace WeLovePdf.Server.Services
{
    public class PdfMergeService : IPdfMergeService
    {
        public async Task<byte[]> MergeAsync(List<byte[]> pdfFiles)
        {
            if (pdfFiles == null || pdfFiles.Count < 2)
                throw new ArgumentException("At least two PDFs are required");

            var outputDocument = new PdfDocument();

            foreach (var pdf in pdfFiles)
            {
                using var inputStream = new MemoryStream(pdf);
                var inputDocument = PdfReader.Open(inputStream, PdfDocumentOpenMode.Import);

                foreach (var page in inputDocument.Pages)
                {
                    outputDocument.AddPage(page);
                }
            }

            using var outputStream = new MemoryStream();
            outputDocument.Save(outputStream);

            return await Task.FromResult(outputStream.ToArray());
        }
    }
}
