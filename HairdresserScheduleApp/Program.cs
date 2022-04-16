using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using HairdresserScheduleApp.BusinessLogic.Configurations;
using HairdresserScheduleApp.BusinessLogic.Models;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using WebApi.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");
var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureLogging((hostingContext, logging) =>
{
    logging.ClearProviders();
    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));

    if (!hostingContext.HostingEnvironment.IsDevelopment()) return;

    logging.AddDebug();
    logging.AddConsole();
}).UseNLog();

// Add services to the container.
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowAll",
    builder =>
    {
      builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});
builder.Services.AddControllers(options =>
{
    options.OutputFormatters.RemoveType<SystemTextJsonOutputFormatter>();
    options.OutputFormatters.Add(new SystemTextJsonOutputFormatter(new JsonSerializerOptions(JsonSerializerDefaults.Web)
    {
        ReferenceHandler = ReferenceHandler.Preserve,
    }));
});

builder.Services.AddApiVersioning(setup =>
{
    setup.DefaultApiVersion = new ApiVersion(1, 0);
    setup.AssumeDefaultVersionWhenUnspecified = true;
    setup.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerGen(c =>
{
    /* var jwtSecurityScheme = new OpenApiSecurityScheme
     {
         Scheme = "bearer",
         BearerFormat = "JWT",
         Name = "JWT Authentication",
         In = ParameterLocation.Header,
         Type = SecuritySchemeType.Http,
         Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

         Reference = new OpenApiReference
         {
             Id = JwtBearerDefaults.AuthenticationScheme,
             Type = ReferenceType.SecurityScheme
         }
     };

     c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

     c.AddSecurityRequirement(new OpenApiSecurityRequirement
     {
         { jwtSecurityScheme, Array.Empty<string>() }
     });
    */
});

builder.Services.AddOptions();

builder.Services.AddScoped<LoggingMiddleware>();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddDbContext<AppDbContext>(options => 
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")),
    ServiceLifetime.Scoped);
builder.Services.AddAutoMapper(typeof(HairdresserScheduleApp.BusinessLogic.Services.Mappers.ScheduleItemProfile));

builder.Services.AddScoped<HairdresserScheduleApp.BusinessLogic.Repositories.IUser, HairdresserScheduleApp.BusinessLogic.Repositories.User>();
builder.Services.AddScoped<HairdresserScheduleApp.BusinessLogic.Services.IUser, HairdresserScheduleApp.BusinessLogic.Services.User>();
builder.Services.AddScoped<HairdresserScheduleApp.BusinessLogic.UnitOfWorks.IUsersUoW, HairdresserScheduleApp.BusinessLogic.UnitOfWorks.UsersUoW>();
builder.Services.AddScoped<HairdresserScheduleApp.BusinessLogic.Services.IJwtService, HairdresserScheduleApp.BusinessLogic.Services.JwtService>();
builder.Services.AddScoped<HairdresserScheduleApp.BusinessLogic.Services.IDailySchedule, HairdresserScheduleApp.BusinessLogic.Services.DailySchedule>();
builder.Services.AddScoped<HairdresserScheduleApp.BusinessLogic.Services.IScheduleItem, HairdresserScheduleApp.BusinessLogic.Services.ScheduleItem>();
builder.Services.AddScoped<HairdresserScheduleApp.BusinessLogic.Repositories.IDailySchedule, HairdresserScheduleApp.BusinessLogic.Repositories.DailySchedule>();
builder.Services.AddScoped<HairdresserScheduleApp.BusinessLogic.Repositories.IScheduleItem, HairdresserScheduleApp.BusinessLogic.Repositories.ScheduleItem>();
builder.Services.AddScoped<HairdresserScheduleApp.BusinessLogic.UnitOfWorks.IDailyScheduleUnitOfWork, HairdresserScheduleApp.BusinessLogic.UnitOfWorks.DailyScheduleUoW>();
builder.Services.AddScoped<HairdresserScheduleApp.BusinessLogic.UnitOfWorks.IScheduleItemUnitOfWork, HairdresserScheduleApp.BusinessLogic.UnitOfWorks.ScheduleItemUoW>();
builder.Services.AddScoped<HairdresserScheduleApp.BusinessLogic.Utilities.IMemoryStreamPool, HairdresserScheduleApp.BusinessLogic.Utilities.MemoryStreamPool>();
builder.Services.AddScoped<HairdresserScheduleApp.BusinessLogic.Models.Logging.LogRequest>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IJsonSerializer, JsonNetSerializer>();
builder.Services.AddScoped<IDateTimeProvider, UtcDateTimeProvider>();
builder.Services.AddScoped<IBase64UrlEncoder, JwtBase64UrlEncoder>();
builder.Services.AddScoped<IJwtAlgorithm, HMACSHA256Algorithm>();
builder.Services.AddScoped<IJwtValidator, JwtValidator>();
builder.Services.AddScoped<IJwtDecoder, JwtDecoder>();
var appSettingsSection = builder.Configuration.GetSection("JwtSettings");
var appSettings = appSettingsSection.Get<JwtSettings>();
var key = Encoding.ASCII.GetBytes(appSettings.Secret);
builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

var app = builder.Build();
// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HairdresserScheduleApp v1"));

app.UseCors(x => x
        .AllowAnyMethod()
        .AllowAnyHeader()
        .SetIsOriginAllowed(origin => true) // allow any origin
);

app.UseRouting(); 
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<LoggingMiddleware>();
app.UseEndpoints(endpoints =>
{

    endpoints.MapControllers();
    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("Hello World!");
    });
});

app.Run();
