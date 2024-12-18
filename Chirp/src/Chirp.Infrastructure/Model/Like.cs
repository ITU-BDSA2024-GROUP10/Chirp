namespace Chirp.Infrastructure.Model
{
    /// <summary>
    /// The Like class is used to associate likes given by Authors to Cheeps
    /// </summary>
    public class Like
    {
        public int Id { get; set; }
        public required Author Author { get; set; }
        public required Cheep Cheep { get; set; }
    }
}