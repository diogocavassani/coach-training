using CoachTraining.Infra.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace CoachTraining.Infra.Persistence;

public class CoachTrainingDbContext : DbContext
{
    public CoachTrainingDbContext(DbContextOptions<CoachTrainingDbContext> options)
        : base(options)
    {
    }

    public DbSet<AtletaModel> Atletas => Set<AtletaModel>();
    public DbSet<ProfessorModel> Professores => Set<ProfessorModel>();
    public DbSet<SessaoDeTreinoModel> SessoesDeTreino => Set<SessaoDeTreinoModel>();
    public DbSet<ProvaAlvoModel> ProvasAlvo => Set<ProvaAlvoModel>();
    public DbSet<LinkPublicoIntegracaoModel> LinksPublicosIntegracao => Set<LinkPublicoIntegracaoModel>();
    public DbSet<ConexaoWearableModel> ConexoesWearable => Set<ConexaoWearableModel>();
    public DbSet<CredencialWearableModel> CredenciaisWearable => Set<CredencialWearableModel>();
    public DbSet<EventoWebhookRecebidoModel> EventosWebhookRecebidos => Set<EventoWebhookRecebidoModel>();
    public DbSet<AtividadeImportadaModel> AtividadesImportadas => Set<AtividadeImportadaModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProfessorModel>(builder =>
        {
            builder.ToTable("professores");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Nome).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Email).HasMaxLength(320).IsRequired();
            builder.Property(x => x.SenhaHash).HasMaxLength(500).IsRequired();
            builder.Property(x => x.DataCriacao).IsRequired();
            builder.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<AtletaModel>(builder =>
        {
            builder.ToTable("atletas");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ProfessorId).IsRequired();
            builder.Property(x => x.Nome).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Email).HasMaxLength(320);
            builder.Property(x => x.ObservacoesClinicas).HasMaxLength(2000);
            builder.Property(x => x.NivelEsportivo).HasMaxLength(120);
            builder.Property(x => x.TreinosPlanejadosPorSemana);
            builder.HasIndex(x => x.ProfessorId);
            builder.HasOne(x => x.Professor)
                .WithMany(x => x.Atletas)
                .HasForeignKey(x => x.ProfessorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LinkPublicoIntegracaoModel>(builder =>
        {
            builder.ToTable("links_publicos_integracao");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.TokenHash).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Ativo).IsRequired();
            builder.Property(x => x.CriadoEmUtc).IsRequired();
            builder.HasIndex(x => x.TokenHash).IsUnique();
            builder.HasIndex(x => new { x.AtletaId, x.Ativo })
                .HasFilter("\"Ativo\" = true")
                .IsUnique();
            builder.HasOne(x => x.Atleta)
                .WithMany()
                .HasForeignKey(x => x.AtletaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ConexaoWearableModel>(builder =>
        {
            builder.ToTable("conexoes_wearable");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Provedor).IsRequired();
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.ExternalAthleteId).HasMaxLength(120).IsRequired();
            builder.Property(x => x.ScopesConcedidos).HasMaxLength(500);
            builder.Property(x => x.ConectadoEmUtc).IsRequired();
            builder.Property(x => x.UltimoErro).HasMaxLength(500);
            builder.HasIndex(x => new { x.AtletaId, x.Provedor }).IsUnique();
            builder.HasOne(x => x.Atleta)
                .WithMany()
                .HasForeignKey(x => x.AtletaId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Credencial)
                .WithOne(x => x.ConexaoWearable)
                .HasForeignKey<CredencialWearableModel>(x => x.ConexaoWearableId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CredencialWearableModel>(builder =>
        {
            builder.ToTable("credenciais_wearable");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.AccessTokenProtegido).IsRequired();
            builder.Property(x => x.RefreshTokenProtegido).IsRequired();
            builder.Property(x => x.ExpiresAtUtc).IsRequired();
            builder.Property(x => x.AtualizadoEmUtc).IsRequired();
        });

        modelBuilder.Entity<EventoWebhookRecebidoModel>(builder =>
        {
            builder.ToTable("eventos_webhook_recebidos");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Provedor).IsRequired();
            builder.Property(x => x.ObjectType).HasMaxLength(40).IsRequired();
            builder.Property(x => x.ObjectId).HasMaxLength(120).IsRequired();
            builder.Property(x => x.OwnerId).HasMaxLength(120).IsRequired();
            builder.Property(x => x.AspectType).HasMaxLength(40).IsRequired();
            builder.Property(x => x.PayloadJson).IsRequired();
            builder.Property(x => x.Fingerprint).HasMaxLength(200).IsRequired();
            builder.Property(x => x.RecebidoEmUtc).IsRequired();
            builder.Property(x => x.StatusProcessamento).HasMaxLength(40);
            builder.Property(x => x.ErroProcessamento).HasMaxLength(1000);
            builder.HasIndex(x => x.Fingerprint);
        });

        modelBuilder.Entity<AtividadeImportadaModel>(builder =>
        {
            builder.ToTable("atividades_importadas");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Provedor).IsRequired();
            builder.Property(x => x.ExternalActivityId).HasMaxLength(120).IsRequired();
            builder.Property(x => x.ImportadoEmUtc).IsRequired();
            builder.HasIndex(x => new { x.Provedor, x.ExternalActivityId }).IsUnique();
            builder.HasOne(x => x.ConexaoWearable)
                .WithMany()
                .HasForeignKey(x => x.ConexaoWearableId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SessaoDeTreinoModel>(builder =>
        {
            builder.ToTable("sessoes_treino");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Tipo).IsRequired();
            builder.Property(x => x.DuracaoMinutos).IsRequired();
            builder.Property(x => x.DistanciaKm).IsRequired();
            builder.Property(x => x.Rpe).IsRequired();
            builder.Property(x => x.OrigemTreino).IsRequired().HasDefaultValue(0);
            builder.HasIndex(x => x.AtletaId);
            builder.HasIndex(x => x.Data);
            builder.HasIndex(x => new { x.AtletaId, x.Data });
            builder.HasOne(x => x.Atleta)
                .WithMany(x => x.SessoesDeTreino)
                .HasForeignKey(x => x.AtletaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProvaAlvoModel>(builder =>
        {
            builder.ToTable("provas_alvo");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.DistanciaKm).IsRequired();
            builder.Property(x => x.Objetivo).HasMaxLength(300);
            builder.HasIndex(x => x.AtletaId).IsUnique();
            builder.HasOne(x => x.Atleta)
                .WithOne(x => x.ProvaAlvo)
                .HasForeignKey<ProvaAlvoModel>(x => x.AtletaId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
