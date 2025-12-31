// Models/ViewModels/CreateUserViewModel.cs
using System.ComponentModel.DataAnnotations;
using Flunt.Notifications;
using Flunt.Validations;
using Login.Models.Entities;

namespace Login.Models.ViewModels;

public class CreateUserViewModel : Notifiable<Notification>
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 100 caracteres")]
    public string? Nome { get; set; }

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Senha é obrigatória")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
    [DataType(DataType.Password)]
    public string? Senha { get; set; } // ← Mude de SenhaHash para Senha

    [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
    [Compare("Senha", ErrorMessage = "As senhas não conferem")]
    [DataType(DataType.Password)]
    public string? ConfirmarSenha { get; set; }

    public bool AceitouTermos { get; set; } = false;

    public NFUser MapTo()
    {
        Validate();

        if (!IsValid)
            return new NFUser(); // Retorna objeto vazio se inválido

        return new NFUser
        {
            nome = Nome!,
            email = Email!,
            senha_hash = Senha!, // Em produção, faça hash aqui
            salt = Guid.NewGuid().ToString("N")[..20],
            tipo_usuario = TipoUsuario.COMUM,
            ativo = true,
            data_criacao = DateTime.UtcNow,
            data_ultima_atualizacao = DateTime.UtcNow,
            linguagem_preferida = "pt-BR",
            fuso_horario = "America/Sao_Paulo",
            aceitou_termos = AceitouTermos,
            total_logins = 0
        };
    }

    private void Validate()
    {
        Clear();

        AddNotifications(new Contract<Notification>()
            .Requires()
            .IsNotNullOrEmpty(Nome, "Nome", "Nome é obrigatório")
            .IsNotNullOrEmpty(Email, "Email", "Email é obrigatório")
            .IsEmail(Email, "Email", "Email inválido")
            .IsNotNullOrEmpty(Senha, "Senha", "Senha é obrigatória")            
            .AreEquals(Senha, ConfirmarSenha, "ConfirmarSenha", "As senhas não conferem")
            .IsTrue(AceitouTermos, "AceitouTermos", "Você deve aceitar os termos de uso")
        );
    }
}