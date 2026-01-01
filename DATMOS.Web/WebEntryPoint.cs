using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using DATMOS.Data;
using DATMOS.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace DATMOS.Web
{
    public static class WebEntryPoint
    {
        public static IHost CreateHostBuilder(string[] args, int port)
        {
            // Cấu hình Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information() // Mức độ log tối thiểu
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning) // Giảm log rác từ Microsoft
                .Enrich.FromLogContext()
                .WriteTo.Console() // Ghi ra console
                .WriteTo.File(
                    path: "Log/DATMOS.Web-.txt", // Đường dẫn file, dấu "-" để nối ngày
                    rollingInterval: RollingInterval.Day, // Tạo file mới mỗi ngày
                    retainedFileCountLimit: 30, // Giữ log trong 30 ngày
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();

            var host = Host.CreateDefaultBuilder(args)
                .UseSerilog() // Đăng ký Serilog với Host
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(options =>
                    {
                        options.ListenLocalhost(port);
                    })
                    .PreferHostingUrls(false)
                    .ConfigureServices((context, services) =>
                    {
                        // Add MVC services
                        services.AddControllersWithViews()
                            .AddNewtonsoftJson(options =>
                            {
                                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                            })
                            .AddRazorOptions(options =>
                            {
                                // Tự động tìm View/Partial trong thư mục AdminSections mới
                                options.AreaViewLocationFormats.Add("/Areas/Admin/Views/Shared/AdminSections/{0}.cshtml");
                            });

                        // Get configuration
                        var configuration = context.Configuration;

                        // Add other services
                        services.AddHttpContextAccessor();
                        services.AddRouting();
                        services.AddSession();
                        services.AddMemoryCache(); // Add memory cache for ProfileService

                        // Register custom services
                        services.AddScoped<DATMOS.Web.Services.IExamSubjectService, DATMOS.Web.Services.ExamSubjectService>();
                        services.AddScoped<DATMOS.Web.Services.IExamListService, DATMOS.Web.Services.ExamListService>();
                        services.AddScoped<DATMOS.Web.Services.ICoursesService, DATMOS.Web.Services.CoursesService>();
                        services.AddScoped<DATMOS.Web.Services.ILessonService, DATMOS.Web.Services.LessonService>();
                        services.AddScoped<DATMOS.Web.Services.Navigation.IMenuService, DATMOS.Web.Services.Navigation.MenuService>();
                        services.AddScoped<DATMOS.Web.Services.Navigation.IFooterService, DATMOS.Web.Services.Navigation.FooterService>();
                        
                        // Register Profile Service
                        services.AddScoped<DATMOS.Web.Interfaces.IProfileService, DATMOS.Web.Areas.Identity.Services.ProfileService>();
                        
                        // Register DummyEmailSender for Identity email sending (development only)
                        services.AddScoped<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, DATMOS.Web.Services.DummyEmailSender>();

                        // Register DbContext with PostgreSQL
                        services.AddDbContext<AppDbContext>(options =>
                            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

                        // Add Identity services
                        services.AddIdentity<AddUsers, IdentityRole>(options =>
                        {
                            // Configure password settings for better UX
                            options.SignIn.RequireConfirmedAccount = false;
                            options.Password.RequireDigit = true;
                            options.Password.RequireLowercase = true;
                            options.Password.RequireUppercase = true;
                            options.Password.RequireNonAlphanumeric = false;
                            options.Password.RequiredLength = 6;
                            options.Password.RequiredUniqueChars = 1;

                            // Configure user settings
                            options.User.RequireUniqueEmail = true;
                            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

                            // Configure lockout settings
                            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                            options.Lockout.MaxFailedAccessAttempts = 5;
                            options.Lockout.AllowedForNewUsers = true;
                        })
                        .AddEntityFrameworkStores<AppDbContext>()
                        .AddDefaultTokenProviders();

                        // Configure application cookie for better UX
                        services.ConfigureApplicationCookie(options =>
                        {
                            options.Cookie.HttpOnly = true;
                            options.ExpireTimeSpan = TimeSpan.FromDays(30);
                            options.LoginPath = "/Identity/Account/Login";
                            options.LogoutPath = "/Identity/Account/Logout";
                            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                            options.SlidingExpiration = true;
                        });
                    })
                    .Configure((context, app) =>
                    {
                        var env = context.HostingEnvironment;

                        if (env.IsDevelopment())
                        {
                            app.UseDeveloperExceptionPage();
                        }
                        else
                        {
                            app.UseExceptionHandler("/Home/Error");
                            app.UseHsts();
                        }

                        app.UseHttpsRedirection();
                        app.UseStaticFiles();
                        app.UseRouting();
                        app.UseAuthentication();
                        app.UseAuthorization();
                        app.UseSession();

                        app.UseEndpoints(endpoints =>
                        {
                            // Special route for /Admin to redirect to /Admin/Home
                            endpoints.MapControllerRoute(
                                name: "adminRoot",
                                pattern: "Admin",
                                defaults: new { area = "Admin", controller = "Home", action = "Index" });

                            // Area route
                            endpoints.MapControllerRoute(
                                name: "areaRoute",
                                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                            // Default route
                            endpoints.MapControllerRoute(
                                name: "default",
                                pattern: "{controller=Home}/{action=Index}/{id?}");
                        });
                    });
                })
                .Build();

            return host;
        }
    }
}
