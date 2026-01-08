using FluentAssertions;
using NUnit.Framework;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeLovePdf.Server.Services;

namespace WeLovePdf.Server.Tests.Services
{
    [TestFixture]
    public class PdfSplitServiceTests
    {
        private PdfSplitService _service;

        [SetUp]
        public void Setup()
        {
            _service = new PdfSplitService();
        }

        [Test]
        public async Task SplitAsync_WithEmptyPdf_ShouldThrow()
        {
            var act = async () =>
                await _service.SplitAsync(Array.Empty<byte>());

            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        public async Task SplitAsync_WithValidPdf_ShouldReturnEachPage()
        {
            var pdf = CreateSamplePdf(3);

            var result = await _service.SplitAsync(pdf);

            result.Should().HaveCount(3);

            foreach (var pageBytes in result)
            {
                using var ms = new MemoryStream(pageBytes);
                var doc = PdfReader.Open(ms, PdfDocumentOpenMode.ReadOnly);

                doc.PageCount.Should().Be(1);
            }
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
