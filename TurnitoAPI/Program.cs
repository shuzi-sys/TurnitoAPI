using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using System.Text;
using TurnitoAPI.Data;
using TurnitoAPI.Services;
using TurnitoAPI.Services.Interfaces;
using TurnitoAPI.Exceptions;
using Scalar.AspNetCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddRateLimiter(options => 
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
    {
        var key = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? ctx.Connection.RemoteIpAddress?.ToString() ?? "anon";
        return RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 100,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        });
    });

    options.AddPolicy("auth", ctx =>
        RateLimitPartition.GetTokenBucketLimiter(
            ctx.Connection.RemoteIpAddress?.ToString() ?? "anon",
            _ => new TokenBucketRateLimiterOptions
            {
                TokenLimit = 5,                                   
                TokensPerPeriod = 1,                             
                ReplenishmentPeriod = TimeSpan.FromSeconds(12),   
                QueueLimit = 0
            }));

    options.AddPolicy("write", ctx =>
       RateLimitPartition.GetSlidingWindowLimiter(
           ctx.User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? ctx.Connection.RemoteIpAddress?.ToString() ?? "anon",
           _ => new SlidingWindowRateLimiterOptions
           {
               PermitLimit = 30,
               Window = TimeSpan.FromMinutes(1),
               SegmentsPerWindow = 6,
               QueueLimit = 0
           }));

    options.AddPolicy("read", ctx =>
    RateLimitPartition.GetTokenBucketLimiter(
        ctx.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? ctx.Connection.RemoteIpAddress?.ToString() ?? "anon",
        _ => new TokenBucketRateLimiterOptions
        {
            TokenLimit = 10,                                
            TokensPerPeriod = 1,
            ReplenishmentPeriod = TimeSpan.FromSeconds(3),   
            AutoReplenishment = true,
            QueueLimit = 0
        }));

    options.OnRejected = async (context, ct) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
            context.HttpContext.Response.Headers.RetryAfter =
                ((int)retryAfter.TotalSeconds).ToString();

        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            status = 429,
            title = "Too many requests",
            detail = "Demasiadas solicitudes. Probá de nuevo en unos segundos."
        }, ct);
    };


});
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
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
builder.Services.AddScoped<IauthService, authService>();
builder.Services.AddScoped<ItokenService, tokenService>();
builder.Services.AddScoped<IcompanyService, companyService>();
builder.Services.AddScoped<IappointmentService, appointmentService>();
builder.Services.AddScoped<IserviceService, serviceService>();
builder.Services.AddScoped<IproviderService, providerService>();
builder.Services.AddScoped<IadminService, adminService>();
builder.Services.AddScoped<ItokenService, tokenService>();
builder.Services.AddExceptionHandler<APIExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

app.MapControllers();

app.Run();
