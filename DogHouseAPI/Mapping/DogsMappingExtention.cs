using DogHouseAPI.Models;
using DogHouseAPI.Models.DTO;

namespace DogHouseAPI.Mapping
{
    public static class DogsMappingExtention
    {
        public static DogResponseDto ToDto(this Dog dog)
        {
            return new DogResponseDto
            {
                Name = dog.Name,
                Color = dog.Color,
                TailLength = dog.TailLength,
                Weight = dog.Weight
            };
        }

        public static Dog ToDog(this CreateDogDto createDogDto)
        {
            return new Dog
            {
                Name = createDogDto.Name,
                Color = createDogDto.Color,
                TailLength = createDogDto.TailLength,
                Weight = createDogDto.Weight
            };
        }

        public static IEnumerable<DogResponseDto> ToDto(this IEnumerable<Dog> dogs)
        {
            return dogs.Select(dog => dog.ToDto());
        }
    }
}
