using BudgetApp.Infrastructure;
using BudgetApp.Application;
using FluentValidation;

namespace BudgetApp.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(
                        "http://localhost:5173",
                        "http://localhost:5174"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // Global middleware f�r ValidationException fr�n FluentValidation
            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (ValidationException ex)
                {
                    context.Response.StatusCode = 400;
                    context.Response.ContentType = "application/json";
                    var errors = ex.Errors.Select(e => e.ErrorMessage).ToList();
                    await context.Response.WriteAsJsonAsync(errors);
                }
            });

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowFrontend");
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}