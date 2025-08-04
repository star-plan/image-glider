# ImageGlider

![License](https://img.shields.io/badge/license-MIT-blue)
![.NET](https://img.shields.io/badge/.NET-9.0-purple)

**ImageGlider** 是一个纯 C# AOT 实现的跨平台图像格式转换工具套件，使用 C# (.NET 9) 和 [ImageSharp](https://github.com/SixLabors/ImageSharp) 实现。项目采用模块化架构设计，包含核心类库、命令行工具、示例程序和完整的单元测试，支持 AOT 编译以获得原生性能。

通过ImageGlider，您可以轻松进行图像格式转换，完全摆脱命令行依赖，无需 ImageMagick、Node.js 或 Python，适合在 .NET 项目中内嵌、分发或集成自动打包流程中使用。

🚀 跨平台、零依赖、极速转换，一切尽在 ImageGlider！

## 项目架构

- **ImageGlider** - 核心类库，提供图像转换和日志记录功能
- **ImageGlider.Cli** - 命令行工具，支持单文件和批量转换
- **ImageGlider.Example** - 示例程序，展示核心类库的典型用法
- **ImageGlider.Tests** - 单元测试项目，确保代码质量

## ✨ 功能特点

- 🖼️ **格式转换**：支持 JPEG、PNG、GIF、BMP、TIFF、WebP 等多种图像格式的相互转换。
- 📏 **尺寸调整**：按宽度、高度或比例调整图像大小。
- 🗜️ **压缩优化**：对 JPEG、PNG 等格式进行有损或无损压缩，减小文件体积。
- ✂️ **图像裁剪**：按指定坐标和尺寸裁剪图像。
- 🖼️ **缩略图生成**：快速生成指定尺寸的缩略图。
- 💧 **添加水印**：支持文本和图片水印，可自定义位置、透明度和旋转角度。
- 🧹 **元数据清理**：一键清除图像的 EXIF、ICC、XMP 等元数据。
- 🎨 **颜色调整**：调整图像的亮度、对比度、饱和度等。
- ℹ️ **信息提取**：获取图像的详细信息，如尺寸、格式、DPI 等。
- 🚀 **跨平台**：基于 .NET 9 和 ImageSharp，支持 Windows、Linux、macOS 等平台
- ⚡ **AOT 编译**：利用 .NET 9 的 AOT 功能，提供更快的启动速度和原生性能
- 📦 **零依赖**：无需安装额外的图像处理工具或运行时
- 🧩 **模块化设计**：核心功能封装为独立类库，便于集成到其他项目中
- 💻 **命令行工具**：提供功能完整的 CLI 工具，支持丰富的命令行参数
- 📄 **单文件转换**：支持转换单个图像文件，可指定 JPEG 质量参数
- 📁 **批量转换**：自动扫描指定目录中的文件并批量转换
- ⚙️ **灵活配置**：支持自定义源目录、输出目录、日志目录等参数
- 📝 **详细日志**：提供完整的日志记录功能，支持多种日志级别（信息、警告、错误）
- 🛡️ **错误处理**：完善的异常处理机制，提供详细的转换结果信息

## 前置条件

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- 支持的图像格式：JPEG、PNG、GIF、BMP、TIFF、WebP 等（由 ImageSharp 提供支持）

## 依赖项

- **SixLabors.ImageSharp** (3.1.8) - 跨平台图像处理库
- **xUnit** - 单元测试框架（仅测试项目）

## 📦 安装

### 作为 .NET Global Tool 安装

```bash
dotnet tool install --global ImageGlider.Cli
```

### 从源码构建

```bash
git clone https://github.com/Deali-Axy/image-glider.git
cd image-glider
dotnet build -c Release
```

## 🚀 快速开始

### 克隆仓库

```bash
git clone git@github.com:Deali-Axy/image-glider.git
cd image-glider
```

### 恢复依赖

```bash
dotnet restore
```

### 构建项目

```bash
# 构建整个解决方案
dotnet build -c Release

# 或者构建特定项目
dotnet build src/ImageGlider.Cli -c Release
```

### 运行测试

```bash
dotnet test
```

### 使用 AOT 发布 CLI 工具

假设目标平台为 Windows x64（如需其他平台，请更改 `-r` 参数）：

```bash
dotnet publish src/ImageGlider.Cli -c Release -r win-x64 --self-contained true /p:PublishAot=true
```

发布后生成的可执行文件位于 `src/ImageGlider.Cli/bin/Release/net8.0/win-x64/publish` 目录中。

### 运行示例程序

```bash
# 运行示例程序，了解核心类库的用法
dotnet run --project src/ImageGlider.Example
```

## 使用说明

### 命令行工具使用方法

ImageGlider.Cli 提供了丰富的子命令来满足不同的图像处理需求。所有命令都支持单文件处理和批量处理两种模式。

#### 查看帮助信息

```bash
imageglider --help
```

#### 格式转换 (convert)

```bash
# 单文件转换
imageglider convert -s image.jpg -t image.png

# 批量转换
imageglider batch-convert --source-dir ./input --target-ext .png
```

#### 尺寸调整 (resize)

```bash
# 按指定宽度和高度缩放
imageglider resize -s in.jpg -t out.jpg -w 800 -h 600

# 保持宽高比，按宽度缩放
imageglider resize -s in.jpg -t out.jpg -w 500 --keep-aspect
```

#### 压缩优化 (compress)

```bash
# 默认压缩
imageglider compress -s in.jpg -t out.jpg

# 指定压缩质量（1-100）
imageglider compress -s in.jpg -t out.jpg -q 75
```

#### 图像裁剪 (crop)

```bash
# 从 (100, 50) 点开始裁剪一个 400x300 的区域
imageglider crop -s in.png -t out.png -x 100 -y 50 -w 400 -h 300
```

#### 生成缩略图 (thumbnail)

```bash
# 生成 150x150 的缩略图
imageglider thumbnail -s in.jpg -t thumb.jpg -w 150 -h 150
```

#### 添加水印 (watermark)

```bash
# 添加文本水印
imageglider watermark -s in.jpg -t out.jpg --text "Hello World" --font-size 24

# 添加图片水印
imageglider watermark -s in.jpg -t out.jpg --watermark-path logo.png --opacity 0.5
```

#### 元数据清理 (strip-metadata)

```bash
# 清理所有元数据
imageglider strip-metadata -s in.jpg -t out.jpg
```

#### 颜色调整 (adjust-color)

```bash
# 增加亮度并降低对比度
imageglider adjust-color -s in.jpg -t out.jpg --brightness 1.2 --contrast 0.8
```

#### 提取信息 (info)

```bash
# 显示图像基本信息
imageglider info -s image.jpg

# 以 JSON 格式输出详细信息
imageglider info -s image.jpg --json
```

#### 查看帮助信息

```bash
imageglider --help
```



### 核心类库 (ImageGlider)

如果你想在自己的项目中使用 ImageGlider 的核心功能，可以参考以下示例：

```csharp
using ImageGlider;

// 单文件转换
bool success = ImageConverter.ConvertImage("source.jfif", "target.jpeg", quality: 85);

// 批量转换
var result = ImageConverter.BatchConvert(
    sourceDirectory: "./input",
    outputDirectory: "./output",
    sourceExtension: ".jfif",
    targetExtension: ".jpeg",
    quality: 90
);

// 使用日志服务
using var logger = new LoggingService("./logs");
logger.LogInfo("转换开始");
logger.LogError("转换失败", exception);
```

## 🤝 贡献

欢迎贡献代码、提交 issue 或 pull request，共同完善这个项目。

## 开发和测试

### 项目结构

```
ImageGlider/
├── src/
│   ├── ImageGlider/              # 核心类库
│   │   ├── ImageConverter.cs     # 图像转换服务
│   │   └── LoggingService.cs     # 日志记录服务
│   ├── ImageGlider.Cli/          # 命令行工具
│   │   └── Program.cs            # CLI 程序入口
│   └── ImageGlider.Example/      # 示例程序
│       └── Program.cs            # 示例代码
└── tests/
    └── ImageGlider.Tests/        # 单元测试
        └── UnitTest1.cs          # 测试用例
```

### 运行开发环境

```bash
# 监视模式运行测试
dotnet watch test

# 运行特定测试
dotnet test --filter "TestMethodName"

# 生成测试覆盖率报告
dotnet test --collect:"XPlat Code Coverage"
```

## 🛠️ 开发者指南

### 构建项目

```bash
dotnet build
```

### 发布模式

ImageGlider 支持两种发布模式：

#### AOT 发布 (原生性能，无需 .NET 运行时)

```bash
dotnet publish src/ImageGlider.Cli -r win-x64 -c release /p:PublishAot=true /p:TrimMode=full /p:InvariantGlobalization=true /p:IlcGenerateStackTraceData=false /p:IlcOptimizationPreference=Size /p:IlcFoldIdenticalMethodBodies=true /p:JsonSerializerIsReflectionEnabledByDefault=true
```

支持的平台:

- Windows: win-x64
- macOS: osx-x64, osx-arm64
- Linux: linux-x64, linux-arm64

#### Framework Dependent 发布 (需要 .NET 运行时)

```bash
# 发布为 NuGet 包 (.NET Tool)
dotnet pack src/ImageGlider.Cli

# 安装本地打包的工具
dotnet tool install --global --add-source ./src/ImageGlider.Cli/nupkg ImageGlider.Cli
```

### 两种发布流程

#### AOT 发布流程 (独立应用)

1. 编译 AOT 版本:
```bash
dotnet publish src/ImageGlider.Cli -r win-x64 -c release /p:PublishAot=true /p:TrimMode=full /p:InvariantGlobalization=true /p:IlcGenerateStackTraceData=false /p:IlcOptimizationPreference=Size /p:IlcFoldIdenticalMethodBodies=true /p:JsonSerializerIsReflectionEnabledByDefault=true
```

2. 打包生成的文件:
```bash
# 进入发布目录
cd src/ImageGlider.Cli/bin/release/net9.0/win-x64/publish/
# 创建 zip 包
powershell Compress-Archive -Path * -DestinationPath imageglider-win-x64.zip
```

3. 将生成的 zip 文件上传到 GitHub Releases

#### .NET Tool 发布流程

1. 打包为 NuGet 包:
```bash
dotnet pack src/ImageGlider.Cli
```

2. 生成的包将位于 `./src/ImageGlider.Cli/nupkg` 目录中

3. 发布到 NuGet:
```bash
dotnet nuget push ./src/ImageGlider.Cli/nupkg/ImageGlider.Cli.1.0.0.nupkg --api-key 您的API密钥 --source https://api.nuget.org/v3/index.json
```

### 本地测试

#### 测试 .NET Tool

```bash
# 安装本地打包的工具
dotnet tool install --global --add-source ./src/ImageGlider.Cli/nupkg ImageGlider.Cli

# 卸载工具
dotnet tool uninstall --global ImageGlider.Cli
```

#### 测试 AOT 发布版本

直接运行生成的可执行文件:
```bash
./src/ImageGlider.Cli/bin/release/net9.0/win-x64/publish/ImageGlider.Cli.exe
```

## 📄 许可证

MIT License

## 致谢

- 感谢 [SixLabors ImageSharp](https://github.com/SixLabors/ImageSharp) 团队提供强大的跨平台图像处理库
- 感谢 Microsoft .NET 团队提供 .NET 9 的跨平台支持和 AOT 编译特性
- 感谢 xUnit 团队提供优秀的测试框架
- 感谢所有开源社区的贡献者们

