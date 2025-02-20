using API._Repositories;
using API._Services.Interfaces.Forums;
using API.Helpers.Base;
using Microsoft.EntityFrameworkCore;
using ViewModels.Forum;

namespace API._Services.Services.Forum;
public class S_Statistics(IRepositoryAccessor repoStore) : BaseServices(repoStore), I_Statistics
{
    public async Task<OperationResult<List<MonthlyCommentsVM>>> GetMonthlyNewCommentsAsync(int year)
    {
        List<MonthlyCommentsVM>? data = await _repoStore.Comments.FindAll(x => x.CreatedDate.Date.Year == year)
                .GroupBy(x => x.CreatedDate.Date.Month)
                .OrderBy(x => x.Key)
                .Select(g => new MonthlyCommentsVM()
                {
                    Month = g.Key,
                    NumberOfComments = g.Count()
                })
                .ToListAsync();

        return OperationResult<List<MonthlyCommentsVM>>.Success(data, "Get monthly new comments successfully.");
    }

    public async Task<OperationResult<List<MonthlyNewKbsVM>>> GetMonthlyNewKbsAsync(int year)
    {
        List<MonthlyNewKbsVM>? data = await _repoStore.Forums.FindAll(x => x.CreatedDate.Date.Year == year)
                .GroupBy(x => x.CreatedDate.Date.Month)
                .Select(g => new MonthlyNewKbsVM()
                {
                    Month = g.Key,
                    NumberOfNewKbs = g.Count()
                })
                .ToListAsync();

        return OperationResult<List<MonthlyNewKbsVM>>.Success(data, "Get monthly new comments successfully.");
    }

    public async Task<OperationResult<List<MonthlyNewKbsVM>>> GetMonthlyNewRegistersAsync(int year)
    {
        List<MonthlyNewKbsVM>? data = await _repoStore.Users.FindAll(x => x.CreatedDate.Date.Year == year)
              .GroupBy(x => x.CreatedDate.Date.Month)
              .Select(g => new MonthlyNewKbsVM()
              {
                  Month = g.Key,
                  NumberOfNewKbs = g.Count()
              })
              .ToListAsync();

        return OperationResult<List<MonthlyNewKbsVM>>.Success(data, "Get monthly new comments successfully.");

    }
}
