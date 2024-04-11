using API.Helpers.Constants;
using Microsoft.AspNetCore.Mvc;

namespace API.Filters.Authorization;

public class ClaimRequirementAttribute: TypeFilterAttribute
{
    public ClaimRequirementAttribute(FunctionCode functionId, CommandCode commandId)
        : base(typeof(ClaimRequirementFilter))
    {
        Arguments = new object[] { functionId, commandId };
    }
}