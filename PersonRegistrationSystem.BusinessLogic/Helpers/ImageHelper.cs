using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Http;
using System.IO;

public static class ImageHelper
{
    public static void SaveResizedImage(string filePath, IFormFile imageFile, int width, int height)
    {
        try
        {
            using var image = Image.FromStream(imageFile.OpenReadStream());
            using var bitmap = new Bitmap(image, new Size(width, height));
            bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Error processing the image", ex);
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

    public static void DeleteImage(string filePath)
    {
        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}

