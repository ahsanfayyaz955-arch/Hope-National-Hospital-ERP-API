using Hope_National_Hospoital.Infrastructure;
using Hope_National_Hospital.Infrastructure.Identity;
using Hope_National_Hospital.Infrastructure.SeedRoles;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Hope_National_Hospital.Middleware;
using Hope_National_Hospital.Application.Constants;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Serilog;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.File("Logs/hospital-.log",
    rollingInterval: RollingInterval.Day,
    retainedFileCountLimit: 30).CreateLogger();

builder.Host.UseSerilog();

// Controllers
builder.Services.AddControllers();

builder.Services.AddMemoryCache();

builder.Services.AddHealthChecks()
    .AddSqlServer(
        builder.Configuration.GetConnectionString("constr")!,
        name: "sql-server",
        tags: new[] { "database" });


// CORS 
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactFrontend", policy =>
    {
        policy.WithOrigins("Front-End-Url")
        .AllowAnyHeader()
        .AllowAnyMethod();

    });
});

// Rate Limiting 
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("AuthPolicy", HttpContext =>
    RateLimitPartition.GetFixedWindowLimiter(partitionKey: HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 5,
        Window = TimeSpan.FromMinutes(1),
        QueueLimit = 0,
        AutoReplenishment = true
    }));

    options.AddPolicy("ApiPolicy", HttpContent => RateLimitPartition.GetFixedWindowLimiter(partitionKey
        : HttpContent.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 100,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
            AutoReplenishment = true
        }));
    
});

// Versioning Api 
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);

    options.AssumeDefaultVersionWhenUnspecified = true;

    options.ReportApiVersions = true;
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "jwt",
        In = ParameterLocation.Header,
        Description = "Enter Youre Jwt Token"
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

// Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
});

// JWT Authentication
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"]!)),

            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SuperAdminOnly", policy => policy.RequireRole(Roles.SuperAdmin));

    options.AddPolicy("AdminOrSuperAdmin", policy => policy.RequireRole(Roles.SuperAdmin, Roles.Admin));

    options.AddPolicy("DoctorOrAdmin", policy => policy.RequireRole(Roles.SuperAdmin, Roles.Admin, Roles.Doctor));

    options.AddPolicy("ReceptionistOrAdmin", policy => policy.RequireRole(Roles.SuperAdmin, Roles.Admin, Roles.Receptionist));

    options.AddPolicy("AllMedicalStaff", policy =>  policy.RequireRole(Roles.SuperAdmin, Roles.Admin, Roles.Doctor, Roles.Receptionist));
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddlewere>();
app.UseMiddleware<RequestLoggingMiddleware>();

// Seed Roles & Admin
using (var scope = app.Services.CreateScope())
{
    var roleManager =
        scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

    var userManager =
        scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

    await RoleSeeder.SeedRolesAsync(roleManager);
    await AdminSeeder.SeedAsync(userManager);
}



// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("ReactFrontend");

app.UseAuthentication();

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
      Predicate = _ => true,
      ResponseWriter = async (context, report) =>
      {
          context.Response.ContentType = "application/json";

          var statusCode = report.Status switch
          {
              HealthStatus.Healthy => StatusCodes.Status200OK,
              HealthStatus.Degraded => StatusCodes.Status200OK,
              _ => StatusCodes.Status503ServiceUnavailable
          };

          context.Response.StatusCode = statusCode;

          var response = new
          {
              Status = report.Status.ToString()
          };

          await context.Response.WriteAsJsonAsync(response);
      }
});

app.Run();
