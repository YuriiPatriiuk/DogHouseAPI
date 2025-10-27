using DogHouseAPI.API;
using DogHouseAPI.Models;
using DogHouseAPI.Models.DTO;
using DogHouseAPI.Services.DogHouseService;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection.Metadata;

namespace DogHouseAPI.Tests;

public class ServiceTests
{
    private DogHouseService _dogHouseService;
    private Mock<IDogHouseRepository> _dogRepositoryMock;
    private Mock<ILogger<DogHouseService>> _loggerMock;

    [SetUp]
    public void Setup()
    {
        _dogRepositoryMock = new Mock<IDogHouseRepository>();
        _loggerMock = new Mock<ILogger<DogHouseService>>();

        _dogHouseService = new DogHouseService(
            _dogRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    #region Get Tests

    [Test]
    public async Task Get_ReturnsDogList_WhenSuccessful()
    {
        // Arrange
        var searchParams = new DogSearchAttributesDto
        {
            PageNumber = 2,
            PageSize = 5,
            Attribute = "name",
            Order = "asc"
        };

        _dogRepositoryMock
            .Setup(r => r.GetAllAsync(searchParams))
            .ReturnsAsync(new List<Dog>());

        // Act
        var result = await _dogHouseService.Get(searchParams);

        // Assert
        _dogRepositoryMock.Verify(r => r.GetAllAsync(
            It.Is<DogSearchAttributesDto>(a => 
            a.PageNumber == 2 && a.PageSize == 5 && a.Attribute == "name" && a.Order == "asc")), Times.Once);
    }

    [Test]
    [TestCase(0, 1)]
    [TestCase(5, 0)]
    public void Get_ThrowsException_WhenPaginationInvalid(int pageSize, int pageNumber) 
    {
        // Arrange
        var searchParams = new DogSearchAttributesDto { PageSize = pageSize, PageNumber = pageNumber }; 
        
        // Act
        var result = Assert.ThrowsAsync<Exception>(async () => await _dogHouseService.Get(searchParams));

        //Assert
        Assert.That(result.Message, Does.Contain("PageNumber and PageSize must be greater than zero."));
    }

    [Test]
    public void Get_ThrowsException_WhenAttributeInvalid()
    {
        // Arrange
        string invalidAttribute = "InvalidAttribute";
        var searchParams = new DogSearchAttributesDto { Attribute = invalidAttribute };

        // Act
        var result = Assert.ThrowsAsync<Exception>(async () => await _dogHouseService.Get(searchParams));

        //Assert
        Assert.That(result.Message, Does.Contain($"Invalid attribute: {invalidAttribute}."));
    }

    [Test]
    public void Get_ThrowsException_WhenOrderInvalid()
    {
        // Arrange
        string invalidOrder = "InvalidOrder";
        var searchParams = new DogSearchAttributesDto { Order = invalidOrder };

        // Act
        var result = Assert.ThrowsAsync<Exception>(async () => await _dogHouseService.Get(searchParams));

        //Assert
        Assert.That(result.Message, Does.Contain($"Invalid order: {invalidOrder}."));
    }

    [Test]
    public void Get_ThrowsException_WhenRepositoryFails()
    {
        // Arrange
        var searchParams = new DogSearchAttributesDto();

        _dogRepositoryMock
            .Setup(r => r.GetAllAsync(searchParams))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = Assert.ThrowsAsync<Exception>(async () => await _dogHouseService.Get(searchParams));

        // Assert
        Assert.That(result.Message, Does.Contain("Database error"));
    }

    #endregion

    #region AddDog Tests
    [Test]
    public async Task AddDog_ReturnsAddedDog_WhenSuccessful()
    {
        // Arrange
        var createDto = new CreateDogDto
        {
            Name = "Buddy",
            Color = "Brown",
            TailLength = 10,
            Weight = 20
        };

        var addedDog = new Dog
        {
            Id = 1,
            Name = "Buddy",
            Color = "Brown",
            TailLength = 10,
            Weight = 20
        };

        _dogRepositoryMock
            .Setup(r => r.AddDogAsync(It.IsAny<Dog>()))
            .ReturnsAsync(addedDog);

        // Act
        var result = await _dogHouseService.AddDog(createDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<DogResponseDto>());

        _dogRepositoryMock.Verify(r => r.AddDogAsync(
        It.Is<Dog>(d =>
            d.Name == createDto.Name &&
            d.Color == createDto.Color &&
            d.TailLength == createDto.TailLength &&
            d.Weight == createDto.Weight
        )),
        Times.Once);
    }

    [Test]
    public void AddDog_Throws_WhenDogExists()
    {
        // Arrange
        var createDto = new CreateDogDto { Name = "Buddy" };
        string exceptionMessage = $"Dog with name {createDto.Name} already exists.";

        _dogRepositoryMock
            .Setup(r => r.IsDogExist(createDto.Name))
            .ReturnsAsync(true);

        // Act
        var result = Assert.ThrowsAsync<Exception>(async () => await _dogHouseService.AddDog(createDto));

        // Assert
        Assert.That(result.Message, Does.Contain(exceptionMessage));
    }

    [Test]
    [TestCase(-5, 20)]
    [TestCase(10, -1)]
    public void AddDog_Throws_WhenParametersInvalid(int tailLength, float weight)
    {
        // Arrange
        var createDto = new CreateDogDto
        {
            Name = "Buddy",
            Color = "Brown",
            TailLength = tailLength,
            Weight = weight
        };
        string exceptionMessage = "Dog parameters are not valid.";
        _dogRepositoryMock
            .Setup(r => r.IsDogExist(createDto.Name))
            .ReturnsAsync(false);

        // Act
        var result = Assert.ThrowsAsync<Exception>(async () => await _dogHouseService.AddDog(createDto));

        // Assert
        Assert.That(result.Message, Does.Contain(exceptionMessage));
    }

    #endregion
}
