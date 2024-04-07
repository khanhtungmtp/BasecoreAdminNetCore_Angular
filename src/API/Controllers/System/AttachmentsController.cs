using API._Services.Interfaces.System;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.System;

public class AttachmentsController(I_Attachments attachmentService) : BaseController
{
    private readonly I_Attachments _attachmentService = attachmentService;

    [HttpGet("{forumId}/attachments")]
    public async Task<IActionResult> GetAttachments(int forumId)
    {
        var attachments = await _attachmentService.GetAttachmentsAsync(forumId);
        if (!attachments.Succeeded)
            return BadRequest(attachments);

        return Ok(attachments);
    }

    [HttpDelete("{forumId}/attachments/{attachmentId}")]
    public async Task<IActionResult> DeleteAttachment(int attachmentId)
    {
        var result = await _attachmentService.DeleteAsync(attachmentId);
        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }
}
