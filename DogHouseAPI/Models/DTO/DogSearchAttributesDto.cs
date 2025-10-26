namespace DogHouseAPI.Models.DTO
{
    public class DogSearchAttributesDto
    {
        public string? Attribute { get; set; }
        public string? Order { get; set; }
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;

    }
}
