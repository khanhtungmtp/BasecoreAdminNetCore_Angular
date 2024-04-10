using API._Repositories;
using API._Services.Interfaces.Forum;
using API.Helpers.Base;
using API.Models;
using Microsoft.EntityFrameworkCore;
using ViewModels.Forum;

namespace API._Services.Services.Forum;
public class S_Votes(IRepositoryAccessor repoStore) : BaseServices(repoStore), I_Votes
{
    public async Task<OperationResult<int>> CreateAsync(int forumId, string userId)
    {
        var forum = await _repoStore.Forums.FindAsync(forumId);
        if (forum is null)
            return OperationResult<int>.NotFound($"Cannot found forum with id {forumId}");

        int numberOfVotes = await _repoStore.Votes.CountAsync(x => x.ForumId == forumId);
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
            return OperationResult<int>.Success(numberOfVotes, "Vote successfully.");
        else
            return OperationResult<int>.BadRequest("Vote failed.");
    }

    public async Task<OperationResult<string>> DeleteAsync(int forumId, string userId)
    {
        var vote = await _repoStore.Votes.FindAsync(forumId, userId);
        if (vote is null)
            return OperationResult<string>.NotFound($"Cannot found vote with id {userId}");

        var forum = await _repoStore.Forums.FindAsync(forumId);
        if (forum is null)
            return OperationResult<string>.NotFound($"Cannot found forum with id {forumId}");

        forum.NumberOfVotes = forum.NumberOfVotes.GetValueOrDefault(0) - 1;
        _repoStore.Forums.Update(forum);

        _repoStore.Votes.Remove(vote);
        bool result = await _repoStore.SaveChangesAsync();
        if (result)
        {
            return OperationResult<string>.Success(userId, "Delete vote successfully.");
        }
        return OperationResult<string>.BadRequest("Delete vote failed.");
    }

    public async Task<OperationResult<List<VoteVM>>> GetListAsync(int forumId)
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

        return OperationResult<List<VoteVM>>.Success(votes, "Get votes successfully.");
    }
}
