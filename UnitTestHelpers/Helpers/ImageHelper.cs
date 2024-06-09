using System.Drawing;
using System.IO;

namespace UnitTestHelpers.Helpers
{
    public static class ImageHelper
    {
        public static byte[] GenerateMockImageBytes()
        {
            using (var bitmap = new Bitmap(100, 100))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.FillRectangle(Brushes.Red, 0, 0, 100, 100);
                }

                using (var stream = new MemoryStream())
                {
                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    return stream.ToArray();
                }
            }
        }
    }
}
