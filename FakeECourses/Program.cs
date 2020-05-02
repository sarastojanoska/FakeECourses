using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeECourses.Data;
using FakeECourses.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FakeECourses
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // CreateHostBuilder(args).Build().Run();
            var host = CreateWebHostBuilder(args).Build(); 
            using (var scope = host.Services.CreateScope()) 
            { var services = scope.ServiceProvider; 
                try 
                { var context = services.GetRequiredService<FakeECoursesContext>(); 
                    context.Database.Migrate(); 
                    SeedData.Initialize(services); 
                } catch (Exception ex) 
                {
                    var logger = services.GetRequiredService<ILogger<Program>>(); 
                    logger.LogError(ex, "An error occurred seeding the DB."); 
                }
            }
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                 .UseStartup<Startup>();
                
    }
}
