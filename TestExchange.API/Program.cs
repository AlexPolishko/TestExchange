using Microsoft.Extensions.Configuration;
using Swashbuckle.Swagger;
using System.Reflection;
using TestExchange.Application;

namespace TestExchange.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var config = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: false)
                        .Build();
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            builder.Services.AddSingleton<IWalletService, WalletService>();
            builder.Services.AddSingleton<ICryptoExchangeStore, CryptoExchangeStore>();
            builder.Services.AddSingleton<IOrderBookReader, OrderBookReader>();
            builder.Services.AddScoped<IResolver, Resolver>();
            builder.Services.Configure<AppSettings>(config.GetSection("AppSettings"));


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}