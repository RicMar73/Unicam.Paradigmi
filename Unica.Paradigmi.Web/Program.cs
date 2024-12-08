using Microsoft.EntityFrameworkCore; // Per il DbContext
using Microsoft.IdentityModel.Tokens; // Per la configurazione JWT
using System.Text; // Per codifica delle chiavi
using Unicam.Paradigmi.Models.Context; // Per il DbContext specifico
using Unicam.Paradigmi.Application.Middlewares; // Per i middleware personalizzati

var builder = WebApplication.CreateBuilder(args);


// **Logging**
// Configura il servizio di logging per catturare e registrare gli eventi dell'applicazione
builder.Services.AddLogging();

// **DbContext**
// Registra il contesto del database (Entity Framework Core con SQL Server)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// **Authentication (JWT)**
// Leggi i valori di configurazione dal file appsettings.json
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = jwtSettings["Key"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

// Configura l'autenticazione tramite token JWT
builder.Services.AddAuthentication("Bearer") // Specifica lo schema "Bearer"
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // Controlla il campo "issuer" del token
            ValidateAudience = true, // Controlla il campo "audience" del token
            ValidateLifetime = true, // Verifica la scadenza del token
            ValidateIssuerSigningKey = true, // Verifica la firma del token
            ValidIssuer = issuer, // Il valore che deve corrispondere all'issuer del token
            ValidAudience = audience, // Il valore che deve corrispondere all'audience del token
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)) // Chiave per validare il token
        };
    });

// **Authorization**
builder.Services.AddAuthorization(); // Aggiunge il servizio richiesto

// **Controllers**
builder.Services.AddControllers(); // Registra il supporto per i controller


// **Swagger**
// Aggiunge e configura Swagger per la documentazione delle API
builder.Services.AddEndpointsApiExplorer(); // Genera l'elenco degli endpoint
// Configura Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Inserisci il token JWT",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


// **Costruzione dell'applicazione**
var app = builder.Build();

// **Swagger Middleware**
// Aggiunge l'interfaccia Swagger per testare le API
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    options.RoutePrefix = string.Empty; // Imposta Swagger come pagina predefinita
});

// **Middleware Personalizzati**
// Registra i middleware personalizzati nell'ordine corretto

// 1. ErrorHandlingMiddleware: Gestisce le eccezioni globali e fornisce una risposta uniforme
app.UseMiddleware<ErrorHandlingMiddleware>();

// 2. RequestLoggingMiddleware: Logga tutte le richieste HTTP
app.UseMiddleware<RequestLoggingMiddleware>();

// **Autenticazione e Autorizzazione**
// Aggiunge i middleware per gestire l'autenticazione e l'autorizzazione tramite JWT
app.UseAuthentication(); // Verifica i token JWT nelle richieste
app.UseAuthorization(); // Controlla i permessi dell'utente autenticato

// **Mapping dei Controller**
// Registra i controller e associa gli endpoint API
app.MapControllers();

// **Esecuzione dell'applicazione**
app.Run();
