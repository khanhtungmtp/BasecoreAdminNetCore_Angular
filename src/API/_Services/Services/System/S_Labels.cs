using API._Repositories;
using API._Services.Interfaces.System;
using API.Helpers.Base;
using API.Models;
using Microsoft.EntityFrameworkCore;
using ViewModels.System;

namespace API._Services.Services.System;
public class S_Labels(IRepositoryAccessor repoStore) : BaseServices(repoStore), I_Labels
{
    public async Task<OperationResult<LabelVM>> FindByIdAsync(string id)
    {
        Label? label = await _repoStore.Labels.FindAsync(id);
        if (label is null)
            return OperationResult<LabelVM>.NotFound($"Label with id: {id} is not found");

        LabelVM? labelVm = new()
        {
            Id = label.Id,
            Name = label.Name
        };

        return OperationResult<LabelVM>.Success(labelVm, "Get label successfully.");
    }

    public async Task<OperationResult<List<LabelVM>>> GetPopularLabelsAsync(int take)
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
            List<LabelVM>? labels = await query.OrderByDescending(x => x.Count).Take(take)
                .Select(l => new LabelVM()
                {
                    Id = l.Id,
                    Name = l.Name
                }).ToListAsync();

        return OperationResult<List<LabelVM>>.Success(labels, "Get popular labels successfully.");
    }
}
