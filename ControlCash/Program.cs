using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using ControlCash.Domain.Interfaces.Repositories;
using ControlCash.Domain.Interfaces.Services;
using ControlCash.Domain.Interfaces.UnitOfWork;
using ControlCash.Infrastructure.Persistence;
using ControlCash.Application.UseCases.Auth;
using ControlCash.Application.UseCases.Categoria;
using ControlCash.Application.UseCases.Exportacion;
using ControlCash.Application.UseCases.Gasto;
using ControlCash.Infrastructure.Services;
using ControlCash.Application.Validators;

var builder = WebApplication.CreateBuilder(args);

// Leer configuración JWT desde appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings.GetValue<string>("SecretKey") 
    ?? throw new InvalidOperationException("La clave secreta JWT no está definida en appsettings.json");

// CORS: permitir cualquier origen (útil para frontend externo como React Native)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// Configurar Kestrel para escuchar desde cualquier IP (no solo localhost)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5164); // Puerto para acceder desde IP real
});

// Swagger con JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ControlCash API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingrese 'Bearer {token}' en el campo",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
            Array.Empty<string>()
        }
    });
});

// Dependencias (repositorios y servicios)
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Use Cases
builder.Services.AddScoped<RegistrarUsuarioUseCase>();
builder.Services.AddScoped<LoginUsuarioUseCase>();
builder.Services.AddScoped<CrearUsuarioPorAdminUseCase>();
builder.Services.AddScoped<PromoverUsuarioAAdminUseCase>();
builder.Services.AddScoped<ObtenerTodosUsuariosUseCase>();
builder.Services.AddScoped<EliminarUsuarioUseCase>();
builder.Services.AddScoped<CambiarPasswordUseCase>();
builder.Services.AddScoped<CrearGastoUseCase>();
builder.Services.AddScoped<ObtenerGastosUseCase>();
builder.Services.AddScoped<EliminarGastoUseCase>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<CrearCategoriaUseCase>();
builder.Services.AddScoped<EliminarCategoriaUseCase>();
builder.Services.AddScoped<ActualizarCategoriaUseCase>();
builder.Services.AddScoped<ObtenerCategoriasUseCase>();
builder.Services.AddScoped<GenerarReportePdfUseCase>();
builder.Services.AddScoped<IPdfExportService, PdfExportService>();

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new { error = "Token inválido o ausente." });
            return context.Response.WriteAsync(result);
        },
        OnForbidden = context =>
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new { error = "Acceso denegado. Solo administradores pueden acceder." });
            return context.Response.WriteAsync(result);
        }
    };
});

builder.Services.AddAuthorization();

// DbContext PostgreSQL
builder.Services.AddDbContext<ControlCashDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// FluentValidation (puedes registrar aquí todos los validadores)
builder.Services.AddControllers()
    .AddFluentValidation(config =>
    {
        config.RegisterValidatorsFromAssemblyContaining<RegisterRequestValidator>();
    });

var app = builder.Build();

// Manejo de errores
app.UseExceptionHandler("/Home/Error");

// Swagger habilitado
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ControlCash API V1");
    c.RoutePrefix = string.Empty; // Swagger en root /
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
