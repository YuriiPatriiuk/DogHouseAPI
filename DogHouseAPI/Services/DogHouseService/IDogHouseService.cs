using DogHouseAPI.Models;
using DogHouseAPI.Models.DTO;

namespace DogHouseAPI.Services.DogHouseService
{
    public interface IDogHouseService
    {
        public Task<IEnumerable<Dog>> Get();
        public Task<Dog> AddDog(CreateDogDto dog);
        public Task ValidateDogParametrs(CreateDogDto dog);
    }
}
