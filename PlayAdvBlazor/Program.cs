using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PlayAdvBlazor.Clients.AdventureServer;

namespace PlayAdvBlazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            
             // Setup the HTTP Client and the Service 
             builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri("http://advsrv.azurewebsites.net") });
             builder.Services.AddSingleton<IAdventureServerClient, AdventureServerClient>();



            await builder.Build().RunAsync();
        }
    }
}
