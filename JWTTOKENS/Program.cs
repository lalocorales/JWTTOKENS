using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

// Leer datos del appsettings.json
var jwtKey = config["JwtSettings:Key"];
var issuer = config["JwtSettings:Issuer"];
var audience = config["JwtSettings:Audience"];

builder.Services.AddControllers();

// Configurar JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
        };
    });

// Swagger con soporte Bearer PERO SIN APLICARLO AUTOMÁTICO
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JWTTOKENS API", Version = "v1" });

    // Solo lo describe, no lo fuerza
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "AGREGA el token manualmente en cada petición.\nFormato: Bearer <tu_token>",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    // ❌ NO PONEMOS AddSecurityRequirement
    // Esto permite que el alumno tenga que enviarlo manualmente en el header
});

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
