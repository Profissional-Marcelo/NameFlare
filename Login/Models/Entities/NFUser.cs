namespace Login.Models.Entities
{
    public enum TipoUsuario
    {
        COMUM,
        VIP,
        ADMINISTRADOR
    }

    public partial class NFUser
    {
        public int id { get; set; }
        public string? nome { get; set; }
        public string? email { get; set; }
        public bool email_verificado { get; set; }
        public string? senha_hash { get; set; }
        public string? salt { get; set; }
        public TipoUsuario tipo_usuario { get; set; }
        public bool ativo { get; set; }
        public string? motivo_inativacao { get; set; }
        public DateTime? data_banimento { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_ultima_atualizacao { get; set; }
        public int? criado_por { get; set; }
        public int? atualizado_por { get; set; }
        public bool dois_fatores_habilitado { get; set; }
        public string? chave_dois_fatores { get; set; }
        public DateTime data_ultima_senha_alterada { get; set; }
        public DateOnly? senha_expira_em { get; set; }
        public DateTime? ultimo_login { get; set; }
        public string? ultimo_ip { get; set; }
        public int total_logins { get; set; }
        public string linguagem_preferida { get; set; } = "pt-BR";
        public string fuso_horario { get; set; } = "America/Sao_Paulo";
        public bool aceitou_termos { get; set; }
        public DateTime? data_aceitacao_termos { get; set; }

        // Propriedades de navegação (opcionais, mas úteis)
        public virtual NFUser? Criador { get; set; }
        public virtual NFUser? Atualizador { get; set; }
    }
}