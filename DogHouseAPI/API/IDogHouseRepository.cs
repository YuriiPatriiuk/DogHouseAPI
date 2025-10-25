using DogHouseAPI.Models;

namespace DogHouseAPI.API
{
    public interface IDogHouseRepository
    {
        public Task<IEnumerable<Dog>> GetAll();

        public Task<Dog> AddDog(Dog dog);

        public Task<bool> IsDogExist(string name);
    }
}
