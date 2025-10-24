using DogHouseAPI.Models;

namespace DogHouseAPI.Services.DogHouseService
{
    public class DogHouseService : IDogHouseService
    {
        public IEnumerable<Dog> Get()
        {
            return new List<Dog>
            {
                new Dog { Id = 1, Name = "Buddy", Color = "Brown", TailLength = 15.5f, Weight = 30.0f },
                new Dog { Id = 2, Name = "Max", Color = "Black", TailLength = 12.0f, Weight = 25.0f },
                new Dog { Id = 3, Name = "Bella", Color = "White", TailLength = 10.0f, Weight = 20.0f },
                new Dog { Id = 4, Name = "Lucy", Color = "Golden", TailLength = 14.0f, Weight = 28.0f },
                new Dog { Id = 5, Name = "Charlie", Color = "Gray", TailLength = 13.5f, Weight = 22.0f }
            };
        }
    }
}
