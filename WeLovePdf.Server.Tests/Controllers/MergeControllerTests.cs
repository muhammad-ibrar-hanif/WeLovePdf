using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using WeLovePdf.Server.Contracts;
using WeLovePdf.Server.Controllers;

namespace WeLovePdf.Server.Tests.Controllers
{
    public class MergeControllerTests
    {
        private static IFormFile CreateFormFile(byte[] contents, string fileName = "file.pdf")
        {
            var stream = new MemoryStream(contents);
            return new FormFile(stream, 0, contents.Length, "files", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            };
        }

        [Test]
        public async Task Merge_WithLessThanTwoFiles_ReturnsBadRequest()
        {
            // Arrange
            var mockService = new Mock<IPdfMergeService>(MockBehavior.Strict);
            var controller = new MergeController(mockService.Object);
            var files = new List<IFormFile>
            {
                CreateFormFile(Encoding.UTF8.GetBytes("pdf1"))
            };

            // Act
            var result = await controller.Merge(files);

            // Assert
            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
            var badRequest = result as BadRequestObjectResult;
            Assert.That(badRequest.Value, Is.EqualTo("At least two PDF files required"));
            mockService.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Merge_WithTwoFiles_CallsServiceAndReturnsFileResult()
        {
            // Arrange
            var pdf1 = Encoding.UTF8.GetBytes("pdf1-content");
            var pdf2 = Encoding.UTF8.GetBytes("pdf2-content");
            var expectedMerged = Encoding.UTF8.GetBytes("merged-result");

            var mockService = new Mock<IPdfMergeService>(MockBehavior.Strict);
            mockService
                .Setup(s => s.MergeAsync(It.Is<List<byte[]>>(l =>
                    l != null &&
                    l.Count == 2 &&
                    l[0].SequenceEqual(pdf1) &&
                    l[1].SequenceEqual(pdf2))))
                .ReturnsAsync(expectedMerged)
                .Verifiable();

            var controller = new MergeController(mockService.Object);
            var files = new List<IFormFile>
            {
                CreateFormFile(pdf1, "a.pdf"),
                CreateFormFile(pdf2, "b.pdf")
            };

            // Act
            var result = await controller.Merge(files);

            // Assert
            Assert.That(result, Is.TypeOf<FileContentResult>());
            var fileResult = result as FileContentResult;
            Assert.That(fileResult.ContentType, Is.EqualTo("application/pdf"));
            Assert.That(fileResult.FileDownloadName, Is.EqualTo("merged.pdf"));
            Assert.That(expectedMerged.SequenceEqual(fileResult.FileContents), Is.True);
            mockService.Verify(s => s.MergeAsync(It.IsAny<List<byte[]>>()), Times.Once);
            mockService.Verify();
        }

        [Test]
        public void Merge_WithNullFiles_ThrowsNullReferenceException()
        {
            // Arrange
            var mockService = new Mock<IPdfMergeService>(MockBehavior.Strict);
            var controller = new MergeController(mockService.Object);

            // Act & Assert
            Assert.ThrowsAsync<System.NullReferenceException>(async () => await controller.Merge(null));
            mockService.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Merge_WithEmptyFile_IncludesEmptyByteArray()
        {
            // Arrange
            var empty = new byte[0];
            var pdf2 = Encoding.UTF8.GetBytes("pdf2-content");
            var expectedMerged = Encoding.UTF8.GetBytes("merged-result");

            var mockService = new Mock<IPdfMergeService>(MockBehavior.Strict);
            mockService
                .Setup(s => s.MergeAsync(It.Is<List<byte[]>>(l =>
                    l != null &&
                    l.Count == 2 &&
                    l[0].Length == 0 &&
                    l[1].SequenceEqual(pdf2))))
                .ReturnsAsync(expectedMerged)
                .Verifiable();

            var controller = new MergeController(mockService.Object);
            var files = new List<IFormFile>
            {
                CreateFormFile(empty, "empty.pdf"),
                CreateFormFile(pdf2, "b.pdf")
            };

            // Act
            var result = await controller.Merge(files);

            // Assert
            Assert.That(result, Is.TypeOf<FileContentResult>());
            var fileResult = result as FileContentResult;
            Assert.That(fileResult.FileContents, Is.EqualTo(expectedMerged));
            mockService.Verify(s => s.MergeAsync(It.IsAny<List<byte[]>>()), Times.Once);
            mockService.Verify();
        }

        [Test]
        public void Merge_WhenServiceThrows_PropagatesException()
        {
            // Arrange
            var pdf1 = Encoding.UTF8.GetBytes("pdf1-content");
            var pdf2 = Encoding.UTF8.GetBytes("pdf2-content");

            var mockService = new Mock<IPdfMergeService>(MockBehavior.Strict);
            mockService
                .Setup(s => s.MergeAsync(It.IsAny<List<byte[]>>()))
                .ThrowsAsync(new System.InvalidOperationException("merge failed"));

            var controller = new MergeController(mockService.Object);
            var files = new List<IFormFile>
            {
                CreateFormFile(pdf1, "a.pdf"),
                CreateFormFile(pdf2, "b.pdf")
            };

            // Act & Assert
            Assert.ThrowsAsync<System.InvalidOperationException>(async () => await controller.Merge(files));
            mockService.Verify(s => s.MergeAsync(It.IsAny<List<byte[]>>()), Times.Once);
        }
    }
}
