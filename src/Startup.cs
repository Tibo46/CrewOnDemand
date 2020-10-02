using CrewOnDemand.Database;
using CrewOnDemand.Events;
using CrewOnDemand.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace CrewOnDemand
{
    public class Startup
    {
        private IConfiguration _configuration { get; }
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CrewOnDemandContext>(x => x.UseSqlServer(_configuration.GetConnectionString("SqlServer")));
            services.AddControllers();
            services.AddApplicationInsightsTelemetry();
            services.AddHealthChecks()
                    .AddDbContextCheck<CrewOnDemandContext>("Health", tags: new[] { "health" });
            services.AddHealthChecks()
                .AddCheck("Ready", () =>
                    HealthCheckResult.Healthy("Ready"), tags: new[] { "ready" }
                );

            services.AddScoped<ICrewOnDemandRepository, CrewOnDemandRepository>();
            services.AddScoped<IBookingCreatedPublisher, BookingCreatedPublisher>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Contact = new OpenApiContact
                    {
                        Name = "Thibaud Lacan",
                        Email = "tibo.lacan@gmail.com"
                    },
                    Title = "Crew on Demand",
                    Version = "v1"
                });
            });

            services.AddCors(o => o.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Crew on Demand V1");
            });

            app.UseCors("AllowAll");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/status/health", new HealthCheckOptions()
                {
                    Predicate = (check) => check.Tags.Contains("health")
                });

                endpoints.MapHealthChecks("/status/ready", new HealthCheckOptions()
                {
                    Predicate = (check) => check.Tags.Contains("ready"),
                    ResponseWriter = async (context, healthReport) =>
                    {
                        await context.Response.WriteAsync("Ready");
                    }
                });

                endpoints.MapControllers();
            });
        }
    }
}
