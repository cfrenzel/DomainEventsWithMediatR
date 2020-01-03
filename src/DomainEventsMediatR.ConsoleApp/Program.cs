using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using MediatR;
using DomainEventsMediatR.Domain;
using DomainEventsMediatR.Application;
using Microsoft.EntityFrameworkCore.Design;
using DomainEventsMediatR.Persistence;

namespace DomainEventsMediatR.ConsoleApp
{
    public class Program : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        private static IConfigurationRoot _config;
        private static ILogger<Program> _log;
        private static IServiceProvider _serviceProvider;

        public static async Task Main(string[] args)
        {
            _init();
          
            var db = _serviceProvider.GetRequiredService<ApplicationDbContext>();

            Sprint s = new Sprint(DateTime.UtcNow.Date, DateTime.UtcNow.AddDays(7).Date);
            db.Add(s);
           
            BacklogItem b = new BacklogItem("Comments feature");
            db.Add(b);
            
            _log.LogDebug($"Calling CommitTo on BacklogItem: {b.Id} to Sprint: {s.Id}");
            
            b.CommitTo(s);
            
            _log.LogDebug("Calling SaveChanges");
           
            await db.SaveChangesAsync();
            
            _log.LogDebug("SaveChanges Completed");

            //Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }

        private static void _init()
        {
            _config = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: true)
           .AddUserSecrets<Program>()
           .AddEnvironmentVariables()
           .Build();

            var services = new ServiceCollection();

            services.AddSingleton(_config);
            
            services.AddLogging(builder => {
                builder.AddConfiguration(_config.GetSection("Logging"));
                builder.AddDebug();
                builder.AddConsole();
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                 options.UseSqlServer(
                    _config.GetConnectionString("ApplicationConnection")
            ));

            //configure mediatr to look for handlers in Application Layer
            services.AddMediatR(typeof(MediatrDomainEventDispatcher).GetTypeInfo().Assembly);
            services.AddTransient<IDomainEventDispatcher, MediatrDomainEventDispatcher>();

            _serviceProvider = services.BuildServiceProvider();
            _log = _serviceProvider.GetService<ILogger<Program>>();
        }


        public ApplicationDbContext CreateDbContext(string[] args)
        {
            _init();
            var db = _serviceProvider.GetRequiredService<ApplicationDbContext>();
            return db;
        }


    }

  
}
