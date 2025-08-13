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
