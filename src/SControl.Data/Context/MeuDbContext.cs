using Microsoft.EntityFrameworkCore;
using SControl.Business.Models;

namespace SControl.Data.Context;

// Classe que representa o contexto de dados da aplicação
public class MeuDbContext : DbContext
{
    // Construtor que recebe as opções de configuração do contexto
    public MeuDbContext(DbContextOptions options) : base(options)
    {
        // Define o comportamento de rastreamento do ChangeTracker para ler as entidades do banco de dados como somente leitura.
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        // Desabilita a detecção automática de mudanças nas entidades rastreadas pelo ChangeTracker.
        ChangeTracker.AutoDetectChangesEnabled = false; 
    }

    // Definição dos DbSet's que representam as entidades do modelo de domínio
    public DbSet<Curso> Cursos { get; set; }

    // Sobrescrita do método OnModelCreating para configurar o mapeamento das entidades
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
        {
            property.SetColumnType("varchar(100)");
        }

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MeuDbContext).Assembly);

        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;
        }

        base.OnModelCreating(modelBuilder);
    }

    // Sobrescrita do método SaveChangesAsync para automatizar a atualização da propriedade DataCadastro
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entry in ChangeTracker.Entries().Where(entry => entry.Entity.GetType().GetProperty("DataCadastro") != null))
        {
            // Atualiza automaticamente a propriedade DataCadastro em caso de inserção
            if (entry.State == EntityState.Added)
            {
                entry.Property("DataCadastro").CurrentValue = DateTime.Now;
            }

            // Desabilita da alteração da propriedade DataCadastro em caso de atualização
            if (entry.State == EntityState.Modified)
            {
                entry.Property("DataCadastro").IsModified = false;
            }
        }

        // Chamada do método da classe base para salvar as alterações no banco de dados
        return base.SaveChangesAsync(cancellationToken);
    }
}