

using System.Net;
using API._Repositories;
using API._Services.Interfaces.System;
using API.Helpers.Base;
using API.Helpers.Constants;
using Microsoft.EntityFrameworkCore;
using ViewModels.System;

namespace API._Services.Services.System;
public class S_Labels(IRepositoryAccessor repoStore, I_Cache cacheService) : BaseServices(repoStore), I_Labels
{
    private readonly I_Cache _cacheService = cacheService;

    public async Task<ApiResponse<LabelVM>> FindByIdAsync(string id)
    {
        var label = await _repoStore.Labels.FindAsync(id);
        if (label is null)
            return Fail<LabelVM>((int)HttpStatusCode.NotFound, $"Label with id: {id} is not found");

        var labelVm = new LabelVM()
        {
            Id = label.Id,
            Name = label.Name
        };

        return Success((int)HttpStatusCode.OK, labelVm, "Get label successfully.");
    }

    public async Task<ApiResponse<List<LabelVM>>> GetPopularLabelsAsync(int take)
    {
        var cachedData = await _cacheService.GetAsync<List<LabelVM>>(CacheConstants.PopularLabels);
        if (cachedData is null)
        {
            var query = from l in _repoStore.Labels.FindAll(true)
                        join lik in _repoStore.LabelInForums.FindAll(true) on l.Id equals lik.LabelId
                        group new { l.Id, l.Name } by new { l.Id, l.Name } into g
                        select new
                        {
                            g.Key.Id,
                            g.Key.Name,
                            Count = g.Count()
                        };
            var labels = await query.OrderByDescending(x => x.Count).Take(take)
                .Select(l => new LabelVM()
                {
                    Id = l.Id,
                    Name = l.Name
                }).ToListAsync();
            await _cacheService.SetAsync(CacheConstants.PopularLabels, labels);
            cachedData = labels;
        }

        return Success((int)HttpStatusCode.OK, cachedData, "Get popular labels successfully.");
    }
}
