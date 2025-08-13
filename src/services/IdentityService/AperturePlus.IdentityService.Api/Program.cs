using AperturePlus.IdentityService.Application.Commands;
using AperturePlus.IdentityService.Application.Validators;
using AperturePlus.IdentityService.Domain.Entities;
using AperturePlus.IdentityService.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AperturePlus.IdentityService.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<IdentityServiceDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
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
