# ImageGlider

**ImageGlider** 是一个跨平台的图像格式转换工具套件，使用 C# (.NET 8) 和 [ImageSharp](https://github.com/SixLabors/ImageSharp) 实现。项目采用模块化架构设计，包含核心类库、命令行工具、示例程序和完整的单元测试，支持 AOT 编译以获得原生性能。

## 项目架构

- **ImageGlider** - 核心类库，提供图像转换和日志记录功能
- **ImageGlider.Cli** - 命令行工具，支持单文件和批量转换
- **ImageGlider.Example** - 示例程序，展示核心类库的典型用法
- **ImageGlider.Tests** - 单元测试项目，确保代码质量

## 特性

- **跨平台**：基于 .NET 8 和 ImageSharp，支持 Windows、Linux、macOS 等平台
- **AOT 编译**：利用 .NET 8 的 AOT 功能，提供更快的启动速度和原生性能
- **模块化设计**：核心功能封装为独立类库，便于集成到其他项目中
- **命令行工具**：提供功能完整的 CLI 工具，支持丰富的命令行参数
- **单文件转换**：支持转换单个图像文件，可指定 JPEG 质量参数
- **批量转换**：自动扫描指定目录中的文件并批量转换
- **灵活配置**：支持自定义源目录、输出目录、日志目录等参数
- **详细日志**：提供完整的日志记录功能，支持多种日志级别（信息、警告、错误）
- **错误处理**：完善的异常处理机制，提供详细的转换结果信息

## 前置条件

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- 支持的图像格式：JPEG、PNG、GIF、BMP、TIFF、WebP 等（由 ImageSharp 提供支持）

## 依赖项

- **SixLabors.ImageSharp** (3.1.11) - 跨平台图像处理库
- **xUnit** - 单元测试框架（仅测试项目）

## 快速开始

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

### 命令行工具 (ImageGlider.Cli)

#### 查看帮助信息

```bash
ImageGlider.Cli help
```

#### 单文件转换

```bash
# 基本用法
ImageGlider.Cli convert --source image.jfif --target image.jpeg

# 指定 JPEG 质量
ImageGlider.Cli convert -s image.jfif -t image.jpeg -q 85
```

#### 批量转换

```bash
# 基本批量转换（当前目录）
ImageGlider.Cli batch --source-ext .jfif --target-ext .jpeg

# 完整参数示例
ImageGlider.Cli batch -se .jfif -te .jpeg -sd ./input -od ./output -ld ./logs -q 90
```

#### 参数说明

**convert 命令参数：**
- `--source, -s`: 源文件路径（必需）
- `--target, -t`: 目标文件路径（必需）
- `--quality, -q`: JPEG 质量，范围 1-100（默认：90）

**batch 命令参数：**
- `--source-ext, -se`: 源文件扩展名（必需，如：.jfif）
- `--target-ext, -te`: 目标文件扩展名（必需，如：.jpeg）
- `--source-dir, -sd`: 源目录路径（默认：当前目录）
- `--output-dir, -od`: 输出目录路径（默认：./output）
- `--log-dir, -ld`: 日志目录路径（默认：./log）
- `--quality, -q`: JPEG 质量，范围 1-100（默认：90）

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

## 贡献

欢迎贡献代码、提交 issue 或 pull request，共同完善这个项目。

## 许可

本项目采用 MIT License 许可协议。

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

## 致谢

- 感谢 [SixLabors ImageSharp](https://github.com/SixLabors/ImageSharp) 团队提供强大的跨平台图像处理库
- 感谢 Microsoft .NET 团队提供 .NET 8 的跨平台支持和 AOT 编译特性
- 感谢 xUnit 团队提供优秀的测试框架
- 感谢所有开源社区的贡献者们

