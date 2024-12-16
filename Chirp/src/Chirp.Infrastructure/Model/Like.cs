namespace Chirp.Infrastructure.Model
{
    public class Like
    {
        public int Id { get; set; }
        public required Author Author { get; set; }
        public required Cheep Cheep { get; set; }
    }
}