using System;
using System.IO;
using System.Globalization;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

// 当前目录路径
var currentDir = Directory.GetCurrentDirectory();
// 定义输出、失败和日志目录
var outputDir = Path.Combine(currentDir, "output");
var failedDir = Path.Combine(currentDir, "failed");
var logDir = Path.Combine(currentDir, "log");

// 确保目录存在
Directory.CreateDirectory(outputDir);
Directory.CreateDirectory(failedDir);
Directory.CreateDirectory(logDir);

// 构造日志文件名称（程序名_时间戳.log）
var timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
var logFilePath = Path.Combine(logDir, $"ImageGlider_{timeStamp}.log");

using StreamWriter logWriter = new StreamWriter(logFilePath, append: true);
Log(logWriter, "程序启动。");

// 提示用户输入源扩展名和目标扩展名
var reEnterFlag = true;
var sourceExt = "";
var targetExt = "";
while (reEnterFlag) {
    Console.Write("请输入原始文件扩展名（例如 .jfif）：");
    var input = Console.ReadLine()?.Trim().ToLowerInvariant();
    if (!string.IsNullOrWhiteSpace(input)) {
        reEnterFlag = false;
        continue;
    }
    
    targetExt = input;

    if (sourceExt != null && !sourceExt.StartsWith('.'))
        sourceExt = "." + sourceExt;
}

reEnterFlag = true;
while (reEnterFlag) {
    Console.Write("请输入目标文件扩展名（例如 .jpeg 或 .png）：");
    var input = Console.ReadLine()?.Trim().ToLowerInvariant();
    if (!string.IsNullOrWhiteSpace(input)) {
        reEnterFlag = false;
        continue;
    }

    targetExt = input;

    if (targetExt != null && !targetExt.StartsWith('.'))
        targetExt = "." + targetExt;
}

Log(logWriter, $"用户指定的源扩展名: {sourceExt}，目标扩展名: {targetExt}");

// 根据用户输入的源扩展名查找文件（仅在当前目录下）
var files = Directory.GetFiles(currentDir, "*" + sourceExt);
Log(logWriter, $"找到 {files.Length} 个文件需要转换。");

foreach (string file in files) {
    var fileName = Path.GetFileName(file);
    try {
        Log(logWriter, $"开始转换文件: {fileName}");
        // 加载图片（JFIF 实际上是 JPEG 变种，ImageSharp 可直接加载）
        using var image = Image.Load(file);
        // 构造新的文件名，扩展名修改为目标扩展名
        var newFileName = Path.GetFileNameWithoutExtension(file) + targetExt;
        var newFilePath = Path.Combine(outputDir, newFileName);

        // 若目标格式为 JPEG，则使用 JpegEncoder 设置质量；其他格式则直接保存
        if (targetExt == ".jpeg" || targetExt == ".jpg") {
            var encoder = new JpegEncoder { Quality = 90 };
            image.Save(newFilePath, encoder);
        }
        else {
            image.Save(newFilePath);
        }

        Log(logWriter, $"成功转换文件: {fileName} -> {newFileName}");
    }
    catch (Exception ex) {
        Log(logWriter, $"转换文件 {fileName} 失败，原因: {ex.Message}");
        // 转换失败时，将文件移动到 failed 目录
        var failedFilePath = Path.Combine(failedDir, fileName);
        try {
            // 如果目标位置已有同名文件则先删除
            if (File.Exists(failedFilePath)) {
                File.Delete(failedFilePath);
            }

            File.Move(file, failedFilePath);
            Log(logWriter, $"文件 {fileName} 已移动到失败目录。");
        }
        catch (Exception moveEx) {
            Log(logWriter, $"移动文件 {fileName} 到失败目录时失败，原因: {moveEx.Message}");
        }
    }
}

Log(logWriter, "转换完成。");
Console.WriteLine("转换完成。请检查 output 目录、failed 目录及日志文件。");
return;


// 日志辅助方法：将日志输出到控制台并写入日志文件
static void Log(StreamWriter writer, string message) {
    var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
    Console.WriteLine(logMessage);
    writer.WriteLine(logMessage);
    writer.Flush();
}