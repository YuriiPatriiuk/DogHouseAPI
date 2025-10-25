using DogHouseAPI.Models;
using DogHouseAPI.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace DogHouseAPI.API
{
    public class DogHouseRepository : IDogHouseRepository
    {
        private readonly DogHouseDbContext _dbContext;

        public DogHouseRepository(DogHouseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Dog>> GetAll()
        {
            return await _dbContext.Dogs.ToListAsync();
        }

        public async Task<Dog> AddDog(Dog dog)
        {
            _dbContext.Dogs.Add(dog);
            await _dbContext.SaveChangesAsync();
            return dog;
        }

        public async Task<bool> IsDogExist(string name)
        {
            return await _dbContext.Dogs.AnyAsync(d => d.Name == name);
        }
    }
}
