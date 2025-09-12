// See https://aka.ms/new-console-template for more information

using ImageGlider;
using ImageGlider.Enums;
using ImageGlider.Utilities;

Console.Write("Please enter work dir: ");
var workDir = Console.ReadLine();
if (string.IsNullOrEmpty(workDir)) {
    Console.WriteLine("Work dir is empty");
    return;
}

if (!Directory.Exists(workDir)) {
    Console.WriteLine("Work dir not exists");
    return;
}

// 遍历目录下所有子目录和文件
var files = Directory.GetFiles(workDir, "*.*", SearchOption.AllDirectories);
var imageFiles = new List<string>();
foreach (var file in files) {
    if (ImageValidator.IsValidImageExtension(file)) {
        imageFiles.Add(file);
    }
}

Console.Write($"Total {imageFiles.Count} image files, please enter target format: ");
var targetFormat = Console.ReadLine();
if (string.IsNullOrEmpty(targetFormat)) {
    Console.WriteLine("Target format is empty");
    return;
}

targetFormat = targetFormat.ToLower();

if (!Enum.TryParse(targetFormat, ignoreCase: true, out ImageFormat imageFormat)) {
    Console.WriteLine("Target format is not valid");
    return;
}

imageFiles = imageFiles.Where(e => !e.ToLower().EndsWith(targetFormat)).ToList();

Console.WriteLine($"需要转换格式的图片数量: {imageFiles.Count}");


foreach (var imageFile in imageFiles) {
    var dirName = Path.GetDirectoryName(imageFile);
    var outputDir = Path.Combine(dirName, $"convert-{targetFormat}");
    if (!Directory.Exists(outputDir)) {
        Console.WriteLine($"Output dir not exists, create: {outputDir}");
        Directory.CreateDirectory(outputDir);
    }

    var outputFile = Path.Combine(outputDir, $"{Path.GetFileNameWithoutExtension(imageFile)}.{targetFormat}");
    Console.WriteLine($"正在转换 {imageFile}");
    ImageConverter.ConvertImage(imageFile, outputFile);
}


Console.WriteLine("Done");