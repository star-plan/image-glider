# ImageGlider.WebApi.Tests

这是 ImageGlider WebApi 项目的专用测试项目，提供了全面的单元测试和集成测试覆盖。

## 项目结构

```
ImageGlider.WebApi.Tests/
├── Integration/                 # 集成测试
│   ├── WebApiTestBase.cs       # 集成测试基类
│   ├── ImageResizeEndpointsTests.cs    # 图片调整端点测试
│   ├── ImageCompressEndpointsTests.cs  # 图片压缩端点测试
│   ├── ImageDownloadTests.cs           # 图片下载端点测试
│   ├── PerformanceTests.cs             # 性能测试
│   └── SecurityTests.cs                # 安全性测试
├── Unit/                       # 单元测试
│   ├── FileServiceTests.cs     # 文件服务测试
│   ├── GlobalExceptionHandlerTests.cs # 全局异常处理器测试
│   └── ApiResponseTests.cs     # API 响应模型测试
├── TestHelpers/                # 测试辅助工具
│   └── WebTestHelper.cs        # Web 测试辅助类
└── ImageGlider.WebApi.Tests.csproj    # 项目文件
```

## 测试覆盖范围

### 集成测试

1. **图片调整端点测试** (`ImageResizeEndpointsTests.cs`)
   - 有效的图片调整请求
   - 无效文件类型处理
   - 参数验证
   - 不同图片格式支持
   - 缩略图生成

2. **图片压缩端点测试** (`ImageCompressEndpointsTests.cs`)
   - 图片压缩功能
   - 质量参数验证
   - 文件大小限制
   - 并发请求处理

3. **图片下载端点测试** (`ImageDownloadTests.cs`)
   - 文件下载功能
   - 文件不存在处理
   - 路径遍历攻击防护
   - 响应头验证

4. **性能测试** (`PerformanceTests.cs`)
   - 高负载下的性能表现
   - 并发请求处理
   - 内存使用监控
   - 响应时间测试

5. **安全性测试** (`SecurityTests.cs`)
   - 路径遍历攻击防护
   - 文件类型验证
   - 输入参数安全性
   - XSS 和注入攻击防护

### 单元测试

1. **文件服务测试** (`FileServiceTests.cs`)
   - 文件上传和保存
   - 文件验证逻辑
   - 错误处理
   - 文件路径处理

2. **全局异常处理器测试** (`GlobalExceptionHandlerTests.cs`)
   - 异常类型处理
   - HTTP 状态码映射
   - 错误响应格式

3. **API 响应模型测试** (`ApiResponseTests.cs`)
   - 响应模型序列化
   - 数据验证
   - 边界条件测试

## 使用的测试框架和工具

- **xUnit**: 主要测试框架
- **FluentAssertions**: 流畅的断言库
- **Microsoft.AspNetCore.Mvc.Testing**: ASP.NET Core 集成测试
- **Moq**: 模拟对象框架

## 运行测试

### 运行所有测试
```bash
dotnet test tests\ImageGlider.WebApi.Tests\ImageGlider.WebApi.Tests.csproj
```

### 运行特定类别的测试
```bash
# 只运行集成测试
dotnet test tests\ImageGlider.WebApi.Tests\ImageGlider.WebApi.Tests.csproj --filter "FullyQualifiedName~Integration"

# 只运行单元测试
dotnet test tests\ImageGlider.WebApi.Tests\ImageGlider.WebApi.Tests.csproj --filter "FullyQualifiedName~Unit"
```

### 运行特定测试类
```bash
# 运行文件服务测试
dotnet test tests\ImageGlider.WebApi.Tests\ImageGlider.WebApi.Tests.csproj --filter "FullyQualifiedName~FileServiceTests"

# 运行安全性测试
dotnet test tests\ImageGlider.WebApi.Tests\ImageGlider.WebApi.Tests.csproj --filter "FullyQualifiedName~SecurityTests"
```

### 生成测试覆盖率报告
```bash
dotnet test tests\ImageGlider.WebApi.Tests\ImageGlider.WebApi.Tests.csproj --collect:"XPlat Code Coverage"
```

## 测试配置

### 环境要求
- .NET 9.0 或更高版本
- ImageGlider.WebApi 项目正常运行
- 足够的磁盘空间用于临时文件

### 测试数据
测试使用 `WebTestHelper` 类动态生成测试图片，无需外部测试文件。

### 临时文件管理
所有测试都会自动清理生成的临时文件，确保测试环境的整洁。

## 注意事项

1. **集成测试依赖**: 集成测试需要 WebApi 项目能够正常启动
2. **性能测试**: 性能测试可能需要较长时间，建议在 CI/CD 中单独运行
3. **并发测试**: 某些并发测试可能在资源受限的环境中失败
4. **安全测试**: 安全测试验证了基本的安全防护，但不能替代专业的安全审计

## 扩展测试

要添加新的测试：

1. **集成测试**: 继承 `WebApiTestBase` 类
2. **单元测试**: 直接创建测试类
3. **测试辅助**: 在 `WebTestHelper` 中添加通用方法

示例：
```csharp
public class NewEndpointTests : WebApiTestBase
{
    public NewEndpointTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }
    
    [Fact]
    public async Task NewEndpoint_ShouldWork()
    {
        // 测试实现
    }
}
```

## 贡献指南

1. 确保所有新测试都有适当的文档注释
2. 遵循现有的命名约定
3. 添加适当的测试用例覆盖边界条件
4. 确保测试是独立的，不依赖于其他测试的执行顺序
5. 使用 `WebTestHelper` 中的方法创建测试数据