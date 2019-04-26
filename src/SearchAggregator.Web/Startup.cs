using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

using Swashbuckle.AspNetCore.Swagger;

using SearchAggregator.Core.Interfaces;
using SearchAggregator.Infrastructure.Data;
using SearchAggregator.Infrastructure.Processing;
using SearchAggregator.Google;
using SearchAggregator.Bing;

namespace SearchAggregator.Web
{
    public class Startup
    {
        private readonly ILogger _logger;
        
        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddTransient<ISearchEngine, GoogleSearchEngine>();
            _logger.LogInformation("Added GoogleSearchEngine to services");

            services.AddTransient<ISearchEngine, BingSearchEngine>();
            _logger.LogInformation("Added BingSearchEngine to services");
            
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("SearchAggregatorDatabase")));
            _logger.LogInformation("Added AppDbContext with SqlServer to services");

            services.AddScoped<IRepository, EfRepository>();
            _logger.LogInformation("Added EfRepository to services");
            
            services.AddTransient<ISearchProcessor, SearchProcessor>();
            _logger.LogInformation("Added SearchProcessor to services");

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "SearchAggregator API", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

            });
            _logger.LogInformation("Added Swagger to services");

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                _logger.LogInformation("In Development environment");
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            
            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            var cachePeriod = env.IsDevelopment() ? "600" : "604800";
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append(new KeyValuePair<string, StringValues>("Cache-Control", $"public, max-age={cachePeriod}"));
                }
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            
            app.UseMvc();
        }
    }
}
