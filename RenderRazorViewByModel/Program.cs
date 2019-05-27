using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.PlatformAbstractions;
using RenderRazorViewByModel.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace RenderRazorViewByModel
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var _renderer = GetRenderer();

            Console.Write(await _renderer.GetViewForModel(new SampleModel
            {
                Name = "John Doe"
            }));

            Console.ReadKey();
        }

        // Normally you could use the DI container to get an instance implementing IRazorViewToStringRenderer
        private static IRazorViewToStringRenderer GetRenderer()
        {
            var services = new ServiceCollection();
            var applicationEnvironment = PlatformServices.Default.Application;
            services.AddSingleton(applicationEnvironment);

            var appDirectory = Directory.GetCurrentDirectory();

            var environment = new HostingEnvironment
            {
                ApplicationName = Assembly.GetEntryAssembly().GetName().Name
            };
            services.AddSingleton<IHostingEnvironment>(environment);

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.FileProviders.Clear();
                options.FileProviders.Add(new PhysicalFileProvider(appDirectory));
            });

            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();

            var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
            services.AddSingleton<DiagnosticSource>(diagnosticSource);

            services.AddLogging();
            services.AddMvc();
            services.AddSingleton<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
            var provider = services.BuildServiceProvider();
            return provider.GetRequiredService<IRazorViewToStringRenderer>();
        }
    }
}
