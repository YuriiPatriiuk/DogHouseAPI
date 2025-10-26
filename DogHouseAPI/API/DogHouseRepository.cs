using DogHouseAPI.Models;
using DogHouseAPI.Models.Database;
using DogHouseAPI.Models.DTO;
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

        public async Task<IEnumerable<Dog>> GetAllAsync(DogSearchAttributesDto parametrs)
        {
            IQueryable<Dog> query = _dbContext.Dogs;
            query = ApplyFilters(query, parametrs);

            int dogsToSkip = (parametrs.PageNumber - 1) * parametrs.PageSize;
            query = query.Skip(dogsToSkip).Take(parametrs.PageSize);

            return await query.ToListAsync();
        }

        public async Task<Dog> AddDogAsync(Dog dog)
        {
            _dbContext.Dogs.Add(dog);
            await _dbContext.SaveChangesAsync();
            return dog;
        }

        public async Task<bool> IsDogExist(string name)
        {
            return await _dbContext.Dogs.AnyAsync(d => d.Name == name);
        }

        public IQueryable<Dog> ApplyFilters(IQueryable<Dog> query, DogSearchAttributesDto parametrs)
        {
            if (string.IsNullOrWhiteSpace(parametrs.Attribute))
            {
                return query;
            }

            var orderParametr = parametrs.Order?.ToLower();

            switch(parametrs.Attribute.ToLower())
            {
                case "name":
                    query = orderParametr == "desc" ? query.OrderByDescending(d => d.Name) : query.OrderBy(d => d.Name);
                    break;
                case "color":
                    query = orderParametr == "desc" ? query.OrderByDescending(d => d.Color) : query.OrderBy(d => d.Color);
                    break;
                case "tailLength":
                    query = orderParametr == "desc" ? query.OrderByDescending(d => d.TailLength) : query.OrderBy(d => d.TailLength);
                    break;
                case "weight":
                    query = orderParametr == "desc" ? query.OrderByDescending(d => d.Weight) : query.OrderBy(d => d.Weight);
                    break;
                default:
                    break;
            }

            return query;
        }
    }
}
