using API._Services.Interfaces.System;
using API.Helpers.Base;
using Microsoft.AspNetCore.Mvc;
using ViewModels.System;

namespace API.Controllers.System;

public class AttachmentsController(I_Attachments attachmentService) : BaseController
{
    private readonly I_Attachments _attachmentService = attachmentService;

    [HttpGet("{forumId}/attachments")]
    public async Task<IActionResult> GetAttachments(int forumId)
    {
        OperationResult<List<AttachmentVM>>? result = await _attachmentService.GetListAsync(forumId);
        return HandleResult(result);
    }

    [HttpDelete("{forumId}/attachments/{attachmentId}")]
    public async Task<IActionResult> DeleteAttachment(int attachmentId)
    {
        OperationResult? result = await _attachmentService.DeleteAsync(attachmentId);
        return HandleResult(result);
    }
}
