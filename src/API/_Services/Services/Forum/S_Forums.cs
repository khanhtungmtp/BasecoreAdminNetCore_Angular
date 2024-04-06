
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

namespace API._Services.Services.Forums;
public class S_Forums : BaseServices, I_Forums
{
    private readonly I_Sequence _sequenceService;
    private readonly I_Storage _storageService;
    public S_Forums(IRepositoryAccessor repositoryAccessor, I_Sequence sequenceService, I_Storage storageService) : base(repositoryAccessor)
    {
        _sequenceService = sequenceService;
        _storageService = storageService;
    }

    public async Task<ApiResponse<string>> PostForumAsync(ForumCreateRequest request)
    {
        Forum fourm = CreateForumEntity(request);
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
                _repositoryAccessor.Attachments.Add(attachmentEntity);
            }
        }
        _repositoryAccessor.Forums.Add(fourm);

        //Process label
        if (request.Labels?.Length > 0)
        {
            await ProcessLabel(request, fourm);
        }

        var result = await _repositoryAccessor.SaveChangesAsync();

        if (result)
            return new ApiResponse<string>((int)HttpStatusCode.OK, true, "Forum created successfully.", fourm.Id.ToString());
        else
            return new ApiResponse<string>((int)HttpStatusCode.BadRequest, false, "Create forum failed.", null!);
    }

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

    private async Task ProcessLabel(ForumCreateRequest request, Forum forum)
    {
        foreach (var labelText in request.Labels)
        {
            if (labelText == null) continue;
            var labelId = FunctionUtility.GenerateSlug(labelText.ToString());
            var existingLabel = await _repositoryAccessor.Labels.FindAsync(labelId);
            if (existingLabel == null)
            {
                var labelEntity = new Label()
                {
                    Id = labelId,
                    Name = labelText.ToString()
                };
                _repositoryAccessor.Labels.Add(labelEntity);
            }
            if (await _repositoryAccessor.LabelInForums.FindAsync(labelId, forum.Id) == null)
            {
                _repositoryAccessor.LabelInForums.Add(new LabelInForum()
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
    private static Forum CreateForumEntity(ForumCreateRequest request)
    {
        var entity = new Forum()
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

    public async Task<ApiResponse<PagingResult<ForumQuickVM>>> GetForumsPagingAsync(string? filter, PaginationParam pagination, ForumQuickVM forumVM)
    {
        var query = from k in _repositoryAccessor.Forums.FindAll(true)
                    join c in _repositoryAccessor.Categories.FindAll(true) on k.CategoryId equals c.Id
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
}
