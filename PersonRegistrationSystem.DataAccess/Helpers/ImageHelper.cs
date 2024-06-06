using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Http;
using System.IO;

public static class ImageHelper
{
    public static void SaveResizedImage(string filePath, IFormFile imageFile, int width, int height)
    {
        var directory = Path.GetDirectoryName(filePath);

        using (var stream = new MemoryStream())
        {
            imageFile.CopyTo(stream);
            using (var image = Image.FromStream(stream))
            {
                var resizedImage = new Bitmap(width, height);
                using (var graphics = Graphics.FromImage(resizedImage))
                {
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.DrawImage(image, 0, 0, width, height);
                }
                resizedImage.Save(filePath, ImageFormat.Jpeg);
            }
        }
    }
    public static string GenerateImageFileName(string name, string lastName)
    {
        return $"{name}_{lastName}_{DateTime.Now:yyyyMMddHHmmss}.jpg";
    }

    public static string GetImageDirectory()
    {
        return "PersonPhoto";
    }

    public static string GetImageFilePath(string fileName)
    {
        var directory = GetImageDirectory();
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        return Path.Combine(directory, fileName);
    }
}