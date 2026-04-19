using ScrumPilot.API.Services;
using ScrumPilot.Data.Context;
using ScrumPilot.Data.Extensions;
using ScrumPilot.Data.Models;
using ScrumPilot.Data.Seeders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ScrumPilot.Data.Repositories;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add Data services (EF Core, Identity, repositories)
builder.Services.AddDataServices(builder.Configuration);

// Add JWT authentication
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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// Require authentication on every endpoint by default
builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build());

// Add repositories to the container.
builder.Services.AddScoped<ISprintRepository, SprintRepository>();
builder.Services.AddScoped<IEpicRepository, EpicRepository>();
builder.Services.AddScoped<IPbiRepository, PbiRepository>();
builder.Services.AddScoped<IPbiHistoryRepository, PbiHistoryRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IDashboardPreferenceRepository, DashboardPreferenceRepository>();

// Add services to the container.
builder.Services.AddScoped<ISprintService, SprintService>();
builder.Services.AddScoped<IEpicService, EpicService>();
builder.Services.AddScoped<IMetricsDashboardService, MetricsDashboardService>();
builder.Services.AddScoped<IDashboardPreferenceService, DashboardPreferenceService>();
builder.Services.AddHttpClient<IPbiService, PbiService>(client =>
{
    client.Timeout = TimeSpan.FromMinutes(5);
});
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

// Add CORS policy for Blazor WebAssembly
var allowedOrigins = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
{
    "http://localhost:5199",
    "http://127.0.0.1:5199",
    "http://localhost:5219",
    "https://localhost:7195",
    "https://localhost:7280",
    "https://127.0.0.1:7280",
    "https://scrumpilot-web.onrender.com"
};

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
        policy.SetIsOriginAllowed(origin =>
            {
                if (allowedOrigins.Contains(origin)) return true;

                // Also allow Tailscale hostnames and local network IPs
                var uri = new Uri(origin);
                return uri.Host.EndsWith(".ts.net")
                    || uri.Host.StartsWith("192.168.")
                    || uri.Host.StartsWith("100.");  // Tailscale CGNAT range
            })
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});

var app = builder.Build();

// Apply schema and seed at startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ScrumPilotContext>();
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

    // Apply migrations for both Postgres (Render) and SQLite (local dev)
    context.Database.Migrate();

    // Ensure Identity roles exist (needed for registration to work)
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = ["Admin", "Developer"];
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
            Console.WriteLine($"[STARTUP] Created role: {role}");
        }
    }

    // Only seed sample data in Development (and when SCRUMPILOT_SKIP_SEED is not set)
    var skipSeed = Environment.GetEnvironmentVariable("SCRUMPILOT_SKIP_SEED");
    if (app.Environment.IsDevelopment() && !string.Equals(skipSeed, "true", StringComparison.OrdinalIgnoreCase))
    {
        DatabaseSeeder.SeedDatabase(context);

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        await DatabaseSeeder.SeedUsersAsync(userManager, roleManager);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS policy
app.UseCors("AllowBlazor");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
