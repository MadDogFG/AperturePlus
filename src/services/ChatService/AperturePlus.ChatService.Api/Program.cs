using AperturePlus.ChatService.Api.BackgroundServices;
using AperturePlus.ChatService.Api.Hubs;
using AperturePlus.ChatService.Application.Handlers;
using AperturePlus.ChatService.Application.Interfaces;
using AperturePlus.ChatService.Domain.Entities;
using AperturePlus.ChatService.Infrastructure.MessageBus;
using AperturePlus.ChatService.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using RabbitMQ.Client;

namespace AperturePlus.ChatService.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSignalR();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

                    // 为 SignalR 添加自定义的 token 解析逻辑
                    // 因为 WebSocket 和 EventSource 连接无法发送 Authorization 头, 
                    // 客户端会将其作为查询参数发送
                    opt.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            // 1. 尝试从查询字符串中获取 token
                            var accessToken = context.Request.Query["access_token"];

                            // 2. 检查请求路径是否指向我们的 Hub
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/chathub")))
                            {
                                // 3. 如果是，将 token 放入 Bearer 上下文中
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(SendMessageCommandHandler).Assembly);
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
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowVueApp", policy =>
                {
                    policy.WithOrigins("http://localhost:5173") //允许Vue的源
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            builder.Services.AddSingleton<IMongoClient>(sp =>
                new MongoClient(builder.Configuration.GetConnectionString("MongoDbConnection")));
            builder.Services.AddScoped(sp =>
                sp.GetRequiredService<IMongoClient>()
                  .GetDatabase(builder.Configuration["MongoDbSettings:DatabaseName"])
                  .GetCollection<Conversation>(builder.Configuration["MongoDbSettings:CollectionName"]));
            builder.Services.AddScoped(sp =>
                sp.GetRequiredService<IMongoClient>()
                  .GetDatabase(builder.Configuration["MongoDbSettings:DatabaseName"])
                  .GetCollection<UserSummary>("UserSummaries")); // 新增 UserSummary 集合

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowVueApp", policy =>
                {
                    policy.WithOrigins("http://localhost:5173") //允许Vue的源
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); //SignalR 跨域带 token 必须加这个
                });
            });

            builder.Services.AddHostedService<UserEventsConsumer>();//注册后台消费者服务
            builder.Services.AddSingleton<IMessageBus, RabbitMQMessageBus>();
            builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
            builder.Services.AddScoped<IUserSummaryRepository, UserSummaryRepository>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowVueApp");

            app.UseAuthentication();
            app.UseAuthorization();

            // 映射 SignalR Hub
            app.MapHub<ChatHub>("/chathub");

            app.MapControllers();

            app.Run();
        }
    }
}
