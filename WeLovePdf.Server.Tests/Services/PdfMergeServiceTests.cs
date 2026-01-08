using FluentAssertions;
using NUnit.Framework;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WeLovePdf.Server.Services;

namespace WeLovePdf.Server.Tests.Services
{
    [TestFixture]
    public class PdfMergeServiceTests
    {
        private PdfMergeService _service;

        [SetUp]
        public void Setup()
        {
            _service = new PdfMergeService();
        }

        [Test]
        public async Task MergeAsync_WithLessThanTwoFiles_ShouldThrow()
        {
            // Arrange
            var singlePdf = CreateSamplePdf(1);

            // Act
            var act = async () =>
                await _service.MergeAsync(new List<byte[]> { singlePdf });

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*two PDFs*");
        }

        [Test]
        public async Task MergeAsync_WithValidPdfs_ShouldMergePages()
        {
            // Arrange
            var pdf1 = CreateSamplePdf(1);
            var pdf2 = CreateSamplePdf(2);

            // Act
            var result = await _service.MergeAsync(new List<byte[]> { pdf1, pdf2 });

            // Assert
            using var ms = new MemoryStream(result);
            var mergedPdf = PdfReader.Open(ms, PdfDocumentOpenMode.ReadOnly);

            mergedPdf.PageCount.Should().Be(3);
        }

        private static byte[] CreateSamplePdf(int pages)
        {
            var doc = new PdfDocument();

            for (int i = 0; i < pages; i++)
            {
                doc.AddPage();
            }

            using var ms = new MemoryStream();
            doc.Save(ms);

            return ms.ToArray();
        }
    }
}