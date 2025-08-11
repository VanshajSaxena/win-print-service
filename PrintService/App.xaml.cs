using System.Windows;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PrintService
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ApiService _apiService = new();
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Task.Run(async () =>
            {
                try
                {
                    await _apiService.StartAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _apiService.StopAsync().GetAwaiter().GetResult();
        }
    }

    internal class ApiService
    {
        private IHost? _host;
        private CancellationTokenSource? _cancellationTokenSource;

        public async Task StartAsync(string baseUrl = "http://localhost:5000")
        {
            _cancellationTokenSource = new CancellationTokenSource();

            var builder = WebApplication.CreateBuilder();

            // Configure services
            builder.Services.AddControllers();
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });

            // Configure logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            // Configure the web host
            builder.WebHost.UseUrls(baseUrl);

            var app = builder.Build();

            // Configure pipeline
            app.UseCors();
            app.MapControllers();

            // Add a simple health check endpoint
            app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

            _host = app;
            await _host.StartAsync(_cancellationTokenSource.Token);
        }

        public async Task StopAsync()
        {
            if (_host != null)
            {
                await _host.StopAsync();
                _host.Dispose();
                _host = null;
            }

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }

}
