using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace WebApiVersioning.Api
{
    public class Startup
    {
        private const VersioningStrategies SELECTED_STRATEGY = VersioningStrategies.HttpHeader;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            switch (SELECTED_STRATEGY)
            {
                case VersioningStrategies.HttpHeader:
                    #region Versioning-HttpHeader

                    services.AddApiVersioning(options =>
                    {
                        options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
                        options.DefaultApiVersion = ApiVersion.Default;
                        options.AssumeDefaultVersionWhenUnspecified = true;
                        options.ReportApiVersions = true;
                    });
                    services.AddVersionedApiExplorer(options =>
                    {
                        options.ApiVersionParameterSource = new HeaderApiVersionReader("x-api-version");
                        options.DefaultApiVersion = ApiVersion.Default;
                        options.AssumeDefaultVersionWhenUnspecified = true;
                
                    });
            
                    #endregion
                    break;
                case VersioningStrategies.QueryParams:
                    #region Versioning-QueryParams

                    services.AddApiVersioning(options =>
                    {
                        options.ApiVersionReader = new QueryStringApiVersionReader();
                        options.DefaultApiVersion = ApiVersion.Default;
                        options.AssumeDefaultVersionWhenUnspecified = true;
                        options.ReportApiVersions = true;
                    });
                    services.AddVersionedApiExplorer(options =>
                    {
                        options.ApiVersionParameterSource = new QueryStringApiVersionReader();
                        options.DefaultApiVersion = ApiVersion.Default;
                        options.AssumeDefaultVersionWhenUnspecified = true;
                    });

                    #endregion
                    break;
                case VersioningStrategies.Uri:
                    #region Versioning-Uri
            
                    services.AddApiVersioning(options =>
                    {
                        options.ApiVersionReader = new UrlSegmentApiVersionReader();
                        options.DefaultApiVersion = ApiVersion.Default;
                        options.AssumeDefaultVersionWhenUnspecified = true;
                        options.ReportApiVersions = true;
                    });
                    services.AddVersionedApiExplorer(options =>
                    {
                        options.ApiVersionParameterSource = new UrlSegmentApiVersionReader();
                        options.DefaultApiVersion = ApiVersion.Default;
                        options.AssumeDefaultVersionWhenUnspecified = true;
                        options.SubstituteApiVersionInUrl = true;
                    });
            
                    #endregion
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("1", new OpenApiInfo { Title = "WebApiVersioning.Api v1", Version = "1" });
                c.SwaggerDoc("2", new OpenApiInfo { Title = "WebApiVersioning.Api v2", Version = "2" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/1/swagger.json", "1");
                c.SwaggerEndpoint("/swagger/2/swagger.json", "2");
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }

    public enum VersioningStrategies
    {
        HttpHeader,
        QueryParams,
        Uri
    }
}