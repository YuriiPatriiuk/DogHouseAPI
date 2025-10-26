using DogHouseAPI.Models;
using DogHouseAPI.Models.DTO;

namespace DogHouseAPI.API
{
    public interface IDogHouseRepository
    {
        public Task<IEnumerable<Dog>> GetAllAsync(DogSearchAttributesDto parametrs);

        public Task<Dog> AddDogAsync(Dog dog);

        public Task<bool> IsDogExist(string name);
    }
}
