using DogHouseAPI.API;
using DogHouseAPI.Models;
using DogHouseAPI.Models.Database;
using DogHouseAPI.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace DogHouseAPI.Tests;

public class RepositoryTests
{
    private DogHouseRepository _dogHouseRepository;
    private DogHouseDbContext _dogHouseDbContext;
    private IReadOnlyCollection<Dog> _dogs;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<DogHouseDbContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;

        _dogHouseDbContext = new DogHouseDbContext(options);

        _dogs = new List<Dog> {
            new Dog { Id = 1, Name = "Buddy", Color = "Yellow", TailLength = 14, Weight = 55},
            new Dog { Id = 2, Name = "Rex", Color = "Black", TailLength = 23, Weight = 85},
            new Dog { Id = 3, Name = "Rudy", Color = "Red", TailLength = 7, Weight = 13},
            new Dog { Id = 4, Name = "Hans", Color = "White", TailLength = 2, Weight = 47},
            new Dog { Id = 5, Name = "Richie", Color = "Mixed", TailLength = 18, Weight = 33}
        };

        _dogHouseDbContext.Dogs.AddRange(_dogs);
        _dogHouseDbContext.SaveChanges();

        _dogHouseRepository = new DogHouseRepository(_dogHouseDbContext);

    }

    [TearDown]
    public void Dispose()
    {
        _dogHouseDbContext.Dispose();
    }

    #region AddDog Tests
    [Test]
    public async Task AddDog_ReturnsDog_WhenSuccessful()
    {
        // Arrange
        var newDog = new Dog { Name = "John Snow", Color = "White", TailLength = 55, Weight = 23 };
        var startDogsNumber = _dogHouseDbContext.Dogs.Count();

        // Act
        var result = await _dogHouseRepository.AddDogAsync(newDog);

        //Assert
        Assert.That(result.Id, Is.GreaterThan(0));
        var dogInDb = await _dogHouseDbContext.Dogs.FindAsync(result.Id);
        Assert.That(result, Is.Not.Null);
        Assert.That(dogInDb, Is.EqualTo(result));
        Assert.That(_dogHouseDbContext.Dogs.Count(), Is.EqualTo(startDogsNumber + 1));
        
    }

    [Test]
    public async Task IsDogExist_ReturnsTrue_WhenDogExists()
    {
        // Arrange
        var existingDogName = _dogHouseDbContext.Dogs.First().Name;

        // Act
        var result = await _dogHouseRepository.IsDogExist(existingDogName);

        // Assert
        Assert.That(result, Is.True);
    }
    #endregion

    #region GetAll Tests
    [Test]
    public async Task GetAllAsync_ReturnsDogs_WithNoFilter()
    {
        // Arrange
        var filters = new DogSearchAttributesDto();

        // Act
        var result = await _dogHouseRepository.GetAllAsync(filters);

        // Assert
        Assert.That(_dogs, Is.EqualTo(result));
    }

   

    [Test]
    [TestCase("name", "asc", 1, 2, "Buddy", "Hans")]        // Сортування за іменем, 1-а сторінка, 2 собаки
    [TestCase("name", "asc", 2, 2, "Rex", "Richie")]      // Сортування за іменем, 2-а сторінка, 1 собака
    [TestCase("weight", "desc", 1, 1, "Rex", "")]   // Сортування за вагою, 1-а сторінка, 1 собака
    [TestCase("weight", "desc", 2, 1, "Buddy", "")] // Сортування за вагою, 2-а сторінка, 1 собака
    [TestCase("tailLength", "asc", 1, 3, "Hans", "Rudy")] // Сортування за хвостом, 1-а сторінка, 3 собаки
    public async Task GetAllAsync_ReturnsDogs_WithSortingAndPagination(string attribute, string order, int pageNumber, int pageSize, string expectedFirstName, string expectedSecondName)
    {
        // Arrange
        var parameters = new DogSearchAttributesDto
        {
            Attribute = attribute,
            Order = order,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        // Act
        var result = await _dogHouseRepository.GetAllAsync(parameters);
        var resultList = result.ToList();

        // Assert
        Assert.That(resultList.First().Name, Is.EqualTo(expectedFirstName));

        if (!expectedSecondName.IsNullOrEmpty())
        {
            Assert.That(resultList.Count, Is.GreaterThanOrEqualTo(2));
            Assert.That(resultList[1].Name, Is.EqualTo(expectedSecondName));
        }
        else
            Assert.That(resultList.Count, Is.EqualTo(1));
    }

    #endregion
}
