
using API.Helpers.Base;
using ViewModels.System;
using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.System;

[DependencyInjection(ServiceLifetime.Scoped)]
public interface I_Attachments
{
    Task<ApiResponse<List<AttachmentVM>>> GetAttachmentsAsync(int forumId);
    Task<ApiResponse> DeleteAsync(int attachmentId);
}
