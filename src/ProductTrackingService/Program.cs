using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using ProductTrackingService.Data;
using ProductTrackingService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add DB and services
builder.Services.AddDbContext<ProductTrackingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ProductTrackingDataService>();
builder.Services.AddControllers();


// JWT Auth setup
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddHttpClient("LogisticsClient");


// Swagger + Auth modal
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "ProductTrackingService API", Version = "v1" });
    opt.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "JWT Auth header using Bearer scheme. Example: 'Bearer {token}'",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    opt.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddHealthChecks(); // Add health check services

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(); // <- Add this to enable CORS

app.UseAuthentication(); // <- Add this
app.UseAuthorization();
app.MapControllers();
// Map the /health endpoint
app.MapHealthChecks("/health");
app.Run();
