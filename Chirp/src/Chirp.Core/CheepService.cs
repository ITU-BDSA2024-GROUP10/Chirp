using Chirp.Core.DTO;

namespace Chirp.Core;
/// <summary>
/// The ICheepService Interface describes the Methods the CheepService class must implement 
/// </summary>
public interface ICheepService
{
    public IEnumerable<CheepDTO> GetCheepsByPage(int page, int pageSize);
    public IEnumerable<CheepDTO> GetCheepsFromAuthorByPage(string author, int page, int pageSize);
    public IEnumerable<CheepDTO> GetCheepsFromAuthorsByPage(IEnumerable<string> authors, int page, int pageSize);
    public IEnumerable<CheepDTO> GetCheepsFromAuthor(string author);
    public int GetAmountOfCheepPages(int pageSize);
    public int GetAmountOfCheepPagesFromAuthors(IEnumerable<String> authors, int pageSize);
    public bool CreateCheep(CheepDTO cheep);
    public bool AddCommentToCheep(CommentDTO comment);
    public int GetCommentAmountOnCheep(int? cheepId);
    public CheepDTO GetCheepFromId(int cheepId);
    public IEnumerable<CommentDTO> GetCommentsFromCheep(int cheepId);
    public bool LikeCheep(LikeDTO like);
    public bool UnlikeCheep(LikeDTO like);
    public Task<int> GetLikeCount(int cheepId);
    public Task<bool> HasUserLikedCheep(int cheepId, string author);
}
/// <summary>
/// The CheepService class handles calls to the CheepRepository from the UI 
/// </summary>
/// <param name="db"></param>
public class CheepService(ICheepRepository db) : ICheepService
{
    /// <summary>
    /// Gets all Cheeps on the given page by the given page size
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    public IEnumerable<CheepDTO> GetCheepsByPage(int page, int pageSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(pageSize);

        var result = db.GetCheepsByPage(page, pageSize).Result ??
                     throw new ArgumentNullException(nameof(db.GetCheepsByPage));

        return result.ToList();
    }
    /// <summary>
    /// Gets all Cheeps made by some author with the given Name
    /// </summary>
    /// <param name="author"></param>
    public IEnumerable<CheepDTO> GetCheepsFromAuthor(string author)
    {
        return db.GetCheepsFromAuthor(author).Result.ToList();
    }
    /// <summary>
    /// Gets Cheeps made by some author with the given Name by the given page and page size
    /// </summary>
    /// <param name="author"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    public IEnumerable<CheepDTO> GetCheepsFromAuthorByPage(string author, int page, int pageSize)
    {
        return db.GetCheepsFromAuthorByPage(author, page, pageSize).Result.ToList();
    }
    /// <summary>
    /// Gets Cheeps made by some authors with the given Names by the given page and page size
    /// </summary>
    /// <param name="authors"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    public IEnumerable<CheepDTO> GetCheepsFromAuthorsByPage(IEnumerable<string> authors, int page, int pageSize)
    {
        return db.GetCheepsFromAuthorsByPage(authors, page, pageSize).Result.ToList();
    }
    /// <summary>
    /// Gets the amount of pages that are needed given some page size 
    /// </summary>
    /// <param name="pageSize"></param>
    public int GetAmountOfCheepPages(int pageSize)
    {
        return (int)Math.Ceiling(db.GetAmountOfCheeps().Result / (double)pageSize);
    }
    /// <summary>
    /// Gets the amount of pages that are needed for an author given some page size
    /// </summary>
    /// <param name="pageSize"></param>
    public int GetAmountOfCheepPagesFromAuthors(IEnumerable<String> authors, int pageSize)
    {
        return (int)Math.Ceiling(db.GetAmountOfCheepsFromAuthors(authors).Result / (double)pageSize);
    }
    /// <summary>
    /// Creates a new Cheep from the given CheepDTO
    /// </summary>
    /// <param name="cheep"></param>
    public bool CreateCheep(CheepDTO cheep)
    {
        return db.CreateCheep(cheep).Result;
    }
    /// <summary>
    /// Adds a Comment to some Cheep given the CommentDTO
    /// </summary>
    /// <param name="comment"></param>
    public bool AddCommentToCheep(CommentDTO comment)
    {
        return db.AddCommentToCheep(comment).Result;
    }
    /// <summary>
    /// Gets the amount of Comments associated with the given CheepID
    /// </summary>
    /// <param name="cheepId"></param>
    public int GetCommentAmountOnCheep(int? cheepId)
    {
        return db.GetCommentAmountOnCheep(cheepId).Result;
    }
    /// <summary>
    /// Gets the Cheep associated with the given CheepID
    /// </summary>
    /// <param name="cheepId"></param>
    public CheepDTO GetCheepFromId(int cheepId)
    {
        return db.GetCheepById(cheepId).Result;
    }
    /// <summary>
    /// Gets all Comments associated with the given CheepID
    /// </summary>
    /// <param name="cheepId"></param>
    public IEnumerable<CommentDTO> GetCommentsFromCheep(int cheepId)
    {
        return db.GetCommentsForCheep(cheepId).Result.ToList();
    }
    /// <summary>
    /// Likes some Cheep given the LikeDTO
    /// </summary>
    /// <param name="like"></param>
    public bool LikeCheep(LikeDTO like)
    {
        return db.LikeCheep(like).Result;
    }
    /// <summary>
    /// Unlikes some Cheep given the LikeDTO
    /// </summary>
    /// <param name="like"></param>
    public bool UnlikeCheep(LikeDTO like)
    {
        return db.UnlikeCheep(like).Result;
    }
    
    /// <summary>
    /// Gets how many Likes are associated with some Cheep given the CheepID
    /// </summary>
    /// <param name="cheepId"></param>
    public async Task<int> GetLikeCount(int cheepId)
    {
        return await db.GetLikeCount(cheepId);
    }
    /// <summary>
    /// Returns if a User with the given Name has likes the given Cheep
    /// </summary>
    /// <param name="cheepId"></param>
    /// <param name="author"></param>
    public async Task<bool> HasUserLikedCheep(int cheepId, string author)
    {
        return await db.HasUserLikedCheep(cheepId, author);
    }
}