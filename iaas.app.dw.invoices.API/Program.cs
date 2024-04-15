using iaas.app.dw.invoices.API;
using iaas.app.dw.invoices.Application.DTOs;
using iaas.app.dw.invoices.Application.Support;
using iaas.app.dw.invoices.Infrastructure.Support;
using Serilog;
using System.Globalization;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

#region Logs

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Warning()
    .CreateBootstrapLogger();

Log.Information("Starting up");

builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console()
        .MinimumLevel.Warning()
        .ReadFrom.Configuration(ctx.Configuration));

#endregion

builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
builder.Services.AddMemoryCache();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);

var xmlFiles = new[]
{
    // Comentarios de la API
    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml",
    // Comentarios de los DTOs
    $"{Assembly.GetAssembly(typeof(ApiErrorMessageDto)).GetName().Name}.xml",
};

builder.Services.AddSwaggerGen(c =>
{
    c.UseAllOfToExtendReferenceSchemas();
    foreach (var xml in xmlFiles)
    {
        var xmlCommentFile = xml;
        var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);
        if (File.Exists(xmlCommentsFullPath))
            c.IncludeXmlComments(xmlCommentsFullPath, includeControllerXmlComments: true);
    }
});

builder.Services.AddCustomizedSwagger(builder.Configuration, builder.Environment);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}

app.UseRouting()
   .UseEndpoints(endpoints =>
   {
       endpoints.MapDefaultControllerRoute();
       endpoints.MapHealthChecks("/health");
   });

app.UseRequestLocalization(x =>
{
    var defaultCulture = builder.Configuration.GetSection("DefaultCulture").Get<string>() ?? "en-US";

    var ci = new CultureInfo(defaultCulture);

    CultureInfo.DefaultThreadCurrentCulture = ci;
    CultureInfo.DefaultThreadCurrentUICulture = ci;

    x.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(ci);
    x.SupportedCultures = new List<CultureInfo> { ci };
    x.SupportedUICultures = new List<CultureInfo> { ci };
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCustomizedSwagger(app.Environment);

app.MapControllers();

app.UseStaticFiles();

app.Run();
