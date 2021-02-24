using Microsoft.EntityFrameworkCore;

namespace Catflip.Api.DataAccess
{
    public class CatflipDbContext : DbContext
    {

        public CatflipDbContext(DbContextOptions<CatflipDbContext> options) : base(options)
        {
        }


        public DbSet<UserEntity> Users { get; set; }
    }
}
