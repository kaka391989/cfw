using CFW.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;

namespace CFW.EFCore
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Get the assembly that contains your entities
            var assembly = Assembly.GetEntryAssembly();

            // Find all types that implement IEntity<Guid>
            var entityTypes = assembly!.GetTypes()
                .Where(t => t.GetInterfaces()
                    .Any(i => i.IsGenericType &&
                              i.GetGenericTypeDefinition() == typeof(IEntity<>) &&
                              i.GetGenericArguments().Contains(typeof(Guid))));

            // Add each entity type to the model builder
            foreach (var type in entityTypes)
            {
                modelBuilder.Entity(type);
            }
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            ChangeTracker.DetectChanges();
            var entities = ChangeTracker.Entries()
                .Where(t => t.Entity is IAuditable && (t.State == EntityState.Added || t.State == EntityState.Modified));

            var now = DateTime.UtcNow;
            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((IAuditable)entity.Entity).CreatedAt = now;
                }
                else
                {
                    ((IAuditable)entity.Entity).UpdatedAt = now;
                }
            }

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
