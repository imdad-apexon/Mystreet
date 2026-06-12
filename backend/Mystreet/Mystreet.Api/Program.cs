using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Mystreet.Application.DTOs.Assistant;
using Mystreet.Application.DTOs.Products;
using Mystreet.Application.Interfaces;
using Mystreet.Application.Services;
using Mystreet.Infrastructure.Auth;
using Mystreet.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddDbContext<AppDbContext>(opt =>
//    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("MystreetDb"));

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<OllamaOptions>(builder.Configuration.GetSection("Ollama"));
builder.Services.Configure<ShoppingAssistantOptions>(builder.Configuration.GetSection("ShoppingAssistant"));
builder.Services.AddHttpClient();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductQueryUnderstandingService, OllamaProductQueryUnderstandingService>();
builder.Services.AddScoped<IShoppingAssistantService, ShoppingAssistantService>();
builder.Services.AddScoped<IOrderService, OrderService>();

var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
var key = Encoding.UTF8.GetBytes(jwt.Key);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowFrontend", p => p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<Mystreet.Api.Middleware.ExceptionMiddleware>();
app.UseCors("AllowFrontend");
app.UseStaticFiles(); // Enable static files
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    await Mystreet.Infrastructure.Seed.DbSeeder.SeedAsync(db);
}

app.Run();