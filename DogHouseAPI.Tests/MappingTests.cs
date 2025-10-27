using DogHouseAPI.Models;
using DogHouseAPI.Models.DTO;
using DogHouseAPI.Mapping;

namespace DogHouseAPI.Tests;

public class MappingTests
{
    [SetUp]
    public void Setup()
    {}

    [Test]
    public void ToDto_ReturnsDogResponseDto_WhenSuccessful()
    {
        // Arrange
        var dog = new Dog
        {
            Id = 1,
            Name = "Oliver",
            Color = "Yellow",
            TailLength = 14,
            Weight = 99
        };

        // Act
        var result = dog.ToDto();

        // Assert
        Assert.That(result, Is.TypeOf<DogResponseDto>());
        Assert.That(result.Name, Is.EqualTo(dog.Name));
        Assert.That(result.Color, Is.EqualTo(dog.Color));
        Assert.That(result.TailLength, Is.EqualTo(dog.TailLength));
        Assert.That(result.Weight, Is.EqualTo(dog.Weight));
    }

    [Test]
    public void ToDog_ReturnsDog_WhenSuccessful()
    {
        //Arrange
        var createDogDto = new CreateDogDto
        {
            Name = "Saruman",
            Color = "The White",
            TailLength = 20,
            Weight = 50
        };

        // Act
        var result = createDogDto.ToDog();

        // Assert
        Assert.That(result, Is.TypeOf<Dog>());
        Assert.That(result.Id, Is.EqualTo(0));
        Assert.That(result.Name, Is.EqualTo(createDogDto.Name));
        Assert.That(result.Color, Is.EqualTo(createDogDto.Color));
        Assert.That(result.TailLength, Is.EqualTo(createDogDto.TailLength));
        Assert.That(result.Weight, Is.EqualTo(createDogDto.Weight));
    }

    [Test]
    public void ToDto_ReturnsEnumerableOfDogResponseDto_WhenSuccessful()
    {
        // Arrange
        var dogs = new List<Dog>
        {
            new Dog { Id = 1, Name = "John Snow", Color = "White", TailLength = 18, Weight = 65 },
            new Dog { Id = 2, Name = "Daenerys Targarien", Color = "Silver", TailLength = 20, Weight = 55 }, 
            new Dog { Id = 3, Name = "Tyrion Lannister", Color = "Brown", TailLength = 10, Weight = 30 }     
        };

        // Act
        var result = dogs.ToDto().ToList();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(dogs.Count));
        for (int i = 0; i < dogs.Count; i++)
        {
            Assert.That(result.ElementAt(i).Name, Is.EqualTo(dogs[i].Name));
            Assert.That(result.ElementAt(i).Color, Is.EqualTo(dogs[i].Color));
            Assert.That(result.ElementAt(i).TailLength, Is.EqualTo(dogs[i].TailLength));
            Assert.That(result.ElementAt(i).Weight, Is.EqualTo(dogs[i].Weight));
        }
    }
}
