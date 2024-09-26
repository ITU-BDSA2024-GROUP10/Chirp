namespace SimpleDB;

public interface IDatabaseRepository<T>
{
    public IEnumerable<T> GetAll();
    public IEnumerable<T> GetFromAuthor(int authorId);
}
