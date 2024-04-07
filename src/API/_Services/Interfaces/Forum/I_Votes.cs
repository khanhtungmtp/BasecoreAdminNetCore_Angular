using API.Helpers.Base;
using ViewModels.Forum;
using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.Forum;

[DependencyInjection(ServiceLifetime.Scoped)]
public interface I_Votes
{
    Task<ApiResponse<List<VoteVM>>> GetVotesAsync(int forumId);
    Task<ApiResponse<int>> CreateAsync(int forumId, string userId);
    Task<ApiResponse<string>> DeleteAsync(int forumId, string userId);
}
