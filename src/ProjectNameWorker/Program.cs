using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Microsoft.Extensions.Hosting;
using ProjectNameWorker.Config;
using ProjectNameWorker.Workers;
using Serilog.Exceptions;

namespace ProjectNameWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            CreateHostBuilder(args).Build().Run();
        }
        
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog((ctx, cfg) => 
                    cfg.ReadFrom.Configuration(ctx.Configuration)
                        .Enrich.FromLogContext()
                        .Enrich.WithExceptionDetails())
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder
                        .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                        .AddJsonFile("appsettings.json", optional: true)
                        .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<HostOptions>(options =>
                    {
                        options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
                    });
                    
                    var configuration = hostContext.Configuration;

                    services.Configure<ExampleWorkerConfig>(configuration.GetSection("ExampleWorkerConfig"));

                    services.AddHostedService<ExampleWorker>();
                    services.AddHttpClient();
                    
                });
        }
    }
}