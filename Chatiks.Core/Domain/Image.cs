using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace Chatiks.Core.Domain;

public class Image
{
    private static int _maxImageBytes = 12000;

    public long Id { get; }

    public int Width { get; private set; }

    public int Height { get; private set; }

    public string Base64String { get; private set; }

    public Image(byte[] imageBytes)
    {
        var image = GetImage(imageBytes);

        Base64String = image.ToBase64String(PngFormat.Instance);
        Width = image.Width;
        Height = image.Height;
    }

    public Image(string base64String)
    {
        var image = GetImage(Convert.FromBase64String(base64String));

        Base64String = image.ToBase64String(PngFormat.Instance);
        Width = image.Width;
        Height = image.Height;
    }

    private static SixLabors.ImageSharp.Image GetImage(byte[] imageBytes)
    {
        SixLabors.ImageSharp.Image image;

        try
        {
            image = SixLabors.ImageSharp.Image.Load(imageBytes);
        }
        catch
        {
            throw new ArgumentException("Invalid image", nameof(imageBytes));
        }

        var delta = Math.Sqrt(imageBytes.Length / _maxImageBytes);

        image.Mutate(o => o.Resize(new Size
        {
            Width = (int)(image.Width / delta),
            Height = (int)(image.Height / delta)
        }));

        return image;
    }
}