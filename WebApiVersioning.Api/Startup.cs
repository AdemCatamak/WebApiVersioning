using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace WebApiVersioning.Api
{
    public class Startup
    {
        public const string APP_NAME = "WebApiVersioning";

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
                        options.RegisterMiddleware = true;
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
                        options.RegisterMiddleware = true;
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
                        options.RegisterMiddleware = true;
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

            AddSwagger(services);
        }

        private static void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
#pragma warning disable ASP0000
                using ServiceProvider serviceProvider = services.BuildServiceProvider();
#pragma warning restore ASP0000
                using IServiceScope scope = serviceProvider.CreateScope();
                var versionProvider = scope.ServiceProvider.GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (ApiVersionDescription description in versionProvider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, new()
                                                              {
                                                                  Title = $"{APP_NAME} {description.ApiVersion}",
                                                                  Version = description.ApiVersion.ToString(),
                                                              });
                }

                options.IncludeXmlComments($"{AppContext.BaseDirectory}/{APP_NAME}.Api.xml");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "";

                using IServiceScope serviceScope = app.ApplicationServices.CreateScope();
                var apiVersionDescriptionProvider = serviceScope.ServiceProvider.GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (ApiVersionDescription apiVersionDescription in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint($"/swagger/{apiVersionDescription.ApiVersion}/swagger.json", $"{APP_NAME} {apiVersionDescription.ApiVersion}");
                }
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