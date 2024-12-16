using System.Collections;
using Chirp.Core.DTO;

namespace Chirp.Core;

public interface ICheepRepository
{
    /// <summary>
    /// Gets all Cheeps on the given page by the given page size
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    public Task<IEnumerable<CheepDTO>?> GetCheepsByPage(int page, int pageSize);
    /// <summary>
    /// Gets Cheeps made by some author with the given Name by the given page and page size
    /// </summary>
    /// <param name="author"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    public Task<IEnumerable<CheepDTO>> GetCheepsFromAuthorByPage(String author, int page, int pageSize);
    /// <summary>
    /// Gets all Cheeps made by some author with the given Name
    /// </summary>
    /// <param name="author"></param>
    public Task<IEnumerable<CheepDTO>> GetCheepsFromAuthor(String author);
    /// <summary>
    /// Gets Cheeps made by some authors with the given Names by the given page and page size
    /// </summary>
    /// <param name="authors"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    public Task<IEnumerable<CheepDTO>> GetCheepsFromAuthorsByPage(IEnumerable<String> authors, int page, int pageSize);
    /// <summary>
    /// Returns all Cheeps liked by the given Author given the current page and page size 
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    public Task<IEnumerable<CheepDTO>> GetCheepsWithLikesByPage(string userName, int page, int pageSize);
    /// <summary>
    /// Creates a new Cheep from the given CheepDTO
    /// </summary>
    /// <param name="cheep"></param>
    public Task<bool> CreateCheep(CheepDTO cheep);
    /// <summary>
    /// Gets the amount of Cheeps in the Database 
    /// </summary>
    /// <returns></returns>
    public Task<int> GetAmountOfCheeps();
    /// <summary>
    /// Gets the amount Cheeps made by the Authors with the given Names
    /// </summary>
    public Task<int> GetAmountOfCheepsFromAuthors(IEnumerable<String> authors);
    /// <summary>
    /// Adds a Comment to some Cheep given the CommentDTO
    /// </summary>
    /// <param name="comment"></param>
    public Task<bool> AddCommentToCheep(CommentDTO comment);
    /// <summary>
    /// Gets the amount of Comments associated with the given CheepID
    /// </summary>
    /// <param name="cheepId"></param>
    public Task<int> GetCommentAmountOnCheep(int? cheepId);
    /// <summary>
    /// Gets the Cheep associated with the given CheepID
    /// </summary>
    /// <param name="cheepId"></param>
    public Task<CheepDTO> GetCheepById(int cheepId);
    /// <summary>
    /// Gets all Comments associated with the given CheepID
    /// </summary>
    /// <param name="cheepId"></param>
    public Task<IEnumerable<CommentDTO>> GetCommentsForCheep(int cheepId);
    /// <summary>
    /// Likes some Cheep given the LikeDTO
    /// </summary>
    /// <param name="like"></param>
    public Task<bool> LikeCheep(LikeDTO like);
    /// <summary>
    /// Unlikes some Cheep given the LikeDTO
    /// </summary>
    public Task<bool> UnlikeCheep(LikeDTO like);
    /// <summary>
    /// Gets how many Likes are associated with some Cheep given the CheepID
    /// </summary>
    /// <param name="cheepId"></param>
    Task<int> GetLikeCount(int cheepId);
    /// <summary>
    /// Returns if a User with the given Name has likes the given Cheep
    /// </summary>
    /// <param name="cheepId"></param>
    /// <param name="author"></param>
    Task<bool> HasUserLikedCheep(int cheepId, string authorName);
}