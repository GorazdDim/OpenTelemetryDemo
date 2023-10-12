using Microsoft.EntityFrameworkCore;

namespace OpenTelemetryDemo
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            
        }

        public DbSet<Student> Students => Set<Student>();
    }
}
