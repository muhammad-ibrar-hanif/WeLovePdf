using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using WeLovePdf.Server.Contracts;

namespace WeLovePdf.Server.Services
{
    public class PdfSplitService : IPdfSplitService
    {
        public async Task<List<byte[]>> SplitAsync(byte[] pdfFile)
        {
            if (pdfFile == null || pdfFile.Length == 0)
                throw new ArgumentException("PDF file is required");

            using var inputStream = new MemoryStream(pdfFile);
            var inputDocument = PdfReader.Open(inputStream, PdfDocumentOpenMode.Import);

            if (inputDocument.PageCount == 0)
                throw new InvalidOperationException("PDF has no pages");

            var result = new List<byte[]>();

            for (int i = 0; i < inputDocument.PageCount; i++)
            {
                var outputDocument = new PdfDocument();
                outputDocument.AddPage(inputDocument.Pages[i]);

                using var outputStream = new MemoryStream();
                outputDocument.Save(outputStream);

                result.Add(outputStream.ToArray());
            }

            return await Task.FromResult(result);
        }
    }
}
