using FluentValidation;

namespace Chirp.Infrastructure.Model;

public class Cheep
{
    public int Id { get; set;  } 
    public required string Message { get; set; } 
    public DateTime TimeStamp { get; set; } 
    public required Author Author { get; set; }

    public List<Comment> Comments { get; set; } = [];
    public Cheep()
    {
        
    }

    public Cheep(int id, string message, DateTime timeStamp, Author author)
    {
        Id = id;
        Message = message;
        TimeStamp = timeStamp;
        Author = author;
    }

    public class CheepValidator : AbstractValidator<Cheep>
    {
        public CheepValidator()
        {
            RuleFor(x => x.Message).MaximumLength(160);
        }
    }
}