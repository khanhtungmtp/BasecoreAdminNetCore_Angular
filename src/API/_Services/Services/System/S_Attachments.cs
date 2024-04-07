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

        return new ApiResponse<List<AttachmentVM>>((int)HttpStatusCode.OK, true, "Get attachments successfully.", query);
    }

    public async Task<ApiResponse> DeleteAsync(int attachmentId)
    {
        var attachment = await _repoStore.Attachments.FindAsync(attachmentId);
        if (attachment == null)
            return new ApiNotFoundResponse($"Cannot found attachment with id {attachmentId}");

        _repoStore.Attachments.Remove(attachment);

        var result = await _repoStore.SaveChangesAsync();
        if (result)
        {
            return new ApiResponse((int)HttpStatusCode.OK, true, "Delete attachment Successfully");
        }
        return new ApiBadRequestResponse("Delete attachment failed");
    }
}
