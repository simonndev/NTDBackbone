using Microsoft.EntityFrameworkCore;
using NtdEntities.Migrations.SqlServer;

namespace NtdApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(Program).Assembly));
            //builder.Services.AddMediatR(typeof(Application.AssemblyReference).Assembly);

            builder.Services.AddControllers();
            
            builder.Services.AddDbContext<NtdDbContext>(options =>
            {
                options.UseSqlServer("name=ConnectionStrings:Dev");
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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