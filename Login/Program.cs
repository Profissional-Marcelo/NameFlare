using Login.Common.Extensions;

var builder = WebApplication.CreateBuilder(args);

// APENAS ISSO (as extensões já fazem o resto)
builder
    .AddArchitectures()  
    .AddServices()
    .AddToken()
    .UseSerilog();       

var app = builder.Build();

// Configuração centralizada
app.UseArchitectures(); 

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.Run();