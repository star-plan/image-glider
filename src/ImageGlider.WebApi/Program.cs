using ImageGlider.WebApi.Endpoints.Internal;
using ImageGlider.WebApi.Exceptions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails(); 
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// 配置JSON序列化选项
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    options.SerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
});

builder.Services.AddServices<Program>(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        opt => opt.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithExposedHeaders("http://localhost:3000/", "https://localhost:3000/api")
        );
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("ImageGlider API"); // 设置标题
    });
}
app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseExceptionHandler();

// 注册端点
app.UseEndpoints<Program>();

app.Run();

// 使 Program 类可以被测试项目访问
public partial class Program { }
