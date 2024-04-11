using API._Repositories;
using API._Services.Interfaces.System;
using API.Helpers.Base;
using API.Helpers.Utilities;
using API.Models;
using Microsoft.EntityFrameworkCore;
using ViewModels.System;

namespace API._Services.Services.System;
public class S_Reports(IRepositoryAccessor repoStore) : BaseServices(repoStore), I_Reports
{
    public async Task<OperationResult> CreateAsync(int forumId, ReportCreateRequest request)
    {
        var report = new Report()
        {
            Content = request.Content,
            ForumId = forumId,
            ReportUserId = request.UserId,
            IsProcessed = false
        };
        _repoStore.Reports.Add(report);

        var forum = await _repoStore.Forums.FindAsync(forumId);
        if (forum is null)
            return OperationResult.NotFound($"Cannot found knowledge base with id {forumId}");

        forum.NumberOfReports = forum.NumberOfReports.GetValueOrDefault(0) + 1;
        _repoStore.Forums.Update(forum);

        bool result = await _repoStore.SaveChangesAsync();
        if (result)
            return OperationResult.Success("Create report successfully.");
        else
            return OperationResult.BadRequest("Create report failed.");
    }

    public async Task<OperationResult<ReportVM>> FindByIdAsync(int reportId)
    {
        var report = await _repoStore.Reports.FindAsync(reportId);
        if (report is null)
            return OperationResult<ReportVM>.NotFound("Report not found.");
        var user = await _repoStore.Users.FindAsync(report.ReportUserId);
        if (user is null)
            return OperationResult<ReportVM>.NotFound("User not found.");
        var reportVm = new ReportVM()
        {
            Id = report.Id,
            Content = report.Content,
            CreatedDate = report.CreatedDate,
            ForumId = report.ForumId,
            UpdatedDate = report.UpdatedDate,
            IsProcessed = report.IsProcessed,
            ReportUserId = report.ReportUserId,
            ReportUserName = user.FullName
        };

        return OperationResult<ReportVM>.Success(reportVm, "Get report successfully.");
    }

    public async Task<OperationResult<PagingResult<ReportVM>>> GetPagingAsync(string? filter, PaginationParam pagination, ReportVM reportVM)
    {
        var query = from r in _repoStore.Reports.FindAll(true)
                    join u in _repoStore.Users.FindAll(true)
                        on r.ReportUserId equals u.Id
                    select new { r, u };
        if (reportVM.ForumId.HasValue)
        {
            query = query.Where(x => x.r.ForumId == reportVM.ForumId.Value);
        }

        if (!string.IsNullOrEmpty(filter))
        {
            query = query.Where(x => x.r.Content.Contains(filter));
        }
        var result = await query.Select(c => new ReportVM()
        {
            Id = c.r.Id,
            Content = c.r.Content,
            CreatedDate = c.r.CreatedDate,
            ForumId = c.r.ForumId,
            UpdatedDate = c.r.UpdatedDate,
            IsProcessed = false,
            ReportUserId = c.r.ReportUserId,
            ReportUserName = c.u.FullName
        }).ToListAsync();
        var resultPaging = PagingResult<ReportVM>.Create(result, pagination.PageNumber, pagination.PageSize);
        return OperationResult<PagingResult<ReportVM>>.Success(resultPaging, "Get function successfully.");
    }

    public async Task<OperationResult> DeleteAsync(int forumId, int reportId)
    {
        var report = await _repoStore.Reports.FindAsync(reportId);
        if (report is null)
            return OperationResult.NotFound($"Cannot found report with id {reportId}");

        _repoStore.Reports.Remove(report);

        var forum = await _repoStore.Forums.FindAsync(forumId);
        if (forum is null)
            return OperationResult.NotFound($"Cannot found forum with id {forumId}");

        forum.NumberOfReports = forum.NumberOfReports.GetValueOrDefault(0) - 1;
        _repoStore.Forums.Update(forum);

        bool result = await _repoStore.SaveChangesAsync();
        if (result)
            return OperationResult.Success("Delete report successfully.");
        return OperationResult.BadRequest("Delete report failed.");
    }
}
