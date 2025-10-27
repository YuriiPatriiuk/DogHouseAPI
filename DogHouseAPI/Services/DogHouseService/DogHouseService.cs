using DogHouseAPI.Models;
using DogHouseAPI.API;
using DogHouseAPI.Models.DTO;
using DogHouseAPI.Mapping;

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
        public async Task<IEnumerable<DogResponseDto>> Get(DogSearchAttributesDto parametrs)
        {
            _logger.LogInformation("Get dogs from the repository.");
            
            try
            {
                ValidateSearchParametrs(parametrs);
                var dogs = await _dogHouseRepository.GetAllAsync(parametrs);
                var dogDtos = dogs.ToDto();
                return dogDtos;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting dogs from the repository.");
                throw;
            }
        }
        public async Task<DogResponseDto> AddDog(CreateDogDto dog)
        {
            _ = dog ?? throw new ArgumentNullException(nameof(dog));
            _logger.LogInformation($"Start adding a new dog");
            
            try
            {
                await ValidateDogParametrs(dog);
    
                var newDog = dog.ToDog();
            
                var addedDog = await _dogHouseRepository.AddDogAsync(newDog);
                _logger.LogInformation($"Adding a new dog is successful");
                return addedDog.ToDto();
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
        public void ValidateSearchParametrs(DogSearchAttributesDto parametrs)
        {
            if(parametrs.Attribute != null)
            {
                var validAttributes = new List<string> { "name", "color", "taillength", "weight" };
                if (!validAttributes.Contains(parametrs.Attribute.ToLower()))
                {
                    throw new Exception($"Invalid attribute: {parametrs.Attribute}.");
                }
            }
            if (parametrs.Order != null)
            {
                var validOrders = new List<string> { "asc", "desc" };
                if (!validOrders.Contains(parametrs.Order.ToLower()))
                {
                    throw new Exception($"Invalid order: {parametrs.Order}.");
                }
            }
            if (parametrs.PageNumber <= 0 || parametrs.PageSize <= 0)
            {
                throw new Exception("PageNumber and PageSize must be greater than zero.");
            }
        }
        public async Task<bool> IsDogExist(string name)
        {
            return await _dogHouseRepository.IsDogExist(name);
        }
        public bool IsParametrsRight(CreateDogDto dog)
        {
            return (dog.TailLength < 0 || dog.Weight < 0);
        }
    }
}
