using ArgoJanitor.WebApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace ArgoJanitor.WebApi.Infrastructure.Persistence
{
    public class ArgoJanitorDbContext : DbContext
    {
        public ArgoJanitorDbContext(DbContextOptions<ArgoJanitorDbContext> options) : base(options)
        {
        }

        public DbSet<Capability> Capabilities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Capability>(cfg => { cfg.ToTable("Capability"); });
        }
    }
}