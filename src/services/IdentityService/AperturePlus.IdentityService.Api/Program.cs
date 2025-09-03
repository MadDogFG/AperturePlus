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
                opt.Lockout.MaxFailedAccessAttempts = 5;//���ʧ�ܴ���
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);//����ʱ��
                opt.Password.RequireDigit = false;//�Ƿ���Ҫ����
                opt.Password.RequireLowercase = false;//�Ƿ���ҪСд��ĸ
                opt.Password.RequireUppercase = false; //�Ƿ���Ҫ��д��ĸ
                opt.Password.RequireNonAlphanumeric = false;//�Ƿ���Ҫ�����ַ�
                opt.Password.RequiredLength = 6;//���볤��
                //opt.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;//���������ù��ܵ�������������֤�߼����󶨵�Ĭ�ϵ����ʼ������ṩ�ߣ�DefaultEmailProvider��
                //opt.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;//�������ʼ�ȷ�Ϲ��ܵ�������������֤�߼����󶨵�Ĭ�ϵ����ʼ������ṩ�ߣ�DefaultEmailProvider��
            }).AddRoles<ApplicationRole>().AddEntityFrameworkStores<IdentityServiceDbContext>();

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
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterCommandValidator>();//ע������е�������֤��

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(RegisterCommand).Assembly);
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

            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));//�������ļ��е�JwtSettings���ְ󶨵�JwtSettings��
            builder.Services.Configure<RoleSettings>(builder.Configuration.GetSection("RoleSettings"));//�������ļ��е�RoleSettings���ְ󶨵�RoleSettings��

            builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();//ע��JWT��������������

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
                Console.WriteLine("�޷����ӵ�RabbitMQ������: " + ex.Message);
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
                    logger.LogInformation("���ݲ��ֳɹ�");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "���ݲ��ִ���");
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
