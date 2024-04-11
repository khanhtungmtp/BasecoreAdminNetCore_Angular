using API._Repositories;
using API._Services.Interfaces.System;
using API.Helpers.Base;
using Microsoft.EntityFrameworkCore;
using ViewModels.System;

namespace API._Services.Services.System;
public class S_Attachments(IRepositoryAccessor repoStore) : BaseServices(repoStore), I_Attachments
{
    public async Task<OperationResult<List<AttachmentVM>>> GetListAsync(int forumId)
    {
        var query = await _repoStore.Attachments.FindAll(true)
                .Where(x => x.ForumId == forumId)
                .Select(c => new AttachmentVM()
                {
                    Id = c.Id,
                    LastModifiedDate = c.UpdatedDate,
                    CreatedDate = c.CreatedDate,
                    FileName = c.FileName,
                    FilePath = c.FilePath,
                    FileSize = c.FileSize,
                    FileType = c.FileType,
                    ForumId = c.ForumId ?? 0
                }).ToListAsync();

        return OperationResult<List<AttachmentVM>>.Success(query, "Get attachments successfully.");
    }

    public async Task<OperationResult> DeleteAsync(int attachmentId)
    {
        var attachment = await _repoStore.Attachments.FindAsync(attachmentId);
        if (attachment is null)
            return OperationResult.NotFound($"Cannot found attachment with id {attachmentId}");

        _repoStore.Attachments.Remove(attachment);

        bool result = await _repoStore.SaveChangesAsync();
        if (result)
        {
            return OperationResult.Success("Delete attachment Successfully");
        }
        return OperationResult.BadRequest("Delete attachment failed");
    }
}
