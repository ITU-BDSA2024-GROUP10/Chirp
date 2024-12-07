﻿using Chirp.Core;
using Chirp.Core.DTO;
using Chirp.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class CheepRepository(ChirpDBContext context) : ICheepRepository
{
    private readonly ChirpDBContext context = context;

    public async Task<IEnumerable<CheepDTO>?> GetCheepsByPage(int page, int pageSize)
    {
        if (pageSize < 0) return null;

        var query = context.Cheeps
            .Select(cheep => new { cheep.Id, cheep.Author.UserName, cheep.Message, cheep.TimeStamp })
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        var cheeps = await query.ToListAsync();

        return cheeps.Select(cheep =>
            new CheepDTO(cheep.Id, cheep.UserName!, cheep.Message,
                new DateTimeOffset(cheep.TimeStamp).ToUnixTimeSeconds()));
    }

    public async Task<IEnumerable<CheepDTO>> GetCheepsFromAuthorByPage(string author, int page, int pageSize)
    {
        return await GetCheepsFromAuthorsByPage([author], page, pageSize);
    }

    public async Task<IEnumerable<CheepDTO>> GetCheepsFromAuthor(string author)
    {
        author = author.ToUpper();
        var query = context.Cheeps
            .Where(cheep => cheep.Author.NormalizedUserName == author)
            .Select(cheep => new { cheep.Id, cheep.Author.UserName, cheep.Message, cheep.TimeStamp })
            .OrderByDescending(cheep => cheep.TimeStamp);
        var cheeps = await query.ToListAsync();

        return cheeps.Select(cheep =>
            new CheepDTO(cheep.Id, cheep.UserName!, cheep.Message,
                new DateTimeOffset(cheep.TimeStamp).ToUnixTimeSeconds()));
    }

    public async Task<IEnumerable<CheepDTO>> GetCheepsFromAuthorsByPage(IEnumerable<string> authors, int page,
        int pageSize)
    {
        authors = authors.Select(author => author.ToUpper());
        var query = context.Cheeps
            .Where(cheep => authors.Contains(cheep.Author.NormalizedUserName!))
            .Select(cheep => new { cheep.Id, cheep.Author.UserName, cheep.Message, cheep.TimeStamp })
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        var cheeps = await query.ToListAsync();

        return cheeps.Select(cheep =>
            new CheepDTO(cheep.Id, cheep.UserName!, cheep.Message,
                new DateTimeOffset(cheep.TimeStamp).ToUnixTimeSeconds()));
    }

    public Task<int> GetAmountOfCheeps()
    {
        return context.Cheeps.CountAsync();
    }

    public Task<int> GetAmountOfCheepsFromAuthors(IEnumerable<String> authors)
    {
        authors = authors.Select(author => author.ToUpper());
        return context.Cheeps
            .CountAsync(cheep => authors.Contains(cheep.Author.NormalizedUserName!));
    }

    public async Task<bool> CreateCheep(CheepDTO cheep)
    {
        var author = await context.Authors
            .Where(a => a.UserName == cheep.Author)
            .FirstOrDefaultAsync();
        if (author == null) return false;
        var cheep2 = new Cheep
        {
            Author = author, Message = cheep.Message,
            TimeStamp = DateTimeOffset.FromUnixTimeSeconds(cheep.UnixTimestamp).DateTime
        };
        context.Cheeps.Add(cheep2);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddCommentToCheep(CommentDTO comment)
    {
        var author = await context.Authors
            .Where(a => a.UserName == comment.Author)
            .FirstOrDefaultAsync();
        if (author == null) return false;
        var cheep = await context.Cheeps
            .Where(c => c.Id == comment.CheepId)
            .Include(c => c.Comments)
            .FirstOrDefaultAsync();
        if (cheep == null) return false;
        var newComment = new Comment
        {
            Author = author, Cheep = cheep, Message = comment.Message,
            TimeStamp = DateTimeOffset.FromUnixTimeSeconds(comment.UnixTimestamp).DateTime
        };
        cheep.Comments.Add(newComment);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetCommentAmountOnCheep(int? cheepId)
    {
        if (cheepId == null) return 0;
        var query = await context.Cheeps
            .Include(c => c.Comments)
            .Where(c => c.Id == cheepId)
            .FirstOrDefaultAsync();
        var commentCount = query!.Comments.Count;
        return commentCount;
    }

    public async Task<CheepDTO> GetCheepById(int cheepId)
    {
        var cheep = await context.Cheeps
            .Where(c => c.Id == cheepId)
            .Select(cheep => new { cheep.Id, cheep.Author.UserName, cheep.Message, cheep.TimeStamp })
            .FirstOrDefaultAsync();

        return new CheepDTO(cheep!.Id, cheep.UserName!, cheep.Message,
            new DateTimeOffset(cheep.TimeStamp).ToUnixTimeSeconds());
        ;
    }

    public async Task<IEnumerable<CommentDTO>> GetCommentsForCheep(int cheepId)
    {
        var query = await context.Cheeps
            .Where(c => c.Id == cheepId)
            .Include(c => c.Comments)
            .ThenInclude(comment => comment.Author)
            .FirstOrDefaultAsync();
        var comments = query!.Comments.OrderByDescending(c => c.TimeStamp);
        return comments.Select(comment =>
            new CommentDTO(comment.Author.UserName!, comment.Id, comment.Message,
                new DateTimeOffset(comment.TimeStamp).ToUnixTimeSeconds()));
    }

    public async Task<bool> LikeCheep(LikeDTO like)
    {
        var author = await context.Authors.FirstOrDefaultAsync(a => a.NormalizedUserName == like.Author.ToUpper());
        if (author == null) return false;

        var cheep = await context.Cheeps.Include(c => c.Likes).FirstOrDefaultAsync(c => c.Id == like.CheepId);
        if (cheep == null) return false;

        if (cheep.Likes.Any(l => l.Author.UserName == like.Author)) return false; // already liked

        var likeEntity = new Like { Author = author, Cheep = cheep };
        cheep.Likes.Add(likeEntity);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnlikeCheep(LikeDTO like)
    {
        var cheep = await context.Cheeps
            .Include(c => c.Likes)
            .ThenInclude(l => l.Author)
            .FirstOrDefaultAsync(c => c.Id == like.CheepId);
        if (cheep == null) return false;

        var likeEntity = cheep.Likes
            .Where(l => l.Author.UserName == like.Author)
            .FirstOrDefault(l => l.Author.NormalizedUserName == like.Author.ToUpper());
        if (likeEntity == null) return false;

        cheep.Likes.Remove(likeEntity);
        //context.Likes.Remove(likeEntity);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetLikeCount(int cheepId)
    {
        var cheep = await context.Cheeps.Include(c => c.Likes).FirstOrDefaultAsync(c => c.Id == cheepId);
        return cheep?.Likes.Count ?? 0;
    }

    public async Task<bool> HasUserLikedCheep(int cheepId, string authorName)
    {
        var cheep = await context.Cheeps
            .Include(c => c.Likes)
            .ThenInclude(l => l.Author)
            .FirstOrDefaultAsync(c => c.Id == cheepId);

        return cheep?.Likes.Any(l => l.Author.UserName == authorName) ?? false;
    }

    public async Task<IEnumerable<CheepDTO>> GetCheepsWithLikesByPage(string userName, int page, int pageSize)
    {
        var user = await context.Authors.FirstOrDefaultAsync(a => a.UserName == userName);
        if (user == null) throw new ArgumentException("User not found");

        var cheeps = await context.Cheeps
            .Include(c => c.Likes)
            .OrderByDescending(c => c.TimeStamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new
            {
                c.Id,
                c.Author.UserName,
                c.Message,
                c.TimeStamp,
                LikeCount = c.Likes.Count,
                HasLiked = c.Likes.Any(l => l.Author.Id == user.Id)
            })
            .ToListAsync();

        return cheeps.Select(c => new CheepDTO(
            c.Id,
            c.UserName!,
            c.Message,
            new DateTimeOffset(c.TimeStamp).ToUnixTimeSeconds())
        {
            LikeCount = c.LikeCount,
        });
    }
}