﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Net.Core.Template.CrossCutting.Ioc;
using System.IO;

namespace Net.Core.Template.Worker
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = ReturnEnvironment(config);
                    config
                         .SetBasePath(Directory.GetCurrentDirectory())
                         .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                         .AddJsonFile($"appsettings.{(string.IsNullOrEmpty(env) ? "Development" : env)}.json", optional: true, reloadOnChange: true)
                         .AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.ConfigureContainer(hostContext.Configuration);
                });

        private static string ReturnEnvironment(IConfigurationBuilder configBuilder)
        => configBuilder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build()
            .GetSection("Environment")?.Value;
    }
}