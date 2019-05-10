using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;

namespace Expenses.Api {
    public class Program {
        public static int Main(string[] args) {
            IWebHost webHost = BuildWebHost(args);

            var configuration = (IConfiguration)webHost.Services.GetService(typeof(IConfiguration));

            Log.Logger = CreateLogger(configuration);

            try {
                Log.Information("Expenses.Api service is starting...");
                webHost.Run();
                return 0;

            } catch(Exception e) {
                Log.Fatal(e, "Expenses.Api service terminated unexpectedly");
                return 1;

            } finally {
                Log.CloseAndFlush();
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseLamar()
                .UseSerilog()
                .Build();

        public static Logger CreateLogger(IConfiguration configuration) =>
            new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.ColoredConsole()
                .CreateLogger();
                
    }
}
