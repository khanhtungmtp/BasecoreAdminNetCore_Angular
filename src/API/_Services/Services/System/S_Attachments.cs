using System.Net;
using API._Repositories;
using API._Services.Interfaces.System;
using API.Helpers.Base;
using Microsoft.EntityFrameworkCore;
using ViewModels.System;

namespace API._Services.Services.System;
public class S_Attachments(IRepositoryAccessor repoStore) : BaseServices(repoStore), I_Attachments
{
    public async Task<ApiResponse<List<AttachmentVM>>> GetAttachmentsAsync(int forumId)
    {
        var query = await _repoStore.Attachments.FindAll(true)
                .Where(x => x.ForumId == forumId)
                .Select(c => new AttachmentVM()
                {
                    Id = c.Id,
                    LastModifiedDate = c.UpdateDate,
                    CreateDate = c.CreateDate,
                    FileName = c.FileName,
                    FilePath = c.FilePath,
                    FileSize = c.FileSize,
                    FileType = c.FileType,
                    ForumId = c.ForumId ?? 0
                }).ToListAsync();

        return Success((int)HttpStatusCode.OK, query, "Get attachments successfully.");
    }

    public async Task<ApiResponse> DeleteAsync(int attachmentId)
    {
        var attachment = await _repoStore.Attachments.FindAsync(attachmentId);
        if (attachment is null)
            return Fail((int)HttpStatusCode.NotFound, $"Cannot found attachment with id {attachmentId}");

        _repoStore.Attachments.Remove(attachment);

        bool result = await _repoStore.SaveChangesAsync();
        if (result)
        {
            return Success((int)HttpStatusCode.OK, "Delete attachment Successfully");
        }
        return Fail((int)HttpStatusCode.BadRequest, "Delete attachment failed");
    }
}
