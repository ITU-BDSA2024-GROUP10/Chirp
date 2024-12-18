using System.Collections;
using Chirp.Core.DTO;

namespace Chirp.Core;

public interface ICheepRepository
{
    /// <summary>
    /// Gets all Cheeps on the given page by the given page size
    /// </summary>
    /// <param name="page">if it's less than or equal to zero, it's treated as 1</param>
    /// <param name="pageSize">how many cheeps are on a page. Must be greater than zero</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="pageSize"/> is less than one</exception>
    public Task<IEnumerable<CheepDTO>?> GetCheepsByPage(int page, int pageSize);

    /// <summary>
    /// Gets all Cheeps made by the author with the given username
    /// </summary>
    /// <param name="username"></param>
    public Task<IEnumerable<CheepDTO>> GetCheepsFromAuthor(string username);

    /// <summary>
    /// Gets Cheeps made by some authors with one of the given usernames by the given page and page size
    /// </summary>
    /// <param name="usernames"></param>
    /// <param name="page">if it's less than or equal to zero, it's treated as 1</param>
    /// <param name="pageSize">how many cheeps are on a page. Must be greater than zero</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="pageSize"/> is less than one</exception>
    public Task<IEnumerable<CheepDTO>>
        GetCheepsFromAuthorsByPage(IEnumerable<string> usernames, int page, int pageSize);

    /// <summary>
    /// Creates a new Cheep from the given CheepDTO
    /// </summary>
    /// <param name="cheep"></param>
    /// <returns>True if a cheep was successfully created,
    /// false if the author said to have created the cheep doesn't exist</returns>
    public Task<bool> CreateCheep(CheepDTO cheep);

    /// <summary>
    /// Gets the amount of Cheeps in the Database 
    /// </summary>
    /// <returns></returns>
    public Task<int> GetAmountOfCheeps();

    /// <summary>
    /// Gets the amount Cheeps made by the Authors by the given usernames
    /// </summary>
    /// <param name="usernames"></param>
    public Task<int> GetAmountOfCheepsFromAuthors(IEnumerable<string> usernames);

    /// <summary>
    /// Creates a new comment from the given CommentDTO
    /// </summary>
    /// <param name="comment"></param>
    /// <returns>True if a comment was successfully created,
    /// false if either the author said to have created the comment,
    /// or the cheep said to have been commented, doesn't exist</returns>
    public Task<bool> CreateComment(CommentDTO comment);

    /// <summary>
    /// Gets the amount of Comments associated with the cheep that has the given CheepId
    /// </summary>
    /// <param name="cheepId"></param>
    /// <returns>If <paramref name="cheepId"/> is null, 0 is returned</returns>
    public Task<int> GetCommentAmountOnCheep(int? cheepId);

    /// <summary>
    /// Gets the Cheep associated with the given CheepId
    /// </summary>
    /// <param name="cheepId"></param>
    public Task<CheepDTO> GetCheepById(int cheepId);

    /// <summary>
    /// Gets all Comments associated with the cheep that has the given CheepId
    /// </summary>
    /// <param name="cheepId"></param>
    public Task<IEnumerable<CommentDTO>> GetCommentsForCheep(int cheepId);

    /// <summary>
    /// Makes an author like some Cheep, described by the LikeDTO
    /// </summary>
    /// <param name="like"></param>
    /// <returns>True if the like went through,
    /// false if the author said to have liked the cheep,
    /// or if the cheep said to have been liked doesn't exist</returns>
    public Task<bool> LikeCheep(LikeDTO like);

    /// <summary>
    /// Makes an author unlike some Cheep, described by the LikeDTO
    /// </summary>
    /// <param name="like"></param>
    /// <returns>True if the unlike went through,
    /// false if the cheep said to have been unliked doesn't exist,
    /// or if the cheep isn't liked by the author it is said to be liked by</returns>
    public Task<bool> UnlikeCheep(LikeDTO like);

    /// <summary>
    /// Gets how many Likes are associated with some Cheep given the CheepId
    /// </summary>
    /// <param name="cheepId"></param>
    /// <returns>0 if there isn't a cheep associated with the given <paramref name="cheepId"/></returns>
    Task<int> GetLikeCount(int cheepId);

    /// <summary>
    /// Returns whether an author with the given username has liked the cheep with the given CheepId
    /// </summary>
    /// <param name="cheepId"></param>
    /// <param name="username"></param>
    Task<bool> HasUserLikedCheep(int cheepId, string username);
}