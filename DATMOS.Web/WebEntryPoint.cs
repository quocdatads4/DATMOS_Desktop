using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DATMOS.Web
{
    public static class WebEntryPoint
    {
        public static IHost CreateHostBuilder(string[] args, int port)
        {
            var host = Host.CreateDefaultBuilder(args)
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
                        // app.UseAuthorization(); // Temporarily disabled
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
