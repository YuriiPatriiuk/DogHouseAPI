using DogHouseAPI.Models;

namespace DogHouseAPI.Services.DogHouseService
{
    public interface IDogHouseService
    {
        public IEnumerable<Dog> Get();
    }
}
