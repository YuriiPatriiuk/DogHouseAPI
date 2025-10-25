using DogHouseAPI.Models;
using DogHouseAPI.API;
using DogHouseAPI.Models.DTO;

namespace DogHouseAPI.Services.DogHouseService
{
    public class DogHouseService : IDogHouseService
    {
        private readonly IDogHouseRepository _dogHouseRepository;
        private readonly ILogger<DogHouseService> _logger;
        public DogHouseService(IDogHouseRepository dogHouseRepository, ILogger<DogHouseService> logger)
        {
            _dogHouseRepository = dogHouseRepository;
            _logger = logger;
        }
        public Task<IEnumerable<Dog>> Get()
        {
            _logger.LogInformation("Get all dogs from the repository.");
            return _dogHouseRepository.GetAll();
        }
        public async Task<Dog> AddDog(CreateDogDto dog)
        {
            _ = dog ?? throw new ArgumentNullException(nameof(dog));
            _logger.LogInformation($"Start adding a new dog");

            await ValidateDogParametrs(dog);

            var newDog = new Dog
            {
                Name = dog.Name,
                Color = dog.Color,
                TailLength = dog.TailLength,
                Weight = dog.Weight
            };

            try
            {
               var addedDog = await _dogHouseRepository.AddDog(newDog);
                _logger.LogInformation($"Adding a new dog is succesfull");
                return addedDog;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a new dog to the repository.");
                throw;
            }

        }
        public async Task ValidateDogParametrs(CreateDogDto dog)
        {
            if (await IsDogExist(dog.Name))
            {
                throw new Exception($"Dog with name {dog.Name} already exists.");
            }
            if (IsParametrsRight(dog))
            {
                throw new Exception($"Dog parameters are not valid.");
            }
        }

        public Task<bool> IsDogExist(string name)
        {
            return _dogHouseRepository.IsDogExist(name);
        }

        public bool IsParametrsRight(CreateDogDto dog)
        {
            return (dog.TailLength < 0 || dog.Weight < 0);
        }
    }
}
