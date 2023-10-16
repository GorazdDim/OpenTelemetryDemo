using Microsoft.EntityFrameworkCore;

namespace OpenTelemetryDemo.EF.Entities
{
    public class ApplicationDbContext: DbContext
    {

        public DbSet<Student> Students { get; set; }
        public DbSet<Professor> Professors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=OpenTelemetryDemo;Trusted_Connection=True;TrustServerCertificate=True;User id=GDimovski;Password=Gorazd2023!");
        }
    }
}
