namespace SimpleDB;

public interface IDatabaseRepository<T>
{
    public IEnumerable<T> GetByPage(int page, int pageSize);
    public IEnumerable<T> GetFromAuthorByPage(String author, int page, int pageSize);
}
