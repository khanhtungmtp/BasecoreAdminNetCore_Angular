using API._Repositories;
using API.Helpers.Base;

namespace API._Services.Services
{
    public class BaseServices(IRepositoryAccessor repoStore)
    {
        protected readonly IRepositoryAccessor _repoStore = repoStore;
    }
}