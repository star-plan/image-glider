# ImageGlider

**ImageGlider** 是一个跨平台的命令行工具，使用 C# (.NET 8) 和 [ImageSharp](https://github.com/SixLabors/ImageSharp) 实现批量图像格式转换。该工具支持 AOT 编译，提供原生性能，并能够将转换后的图像、失败转换的文件和日志文件分别整理到专门的目录中。

## 特性

- **跨平台**：基于 .NET 8 和 ImageSharp，支持 Windows、Linux、macOS 等平台。
- **AOT 编译**：利用 .NET 8 的 AOT 功能，提供更快的启动速度和原生性能。
- **批量转换**：自动扫描当前目录中所有指定扩展名的文件，并批量转换。
- **用户自定义**：运行时可输入源文件扩展名和目标文件扩展名，自由控制转换格式。
- **目录管理**：转换成功的文件存放在 `output` 目录中，转换失败的文件自动移至 `failed` 目录。
- **详细日志**：整个转换过程会记录详细日志，日志文件存放在 `log` 目录中，文件名格式为 `ImageGlider_时间戳.log`。

## 前置条件

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- ImageSharp 库（已通过 NuGet 引入）

## 快速开始

### 克隆仓库

```bash
git clone git@github.com:Deali-Axy/image-glider.git
cd image-glider
```

### 恢复依赖

```
dotnet restore
```

### 构建项目

```
dotnet build -c Release
```

### 使用 AOT 发布

假设目标平台为 Windows x64（如需其他平台，请更改 `-r` 参数）：

```
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true
```

发布后生成的可执行文件位于 `bin/Release/net8.0/win-x64/publish` 目录中。

### 运行程序

在发布目录下运行生成的可执行文件，程序将提示你输入源文件扩展名和目标文件扩展名，然后开始批量转换。

## 使用说明

1. **输入扩展名**
   程序启动时，会要求输入原始文件扩展名（例如 `.jfif`）和目标文件扩展名（例如 `.jpeg` 或 `.png`）。输入后，程序会自动检查并补全扩展名前的点号。
2. **文件转换**
    - 程序在当前目录中查找所有符合源扩展名的文件。
    - 转换成功的文件会存放在 `output` 目录中。
    - 转换失败的文件会被移动到 `failed` 目录中。
3. **日志记录**
   所有转换过程的详细日志会写入到 `log` 目录下，日志文件名称格式为 `ImageGlider_YYYYMMDD_HHmmss.log`。

## 贡献

欢迎贡献代码、提交 issue 或 pull request，共同完善这个项目。

## 许可

本项目采用 MIT License 许可协议。

## 致谢

- 感谢 [ImageSharp](https://github.com/SixLabors/ImageSharp) 提供强大的图像处理支持。
- 感谢 .NET 8 提供跨平台与 AOT 编译的先进特性。
- 感谢所有开源社区的成员对本项目的支持和贡献。

