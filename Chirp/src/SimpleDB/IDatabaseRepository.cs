namespace SimpleDB;

public interface IDatabaseRepository<T>
{
    public IEnumerable<T> GetAll();
    public IEnumerable<T> GetByPage(int page, int pageSize);
    public IEnumerable<T> GetFromAuthor(String author);
}
