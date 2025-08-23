#:package SixLabors.ImageSharp@3.1.8

using System.Collections.Generic;
using System.IO;
using SixLabors.ImageSharp;

const string imageDir = @"C:\Users\deali\Pictures\image-lib\壁纸-原神-横图";

var wallpapers = new List<string>();

foreach (var imageFile in Directory.GetFiles(imageDir)) {
    if (!IsImage(imageFile)) {
        continue;
    }

    using var image = Image.Load(imageFile);
    Console.WriteLine($"{imageFile}: {image.Width}x{image.Height}");

    // select image which width >= 1920 and height >= 1080
    if (image.Width >= 1920 && image.Height >= 1080) {
        wallpapers.Add(Path.GetFileName(imageFile));
    }
}

// print wallpapers as json array
var json = $"[{string.Join(",", wallpapers.Select(x => $"\"file:///c:/Users/deali/Pictures/image-lib/壁纸-原神-横图/{x}\""))}]";
File.WriteAllText("wallpapers.json", json);

return;


bool IsImage(string filePath) {
    // bmp, jpeg, gif, tga, png, webp, pbm, qoi, tiff
    string[] imageExts = { ".bmp", ".jpeg", ".gif", ".tga", ".png", ".webp", ".pbm", ".qoi", ".tiff" };
    return imageExts.Contains(Path.GetExtension(filePath).ToLower());
}