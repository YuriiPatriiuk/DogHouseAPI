using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using DogHouseAPI.Configurations;
using DogHouseAPI.Models;
using DogHouseAPI.Services.DogHouseService;
using DogHouseAPI.Models.DTO;

namespace DogHouseAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DogHouseController : ControllerBase
    {
        private readonly IDogHouseService _dogHouseService;
        private readonly ILogger<DogHouseController> _logger;
        private readonly string _version;

        public DogHouseController(IDogHouseService dogHouseService, ILogger<DogHouseController> logger, IOptions<AppInfoOptions> options)
        {
            _dogHouseService = dogHouseService;
            _logger = logger;
            _version = options.Value.Version;
        }

        [HttpGet, Route("/ping")]
        public ActionResult<string> Ping()
        {
            return Ok(_version);
        }

        [HttpGet, Route("/dogs")] 
        public async Task<ActionResult<IEnumerable<DogResponseDto>>> GetDogs([FromQuery] DogSearchAttributesDto parametrs)
        {
            try
            {
                var dogs = await _dogHouseService.Get(parametrs);
                return Ok(dogs);
            }
            catch (Exception ex)
            {
                var message = $"Error occurred while retrieving dogs: {ex.Message}";
                _logger.LogError(ex, message);
                return StatusCode(StatusCodes.Status500InternalServerError, message);
            }
        }

        [HttpPost, Route("/dog")]
        public async Task<ActionResult<Dog>> AddDog([FromBody] CreateDogDto dog)
        {
            if (!ModelState.IsValid)
            { 
                return BadRequest(ModelState);
            }
            if (dog == null)
            {
                return BadRequest("Dog data is null.");
            }

            try
            {
                var dogAdded = await _dogHouseService.AddDog(dog);

                return CreatedAtAction(nameof(AddDog), new { id = dogAdded.Id }, dogAdded);
            }
            catch (Exception ex)
            {
                var message = $"Error occurred while adding a new dog: {ex.Message}";
                _logger.LogError(ex, message);
                return BadRequest(message);
            }
        }
    }
}
