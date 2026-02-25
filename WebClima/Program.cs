using Application.Interfaces;
using Application.Services;
using Application.UseCases;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Data.Context;
using Infrastructure.Data.Repositories;
using Infrastructure.ExternalServices;
using Infrastructure.ExternalServices.Policies;
using Infrastructure.HealthChecks;
using Infrastructure.Options;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using System.Reflection;
using WebClima.Middleware;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

try
{

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        // Configurar segurança JWT no Swagger
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Insira o token JWT com o prefixo 'Bearer' (ex: Bearer seu_token_aqui)",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                new string[] { }
            }
        });

        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "WebClima API",
            Description = "API para consulta de informações climáticas por coordenadas geográficas ou nome de cidade, com histórico de registros.",
        });


        // Incluir comentários XML
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }

        // Ordenar actions por ordem alfabética
        options.OrderActionsBy(apiDesc => apiDesc.RelativePath);

    });

    // Configuração de CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                   .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });


    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection")));

    //Domain
    builder.Services.AddScoped<ICidadeRepository, CidadeRepository>();
    builder.Services.AddScoped<IClimaRepository, ClimaRepository>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IRegistroClimaService, RegistroClimaService>();
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


    //Application
    builder.Services.AddScoped<ObterClimaPorCoordenadasUseCase>();
    builder.Services.AddScoped<ObterClimaPorCidadeUseCase>();
    builder.Services.AddScoped<ObterHistoricoClimaUseCase>();
    builder.Services.AddScoped<ValidarCredenciaisUseCase>();
    builder.Services.AddScoped<CriarUsuarioUseCase>();
    builder.Services.AddScoped<IPasswordHashService, PasswordHashService>();


    // Infrastructure
    builder.Services.AddHealthChecks()
        .AddNpgSql(
            builder.Configuration.GetConnectionString("DefaultConnection")!,
            name: "database",
            tags: new[] { "db", "postgresql" })
        .AddCheck<OpenWeatherApiHealthCheck>(
            name: "openweather-api",
            tags: new[] { "external", "api" })
        .AddCheck(
            name: "self",
            () => HealthCheckResult.Healthy("Aplicação está respondendo"),
            tags: new[] { "self" });


    builder.Services.AddHttpClient<IClimaService, WeatherService>()
        .AddPolicyHandler(WeatherServicePolicies.GetRetryPolicy())
        .AddPolicyHandler(WeatherServicePolicies.GetCircuitBreakerPolicy());

    builder.Services.AddOptions<OpenWeatherOptions>()
    .Bind(builder.Configuration.GetSection("OpenWeather"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

    string jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Configuração 'Jwt:Key' não encontrada");
    string? jwtIssuer = builder.Configuration["Jwt:Issuer"];
    string? jwtAudience = builder.Configuration["Jwt:Audience"];

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });


    var app = builder.Build();

    app.UseCors("AllowFrontend");

    // Não esqueça de ativar os middlewares na ordem correta
    app.UseAuthentication();
    app.UseAuthorization();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "WebClima API v1");
            options.DocumentTitle = "WebClima API - Documentação";
            options.DisplayRequestDuration();
        });
    }

    app.UseHttpsRedirection();

    app.UseMiddleware<GlobalExceptionMiddleware>();

    app.UseAuthorization();

    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";

            var result = JsonSerializer.Serialize(new
            {
                status = report.Status.ToString(),
                timestamp = DateTime.UtcNow,
                duration = report.TotalDuration,
                checks = report.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    description = entry.Value.Description,
                    duration = entry.Value.Duration,
                    tags = entry.Value.Tags
                })
            }, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(result);
        }
    });

    app.MapControllers();

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();


        // Só executa Migrate() se não for InMemory
        if (db.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
        {
            db.Database.Migrate();
        }
        else
        {
            db.Database.EnsureCreated();
        }
    }



    app.Run();
    

}
catch (Exception ex)
{
    Console.WriteLine($"ERRO CRÍTICO NA INICIALIZAÇÃO: {ex.Message}");
    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
    Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
    throw;
}

public partial class Program { }