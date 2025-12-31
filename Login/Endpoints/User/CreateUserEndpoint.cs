// Endpoints/User/CreateUserEndpoint.cs (versão simplificada)
using Login.Data.Context;
using Login.Models.Entities;
using Login.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Login.Endpoints.User;

public class CreateUserEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/users", Handler)
            .WithName("CreateUser")
            .WithTags("Users")
            .WithSummary("Cria um novo usuário")
            .WithDescription("Endpoint para criação de usuários no sistema")
            .Produces<NFUser>(201)
            .ProducesValidationProblem(400)
            .AllowAnonymous();

    private static async Task<IResult> Handler(
        AppDbContext context,
        CreateUserViewModel model,
        ILogger<CreateUserEndpoint> logger)
    {
        logger.LogInformation("Criando usuário: {Email}", model.Email);

        // Validação do ViewModel
        if (!model.IsValid)
        {
            return Results.ValidationProblem(model.Notifications
                .GroupBy(n => n.Key)
                .ToDictionary(g => g.Key, g => g.Select(n => n.Message).ToArray()));
        }

        // Mapeia para entidade
        var usuario = model.MapTo();

        try
        {
            // Verifica se email já existe
            var emailExistente = await context.Usuarios
                .AnyAsync(u => u.email == model.Email);

            if (emailExistente)
            {
                return Results.ValidationProblem(
                    new Dictionary<string, string[]>
                    {
                        ["Email"] = new[] { "Email já está cadastrado" }
                    });
            }

            // Adiciona e salva
            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();

            logger.LogInformation("Usuário criado com ID: {Id}", usuario.id);

            return Results.Created($"/users/{usuario.id}", usuario);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao salvar usuário");
            return Results.Problem(
                title: "Erro interno",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}