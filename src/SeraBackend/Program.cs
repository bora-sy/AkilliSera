
using Microsoft.Extensions.Configuration;
using SeraBackend.Configurations;
using SeraBackend.Greenhouse;

namespace SeraBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<GreenhouseConfiguration>(builder.Configuration.GetSection("GreenhouseConfig"));
            builder.Services.AddSingleton<GreenhouseService>();

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
