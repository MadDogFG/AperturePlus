using AperturePlus.IdentityService.Api.Options;
using AperturePlus.IdentityService.Application.Commands;
using AperturePlus.IdentityService.Application.Interfaces;
using AperturePlus.IdentityService.Application.Validators;
using AperturePlus.IdentityService.Domain.Entities;
using AperturePlus.IdentityService.Domain.Events;
using AperturePlus.IdentityService.Infrastructure.MessageBus;
using AperturePlus.IdentityService.Infrastructure.Persistence;
using AperturePlus.IdentityService.Infrastructure.Services;
using AperturePlus.IdentityService.Infrastructure.Settings;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AperturePlus.IdentityService.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<IdentityServiceDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("SQLConnection"));
            });

            builder.Services.AddIdentityCore<ApplicationUser>(opt =>
            {
                opt.Lockout.MaxFailedAccessAttempts = 5;//最大失败次数
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);//锁定时间
                opt.Password.RequireDigit = false;//是否需要数字
                opt.Password.RequireLowercase = false;//是否需要小写字母
                opt.Password.RequireUppercase = false; //是否需要大写字母
                opt.Password.RequireNonAlphanumeric = false;//是否需要特殊字符
                opt.Password.RequiredLength = 6;//密码长度
                //opt.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;//将密码重置功能的令牌生成与验证逻辑，绑定到默认电子邮件令牌提供者（DefaultEmailProvider）
                //opt.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;//将电子邮件确认功能的令牌生成与验证逻辑，绑定到默认电子邮件令牌提供者（DefaultEmailProvider）
            }).AddRoles<ApplicationRole>().AddEntityFrameworkStores<IdentityServiceDbContext>();

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
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterCommandValidator>();//注册程序集中的所有验证器

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(RegisterCommand).Assembly);
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

            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));//将配置文件中的JwtSettings部分绑定到JwtSettings类
            builder.Services.Configure<RoleSettings>(builder.Configuration.GetSection("RoleSettings"));//将配置文件中的RoleSettings部分绑定到RoleSettings类

            builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();//注册JWT令牌生成器服务

            builder.Services.AddTransient<DataSeeder>();
            
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
            catch(Exception ex)
            {
                Console.WriteLine("无法连接到RabbitMQ服务器: " + ex.Message);
            }

            builder.Services.AddSingleton<IMessageBus, RabbitMQMessageBus>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                try
                {
                    var seeder = services.GetRequiredService<DataSeeder>();
                    await seeder.SeedAsync();
                    logger.LogInformation("数据播种成功");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "数据播种错误");
                }
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
