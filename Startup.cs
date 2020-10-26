using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using waoeml.Providers;

namespace waoeml
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        private IWebHostEnvironment WebHostEnvironment { get; }
        private readonly string CORSPOLICY = "MrMiyagi";

        public Startup(IWebHostEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            WebHostEnvironment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton(Configuration);
            services.AddSingleton(WebHostEnvironment);
            services.AddResponseCompression();

            // Resolve and add current provider
            services.AddPredictionProvider();

            // Allow CORS
            services.AddCors(corsBuilder => corsBuilder.AddPolicy(CORSPOLICY, policyBuilder =>
            {
                policyBuilder
                    .SetIsOriginAllowed((host) => true)
                    .AllowCredentials()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(CORSPOLICY);
            app.UseDefaultFiles();
            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseResponseCompression();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
