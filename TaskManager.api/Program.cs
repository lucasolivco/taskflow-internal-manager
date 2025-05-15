using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// 1) Lê a connection string do appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2) Registra o DbContext para injeção de dependência
builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseSqlServer(connectionString));

// (Opcional) Se quiser usar controllers em vez de minimal APIs, descomente:
// builder.Services.AddControllers();

// --- Serviços ---
// explora endpoints para o Swagger
builder.Services.AddEndpointsApiExplorer();
// adiciona o gerador Swagger
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TaskDbContext>(opt =>
  opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adicionar serviços de controller (MVC)
builder.Services.AddControllers();

var app = builder.Build();

// --- Pipeline ---

// em produção redireciona HTTP→HTTPS
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// habilita Swagger e Swagger UI **APENAS em Development**
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();               // expõe /swagger/v1/swagger.json
    app.UseSwaggerUI(c =>
    {
        // aqui você pode personalizar a rota, 
        // mas deixaremos o default: /swagger/index.html
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskManager API v1");
        c.RoutePrefix = "swagger";   // URL final: /swagger
    });
}

// rota raiz simples
app.MapGet("/", () => "API TaskManager rodando! Acesse /weatherforecast ou /swagger");

// endpoint mínimo de exemplo
app.MapGet("/weatherforecast", () =>
{
    var summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool",
        "Mild", "Warm", "Balmy", "Hot",
        "Sweltering", "Scorching"
    };

    return Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
})
.WithName("GetWeatherForecast");

app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
