using Microsoft.EntityFrameworkCore;
using DogHouseAPI.Configurations;
using DogHouseAPI.Services.DogHouseService;
using DogHouseAPI.Models.Database;
using DogHouseAPI.API;
using Microsoft.AspNetCore.RateLimiting;

namespace DogHouseAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<IDogHouseService, DogHouseService>();
            
            builder.Services.AddTransient<IDogHouseRepository, DogHouseRepository>();

            builder.Services.Configure<AppInfoOptions>(builder.Configuration.GetSection(AppInfoOptions.Position));

            builder.Services.AddDbContext<DogHouseDbContext>(o =>             {
                o.UseSqlServer(builder.Configuration.GetConnectionString("DogHouseDBConnection"));
            });

            var raceLimiterConfig = builder.Configuration.GetSection("RateLimiting");
            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.AddFixedWindowLimiter(policyName: raceLimiterConfig["PolicyName"]!, configureOptions =>
                {
                    configureOptions.PermitLimit = int.Parse(raceLimiterConfig["PermitLimit"]!);
                    configureOptions.Window = TimeSpan.Parse(raceLimiterConfig["Window"]!);
                    configureOptions.QueueLimit = int.Parse(raceLimiterConfig["QueueLimit"]!);
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseRateLimiter();

            app.MapControllers();

            app.Run();
        }
    }
}
