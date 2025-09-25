
using AperturePlus.PortfolioService.Application.Commands;
using AperturePlus.PortfolioService.Application.Interfaces;
using AperturePlus.PortfolioService.Domain.Entities;
using AperturePlus.PortfolioService.Infrastructure.Repositories;
using AperturePlus.PortfolioService.Infrastructure.Services;
using Microsoft.AspNetCore.Connections;
using Microsoft.OpenApi.Models;
using Minio;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using RabbitMQ.Client;

namespace AperturePlus.PortfolioService.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAuthentication()
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        // 验证Issuer是否合法
                        ValidateIssuer = true,
                        // 验证Audience是否合法
                        ValidateAudience = true,
                        // 验证令牌的生命周期
                        ValidateLifetime = true,
                        // 验证签名密钥是否有效
                        ValidateIssuerSigningKey = true,
                        // 设置合法的Issuer
                        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                        // 设置合法的Audience
                        ValidAudience = builder.Configuration["JwtSettings:Audience"],
                        // 签名密钥）
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
                    };
                });

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CreatePortfolioForUserCommand).Assembly);
            });

            builder.Services.AddSwaggerGen(c =>
            {
                var scheme = new OpenApiSecurityScheme()
                {
                    Description = "Authorization 请求头. \r\n示例: 'Bearer XXXXXX'",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Authorization"
                    },
                    Scheme = "oauth2",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                };
                c.AddSecurityDefinition("Authorization", scheme);
                var requirement = new OpenApiSecurityRequirement();
                requirement[scheme] = new List<string>();
                c.AddSecurityRequirement(requirement);
            });

            try
            {
                var factory = new ConnectionFactory()
                {
                    Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQConnection"))
                };
                Console.WriteLine(builder.Configuration.GetConnectionString("RabbitMQConnection"));
                var connection = await factory.CreateConnectionAsync();
                builder.Services.AddSingleton<IConnection>(connection);
            }
            catch (Exception ex)
            {
                Console.WriteLine("无法连接到RabbitMQ服务器: " + ex.Message);
            }

            builder.Services.AddSingleton<IMongoClient>(sp =>//注册MongoDB的客户端
            {
                return new MongoClient(builder.Configuration.GetConnectionString("MongoDbConnection"));
            });

            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));//在序列化Guid类型时使用String格式来读写，而不是默认的Binary格式
            builder.Services.AddScoped(sp =>//注册IMongoCollection<Portfolio>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                var database = builder.Configuration["MongoDbSettings:DatabaseName"];
                var collection = builder.Configuration["MongoDbSettings:CollectionName"];
                return client.GetDatabase(database).GetCollection<Portfolio>(collection);
            });

            Console.WriteLine($"{builder.Configuration["MinioSettings:Endpoint"]},{builder.Configuration["MinioSettings:AccessKey"]},{builder.Configuration["MinioSettings:SecretKey"]}");
            // 注册 MinioClient (需要从 appsettings.json 读取 endpoint, accessKey, secretKey)
            builder.Services.AddMinio(options => options
                .WithEndpoint(builder.Configuration["MinioSettings:Endpoint"])
                .WithCredentials(builder.Configuration["MinioSettings:AccessKey"], builder.Configuration["MinioSettings:SecretKey"])
                .Build());

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowVueApp", policy =>
                {
                    policy.WithOrigins("http://localhost:5173") //允许Vue的源
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
            
            builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();
            builder.Services.AddHostedService<BackgroundServices.UserEventsConsumer>();//注册后台服务
            builder.Services.AddScoped<IFileStorageService, FileStorageService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors("AllowVueApp"); // 启用CORS策略
            app.MapControllers();

            app.Run();
        }
    }
}
