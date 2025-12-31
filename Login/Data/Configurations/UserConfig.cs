using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Login.Models.Entities;

namespace Login.Data.Configurations
{
    public class UserConfig : IEntityTypeConfiguration<NFUser>
    {
        public void Configure(EntityTypeBuilder<NFUser> builder)
        {
            // Nome da tabela
            builder.ToTable("usuarios");

            // Chave primária
            builder.HasKey(u => u.id)
                   .HasName("PK_usuarios");

            // Configuração das propriedades
            builder.Property(u => u.id)
                   .HasColumnName("id")
                   .ValueGeneratedOnAdd();

            builder.Property(u => u.nome)
                   .IsRequired()
                   .HasMaxLength(100)
                   .HasColumnName("nome");

            builder.Property(u => u.email)
                   .IsRequired()
                   .HasMaxLength(255)
                   .HasColumnName("email");

            builder.Property(u => u.email_verificado)
                   .IsRequired()
                   .HasDefaultValue(false)
                   .HasColumnName("email_verificado");

            builder.Property(u => u.senha_hash)
                   .IsRequired()
                   .HasMaxLength(255)
                   .HasColumnName("senha_hash");

            builder.Property(u => u.salt)
                   .IsRequired()
                   .HasMaxLength(50)
                   .HasColumnName("salt");

            // Mapeamento do Enum para string no banco
            builder.Property(u => u.tipo_usuario)
              .IsRequired()
              .HasConversion(
                  // Converte ENUM para string ao salvar no banco
                  v => v.ToString().ToLower(), // Converte `COMUM` -> `"comum"`
                                               // Converte string do banco para ENUM ao ler
                  v => (TipoUsuario)Enum.Parse(typeof(TipoUsuario), v, ignoreCase: true) // Converte `"comum"` -> `COMUM`
              )
              .HasMaxLength(50)
              .HasDefaultValue(TipoUsuario.COMUM)
              .HasColumnName("tipo_usuario");

            builder.Property(u => u.ativo)
                   .IsRequired()
                   .HasDefaultValue(true)
                   .HasColumnName("ativo");

            builder.Property(u => u.motivo_inativacao)
                   .HasMaxLength(255)
                   .HasColumnName("motivo_inativacao");

            builder.Property(u => u.data_banimento)
                   .HasColumnName("data_banimento");

            builder.Property(u => u.data_criacao)
                   .IsRequired()
                   .HasDefaultValueSql("CURRENT_TIMESTAMP")
                   .ValueGeneratedOnAdd()
                   .HasColumnName("data_criacao");

            builder.Property(u => u.data_ultima_atualizacao)
                   .IsRequired()
                   .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")
                   .ValueGeneratedOnAddOrUpdate()
                   .HasColumnName("data_ultima_atualizacao");

            builder.Property(u => u.criado_por)
                   .HasColumnName("criado_por");

            builder.Property(u => u.atualizado_por)
                   .HasColumnName("atualizado_por");

            builder.Property(u => u.dois_fatores_habilitado)
                   .IsRequired()
                   .HasDefaultValue(false)
                   .HasColumnName("dois_fatores_habilitado");

            builder.Property(u => u.chave_dois_fatores)
                   .HasMaxLength(100)
                   .HasColumnName("chave_dois_fatores");

            builder.Property(u => u.data_ultima_senha_alterada)
                   .IsRequired()
                   .HasDefaultValueSql("CURRENT_TIMESTAMP")
                   .HasColumnName("data_ultima_senha_alterada");

            builder.Property(u => u.senha_expira_em)
                   .HasColumnName("senha_expira_em")
                   .HasConversion(
                       v => v.HasValue ? v.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                       v => v.HasValue ? DateOnly.FromDateTime(v.Value) : (DateOnly?)null
                   );

            builder.Property(u => u.ultimo_login)
                   .HasColumnName("ultimo_login");

            builder.Property(u => u.ultimo_ip)
                   .HasMaxLength(45)
                   .HasColumnName("ultimo_ip");

            builder.Property(u => u.total_logins)
                   .IsRequired()
                   .HasDefaultValue(0)
                   .HasColumnName("total_logins");

            builder.Property(u => u.linguagem_preferida)
                   .IsRequired()
                   .HasMaxLength(10)
                   .HasDefaultValue("pt-BR")
                   .HasColumnName("linguagem_preferida");

            builder.Property(u => u.fuso_horario)
                   .IsRequired()
                   .HasMaxLength(50)
                   .HasDefaultValue("America/Sao_Paulo")
                   .HasColumnName("fuso_horario");

            builder.Property(u => u.aceitou_termos)
                   .IsRequired()
                   .HasDefaultValue(false)
                   .HasColumnName("aceitou_termos");

            builder.Property(u => u.data_aceitacao_termos)
                   .HasColumnName("data_aceitacao_termos");

            // Configuração dos relacionamentos (auto-relacionamento)
            builder.HasOne(u => u.Criador)
                   .WithMany()
                   .HasForeignKey(u => u.criado_por)
                   .OnDelete(DeleteBehavior.SetNull)
                   .HasConstraintName("FK_usuarios_criado_por");

            builder.HasOne(u => u.Atualizador)
                   .WithMany()
                   .HasForeignKey(u => u.atualizado_por)
                   .OnDelete(DeleteBehavior.SetNull)
                   .HasConstraintName("FK_usuarios_atualizado_por");

            // Índices
            builder.HasIndex(u => u.email)
                   .IsUnique()
                   .HasDatabaseName("idx_email");

            builder.HasIndex(u => u.tipo_usuario)
                   .HasDatabaseName("idx_tipo_usuario");

            builder.HasIndex(u => u.ativo)
                   .HasDatabaseName("idx_ativo");

            builder.HasIndex(u => u.data_criacao)
                   .HasDatabaseName("idx_data_criacao");

            builder.HasIndex(u => u.ultimo_login)
                   .HasDatabaseName("idx_ultimo_login");

            // Configuração de valor padrão para DateOnly (se necessário)
            builder.HasQueryFilter(u => u.ativo == true); // Filtro global para trazer apenas usuários ativos
        }
    }
}