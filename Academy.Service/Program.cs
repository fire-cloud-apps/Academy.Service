using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi.Models;
using Serilog.Events;
using Serilog.Sinks.Logz.Io;
using Serilog;
using System.Reflection;
using Academy.Service;
using Academy.Service.Utility;
using Academy.Service.Utility.Authorization;
using AutoWrapper;
using Serilog.Exceptions;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

#region Configuration Section
//AppSettings
var appSettingsSection = builder.Configuration.GetSection("AppSettings");
var appSettings = appSettingsSection.Get<AppSettings>();
//API Section
var apiSection = builder.Configuration.GetSection("API");
var apiSettings = apiSection.Get<API>();
services.Configure<AppSettings>(appSettingsSection);

#endregion

#region DI InMemory Cache

//Currently not in use.
//Ref: https://learn.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-7.0
//builder.Services.AddSingleton<InternalMemoryCache>();

#endregion

#region Logging Configuration

builder.Host.UseSerilog();
builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});

#endregion

#region Enable Serilog For MongoDB

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .WriteTo.Console()
    //listener.logz.io
    //63e2c4c1-56f5-41c6-ad89-a881734f4e8b
    .WriteTo.LogzIoDurableHttp(
                    "https://listener.logz.io:8071/?type=Academy.Service&token=KzWjBRDgSAQhuRGBYvElboHFcoJMmLAJ",
                    logzioTextFormatterOptions: new LogzioTextFormatterOptions
                    {
                        BoostProperties = true,
                        LowercaseLevel = true,
                        IncludeMessageTemplate = true,
                        FieldNaming = LogzIoTextFormatterFieldNaming.CamelCase,
                        EventSizeLimitBytes = 261120,
                    })
                .MinimumLevel.Information()

    //.WriteTo.MongoDBBson(appSettings.LogMongo,
    //    collectionName: "fc.clients.log",
    //    cappedMaxSizeMb: 10,
    //    cappedMaxDocuments: 1500
    //)//Enable Mongodb Logging
    .CreateLogger();
Log.Information("Log Enabled Serilog Logging with Console, Logz - No Mongodb Logs");

#endregion

#region Google Authentication
// This configures Google.Apis.Auth.AspNetCore3 for use in this app.
// Ref: https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins?view=aspnetcore-6.0
services
    .AddAuthentication(o =>
    {
        // This forces challenge results to be handled by Google OpenID Handler, so there's no
        // need to add an AccountController that emits challenges for Login.
        o.DefaultChallengeScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
        // This forces forbid results to be handled by Google OpenID Handler, which checks if
        // extra scopes are required and does automatic incremental auth.
        o.DefaultForbidScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
        // Default scheme that will handle everything else.
        // Once a user is authenticated, the OAuth2 token info is stored in cookies.
        o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddGoogleOpenIdConnect(options =>
    {
        options.ClientId = "849202717235-pmv36imri972lkgh02ei0i5vmck9e5ib.apps.googleusercontent.com";
        options.ClientSecret = "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";

        //options.ClientId = appSettings.GoogleOAuth.ClientId;
        //options.ClientSecret = appSettings.GoogleOAuth.ClientSecret;
    });

#endregion

#region Application Scope & JWT Middleware
services.AddHttpContextAccessor();//TO Access HTTPContext from constructor.
//services.AddScoped<ICacheManager, CacheManager>();
//services.AddTransient(typeof(IConnectionService), typeof(ConnectionService));
services.AddScoped<IJwtUtils, JwtUtils>();
#endregion

#region Controller, Swagger Enable & Cors
// Add services to the container.
services.AddControllers();
//Enable Cors
services.AddCors();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();

//builder.Services.AddSwaggerGen();
var apiVersion = apiSettings.Version;
//Ref :https://www.c-sharpcorner.com/article/swagger-in-dotnet-core/
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(apiVersion, new OpenApiInfo
    {
        Version = apiVersion,
        Title = apiSettings.Name,
        Description = apiSettings.Description ,
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = apiSettings.Company,
            Email = apiSettings.Email,
            Url = new Uri("https://example.com"),
        },
        License = new OpenApiLicense
        {
            Name = "Use under OpenApiLicense",
            Url = new Uri("https://example.com/license"),
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
"JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    }
    );
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,

                    },
                    new List<string>()
                }
            });
    // Set the comments path for the Swagger JSON and UI.    
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

#endregion

#region Logging Filter DI

//builder.Services.AddScoped<LoggingFilter>();

#endregion

var app = builder.Build();

#region Swagger Configuration

// Configure the HTTP request pipeline.
var swaggerName = $"{apiSettings.Name}-{apiSettings.Version}";
Console.WriteLine($"Swagger Name {swaggerName}");
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint($"/swagger/{apiVersion}/swagger.json", swaggerName);
        c.DocExpansion(DocExpansion.None);
    });
}
#endregion

#region Autowrapper Middleware
//Ref:https://github.com/proudmonkey/AutoWrapper
app.UseApiResponseAndExceptionWrapper<MapResponseObject>();

#endregion

#region Enable CORS
//Ref: https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-6.0
app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin
    .AllowCredentials()); // allow credentials
#endregion

#region Global Error Handler
// global error handler
app.UseMiddleware<ErrorHandlerMiddleware>();
#endregion

#region Enable Static Files
app.UseHttpsRedirection();
app.UseStaticFiles();
#endregion

//Makes the image to upload and store temporarily.
Directory.CreateDirectory(@"\Resources\Images");

#region Authenticate & Authorize
app.UseMiddleware<JwtMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
#endregion

#region Run App
app.MapControllers();

app.Run();
#endregion

