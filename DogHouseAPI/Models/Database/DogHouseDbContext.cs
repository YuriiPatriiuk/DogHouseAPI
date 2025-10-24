using Microsoft.EntityFrameworkCore;

namespace DogHouseAPI.Models.Database
{
    public class DogHouseDbContext : DbContext
    {
        public DogHouseDbContext(DbContextOptions<DogHouseDbContext> options) : base(options)
        {}
        public DbSet<Dog> Dogs { get; set; }
    }
}
