using DogHouseAPI.Controllers;
using DogHouseAPI.Services.DogHouseService;
using DogHouseAPI.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Moq;
using DogHouseAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using DogHouseAPI.Models;

namespace DogHouseAPI.Tests;

public class ControllerTests
{
    private DogHouseController _controller;
    private Mock<IDogHouseService> _dogHouseServiceMock;
    private Mock<ILogger<DogHouseController>> _loggerMock;
    private Mock<IOptions<AppInfoOptions>> _appInfoOptionsMock;

    [SetUp]
    public void Setup()
    {
        _dogHouseServiceMock = new Mock<IDogHouseService>();
        _loggerMock = new Mock<ILogger<DogHouseController>>();
        _appInfoOptionsMock = new Mock<IOptions<AppInfoOptions>>();

        _appInfoOptionsMock.Setup(o => o.Value).Returns(new AppInfoOptions { Version = "1.0.0" });

        _controller = new DogHouseController(
            _dogHouseServiceMock.Object,
            _loggerMock.Object,
            _appInfoOptionsMock.Object
        );
    }

    #region Ping Tests

    [Test]
    public void Ping_ReturnsVersionInfo()
    {
        // Act
        var result = _controller.Ping();
        
        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult?.Value, Is.EqualTo("1.0.0"));
    }

    #endregion
        
    #region AddDog Tests
    [Test]
    public async Task AddDog_ReturnsCreatedDog_WhenSuccessful()
    {
        // Arrange
        var createDto = new CreateDogDto { Name = "Buddy" };
        var createdDog = new Dog { Id = 1, Name = "Buddy" };
        _dogHouseServiceMock
            .Setup(s => s.AddDog(createDto))
            .ReturnsAsync(createdDog);

        // Act
        var result = await _controller.AddDog(createDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
        var createdAtActionResult = result.Result as CreatedAtActionResult;
        Assert.That(createdAtActionResult.Value, Is.InstanceOf<Dog>());
        var returnedDog = createdAtActionResult.Value as Dog;

        Assert.That(returnedDog.Id, Is.EqualTo(createdDog.Id));
        Assert.That(returnedDog.Name, Is.EqualTo(createdDog.Name));

    }

    [Test]
    public async Task AddDog_ThrowsBadRequest_WhenDogIsNull()
    {
        // Act
        var result = await _controller.AddDog(null);
     
        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.That(badRequestResult.Value.ToString(), Does.Contain("Dog data is null."));
    }

    [Test]
    public async Task AddDog_Throws_WhenDogExists()
    {
        // Arrange
        var createDto = new CreateDogDto { Name = "Buddy" };
        string exceptionMessage = "Dog with name Buddy already exists.";

        _dogHouseServiceMock
            .Setup(s => s.AddDog(createDto))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _controller.AddDog(createDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.That(badRequestResult.Value.ToString(), Does.Contain(exceptionMessage));
    }

    #endregion

    #region GetDogs Tests
    [Test]
    public async Task GetDogs_ReturnsDogList_WhenSuccessful()
    {
        // Arrange
        var searchParams = new DogSearchAttributesDto();
        var dogList = new List<DogResponseDto>
        {
            new DogResponseDto { Name = "Buddy" },
            new DogResponseDto { Name = "Max" }
        };

        _dogHouseServiceMock
            .Setup(s => s.Get(searchParams))
            .ReturnsAsync(dogList);

        // Act
        var result = await _controller.GetDogs(searchParams);

        // Assert
        var okResult = result.Result as OkObjectResult;
        var returnedDogs = okResult.Value as IEnumerable<DogResponseDto>;

        Assert.That(result, Is.Not.Null);
        Assert.That(returnedDogs.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetDogs_ThrowsException_WhenServiceFails()
    {
        // Arrange
        var searchParams = new DogSearchAttributesDto();
        var exception = new Exception("Service failure");

        _dogHouseServiceMock
            .Setup(s => s.Get(searchParams))
            .ThrowsAsync(exception);

        // Act
        var result = await _controller.GetDogs(searchParams);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
        var objectResult = result.Result as ObjectResult;

        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        Assert.That(objectResult.Value.ToString(), Does.Contain("Service failure"));
    }

    #endregion
}
