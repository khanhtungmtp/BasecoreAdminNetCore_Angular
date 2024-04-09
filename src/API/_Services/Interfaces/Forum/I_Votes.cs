using API.Helpers.Base;
using ViewModels.Forum;
using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.Forum;

[DependencyInjection(ServiceLifetime.Scoped)]
public interface I_Votes
{
    Task<OperationResult<List<VoteVM>>> GetVotesAsync(int forumId);
    Task<OperationResult<int>> CreateAsync(int forumId, string userId);
    Task<OperationResult<string>> DeleteAsync(int forumId, string userId);
}
