# ImageGlider

![License](https://img.shields.io/badge/license-MIT-blue)
![.NET](https://img.shields.io/badge/.NET-9.0-purple)
![Test Coverage](https://img.shields.io/badge/coverage-69.4%25-green)
![Tests](https://img.shields.io/badge/tests-180%20passed-brightgreen)

**ImageGlider** 是一个功能强大的跨平台图像处理工具套件，使用 C# (.NET 9)、[ImageSharp](https://github.com/SixLabors/ImageSharp) 和 [ImageMagick.NET](https://github.com/dlemstra/Magick.NET) 实现。项目采用模块化架构设计，包含核心类库、命令行工具、Web API、示例程序和完整的单元测试，支持 AOT 编译以获得原生性能。

通过ImageGlider，您可以轻松进行图像处理操作，支持包括现代 AVIF 格式在内的多种图像格式，适合在 .NET 项目中内嵌、分发或集成自动化流程中使用。

🚀 跨平台、零依赖、高性能、全功能的图像处理解决方案！

## 📁 项目架构

- **ImageGlider** - 核心类库，提供完整的图像处理功能
- **ImageGlider.Cli** - 命令行工具，支持16种命令和批量处理
- **ImageGlider.WebApi** - RESTful API服务，提供HTTP接口
- **ImageGlider.Example** - 示例程序，展示核心类库的典型用法
- **ImageGlider.Tests** - 完整的单元测试套件，180个测试用例，69.4%覆盖率

## ✨ 功能特点

### 🎯 核心图像处理功能
- 🖼️ **格式转换**：支持 JPEG、PNG、GIF、BMP、TIFF、WebP、AVIF 等多种图像格式的相互转换
- 📏 **尺寸调整**：支持拉伸、保持宽高比、裁剪等多种调整模式
- 🗜️ **压缩优化**：智能压缩算法，在保持质量的同时减小文件体积
- ✂️ **图像裁剪**：精确裁剪指定区域，支持中心裁剪和自定义坐标裁剪
- 🖼️ **缩略图生成**：快速生成高质量缩略图，支持多种尺寸模式
- 💧 **水印功能**：支持文本和图片水印，9种位置选择，可调透明度和缩放
- 🧹 **元数据清理**：一键清除 EXIF、ICC、XMP 等隐私敏感的元数据信息
- 🎨 **颜色调整**：调整亮度、对比度、饱和度、色相、伽马值等颜色参数
- ℹ️ **信息提取**：获取图像详细信息，支持JSON格式输出
- ✅ **图像验证**：检测文件是否为有效图片，支持扩展名、文件头和深度验证三种模式

### 🛠️ 技术特性
- 🚀 **跨平台**：基于 .NET 9 和 ImageSharp，支持 Windows、Linux、macOS
- ⚡ **AOT 编译**：支持原生AOT编译，启动速度快，内存占用低
- 📦 **零依赖**：无需安装额外的图像处理工具或运行时环境
- 🧩 **模块化设计**：核心功能封装为独立类库，便于集成和扩展
- 🔧 **多种接口**：提供CLI工具、Web API、核心类库三种使用方式

### 💻 使用方式
- **命令行工具**：16种专业命令，支持单文件和批量处理
- **Web API**：RESTful接口，支持HTTP文件上传和处理
- **核心类库**：直接集成到.NET项目中使用
- **批量处理**：自动扫描目录，支持递归处理和文件过滤
- **详细日志**：完整的日志记录，支持多种日志级别
- **错误处理**：完善的异常处理机制，提供详细的处理结果信息

## 前置条件

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- 支持的图像格式：JPEG、PNG、GIF、BMP、TIFF、WebP（由 ImageSharp 提供支持）、AVIF（由 ImageMagick.NET 提供支持）

## 依赖项

- **SixLabors.ImageSharp** (3.1.8) - 跨平台图像处理库，支持常见图像格式
- **Magick.NET-Q16-AnyCPU** (14.8.2) - ImageMagick .NET 绑定，提供 AVIF 格式支持
- **xUnit** - 单元测试框架（仅测试项目）

## 📦 安装方式

### 方式一：作为 .NET Global Tool 安装（推荐）

```bash
dotnet tool install --global ImageGlider.Cli
```

### 方式二：从源码构建

```bash
git clone https://github.com/Deali-Axy/image-glider.git
cd image-glider
dotnet build -c Release
```

### 方式三：发布为单文件可执行程序

```bash
# Windows x64
dotnet publish src/ImageGlider.Cli -c Release -r win-x64 --self-contained -p:PublishSingleFile=true

# Linux x64
dotnet publish src/ImageGlider.Cli -c Release -r linux-x64 --self-contained -p:PublishSingleFile=true

# macOS x64 (Intel)
dotnet publish src/ImageGlider.Cli -c Release -r osx-x64 --self-contained -p:PublishSingleFile=true

# macOS ARM64 (Apple Silicon)
dotnet publish src/ImageGlider.Cli -c Release -r osx-arm64 --self-contained -p:PublishSingleFile=true
```

### 方式四：AOT 编译（性能最佳）

```bash
# Windows x64 AOT
dotnet publish src/ImageGlider.Cli -c Release -r win-x64 -p:PublishAot=true

# Linux x64 AOT
dotnet publish src/ImageGlider.Cli -c Release -r linux-x64 -p:PublishAot=true

# macOS x64 AOT
dotnet publish src/ImageGlider.Cli -c Release -r osx-x64 -p:PublishAot=true

# macOS ARM64 AOT
dotnet publish src/ImageGlider.Cli -c Release -r osx-arm64 -p:PublishAot=true
```

## 🚀 快速开始

### 🖥️ 命令行工具使用

安装完成后，您可以直接使用 `imageglider` 命令：

```bash
# 查看所有可用命令
imageglider --help

# 查看特定命令的帮助
imageglider help convert
```

### 🌐 Web API 服务

启动Web API服务：

```bash
cd src/ImageGlider.WebApi
dotnet run
# 服务将在 http://localhost:5000 启动
```

### 📚 核心类库集成

在您的.NET项目中引用核心类库：

```xml
<PackageReference Include="ImageGlider" Version="1.0.0" />
```

```csharp
using ImageGlider;

// 转换图像格式
ImageConverter.ConvertImage("input.png", "output.jpg", 85);

// 调整图像尺寸
ImageConverter.ResizeImage("input.jpg", "output.jpg", 800, 600);
```

### 🧪 运行测试

```bash
# 运行所有测试
dotnet test

# 运行测试并生成覆盖率报告
dotnet test --collect:"XPlat Code Coverage"
```

### 🎯 运行示例程序

```bash
# 运行示例程序，了解核心类库的用法
dotnet run --project src/ImageGlider.Example
```

## 📖 使用说明

### 🖥️ 命令行工具完整指南

ImageGlider CLI 提供了16种专业命令，支持单文件和批量处理。所有命令都经过精心设计，提供丰富的参数选项。

#### 📋 可用命令列表

| 命令 | 描述 | 批量版本 |
|------|------|----------|
| `convert` | 格式转换 | `batch-convert` |
| `resize` | 尺寸调整 | `batch-resize` |
| `compress` | 图像压缩 | `batch-compress` |
| `crop` | 图像裁剪 | `batch-crop` |
| `thumbnail` | 缩略图生成 | `batch-thumbnail` |
| `watermark` | 添加水印 | `batch-watermark` |
| `strip-metadata` | 清理元数据 | `batch-strip-metadata` |
| `adjust` | 颜色调整 | `batch-adjust` |
| `info` | 信息提取 | `batch-info` |

#### 🆘 查看帮助信息

```bash
# 查看所有命令
imageglider --help

# 查看特定命令的详细帮助
imageglider help convert
imageglider help batch-resize
```

#### 🔄 格式转换 (convert)

```bash
# 单文件转换
imageglider convert -s image.jpg -t image.png -q 85

# 批量转换目录下所有JPEG文件为PNG
imageglider batch-convert -sd ./input -od ./output -se .jpg -te .png -q 90

# 转换特定格式并设置质量
imageglider convert -s photo.webp -t photo.jpg --quality 95

# 转换为现代 AVIF 格式（高压缩比）
imageglider convert -s photo.jpg -t photo.avif --quality 80

# 从 AVIF 格式转换为其他格式
imageglider convert -s photo.avif -t photo.png

# 批量转换为 AVIF 格式
imageglider batch-convert -sd ./photos -od ./avif -se .jpg -te .avif -q 75
```

#### 📏 尺寸调整 (resize)

```bash
# 按指定宽度和高度调整（拉伸模式）
imageglider resize -s input.jpg -t output.jpg -w 800 -h 600

# 保持宽高比调整（只指定宽度）
imageglider resize -s input.jpg -t output.jpg -w 800 --mode keep-aspect

# 批量调整目录下所有图片
imageglider batch-resize -sd ./photos -od ./resized -w 1920 -h 1080 -ext .jpg

#### 🗜️ 压缩优化 (compress)

```bash
# 默认压缩（质量75）
imageglider compress -s input.jpg -t compressed.jpg

# 指定压缩质量（1-100）
imageglider compress -s input.jpg -t output.jpg -q 60

# 批量压缩目录下所有JPEG文件
imageglider batch-compress -sd ./photos -od ./compressed -ext .jpg -q 80
```

#### ✂️ 图像裁剪 (crop)

```bash
# 从指定坐标裁剪
imageglider crop -s input.png -t cropped.png -x 100 -y 50 -w 400 -h 300

# 中心裁剪
imageglider crop -s input.jpg -t cropped.jpg -w 800 -h 600 --center

# 批量裁剪
imageglider batch-crop -sd ./photos -od ./cropped -w 500 -h 500 --center -ext .jpg
```

#### 🖼️ 生成缩略图 (thumbnail)

```bash
# 生成标准缩略图
imageglider thumbnail -s input.jpg -t thumb.jpg -w 150 -h 150

# 保持宽高比的缩略图
imageglider thumbnail -s input.jpg -t thumb.jpg -w 200 --keep-aspect

# 批量生成缩略图
imageglider batch-thumbnail -sd ./photos -od ./thumbs -w 300 -h 300 -ext .jpg
```

#### 💧 添加水印 (watermark)

```bash
# 添加文本水印
imageglider watermark -s input.jpg -t watermarked.jpg --text "© 2024" --position bottom-right

# 添加图片水印
imageglider watermark -s input.jpg -t watermarked.jpg --image logo.png --opacity 50 --scale 0.3

# 批量添加水印
imageglider batch-watermark -sd ./photos -od ./watermarked --text "Sample" -ext .jpg
```

#### 🧹 元数据清理 (strip-metadata)

```bash
# 清理所有元数据
imageglider strip-metadata -s input.jpg -t clean.jpg --all

# 只清理EXIF数据
imageglider strip-metadata -s input.jpg -t clean.jpg --exif

# 批量清理元数据
imageglider batch-strip-metadata -sd ./photos -od ./cleaned -ext .jpg --all
```

#### 🎨 颜色调整 (adjust)

```bash
# 调整亮度和对比度
imageglider adjust -s input.jpg -t adjusted.jpg --brightness 20 --contrast 15

# 调整饱和度和色相
imageglider adjust -s input.jpg -t adjusted.jpg --saturation 30 --hue 45

# 批量颜色调整
imageglider batch-adjust -sd ./photos -od ./adjusted --brightness 10 --gamma 1.2 -ext .jpg
```

#### ℹ️ 信息提取 (info)

```bash
# 显示图像基本信息
imageglider info -s image.jpg

# 以JSON格式输出详细信息
imageglider info -s image.jpg --json

# 批量提取信息并保存到文件
imageglider batch-info -sd ./photos --json --output info.json

### 🌐 Web API 使用指南

ImageGlider 提供了完整的RESTful API服务，支持通过HTTP接口进行图像处理。

#### 启动API服务

```bash
cd src/ImageGlider.WebApi
dotnet run
# 服务将在 http://localhost:5000 启动
```

#### API 端点示例

```bash
# 上传并转换图像格式
curl -X POST "http://localhost:5000/api/convert" \
  -F "file=@input.jpg" \
  -F "targetFormat=png" \
  -F "quality=85"

# 上传并调整图像尺寸
curl -X POST "http://localhost:5000/api/resize" \
  -F "file=@input.jpg" \
  -F "width=800" \
  -F "height=600" \
  -F "mode=KeepAspectRatio"

# 上传并压缩图像
curl -X POST "http://localhost:5000/api/compress" \
  -F "file=@input.jpg" \
  -F "quality=70"

# 获取图像信息
curl -X POST "http://localhost:5000/api/info" \
  -F "file=@input.jpg"
```

### 📚 核心类库集成

在您的.NET项目中直接使用ImageGlider核心功能：

```csharp
using ImageGlider;
using ImageGlider.Enums;

// 格式转换
bool success = ImageConverter.ConvertImage("input.jpg", "output.png", quality: 85);

// 转换为 AVIF 格式（现代高效压缩）
bool avifSuccess = ImageConverter.ConvertImage("input.jpg", "output.avif", quality: 80);

// 从 AVIF 格式转换
bool fromAvif = ImageConverter.ConvertImage("input.avif", "output.png");

// 尺寸调整
bool resized = ImageConverter.ResizeImage("input.jpg", "output.jpg", 800, 600, ResizeMode.KeepAspectRatio);

// 图像压缩
bool compressed = ImageConverter.CompressImage("input.jpg", "compressed.jpg", quality: 70);

// 添加文本水印
bool watermarked = ImageConverter.AddTextWatermark("input.jpg", "watermarked.jpg", "© 2024", WatermarkPosition.BottomRight);

// 批量转换为 AVIF 格式
var avifResult = ImageConverter.BatchConvert("./photos", "./avif", ".jpg", ".avif", quality: 75);
Console.WriteLine($"AVIF 转换成功: {avifResult.SuccessfulConversions}/{avifResult.TotalFiles}");

// 批量处理
var result = ImageConverter.BatchConvert("./input", "./output", ".jpg", ".png", quality: 90);
Console.WriteLine($"成功转换: {result.SuccessfulConversions}/{result.TotalFiles}");

// 获取图像信息
var info = ImageConverter.GetImageInfo("image.jpg");
Console.WriteLine($"尺寸: {info.Width}x{info.Height}, 格式: {info.Format}");

// 图像文件验证
using ImageGlider.Utilities;

// 检测文件是否为有效图片（综合检测）
bool isValidImage = ImageValidator.IsValidImageFile("path/to/file.jpg");

// 启用深度验证（通过ImageSharp加载验证）
bool isValidWithDeepCheck = ImageValidator.IsValidImageFile("path/to/file.jpg", useDeepValidation: true);

// 仅检测文件扩展名
bool hasImageExtension = ImageValidator.IsValidImageExtension("file.png");

// 基于文件头检测（魔数签名）
bool isValidBySignature = ImageValidator.IsValidImageBySignature("path/to/file.jpg");

// 获取支持的图片格式列表
string[] supportedFormats = ImageValidator.GetSupportedExtensions();
Console.WriteLine($"支持的格式: {string.Join(", ", supportedFormats)}");
```

#### 高级功能示例

```csharp
using ImageGlider.Processors;
using ImageGlider.Core;

// 使用处理器工厂
var resizer = ImageProcessorFactory.CreateResizer();
var compressor = ImageProcessorFactory.CreateCompressor();
var watermark = ImageProcessorFactory.CreateWatermark();

// 链式处理
bool processed = resizer.ProcessImage("input.jpg", "temp.jpg", 800, 600) &&
                compressor.ProcessImage("temp.jpg", "final.jpg", 80);
```

## 📊 测试覆盖率

当前测试状态：
- **测试用例数量**: 180个
- **行覆盖率**: 69.4%
- **分支覆盖率**: 57.3%
- **测试通过率**: 100%

运行测试并生成覆盖率报告：

```bash
# 运行所有测试
dotnet test

# 生成覆盖率报告
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults

# 生成HTML覆盖率报告
reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"TestResults/CoverageReport" -reporttypes:Html
```

## 🏗️ 项目架构

### 📁 目录结构

```
ImageGlider/
├── src/
│   ├── ImageGlider/              # 核心类库
│   │   ├── Core/                 # 核心接口和工厂
│   │   ├── Processors/           # 图像处理器
│   │   ├── Utilities/            # 工具类
│   │   ├── Enums/               # 枚举定义
│   │   └── ImageConverter.cs     # 主要API入口
│   ├── ImageGlider.Cli/          # 命令行工具
│   │   ├── Commands/            # 16种命令实现
│   │   └── Program.cs           # CLI程序入口
│   ├── ImageGlider.WebApi/       # Web API服务
│   │   ├── Endpoints/           # API端点
│   │   ├── Services/            # 服务层
│   │   └── Program.cs           # API程序入口
│   └── ImageGlider.Example/      # 示例程序
│       └── Program.cs           # 使用示例
└── tests/
    └── ImageGlider.Tests/        # 单元测试
        ├── TestHelpers/         # 测试辅助工具
        └── *.cs                 # 180个测试用例
```

### 🔧 核心组件

- **ImageConverter**: 主要API入口，提供所有图像处理功能
- **ImageProcessorFactory**: 处理器工厂，创建各种专用处理器
- **ImageSizeCalculator**: 尺寸计算工具，支持多种调整模式
- **ImageValidator**: 图像文件验证工具，提供多种检测模式验证文件是否为有效图片
- **处理器系列**: 格式转换、尺寸调整、压缩、裁剪、水印等专用处理器

## 🛠️ 开发指南

### 环境要求

- .NET 9 SDK
- Visual Studio 2022 或 JetBrains Rider（推荐）
- Git

### 构建和测试

```bash
# 克隆项目
git clone https://github.com/Deali-Axy/image-glider.git
cd image-glider

# 恢复依赖
dotnet restore

# 构建项目
dotnet build -c Release

# 运行测试
dotnet test

# 运行示例
dotnet run --project src/ImageGlider.Example
```

### AOT 发布优化

```bash
# Windows x64 AOT（优化体积）
dotnet publish src/ImageGlider.Cli -c Release -r win-x64 -p:PublishAot=true -p:IlcOptimizationPreference=Size

# Linux x64 AOT（优化性能）
dotnet publish src/ImageGlider.Cli -c Release -r linux-x64 -p:PublishAot=true -p:IlcOptimizationPreference=Speed
```

### 支持的平台

- **Windows**: win-x64, win-arm64
- **macOS**: osx-x64, osx-arm64 (Intel & Apple Silicon)
- **Linux**: linux-x64, linux-arm64

## 🤝 贡献指南

我们欢迎各种形式的贡献！

### 如何贡献

1. **Fork** 本仓库
2. 创建您的特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交您的更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 打开一个 **Pull Request**

### 贡献类型

- 🐛 **Bug 修复**: 发现并修复问题
- ✨ **新功能**: 添加新的图像处理功能
- 📚 **文档**: 改进文档和示例
- 🧪 **测试**: 增加测试覆盖率
- 🎨 **代码质量**: 重构和优化代码
- 🌐 **国际化**: 添加多语言支持

### 开发规范

- 遵循 C# 编码规范
- 为新功能添加单元测试
- 更新相关文档
- 确保所有测试通过

## 📄 许可证

本项目采用 [MIT 许可证](LICENSE) - 查看 LICENSE 文件了解详情。

## 📋 AVIF 格式支持说明

### 🆕 AVIF 格式特性

**AVIF (AV1 Image File Format)** 是基于 AV1 视频编解码器的现代图像格式，具有以下优势：

- **🗜️ 高压缩效率**: 相比 JPEG 可减少 50% 以上的文件大小
- **🎨 优秀画质**: 支持 10-bit 和 12-bit 色深，色彩还原更准确
- **🌐 现代标准**: 由 Alliance for Open Media 开发的开放标准
- **📱 广泛支持**: Chrome、Firefox、Safari 等主流浏览器已支持

### ⚙️ 技术实现

ImageGlider 通过 **ImageMagick.NET** 库提供 AVIF 格式支持：

- **编码器**: 使用 AV1 编码器进行高效压缩
- **质量控制**: 支持 1-100 的质量参数调节
- **兼容性**: 自动处理格式检测和转换
- **性能优化**: 针对批量处理进行了优化

### 💡 使用建议

- **Web 应用**: AVIF 格式特别适合 Web 图像优化
- **质量设置**: 推荐质量参数 75-85 获得最佳压缩比和画质平衡
- **兼容性**: 对于需要广泛兼容的场景，建议同时提供 WebP 或 JPEG 备选
- **批量转换**: 利用批量转换功能可高效处理大量图像

## 🙏 致谢

- [SixLabors.ImageSharp](https://github.com/SixLabors/ImageSharp) - 强大的跨平台图像处理库
- [ImageMagick.NET](https://github.com/dlemstra/Magick.NET) - ImageMagick 的 .NET 绑定，提供 AVIF 格式支持
- [.NET 团队](https://github.com/dotnet) - 提供优秀的开发平台
- 所有贡献者和用户的支持

## 📞 联系方式

- **项目主页**: [GitHub Repository](https://github.com/Deali-Axy/image-glider)
- **问题反馈**: [GitHub Issues](https://github.com/Deali-Axy/image-glider/issues)
- **功能请求**: [GitHub Discussions](https://github.com/Deali-Axy/image-glider/discussions)

---

⭐ 如果这个项目对您有帮助，请给我们一个星标！
