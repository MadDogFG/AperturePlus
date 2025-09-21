
using AperturePlus.RatingService.Api.BackgroundServices;
using AperturePlus.RatingService.Application.Handlers;
using AperturePlus.RatingService.Application.Interfaces;
using AperturePlus.RatingService.Infrastructure.Persistence;
using AperturePlus.RatingService.Infrastructure.Repositories;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;

namespace AperturePlus.RatingService.Api
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
            builder.Services.AddDbContext<RatingServiceDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("SQLConnection"));
            });

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
                cfg.RegisterServicesFromAssembly(typeof(SubmitRatingCommandHandler).Assembly);
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

            builder.Services.AddHostedService<ActivityEventsConsumer>();//ע���̨�����߷���
            builder.Services.AddScoped<IRatingRepository, RatingRepository>();
            builder.Services.AddScoped<IPendingRatingRepository, PendingRatingRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
