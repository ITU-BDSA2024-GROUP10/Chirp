﻿using System.Collections;
using Chirp.Core.DTO;

namespace Chirp.Core;

public interface ICheepRepository
{
    public Task<IEnumerable<CheepDTO>?> GetCheepsByPage(int page, int pageSize);
    public Task<IEnumerable<CheepDTO>> GetCheepsFromAuthorByPage(String author, int page, int pageSize);
    public Task<IEnumerable<CheepDTO>> GetCheepsFromAuthor(String author);
    public Task<IEnumerable<CheepDTO>> GetCheepsFromAuthorsByPage(IEnumerable<String> authors, int page, int pageSize);
    public Task<IEnumerable<CheepDTO>> GetCheepsWithLikesByPage(string userName, int page, int pageSize);
    public Task<bool> CreateCheep(CheepDTO cheep);
    public Task<int> GetAmountOfCheeps();
    public Task<int> GetAmountOfCheepsFromAuthors(IEnumerable<String> authors);
    public Task<bool> AddCommentToCheep(CommentDTO comment);
    public Task<int> GetCommentAmountOnCheep(int? cheepId);
    public Task<CheepDTO> GetCheepById(int cheepId);
    public Task<IEnumerable<CommentDTO>> GetCommentsForCheep(int cheepId);
    public Task<bool> LikeCheep(LikeDTO like);
    public Task<bool> UnlikeCheep(LikeDTO like);
    Task<int> GetLikeCount(int cheepId);
    Task<bool> HasUserLikedCheep(int cheepId, string authorName);
}