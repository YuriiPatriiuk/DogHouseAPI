using System.ComponentModel.DataAnnotations;

namespace DogHouseAPI.Models.DTO
{
    public class CreateDogDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Color { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue)]
        public float TailLength { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public float Weight { get; set; }
    }
}
