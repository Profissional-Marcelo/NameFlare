using Login.Common.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder
    .AddArchitectures()
    .AddServices()
    .AddToken();
//.AddSerilog();

builder.Services.AddEndpointsApiExplorer();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    // Configurar Swagger para abrir automaticamente
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
        options.RoutePrefix = "swagger"; // Acesse em /swagger
        options.DocumentTitle = "Minha API";
    });

    // Para abrir automaticamente no navegador
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// 7. Mapear OpenAPI
app.MapOpenApi(); // Gera o JSON em /openapi/v1.json

app.Run();