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
            builder.Property(x => x.ObservacoesClinicas).HasMaxLength(2000);
            builder.Property(x => x.NivelEsportivo).HasMaxLength(120);
            builder.HasIndex(x => x.ProfessorId);
            builder.HasOne(x => x.Professor)
                .WithMany(x => x.Atletas)
                .HasForeignKey(x => x.ProfessorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SessaoDeTreinoModel>(builder =>
        {
            builder.ToTable("sessoes_treino");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Tipo).IsRequired();
            builder.Property(x => x.DuracaoMinutos).IsRequired();
            builder.Property(x => x.DistanciaKm).IsRequired();
            builder.Property(x => x.Rpe).IsRequired();
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
