using Microsoft.EntityFrameworkCore;
using KYC.Models;

namespace KYC.Models
{
    public class EntityContext : DbContext
    {
        public EntityContext(DbContextOptions<EntityContext> options)
        : base(options)
        {
        }

        public DbSet<Entity> Entities { get; set; } = null!;
    }
}
