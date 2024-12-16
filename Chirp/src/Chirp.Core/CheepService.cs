using Chirp.Core.DTO;

namespace Chirp.Core;

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

public class CheepService(ICheepRepository db) : ICheepService
{
    public IEnumerable<CheepDTO> GetCheepsByPage(int page, int pageSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(pageSize);

        var result = db.GetCheepsByPage(page, pageSize).Result ??
                     throw new ArgumentNullException(nameof(db.GetCheepsByPage));

        return result.ToList();
    }

    public IEnumerable<CheepDTO> GetCheepsFromAuthor(string author)
    {
        return db.GetCheepsFromAuthor(author).Result.ToList();
    }

    public IEnumerable<CheepDTO> GetCheepsFromAuthorByPage(string author, int page, int pageSize)
    {
        return db.GetCheepsFromAuthorByPage(author, page, pageSize).Result.ToList();
    }

    public IEnumerable<CheepDTO> GetCheepsFromAuthorsByPage(IEnumerable<string> authors, int page, int pageSize)
    {
        return db.GetCheepsFromAuthorsByPage(authors, page, pageSize).Result.ToList();
    }

    public int GetAmountOfCheepPages(int pageSize)
    {
        return (int)Math.Ceiling(db.GetAmountOfCheeps().Result / (double)pageSize);
    }

    public int GetAmountOfCheepPagesFromAuthors(IEnumerable<String> authors, int pageSize)
    {
        return (int)Math.Ceiling(db.GetAmountOfCheepsFromAuthors(authors).Result / (double)pageSize);
    }

    public bool CreateCheep(CheepDTO cheep)
    {
        return db.CreateCheep(cheep).Result;
    }

    public bool AddCommentToCheep(CommentDTO comment)
    {
        return db.AddCommentToCheep(comment).Result;
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
        return db.GetCommentsForCheep(cheepId).Result.ToList();
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

    public async Task<bool> HasUserLikedCheep(int cheepId, string author)
    {
        return await db.HasUserLikedCheep(cheepId, author);
    }
}