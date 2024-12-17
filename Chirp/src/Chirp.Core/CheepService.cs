using Chirp.Core.DTO;

namespace Chirp.Core;

/// <summary>
/// The ICheepService Interface describes what information you can get from a Cheep
/// </summary>
public interface ICheepService
{
    /// <summary>
    /// Gets all Cheeps on the given page by the given page size
    /// </summary>
    /// <param name="page">if it's less than or equal to zero, It's treated as 1</param>
    /// <param name="pageSize">how many cheeps are on a page. Must be greater than zero</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="pageSize"/> less than one</exception>
    public IEnumerable<CheepDTO> GetCheepsByPage(int page, int pageSize);

    /// <summary>
    /// Gets Cheeps made by the authors with one of the given usernames by the given page and page size
    /// </summary>
    /// <param name="usernames"></param>
    /// <param name="page">if it's less than or equal to zero, It's treated as 1</param>
    /// <param name="pageSize">how many cheeps are on a page. Must be greater than zero</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="pageSize"/> less than one</exception>
    public IEnumerable<CheepDTO> GetCheepsFromAuthorsByPage(IEnumerable<string> usernames, int page, int pageSize);

    /// <summary>
    /// Gets all Cheeps made by the author with the given username
    /// </summary>
    /// <param name="username"></param>
    public IEnumerable<CheepDTO> GetCheepsFromAuthor(string username);

    /// <summary>
    /// Gets the amount of pages that are needed to display all the cheeps in the database,
    /// given the page size
    /// </summary>
    /// <param name="pageSize">how many cheeps are on a page. Must be greater than zero</param>
    /// <returns>The result is rounded to the next larger int</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="pageSize"/> less than one</exception>
    public int GetAmountOfCheepPages(int pageSize);

    /// <summary>
    /// Gets the amount of pages that are needed author to display all Cheeps made by the given authors,
    /// given the page size
    /// </summary>
    /// <param name="usernames">the usernames of the authors</param>
    /// <param name="pageSize">how many cheeps are on a page. Must be greater than zero</param>
    /// <returns>the result is rounded to the next larger int</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="pageSize"/> less than one</exception>
    public int GetAmountOfCheepPagesFromAuthors(IEnumerable<string> usernames, int pageSize);

    /// <summary>
    /// Creates a new Cheep from the given CheepDTO
    /// </summary>
    /// <param name="cheep"></param>
    /// <returns>True if a cheep was successfully created,
    /// false if the author said to have created the cheep doesn't exist</returns>
    public bool CreateCheep(CheepDTO cheep);

    /// <summary>
    /// Creates a new comment from the given CommentDTO
    /// </summary>
    /// <param name="comment"></param>
    /// <returns>True if a comment vas successfully created,
    /// false if either the author said to have created the comment,
    /// or the cheep said to have been commented, doesn't exist</returns>
    public bool CreateComment(CommentDTO comment);

    /// <summary>
    /// Gets the amount of Comments associated with the cheep that has the given CheepID
    /// </summary>
    /// <param name="cheepId"></param>
    /// <returns>If <paramref name="cheepId"/> is null, 0 is returned</returns>
    public int GetCommentAmountOnCheep(int? cheepId);

    /// <summary>
    /// Gets the Cheep associated with the given CheepID
    /// </summary>
    /// <param name="cheepId"></param>
    public CheepDTO GetCheepFromId(int cheepId);

    /// <summary>
    /// Gets all Comments associated with the cheep that has the given CheepID
    /// </summary>
    /// <param name="cheepId"></param>
    public IEnumerable<CommentDTO> GetCommentsFromCheep(int cheepId);

    /// <summary>
    /// Makes an author like some Cheep, described by the LikeDTO
    /// </summary>
    /// <param name="like"></param>
    /// <returns>True if the like went through,
    /// false if the author said to have liked the cheep,
    /// or if the cheep said to have been liked doesn't exist</returns>
    public bool LikeCheep(LikeDTO like);

    /// <summary>
    /// Makes an author unlike some Cheep, described by the LikeDTO
    /// </summary>
    /// <param name="like"></param>
    /// <returns>True if the unlike went through,
    /// false if the cheep said to have been unliked doesn't exist,
    /// or if the cheep isn't liked by the author it is said to be liked by</returns>
    public bool UnlikeCheep(LikeDTO like);

    /// <summary>
    /// Gets how many Likes are associated with some Cheep given the CheepID
    /// </summary>
    /// <param name="cheepId"></param>
    /// <returns>0 if there isn't a cheep associated with the given <paramref name="cheepId"/></returns>
    public Task<int> GetLikeCount(int cheepId);

    /// <summary>
    /// Returns whether an author with the given usernames has liked the cheep with the given CheepID
    /// </summary>
    /// <param name="cheepId"></param>
    /// <param name="username"></param>
    public Task<bool> HasUserLikedCheep(int cheepId, string username);
}

/// <summary>
/// The CheepService class handles calls to the CheepRepository from the UI 
/// </summary>
/// <param name="db"></param>
public class CheepService(ICheepRepository db) : ICheepService
{
    public IEnumerable<CheepDTO> GetCheepsByPage(int page, int pageSize)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);

        var result = db.GetCheepsByPage(page, pageSize).Result ??
                     throw new ArgumentNullException(nameof(db.GetCheepsByPage));

        return result.ToList();
    }

    public IEnumerable<CheepDTO> GetCheepsFromAuthor(string username)
    {
        return db.GetCheepsFromAuthor(username).Result.ToList();
    }

    public IEnumerable<CheepDTO> GetCheepsFromAuthorsByPage(IEnumerable<string> usernames, int page, int pageSize)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        return db.GetCheepsFromAuthorsByPage(usernames, page, pageSize).Result.ToList();
    }

    public int GetAmountOfCheepPages(int pageSize)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);

        return (int)Math.Ceiling(db.GetAmountOfCheeps().Result / (double)pageSize);
    }

    public int GetAmountOfCheepPagesFromAuthors(IEnumerable<String> usernames, int pageSize)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);

        return (int)Math.Ceiling(db.GetAmountOfCheepsFromAuthors(usernames).Result / (double)pageSize);
    }

    public bool CreateCheep(CheepDTO cheep)
    {
        return db.CreateCheep(cheep).Result;
    }

    public bool CreateComment(CommentDTO comment)
    {
        return db.CreateComment(comment).Result;
    }

    public int GetCommentAmountOnCheep(int? cheepId)
    {
        return db.GetCommentAmountOnCheep(cheepId).Result;
    }

    public CheepDTO GetCheepFromId(int cheepId)
    {
        return db.GetCheepById(cheepId).Result;
    }

    public IEnumerable<CommentDTO> GetCommentsFromCheep(int cheepId)
    {
        return db.GetCommentsForCheep(cheepId).Result;
    }

    public bool LikeCheep(LikeDTO like)
    {
        return db.LikeCheep(like).Result;
    }

    public bool UnlikeCheep(LikeDTO like)
    {
        return db.UnlikeCheep(like).Result;
    }

    public async Task<int> GetLikeCount(int cheepId)
    {
        return await db.GetLikeCount(cheepId);
    }

    public async Task<bool> HasUserLikedCheep(int cheepId, string username)
    {
        return await db.HasUserLikedCheep(cheepId, username);
    }
}