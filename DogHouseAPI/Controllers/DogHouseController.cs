using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using DogHouseAPI.Configurations;
using DogHouseAPI.Models;
using DogHouseAPI.Services.DogHouseService;

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
        public IEnumerable<Dog> GetDogs()
        {
            return _dogHouseService.Get().ToList();
        }
    }
}
