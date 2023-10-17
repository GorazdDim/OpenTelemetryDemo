using Microsoft.EntityFrameworkCore;

namespace OpenTelemetryDemo.EF.Entities
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Professor> Professors { get; set; }

    }
}
