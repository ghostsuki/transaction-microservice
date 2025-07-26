// TransactionManagementService.API/Program.cs
using TransactionManagementService.Application.Services;
using TransactionManagementService.Domain.Interfaces;
using TransactionManagementService.Infrastructure.Configuration;
using TransactionManagementService.Infrastructure.Repositories;
using TransactionManagementService.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuração de settings
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("DatabaseSettings"));

builder.Services.Configure<ServiceBusSettings>(
    builder.Configuration.GetSection("ServiceBusSettings"));

// Registro de dependências
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IMessageService, ServiceBusService>();

// Configuração da API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "Transaction Management Service", 
        Version = "v1",
        Description = "API para gerenciamento de transações financeiras"
    });
});

// CORS (se necessário)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Transaction Management Service V1");
        c.RoutePrefix = string.Empty; // Swagger na raiz
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();