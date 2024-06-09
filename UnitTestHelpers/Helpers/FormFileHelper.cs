using Microsoft.AspNetCore.Http;
using Moq;
using System.IO;

namespace UnitTestHelpers.Helpers
{
    public static class FormFileHelper
    {
        public static IFormFile CreateMockImageFormFile()
        {
            var imageBytes = ImageHelper.GenerateMockImageBytes();
            var stream = new MemoryStream(imageBytes);
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.FileName).Returns("test.jpg");
            fileMock.Setup(f => f.Length).Returns(imageBytes.Length);
            fileMock.Setup(f => f.ContentType).Returns("image/jpeg");
            return fileMock.Object;
        }
    }
}
