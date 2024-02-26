using LabinventTestTask.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LabinventTestTask.Infrastructure.Database
{
    public class DataContext : DbContext
    {
        public DbSet<ModuleData> ModuleData { get; set; }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }
    }
}
