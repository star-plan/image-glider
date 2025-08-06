using ImageGlider.WebApi.Model;
using System.Text.Json;

namespace ImageGlider.WebApi.Tests.Unit;

/// <summary>
/// API 响应模型单元测试
/// </summary>
public class ApiResponseTests
{
    [Fact]
    public void ApiResponse_DefaultConstructor_ShouldSetDefaultValues()
    {
        // Act
        var response = new ApiResponse();
        
        // Assert
        response.StatusCode.Should().Be(200);
        response.Successful.Should().BeTrue();
        response.Message.Should().Be("操作成功");
        response.Data.Should().BeNull();
    }
    
    [Fact]
    public void ApiResponse_WithCustomValues_ShouldSetCorrectly()
    {
        // Arrange
        var statusCode = 400;
        var successful = false;
        var message = "操作失败";
        var data = "test data";
        
        // Act
        var response = new ApiResponse
        {
            StatusCode = statusCode,
            Successful = successful,
            Message = message,
            Data = data
        };
        
        // Assert
        response.StatusCode.Should().Be(statusCode);
        response.Successful.Should().Be(successful);
        response.Message.Should().Be(message);
        response.Data.Should().Be(data);
    }
    
    [Fact]
    public void ApiResponse_Serialization_ShouldSerializeCorrectly()
    {
        // Arrange
        var response = new ApiResponse
        {
            StatusCode = 201,
            Successful = true,
            Message = "创建成功",
            Data = new { Id = 1, Name = "Test" }
        };
        
        // Act
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        
        // Assert
        json.Should().NotBeNullOrEmpty();
        json.Should().Contain("statusCode");
        json.Should().Contain("successful");
        json.Should().Contain("message");
        json.Should().Contain("data");
        json.Should().Contain("201");
        json.Should().Contain("创建成功");
    }
    
    [Fact]
    public void ApiResponse_Deserialization_ShouldDeserializeCorrectly()
    {
        // Arrange
        var json = @"{
            ""statusCode"": 404,
            ""successful"": false,
            ""message"": ""资源未找到"",
            ""data"": null
        }";
        
        // Act
        var response = JsonSerializer.Deserialize<ApiResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        // Assert
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(404);
        response.Successful.Should().BeFalse();
        response.Message.Should().Be("资源未找到");
        response.Data.Should().BeNull();
    }
    
    [Fact]
    public void ApiResponse_WithComplexData_ShouldHandleCorrectly()
    {
        // Arrange
        var complexData = new
        {
            Files = new[] { "file1.jpg", "file2.png" },
            ProcessingTime = TimeSpan.FromSeconds(2.5),
            Metadata = new Dictionary<string, object>
            {
                ["width"] = 1920,
                ["height"] = 1080,
                ["format"] = "JPEG"
            }
        };
        
        // Act
        var response = new ApiResponse
        {
            StatusCode = 200,
            Successful = true,
            Message = "批量处理完成",
            Data = complexData
        };
        
        // Assert
        response.Data.Should().NotBeNull();
        response.Data.Should().Be(complexData);
    }
    
    [Theory]
    [InlineData(200, true, "成功")]
    [InlineData(400, false, "请求错误")]
    [InlineData(404, false, "未找到")]
    [InlineData(500, false, "服务器错误")]
    public void ApiResponse_WithDifferentStatusCodes_ShouldSetCorrectly(int statusCode, bool expectedSuccessful, string message)
    {
        // Act
        var response = new ApiResponse
        {
            StatusCode = statusCode,
            Successful = expectedSuccessful,
            Message = message
        };
        
        // Assert
        response.StatusCode.Should().Be(statusCode);
        response.Successful.Should().Be(expectedSuccessful);
        response.Message.Should().Be(message);
    }
    
    [Fact]
    public void ApiResponse_WithNullMessage_ShouldHandleGracefully()
    {
        // Act
        var response = new ApiResponse
        {
            Message = null!
        };
        
        // Assert
        response.Message.Should().BeNull();
    }
    
    [Fact]
    public void ApiResponse_WithEmptyMessage_ShouldHandleCorrectly()
    {
        // Act
        var response = new ApiResponse
        {
            Message = string.Empty
        };
        
        // Assert
        response.Message.Should().BeEmpty();
    }
    
    [Fact]
    public void ApiResponse_SerializationRoundTrip_ShouldMaintainData()
    {
        // Arrange
        var originalResponse = new ApiResponse
        {
            StatusCode = 201,
            Successful = true,
            Message = "资源已创建",
            Data = new { Id = 123, Name = "测试文件.jpg", Size = 1024 }
        };
        
        // Act
        var json = JsonSerializer.Serialize(originalResponse);
        var deserializedResponse = JsonSerializer.Deserialize<ApiResponse>(json);
        
        // Assert
        deserializedResponse.Should().NotBeNull();
        deserializedResponse!.StatusCode.Should().Be(originalResponse.StatusCode);
        deserializedResponse.Successful.Should().Be(originalResponse.Successful);
        deserializedResponse.Message.Should().Be(originalResponse.Message);
        // 注意：复杂对象在反序列化后可能是 JsonElement 类型
        deserializedResponse.Data.Should().NotBeNull();
    }
    
    [Fact]
    public void ApiResponse_WithLongMessage_ShouldHandleCorrectly()
    {
        // Arrange
        var longMessage = new string('A', 1000); // 1000个字符的长消息
        
        // Act
        var response = new ApiResponse
        {
            Message = longMessage
        };
        
        // Assert
        response.Message.Should().Be(longMessage);
        response.Message.Length.Should().Be(1000);
    }
    
    [Fact]
    public void ApiResponse_WithSpecialCharacters_ShouldHandleCorrectly()
    {
        // Arrange
        var messageWithSpecialChars = "操作成功！文件名：测试图片_2024.jpg，大小：1.5MB，格式：JPEG";
        
        // Act
        var response = new ApiResponse
        {
            Message = messageWithSpecialChars
        };
        
        // Assert
        response.Message.Should().Be(messageWithSpecialChars);
    }
}