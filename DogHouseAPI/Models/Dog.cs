namespace DogHouseAPI.Models
{
    public class Dog
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public float TailLength { get; set; }
        public float Weight { get; set; }   

    }
}
