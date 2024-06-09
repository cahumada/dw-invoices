using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace iaas.app.dw.invoices.API
{
    public static class SwaggerStartup
    {
        public static IServiceCollection AddCustomizedSwagger(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            //if (env.IsProduction())
            //{
            //    return services;
            //}

            SwaggerSettings swaggerSettings = GetSettings(services, configuration);
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(delegate (SwaggerGenOptions options)
            {
                OpenApiInfo openApiInfo = GetOpenApiInfo(swaggerSettings);
                options.DocumentFilter<LowerCaseDocumentFilter>(Array.Empty<object>());
                options.SwaggerDoc("v1", openApiInfo);
                //options.CustomSchemaIds((Type type) => Regex.Replace(type.ToString(), "[^a-zA-Z0-9._-]+", ""));
                string text = (string.IsNullOrEmpty(swaggerSettings.DocumentName) ? "DW_Generic_Documentation" : swaggerSettings.DocumentName);
                string path = text + ".xml";
                string text2 = Path.Combine(AppContext.BaseDirectory, path);
                if (File.Exists(text2))
                {
                    options.IncludeXmlComments(text2);
                }

                options.OperationFilter<SwaggerFileOperationFilter>();
                options.EnableAnnotations();

            });

            return services;
        }

        public static IApplicationBuilder UseCustomizedSwagger(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsProduction())
            {
                return app;
            }

            SwaggerSettings swaggerSettings = GetSettings(app);
            app.UseSwagger();
            app.UseSwaggerUI(delegate (SwaggerUIOptions options)
            {
                options.SwaggerEndpoint("v1/swagger.json", swaggerSettings.DocumentName);
                options.InjectStylesheet("custom.css");
                options.RoutePrefix = "swagger";
            });
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(AppContext.BaseDirectory, "swagger-ui")),
                RequestPath = new PathString("/swagger")
            });
            return app;
        }


        private static SwaggerSettings GetSettings(IApplicationBuilder app)
        {
            IOptions<SwaggerSettings> service = app.ApplicationServices.GetService<IOptions<SwaggerSettings>>();
            return service.Value;
        }

        private static SwaggerSettings GetSettings(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<SwaggerSettings>(configuration.GetSection("SwaggerSettings"));
            IOptions<SwaggerSettings> service = serviceCollection.BuildServiceProvider().GetService<IOptions<SwaggerSettings>>();
            return service.Value;
        }

        private static OpenApiInfo GetOpenApiInfo(SwaggerSettings swaggerSettings)
        {
            return new OpenApiInfo
            {
                Title = (string.IsNullOrEmpty(swaggerSettings.ServiceName) ? "DiWork Soluciones SAS" : swaggerSettings.ServiceName),
                Description = (string.IsNullOrEmpty(swaggerSettings.ServiceDescription) ? "DiWork Soluciones SAS" : swaggerSettings.ServiceDescription),
                Version = (string.IsNullOrEmpty(swaggerSettings.ServiceVersion) ? "1.0.0" : swaggerSettings.ServiceVersion),
                Contact = new OpenApiContact
                {
                    Name = "DiWork Soluciones SAS",
                    Email = string.Empty,
                    Url = new Uri("https://www.diworksoluciones.com.ar/")
                }
            };
        }
    }

    public class SwaggerSettings
    {
        public string ServiceName { get; set; }

        public string ServiceDescription { get; set; }

        public string ServiceVersion { get; set; }

        public string DocumentName { get; set; }
    }

    public class LowerCaseDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            Dictionary<string, OpenApiPathItem> dictionary = swaggerDoc.Paths.ToDictionary((KeyValuePair<string, OpenApiPathItem> entry) => LowercaseEverythingButParameters(entry.Key), (KeyValuePair<string, OpenApiPathItem> entry) => entry.Value);
            swaggerDoc.Paths = new OpenApiPaths();
            foreach (var (key, value) in dictionary)
            {
                swaggerDoc.Paths.Add(key, value);
            }
        }

        private static string LowercaseEverythingButParameters(string key)
        {
            return string.Join('/', from x in key.Split('/')
                                    select x.Contains('{') ? x : x.ToLower());
        }
    }

    public class SwaggerFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            var fileUploadMime = "multipart/form-data";
            if (operation.RequestBody == null || !operation.RequestBody.Content.Any(m => m.Key.Equals(fileUploadMime, StringComparison.InvariantCultureIgnoreCase)))
                return;

            var fileParamNames = context.MethodInfo.GetParameters()
                .Where(p => p.ParameterType == typeof(IFormFile))
                .Select(p => p.Name)
                .ToList();

            if (!fileParamNames.Any())
                return;

            operation.RequestBody.Content[fileUploadMime].Schema.Properties =
                fileParamNames.ToDictionary(
                    name => name,
                    name => new OpenApiSchema
                    {
                        Type = "string",
                        Format = "binary"
                    });
        }
    }

}
