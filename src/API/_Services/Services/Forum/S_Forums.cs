
using System.Net;
using API._Repositories;
using API._Services.Interfaces.Forum;
using API.Helpers.Base;
using ViewModels.Forum;
using API.Models;
using API._Services.Interfaces.System;
using API.Helpers.Utilities;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using API.Helpers.Constants;
using ViewModels.System;

namespace API._Services.Services.Forums;
public class S_Forums : BaseServices, I_Forums
{
    private readonly I_Sequence _sequenceService;
    private readonly I_Storage _storageService;
    private readonly I_Cache _cacheService;
    public S_Forums(IRepositoryAccessor repoStore, I_Sequence sequenceService, I_Storage storageService, I_Cache cacheService) : base(repoStore)
    {
        _sequenceService = sequenceService;
        _storageService = storageService;
        _cacheService = cacheService;
    }
    #region PostForumAsync
    public async Task<ApiResponse<string>> CreateAsync(ForumCreateRequest request)
    {
        var fourm = CreateForumEntity(request);
        if (string.IsNullOrEmpty(fourm.SeoAlias))
        {
            fourm.SeoAlias = FunctionUtility.GenerateSlug(fourm.Title);
        }
        fourm.Id = await _sequenceService.GetNextSequenceValueAsync();
        //Process attachment
        if (request.Attachments != null && request.Attachments.Count > 0)
        {
            foreach (var attachment in request.Attachments)
            {
                var attachmentEntity = await SaveFile(fourm.Id, attachment);
                _repoStore.Attachments.Add(attachmentEntity);
            }
        }
        _repoStore.Forums.Add(fourm);

        //Process label
        if (request.Labels?.Length > 0)
        {
            await ProcessLabel(request, fourm);
        }

        var result = await _repoStore.SaveChangesAsync();

        if (result)
            return new ApiResponse<string>((int)HttpStatusCode.OK, true, "Forum created successfully.", fourm.Id.ToString());
        else
            return new ApiResponse<string>((int)HttpStatusCode.BadRequest, false, "Create forum failed.", null!);
    }
    #endregion
    #region automapper private methods
    private async Task<Attachment> SaveFile(int forumId, IFormFile file)
    {
        if (string.IsNullOrEmpty(file.ContentDisposition))
        {
            throw new InvalidOperationException("File content disposition is missing.");
        }
        var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition)?.FileName?.Trim('"');
        var fileName = $"{originalFileName?.Substring(0, originalFileName.LastIndexOf('.'))}{Path.GetExtension(originalFileName)}";
        await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
        var attachmentEntity = new Attachment()
        {
            FileName = fileName,
            FilePath = _storageService.GetFileUrl(fileName),
            FileSize = file.Length,
            FileType = Path.GetExtension(fileName),
            ForumId = forumId,
            Type = string.Empty
        };
        return attachmentEntity;
    }

    private async Task ProcessLabel(ForumCreateRequest request, Models.Forum forum)
    {
        foreach (var labelText in request.Labels)
        {
            if (labelText == null) continue;
            var labelId = FunctionUtility.GenerateSlug(labelText.ToString());
            var existingLabel = await _repoStore.Labels.FindAsync(labelId);
            if (existingLabel == null)
            {
                var labelEntity = new Label()
                {
                    Id = labelId,
                    Name = labelText.ToString()
                };
                _repoStore.Labels.Add(labelEntity);
            }
            if (await _repoStore.LabelInForums.FindAsync(labelId, forum.Id) == null)
            {
                _repoStore.LabelInForums.Add(new LabelInForum()
                {
                    ForumId = forum.Id,
                    LabelId = labelId
                });
            }
        }
    }

    //
    // *Created Date: 2024-04-06 13:32:46
    // Summary:
    // same automapper
    //
    // Parameters:
    // ForumCreateRequest
    //
    // Returns:
    // entity
    private static Models.Forum CreateForumEntity(ForumCreateRequest request)
    {
        var entity = new Models.Forum()
        {
            CategoryId = request.CategoryId,
            Title = request.Title,
            SeoAlias = request.SeoAlias,
            Description = request.Description,
            Environment = request.Environment,
            Problem = request.Problem,
            StepToReproduce = request.StepToReproduce,
            ErrorMessage = request.ErrorMessage,
            Workaround = request.Workaround,
            OwnerUserId = request.OwnerUserId,
            Note = request.Note
        };
        if (request.Labels?.Length > 0)
        {
            entity.Labels = string.Join(',', request.Labels);
        }
        return entity;
    }

    //
    // Summary:
    // same automapper
    //
    // Parameters:
    // entity Forum
    //
    // Returns:
    // entity
    private static ForumVM CreateForumVM(Models.Forum forum)
    {
        return new ForumVM()
        {
            Id = forum.Id,

            CategoryId = forum.CategoryId,

            Title = forum.Title,

            SeoAlias = forum.SeoAlias,

            Description = forum.Description,

            Environment = forum.Environment,

            Problem = forum.Problem,

            StepToReproduce = forum.StepToReproduce,

            ErrorMessage = forum.ErrorMessage,

            Workaround = forum.Workaround,

            Note = forum.Note,

            OwnerUserId = forum.OwnerUserId,

            Labels = !string.IsNullOrEmpty(forum.Labels) ? forum.Labels.Split(',') : [],

            CreateDate = forum.CreateDate,

            LastModifiedDate = forum.UpdateDate,

            NumberOfComments = forum.NumberOfComments,

            NumberOfVotes = forum.NumberOfVotes,

            NumberOfReports = forum.NumberOfReports,
        };
    }

    private static void UpdateForum(ForumCreateRequest request, Models.Forum forum)
    {
        forum.CategoryId = request.CategoryId;

        forum.Title = request.Title;

        if (string.IsNullOrEmpty(request.SeoAlias))
            forum.SeoAlias = FunctionUtility.GenerateSlug(request.Title);
        else
            forum.SeoAlias = request.SeoAlias;

        forum.Description = request.Description;

        forum.Environment = request.Environment;

        forum.Problem = request.Problem;

        forum.StepToReproduce = request.StepToReproduce;

        forum.ErrorMessage = request.ErrorMessage;

        forum.Workaround = request.Workaround;

        forum.Note = request.Note;

        if (request.Labels != null)
            forum.Labels = string.Join(',', request.Labels);
    }

    #endregion
    public async Task<ApiResponse<PagingResult<ForumQuickVM>>> GetForumsPagingAsync(string? filter, PaginationParam pagination, ForumQuickVM forumVM)
    {
        var query = from k in _repoStore.Forums.FindAll(true)
                    join c in _repoStore.Categories.FindAll(true) on k.CategoryId equals c.Id
                    select new { k, c };
        if (!string.IsNullOrEmpty(filter))
        {
            query = query.Where(x => x.k.Title.Contains(filter));
        }
        if (forumVM.CategoryId.HasValue)
        {
            query = query.Where(x => x.k.CategoryId == forumVM.CategoryId.Value);
        }
        var result = await query.Select(u => new ForumQuickVM()
        {
            Id = u.k.Id,
            CategoryId = u.k.CategoryId,
            Description = u.k.Description,
            SeoAlias = u.k.SeoAlias,
            Title = u.k.Title,
            CategoryAlias = u.c.SeoAlias,
            CategoryName = u.c.Name,
            NumberOfVotes = u.k.NumberOfVotes,
            CreateDate = u.k.CreateDate,
            NumberOfComments = u.k.NumberOfComments

        }).ToListAsync();
        var resultsPaging = PagingResult<ForumQuickVM>.Create(result, pagination.PageNumber, pagination.PageSize);
        return new ApiResponse<PagingResult<ForumQuickVM>>((int)HttpStatusCode.OK, true, "Get forums successfully.", resultsPaging);
    }

    public async Task<ApiResponse<List<ForumQuickVM>>> GetLatestForumAsync(int take)
    {
        var cachedData = await _cacheService.GetAsync<List<ForumQuickVM>>(CacheConstants.LatestForum);
        if (cachedData == null)
        {
            var forum = from k in _repoStore.Forums.FindAll(true)
                        join c in _repoStore.Categories.FindAll(true) on k.CategoryId equals c.Id
                        orderby k.CreateDate descending
                        select new { k, c };

            List<ForumQuickVM>? forums = await forum.Take(take)
                .Select(u => new ForumQuickVM()
                {
                    Id = u.k.Id,
                    CategoryId = u.k.CategoryId,
                    Description = u.k.Description,
                    SeoAlias = u.k.SeoAlias,
                    Title = u.k.Title,
                    CategoryAlias = u.c.SeoAlias,
                    CategoryName = u.c.Name,
                    NumberOfVotes = u.k.NumberOfVotes,
                    CreateDate = u.k.CreateDate
                }).ToListAsync();
            await _cacheService.SetAsync(CacheConstants.LatestForum, forums, 2);
            cachedData = forums;
        }
        return new ApiResponse<List<ForumQuickVM>>((int)HttpStatusCode.OK, true, "Get latest forums successfully.", cachedData);
    }

    public async Task<ApiResponse<List<ForumQuickVM>>> GetPopularForumAsync(int take)
    {
        var cachedData = await _cacheService.GetAsync<List<ForumQuickVM>>(CacheConstants.PopularForum);
        if (cachedData == null)
        {
            var forums = from k in _repoStore.Forums.FindAll(true)
                         join c in _repoStore.Categories.FindAll(true) on k.CategoryId equals c.Id
                         orderby k.ViewCount descending
                         select new { k, c };

            var forumVms = await forums.Take(take)
                .Select(u => new ForumQuickVM()
                {
                    Id = u.k.Id,
                    CategoryId = u.k.CategoryId,
                    Description = u.k.Description,
                    SeoAlias = u.k.SeoAlias,
                    Title = u.k.Title,
                    CategoryAlias = u.c.SeoAlias,
                    CategoryName = u.c.Name,
                    NumberOfVotes = u.k.NumberOfVotes,
                    CreateDate = u.k.CreateDate
                }).ToListAsync();
            await _cacheService.SetAsync(CacheConstants.PopularForum, forumVms, 24);
            cachedData = forumVms;
        }
        return new ApiResponse<List<ForumQuickVM>>((int)HttpStatusCode.OK, true, "Get popular forums successfully.", cachedData);
    }

    public async Task<ApiResponse<PagingResult<ForumQuickVM>>> GetForumByTagIdAsync(string labelId, PaginationParam pagination)
    {
        var query = from k in _repoStore.Forums.FindAll(true)
                    join lik in _repoStore.LabelInForums.FindAll(true) on k.Id equals lik.ForumId
                    join l in _repoStore.Labels.FindAll(true) on lik.LabelId equals l.Id
                    join c in _repoStore.Categories.FindAll(true) on k.CategoryId equals c.Id
                    where lik.LabelId == labelId
                    select new { k, l, c };

        var items = await query.Select(u => new ForumQuickVM()
        {
            Id = u.k.Id,
            CategoryId = u.k.CategoryId,
            Description = u.k.Description,
            SeoAlias = u.k.SeoAlias,
            Title = u.k.Title,
            CategoryAlias = u.c.SeoAlias,
            CategoryName = u.c.Name,
            NumberOfVotes = u.k.NumberOfVotes,
            CreateDate = u.k.CreateDate,
            NumberOfComments = u.k.NumberOfComments
        }).ToListAsync();
        var resultsPaging = PagingResult<ForumQuickVM>.Create(items, pagination.PageNumber, pagination.PageSize);
        return new ApiResponse<PagingResult<ForumQuickVM>>((int)HttpStatusCode.OK, true, "Get forums by tag id successfully.", resultsPaging);
    }

    public async Task<ApiResponse<ForumVM>> FindByIdAsync(int id)
    {
        var forum = await _repoStore.Forums.FindAsync(id);
        if (forum == null)
            return new ApiResponse<ForumVM>((int)HttpStatusCode.NotFound, false, $"Cannot found knowledge base with id: {id}", null!);

        var attachments = await _repoStore.Attachments.FindAll(true)
            .Where(x => x.ForumId == id)
            .Select(x => new AttachmentVM()
            {
                FileName = x.FileName,
                FilePath = x.FilePath,
                FileSize = x.FileSize,
                Id = x.Id,
                FileType = x.FileType
            }).ToListAsync();
        var forums = CreateForumVM(forum);
        forums.Attachments = attachments;
        return new ApiResponse<ForumVM>((int)HttpStatusCode.OK, true, $"Get forum by id {id} successfully.", forums);

    }

    public async Task<ApiResponse> PutAsync(int id, ForumCreateRequest request)
    {
        var forum = await _repoStore.Forums.FindAsync(id);
        if (forum is null)
            return new ApiNotFoundResponse($"Cannot found forum with id {id}");
        UpdateForum(request, forum);

        //Process attachment
        if (request.Attachments != null && request.Attachments.Count > 0)
        {
            foreach (var attachment in request.Attachments)
            {
                var attachmentEntity = await SaveFile(forum.Id, attachment);
                _repoStore.Attachments.Add(attachmentEntity);
            }
        }
        _repoStore.Forums.Update(forum);

        if (request.Labels?.Length > 0)
            await ProcessLabel(request, forum);

        bool result = await _repoStore.SaveChangesAsync();

        if (result)
        {
            await _cacheService.RemoveAsync("LatestForum");
            await _cacheService.RemoveAsync("PopularForum");
            return new ApiResponse((int)HttpStatusCode.OK, true, "Update forum successfully");
        }
        return new ApiResponse((int)HttpStatusCode.BadRequest, false, "Update forum failed");
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        var forum = await _repoStore.Forums.FindAsync(id);
        if (forum is null)
            return new ApiResponse<string>((int)HttpStatusCode.NotFound, false, $"Cannot found forum with id: {id}", null!);

        _repoStore.Forums.Remove(forum);
        var result = await _repoStore.SaveChangesAsync();
        if (result)
        {
            await _cacheService.RemoveAsync(CacheConstants.LatestForum);
            await _cacheService.RemoveAsync(CacheConstants.PopularForum);

            ForumVM forumVM = CreateForumVM(forum);
            return new ApiResponse<string>((int)HttpStatusCode.OK, true, "Delete forum successfully", forumVM.Id.ToString());
        }
        return new ApiResponse<string>((int)HttpStatusCode.BadRequest, false, "Delete forum failed", null!);
    }


    public async Task<ApiResponse> UpdateViewCountAsync(int id)
    {
        var forum = await _repoStore.Forums.FindAsync(id);
        if (forum == null)
            return new ApiNotFoundResponse($"Cannot found forum with id: {id}");
        forum.ViewCount ??= 0;

        forum.ViewCount += 1;
        _repoStore.Forums.Update(forum);
        var result = await _repoStore.SaveChangesAsync();
        if (result)
            return new ApiResponse((int)HttpStatusCode.OK, true, "Update view count successfully");
        return new ApiResponse((int)HttpStatusCode.BadRequest, false, "Update view count failed");
    }

    #region label
    public async Task<ApiResponse<List<LabelVM>>> GetLabelsByForumIdAsync(int forumId)
    {
        var query = from lik in _repoStore.LabelInForums.FindAll(true)
                    join l in _repoStore.Labels.FindAll(true) on lik.LabelId equals l.Id
                    orderby l.Name ascending
                    where lik.ForumId == forumId
                    select new { l.Id, l.Name };

        var labels = await query.Select(u => new LabelVM()
        {
            Id = u.Id,
            Name = u.Name
        }).ToListAsync();

        return new ApiResponse<List<LabelVM>>((int)HttpStatusCode.OK, true, "Get labels by forum id successfully.", labels);
    }

    #endregion
}
