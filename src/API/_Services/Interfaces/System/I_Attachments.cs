
using API.Helpers.Base;
using ViewModels.System;
using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.System;

[DependencyInjection(ServiceLifetime.Scoped)]
public interface I_Attachments
{
    Task<OperationResult<List<AttachmentVM>>> GetListAsync(int forumId);
    Task<OperationResult> DeleteAsync(int attachmentId);
}
