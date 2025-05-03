using Microsoft.Data.SqlClient;
using System.Data;

namespace TaskMaster
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var services = builder.Services;
            var config = builder.Configuration;

            // Get connection string from config
            var connectionString = config.GetConnectionString("TaskMasterDb");

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("❌ Connection string is missing in appsettings.json.");

            // Register connection string and SqlConnection for DI
            services.AddSingleton(connectionString);
            services.AddScoped<IDbConnection>(_ => new SqlConnection(connectionString));
            services.AddTransient<SqlConnection>(_ => new SqlConnection(connectionString));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };



      

            app.MapGet("/", () => Results.Redirect("/swagger"));


            app.Run();
        }
    }
}
