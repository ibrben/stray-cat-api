using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using StrayCat.Application.Interfaces;
using StrayCat.Application.Services;
using StrayCat.Application.Settings;
using StrayCat.Infrastructure.Data;
using StrayCat.Domain.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configure strongly-typed settings
builder.Services.Configure<FrontendSettings>(builder.Configuration.GetSection("Frontend"));
builder.Services.Configure<GoogleAuthSettings>(builder.Configuration.GetSection("Authentication:Google"));
builder.Services.Configure<R2StorageSettings>(builder.Configuration.GetSection("CloudflareR2"));

// Register settings as services for injection
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<FrontendSettings>>().Value);
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<GoogleAuthSettings>>().Value);

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register DbContext
builder.Services.AddDbContext<StrayCatDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register TripService
builder.Services.AddScoped<ITripService, TripService>();

// Register HighlightService
builder.Services.AddScoped<IHighlightService, HighlightService>();

// Register Booking services
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IReferenceCodeGenerator, ReferenceCodeGenerator>();

// Register AuthService
builder.Services.AddScoped<IAuthService, AuthService>();

// Register UrlService
builder.Services.AddScoped<IUrlService, UrlService>();

// TripImage Service
builder.Services.AddScoped<ITripImageService, TripImageService>();
builder.Services.AddScoped<IStorageService, R2StorageService>();

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add HttpClient for Google authentication
builder.Services.AddHttpClient();

// Register HealthCheckService
builder.Services.AddScoped<IHealthCheckService, HealthCheckService>();

// Add Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowAll");

// Use Authentication
app.UseAuthentication();
app.UseAuthorization();


// Seed data
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<StrayCatDbContext>();
        await DataSeeder.SeedDataAsync(context);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Database seeding failed. Database might not exist yet.");
    }
}

app.MapControllers();

app.Run();

