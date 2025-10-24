using Microsoft.EntityFrameworkCore;
using DogHouseAPI.Configurations;
using DogHouseAPI.Services.DogHouseService;
using DogHouseAPI.Models.Database;

namespace DogHouseAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<IDogHouseService, DogHouseService>();

            builder.Services.Configure<AppInfoOptions>(builder.Configuration.GetSection(AppInfoOptions.Position));

            builder.Services.AddDbContext<DogHouseDbContext>(o =>             {
                o.UseSqlServer(builder.Configuration.GetConnectionString("DogHouseDBConnection"));
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


            app.MapControllers();

            app.Run();
        }
    }
}
