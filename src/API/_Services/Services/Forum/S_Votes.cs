
using System.Net;
using API._Repositories;
using API._Services.Interfaces.Forum;
using API.Helpers.Base;
using API.Models;
using Microsoft.EntityFrameworkCore;
using ViewModels.Forum;

namespace API._Services.Services.Forum;
public class S_Votes(IRepositoryAccessor repoStore) : BaseServices(repoStore), I_Votes
{
    public async Task<ApiResponse<int>> CreateAsync(int forumId, string userId)
    {
        var forum = await _repoStore.Forums.FindAsync(forumId);
        if (forum is null)
            return new ApiResponse<int>((int)HttpStatusCode.NotFound, false, $"Cannot found knowledge base with id {forumId}");

        var numberOfVotes = await _repoStore.Votes.CountAsync(x => x.ForumId == forumId);
        var vote = await _repoStore.Votes.FindAsync(forumId, userId);
        if (vote is not null)
        {
            _repoStore.Votes.Remove(vote);
            numberOfVotes -= 1;
        }
        else
        {
            vote = new Vote()
            {
                ForumId = forumId,
                UserId = userId
            };
            _repoStore.Votes.Add(vote);
            numberOfVotes += 1;
        }

        forum.NumberOfVotes = numberOfVotes;
        _repoStore.Forums.Update(forum);

        bool result = await _repoStore.SaveChangesAsync();
        if (result)
            return new ApiResponse<int>((int)HttpStatusCode.OK, true, "Vote successfully.", numberOfVotes);
        else
            return new ApiResponse<int>((int)HttpStatusCode.BadRequest, false, "Vote failed.");
    }

    public async Task<ApiResponse<string>> DeleteAsync(int forumId, string userId)
    {
        var vote = await _repoStore.Votes.FindAsync(forumId, userId);
        if (vote is null)
            return new ApiResponse<string>((int)HttpStatusCode.NotFound, false, $"Cannot found vote with id {userId}");

        var forum = await _repoStore.Forums.FindAsync(forumId);
        if (forum is null)
            return new ApiResponse<string>((int)HttpStatusCode.NotFound, false, $"Cannot found forum with id {forumId}");

        forum.NumberOfVotes = forum.NumberOfVotes.GetValueOrDefault(0) - 1;
        _repoStore.Forums.Update(forum);

        _repoStore.Votes.Remove(vote);
        bool result = await _repoStore.SaveChangesAsync();
        if (result)
        {
            return new ApiResponse<string>((int)HttpStatusCode.OK, true, "Delete vote successfully.", userId);
        }
        return new ApiResponse<string>((int)HttpStatusCode.BadRequest, false, "Delete vote failed.");
    }

    public async Task<ApiResponse<List<VoteVM>>> GetVotesAsync(int forumId)
    {
        var votes = await _repoStore.Votes
                .FindAll(x => x.ForumId == forumId)
                .Select(x => new VoteVM()
                {
                    UserId = x.UserId,
                    ForumId = x.ForumId,
                    CreateDate = x.CreateDate,
                    UpdateDate = x.UpdateDate
                }).ToListAsync();

        return new ApiResponse<List<VoteVM>>((int)HttpStatusCode.OK, true, "Get votes successfully.", votes);
    }
}
