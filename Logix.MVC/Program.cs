
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using DevExpress.XtraReports.Web.Extensions;
using DinkToPdf;
using DinkToPdf.Contracts;
using EInvoiceKSADemo.Helpers.Zatca;
using Logix.Application.Common;
using Logix.Application.Extensions;
using Logix.Application.Helpers;
using Logix.Application.Helpers.Acc;
using Logix.Application.Helpers.SignalHelper;
using Logix.Infrastructure.Extensions;
using Logix.MVC.Filters;
using Logix.MVC.HealthEndpoints;
using Logix.MVC.Helpers;
using Logix.MVC.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.OpenApi.Models;
using OfficeOpenXml;

using QuestPDF.Infrastructure;
using System.Text.Json;
using LicenseContext = OfficeOpenXml.LicenseContext;

//using QuestPDF.Drawing;

QuestPDF.Settings.License = LicenseType.Community;
//FontManager.RegisterFont(new FileStream("wwwroot/fonts/NotoNaskhArabic-Bold.ttf", FileMode.Open));
//FontManager.RegisterFont(new FileStream("wwwroot/fonts/NotoNaskhArabic-Regular.ttf", FileMode.Open));

var builder = WebApplication.CreateBuilder(args);

// Excel License Context
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// Add services to the container
builder.Services.AddControllersWithViews(
    options =>
    {
        options.Filters.Add<ModifyDecimalInputsAttribute>();
    })
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

// Add HTTP Client
builder.Services.AddHttpClient();

// ✅ AddSignalR (مرة واحدة فقط)
builder.Services.AddSignalR(o =>
{
    o.EnableDetailedErrors = true;
});

// Add Application and Infrastructure dependencies
builder.Services.AddApplication();
var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
builder.Services.AddPresistence(configuration);

// Add Swagger support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Logix API",
        Version = "v1",
        Description = "API Services.",
        Contact = new OpenApiContact
        {
            Name = "Logix Contact"
        },
    });

    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// Set up Autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.RegisterModule(new Logix.Application.DI.MainModule());
    builder.RegisterModule(new Logix.Infrastructure.DI.MainModule());
});



// Register Helpers
builder.Services.AddScoped<IDDListHelper, DDListHelper>();
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
builder.Services.AddScoped<IApiDDLHelper, ApiDDLHelper>();
builder.Services.AddScoped<IFilesHelper, FilesHelper>();
builder.Services.AddTransient<ICurrentData, CurrentData>();
builder.Services.AddTransient<IMvcSession, MvcSession>();
builder.Services.AddTransient<IPermissionHelper, PermissionHelper>();
builder.Services.AddTransient<ISysConfigurationHelper, SysConfigurationHelper>();
builder.Services.AddTransient<IScreenPropertiesHelper, ScreenPropertiesHelper>();
builder.Services.AddTransient<IGetAccountIDByCodeHelper, GetAccountIDByCodeHelper>();
builder.Services.AddTransient<IGetRefranceByCodeHelper, GetRefranceByCodeHelper>();
builder.Services.AddTransient<IGetAccJournaCodeHelper, GetAccJournaCodeHelper>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<ISendSmsHelper, SendSmsHelper>();
builder.Services.AddTransient<IDevReportHelper, DevReportHelper>();

// Register Application Layer Helpers
builder.Services.AddTransient<ISysConfigurationAppHelper, SysConfigurationAppHelper>();
builder.Services.AddTransient<Logix.MVC.Helpers.IWorkflowHelper, Logix.MVC.Helpers.WorkflowHelper>();
builder.Services.AddTransient<Logix.Application.Helpers.IWorkflowHelper, Logix.Application.Helpers.WorkflowHelper>();
builder.Services.AddTransient<Logix.Application.Helpers.IEmailAppHelper, Logix.Application.Helpers.EmailAppHelper>();
builder.Services.AddTransient<Logix.Application.Helpers.Sal.IZatcaHelper, Logix.Application.Helpers.Sal.ZatcaHelper>();
builder.Services.AddTransient<ILogError, LogError>();

// SignalR is already registered with builder.Services.AddSignalR();


// Logging Configuration
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options => { options.LogToStandardErrorThreshold = LogLevel.Warning; options.IncludeScopes = true; });
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);

// data that reurn from api in CamelCase policy like this 'camelCase'
builder.Services.AddControllersWithViews().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

// DevExpress
builder.Services.AddDevExpressControls();
builder.Services.AddMvc();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularClient", policy =>
    {
        policy.SetIsOriginAllowed(origin => true) // 👈 يسمح بأي Origin
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // 👈 يجب الانتباه مع هذه
    });
});






// Logging Configuration
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options => { options.LogToStandardErrorThreshold = LogLevel.Warning; options.IncludeScopes = true; });
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);

// data that reurn from api in CamelCase policy like this 'camelCase'
builder.Services.AddControllersWithViews().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

// DevExpress
builder.Services.AddDevExpressControls();
builder.Services.AddMvc();

builder.Services.ConfigureReportingServices(c =>
{
    c.ConfigureWebDocumentViewer(cv =>
    {
        cv.UseCachedReportSourceBuilder();
    });
});

builder.Services.AddScoped<ReportStorageWebExtension, CustomReportStorageWebExtension>();

var provider = builder.Services.BuildServiceProvider();
var Configuration = provider.GetRequiredService<IConfiguration>();
builder.Services.AddZatcaServices(Configuration);

// Build the application
var app = builder.Build();

// Configure Localization Service
var localizationService = app.Services.GetService<ILocalizationService>();
localizationService.ConfigureLocalization(app);

// HTTP request pipeline configuration
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}



app.UseCors("AllowAngularClient");

// Static Files and Routing
app.UseStaticFiles();
app.UseRouting();


// CORS Policy
app.UseCors(options =>
{
    options.AllowAnyOrigin();
    options.AllowAnyMethod();
    options.AllowAnyHeader();
});

// Session Handling
app.UseSession();

// Swagger UI (Only accessible in development)
app.UseSwagger();
app.UseSwaggerUI();

// Custom Session Middleware (if you have custom session handling logic)
app.UseMiddleware<SessionMiddleware>();

// Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// DevExpress
app.UseDevExpressControls();


// MVC Endpoints


app.UseRequestLocalization();

// Area Routing (if using areas in MVC)
app.MapControllerRoute(
    name: "MyArea",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
// MVC Endpoints
app.MapHub<NotifyHub>("/notify", options =>
{
    options.Transports = HttpTransportType.WebSockets |
                         HttpTransportType.ServerSentEvents |
                         HttpTransportType.LongPolling;
});
// Map API Controllers if needed
app.MapControllers();

app.MapHealthEndpoints();

// Run the application
app.Run();