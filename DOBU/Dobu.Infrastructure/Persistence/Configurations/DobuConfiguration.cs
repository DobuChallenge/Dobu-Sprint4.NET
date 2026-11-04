using Dobu.Domain.Commons;
using Dobu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dobu.Infrastructure.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("USUARIO");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID_USUARIO_PK");
        builder.Property(x => x.Nome).HasColumnName("NOME_USUARIO").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Email).HasColumnName("DESC_EMAIL").HasMaxLength(150).IsRequired();
        builder.Property(x => x.Senha).HasColumnName("DESC_SENHA").HasMaxLength(255).IsRequired();
        builder.Property(x => x.TipoUsuario).HasColumnName("TIPO_USUARIO").HasMaxLength(20).IsRequired();
        builder.HasIndex(x => x.Email).IsUnique();
        BaseEntityConfiguration.Configure(builder);
    }
}

public class EspecieConfiguration : IEntityTypeConfiguration<Especie>
{
    public void Configure(EntityTypeBuilder<Especie> builder)
    {
        builder.ToTable("ESPECIE");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID_ESPECIE_PK");
        builder.Property(x => x.Nome).HasColumnName("NOME_ESPECIE").HasMaxLength(80).IsRequired();
        builder.Property(x => x.Descricao).HasColumnName("DESC_ESPECIE").HasMaxLength(300).IsRequired();
        BaseEntityConfiguration.Configure(builder);
    }
}

public class RacaConfiguration : IEntityTypeConfiguration<Raca>
{
    public void Configure(EntityTypeBuilder<Raca> builder)
    {
        builder.ToTable("RACA");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID_RACA_PK");
        builder.Property(x => x.Nome).HasColumnName("NOME_RACA").HasMaxLength(80).IsRequired();
        builder.Property(x => x.Porte).HasColumnName("TIPO_PORTE").HasMaxLength(20).IsRequired();
        builder.Property(x => x.ExpectativaVida).HasColumnName("NUMERO_EXPECTATIVA").IsRequired();
        builder.Property(x => x.Descricao).HasColumnName("DESC_RACA").HasMaxLength(300);
        builder.Property(x => x.Cuidados).HasColumnName("DESC_CUIDADOS").HasMaxLength(500);
        builder.Property(x => x.EspecieId).HasColumnName("ID_ESPECIE_FK");
        builder.HasOne(x => x.Especie).WithMany(x => x.Racas).HasForeignKey(x => x.EspecieId).OnDelete(DeleteBehavior.Restrict);
        BaseEntityConfiguration.Configure(builder);
    }
}

public class PetConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.ToTable("PET");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID_PET_PK");
        builder.Property(x => x.Nome).HasColumnName("NOME_PET").HasMaxLength(80).IsRequired();
        builder.Property(x => x.Idade).HasColumnName("NUMERO_IDADE").IsRequired();
        builder.Property(x => x.RacaId).HasColumnName("ID_RACA_FK");
        builder.Property(x => x.ResponsavelId).HasColumnName("ID_RESPONSAVEL_FK");
        builder.HasOne(x => x.Raca).WithMany(x => x.Pets).HasForeignKey(x => x.RacaId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Responsavel).WithMany(x => x.PetsResponsavel).HasForeignKey(x => x.ResponsavelId).OnDelete(DeleteBehavior.Restrict);
        BaseEntityConfiguration.Configure(builder);
    }
}

public class ConsultaConfiguration : IEntityTypeConfiguration<Consulta>
{
    public void Configure(EntityTypeBuilder<Consulta> builder)
    {
        builder.ToTable("CONSULTA");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID_CONSULTA_PK");
        builder.Property(x => x.DataConsulta).HasColumnName("DATA_CONSULTA").IsRequired();
        builder.Property(x => x.Descricao).HasColumnName("DESC_CONSULTA").HasMaxLength(500).IsRequired();
        builder.Property(x => x.Valor).HasColumnName("VALOR_CONSULTA").HasColumnType("NUMBER(10,2)").IsRequired();
        builder.Property(x => x.PetId).HasColumnName("ID_PET_FK");
        builder.Property(x => x.VeterinarioId).HasColumnName("ID_VETERINARIO_FK");
        builder.HasOne(x => x.Pet).WithMany(x => x.Consultas).HasForeignKey(x => x.PetId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Veterinario).WithMany(x => x.ConsultasVeterinario).HasForeignKey(x => x.VeterinarioId).OnDelete(DeleteBehavior.Restrict);
        BaseEntityConfiguration.Configure(builder);
    }
}

public class AgendamentoConfiguration : IEntityTypeConfiguration<Agendamento>
{
    public void Configure(EntityTypeBuilder<Agendamento> builder)
    {
        builder.ToTable("AGENDAMENTO");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID_AGENDAMENTO_PK");
        builder.Property(x => x.DataAgendamento).HasColumnName("DATA_AGENDAMENTO").IsRequired();
        builder.Property(x => x.Status).HasColumnName("STATUS_AGENDAMENTO").HasMaxLength(30).IsRequired();
        builder.Property(x => x.PetId).HasColumnName("ID_PET_FK");
        builder.Property(x => x.VeterinarioId).HasColumnName("ID_VETERINARIO_FK");
        builder.HasOne(x => x.Pet).WithMany(x => x.Agendamentos).HasForeignKey(x => x.PetId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Veterinario).WithMany(x => x.AgendamentosVeterinario).HasForeignKey(x => x.VeterinarioId).OnDelete(DeleteBehavior.Restrict);
        BaseEntityConfiguration.Configure(builder);
    }
}

public class ProntuarioConfiguration : IEntityTypeConfiguration<Prontuario>
{
    public void Configure(EntityTypeBuilder<Prontuario> builder)
    {
        builder.ToTable("PRONTUARIO");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID_PRONTUARIO_PK");
        builder.Property(x => x.Diagnostico).HasColumnName("DESC_DIAGNOSTICO").HasMaxLength(500).IsRequired();
        builder.Property(x => x.Observacoes).HasColumnName("DESC_OBSERVACOES").HasMaxLength(500);
        builder.Property(x => x.ConsultaId).HasColumnName("ID_CONSULTA_FK");
        builder.HasOne(x => x.Consulta).WithOne(x => x.Prontuario).HasForeignKey<Prontuario>(x => x.ConsultaId).OnDelete(DeleteBehavior.Restrict);
        BaseEntityConfiguration.Configure(builder);
    }
}

public class VacinaConfiguration : IEntityTypeConfiguration<Vacina>
{
    public void Configure(EntityTypeBuilder<Vacina> builder)
    {
        builder.ToTable("VACINA");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID_VACINA_PK");
        builder.Property(x => x.Nome).HasColumnName("NOME_VACINA").HasMaxLength(100).IsRequired();
        builder.Property(x => x.DataAplicacao).HasColumnName("DATA_APLICACAO").IsRequired();
        builder.Property(x => x.DataProximaDose).HasColumnName("DATA_PROXIMA_DOSE");
        builder.Property(x => x.PetId).HasColumnName("ID_PET_FK");
        builder.HasOne(x => x.Pet).WithMany(x => x.Vacinas).HasForeignKey(x => x.PetId).OnDelete(DeleteBehavior.Restrict);
        BaseEntityConfiguration.Configure(builder);
    }
}

public class PagamentoConfiguration : IEntityTypeConfiguration<Pagamento>
{
    public void Configure(EntityTypeBuilder<Pagamento> builder)
    {
        builder.ToTable("PAGAMENTO");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID_PAGAMENTO_PK");
        builder.Property(x => x.Valor).HasColumnName("VALOR_PAGAMENTO").HasColumnType("NUMBER(10,2)").IsRequired();
        builder.Property(x => x.FormaPagamento).HasColumnName("TIPO_FORMA_PAGAMENTO").HasMaxLength(40).IsRequired();
        builder.Property(x => x.DataPagamento).HasColumnName("DATA_PAGAMENTO").IsRequired();
        builder.Property(x => x.ConsultaId).HasColumnName("ID_CONSULTA_FK");
        builder.HasOne(x => x.Consulta).WithOne(x => x.Pagamento).HasForeignKey<Pagamento>(x => x.ConsultaId).OnDelete(DeleteBehavior.Restrict);
        BaseEntityConfiguration.Configure(builder);
    }
}

public class LembreteConfiguration : IEntityTypeConfiguration<Lembrete>
{
    public void Configure(EntityTypeBuilder<Lembrete> builder)
    {
        builder.ToTable("LEMBRETE");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID_LEMBRETE_PK");
        builder.Property(x => x.Descricao).HasColumnName("DESC_LEMBRETE").HasMaxLength(300).IsRequired();
        builder.Property(x => x.DataLembrete).HasColumnName("DATA_LEMBRETE").IsRequired();
        builder.Property(x => x.Status).HasColumnName("STATUS_LEMBRETE").HasMaxLength(30).IsRequired();
        builder.Property(x => x.PetId).HasColumnName("ID_PET_FK");
        builder.HasOne(x => x.Pet).WithMany(x => x.Lembretes).HasForeignKey(x => x.PetId).OnDelete(DeleteBehavior.Restrict);
        BaseEntityConfiguration.Configure(builder);
    }
}

public class DobuCamConfiguration : IEntityTypeConfiguration<DobuCam>
{
    public void Configure(EntityTypeBuilder<DobuCam> builder)
    {
        builder.ToTable("DOBUCAM");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID_DOBUCAM_PK");
        builder.Property(x => x.Localizacao).HasColumnName("DESC_LOCALIZACAO").HasMaxLength(150).IsRequired();
        builder.Property(x => x.StatusCamera).HasColumnName("STATUS_CAMERA").HasMaxLength(30).IsRequired();
        builder.Property(x => x.DataUltimaMovimentacao).HasColumnName("DATA_ULTIMA_MOVIMENTACAO");
        builder.Property(x => x.PetId).HasColumnName("ID_PET_FK");
        builder.HasOne(x => x.Pet).WithMany(x => x.DobuCams).HasForeignKey(x => x.PetId).OnDelete(DeleteBehavior.Restrict);
        BaseEntityConfiguration.Configure(builder);
    }
}

public class AnaliseIaConfiguration : IEntityTypeConfiguration<AnaliseIa>
{
    public void Configure(EntityTypeBuilder<AnaliseIa> builder)
    {
        builder.ToTable("ANALISE_IA");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID_ANALISE_IA_PK");
        builder.Property(x => x.Descricao).HasColumnName("DESC_ANALISE").HasMaxLength(800).IsRequired();
        builder.Property(x => x.Risco).HasColumnName("NUMERO_RISCO").IsRequired();
        builder.Property(x => x.DataAnalise).HasColumnName("DATA_ANALISE").IsRequired();
        builder.Property(x => x.ProntuarioId).HasColumnName("ID_PRONTUARIO_FK");
        builder.HasOne(x => x.Prontuario).WithOne(x => x.AnaliseIa).HasForeignKey<AnaliseIa>(x => x.ProntuarioId).OnDelete(DeleteBehavior.Restrict);
        BaseEntityConfiguration.Configure(builder);
    }
}

public class LogErroConfiguration : IEntityTypeConfiguration<LogErro>
{
    public void Configure(EntityTypeBuilder<LogErro> builder)
    {
        builder.ToTable("LOG_ERRO");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID_LOG_ERRO_PK");
        builder.Property(x => x.NomeProcedure).HasColumnName("NOME_PROCEDURE").HasMaxLength(100).IsRequired();
        builder.Property(x => x.DescricaoErro).HasColumnName("DESC_ERRO").HasMaxLength(1000).IsRequired();
        builder.Property(x => x.DataErro).HasColumnName("DATA_ERRO").IsRequired();
        builder.Property(x => x.UsuarioId).HasColumnName("ID_USUARIO_FK");
        builder.HasOne(x => x.Usuario).WithMany(x => x.LogsErro).HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.Restrict);
        BaseEntityConfiguration.Configure(builder);
    }
}

public class InformacaoCuidadoConfiguration : IEntityTypeConfiguration<InformacaoCuidado>
{
    public void Configure(EntityTypeBuilder<InformacaoCuidado> builder)
    {
        builder.ToTable("INFORMACAO_CUIDADO");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID_INFORMACAO_CUIDADO_PK");
        builder.Property(x => x.Titulo).HasColumnName("TITULO").HasMaxLength(120).IsRequired();
        builder.Property(x => x.Descricao).HasColumnName("DESCRICAO").HasMaxLength(1000).IsRequired();
        builder.Property(x => x.PetId).HasColumnName("ID_PET_FK");
        builder.HasOne(x => x.Pet).WithMany(x => x.InformacoesCuidado).HasForeignKey(x => x.PetId).OnDelete(DeleteBehavior.Cascade);
        BaseEntityConfiguration.Configure(builder);
    }
}

internal static class BaseEntityConfiguration
{
    public static void Configure<T>(EntityTypeBuilder<T> builder) where T : BaseEntity
    {
        builder.Property(x => x.CreatedAt).HasColumnName("CREATED_AT").IsRequired();
        builder.Property(x => x.Active)
            .HasColumnName("ACTIVE")
            .HasColumnType("NUMBER(1)")
            .HasConversion(v => v ? 1 : 0, v => v == 1)
            .IsRequired();
    }
}
