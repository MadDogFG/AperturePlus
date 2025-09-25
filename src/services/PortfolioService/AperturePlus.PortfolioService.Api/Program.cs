
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
                        // ��֤Issuer�Ƿ�Ϸ�
                        ValidateIssuer = true,
                        // ��֤Audience�Ƿ�Ϸ�
                        ValidateAudience = true,
                        // ��֤���Ƶ���������
                        ValidateLifetime = true,
                        // ��֤ǩ����Կ�Ƿ���Ч
                        ValidateIssuerSigningKey = true,
                        // ���úϷ���Issuer
                        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                        // ���úϷ���Audience
                        ValidAudience = builder.Configuration["JwtSettings:Audience"],
                        // ǩ����Կ��
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
                    Description = "Authorization ����ͷ. \r\nʾ��: 'Bearer XXXXXX'",
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
                Console.WriteLine("�޷����ӵ�RabbitMQ������: " + ex.Message);
            }

            builder.Services.AddSingleton<IMongoClient>(sp =>//ע��MongoDB�Ŀͻ���
            {
                return new MongoClient(builder.Configuration.GetConnectionString("MongoDbConnection"));
            });

            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));//�����л�Guid����ʱʹ��String��ʽ����д��������Ĭ�ϵ�Binary��ʽ
            builder.Services.AddScoped(sp =>//ע��IMongoCollection<Portfolio>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                var database = builder.Configuration["MongoDbSettings:DatabaseName"];
                var collection = builder.Configuration["MongoDbSettings:CollectionName"];
                return client.GetDatabase(database).GetCollection<Portfolio>(collection);
            });

            Console.WriteLine($"{builder.Configuration["MinioSettings:Endpoint"]},{builder.Configuration["MinioSettings:AccessKey"]},{builder.Configuration["MinioSettings:SecretKey"]}");
            // ע�� MinioClient (��Ҫ�� appsettings.json ��ȡ endpoint, accessKey, secretKey)
            builder.Services.AddMinio(options => options
                .WithEndpoint(builder.Configuration["MinioSettings:Endpoint"])
                .WithCredentials(builder.Configuration["MinioSettings:AccessKey"], builder.Configuration["MinioSettings:SecretKey"])
                .Build());

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowVueApp", policy =>
                {
                    policy.WithOrigins("http://localhost:5173") //����Vue��Դ
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
            
            builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();
            builder.Services.AddHostedService<BackgroundServices.UserEventsConsumer>();//ע���̨����
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

            app.UseCors("AllowVueApp"); // ����CORS����
            app.MapControllers();

            app.Run();
        }
    }
}
