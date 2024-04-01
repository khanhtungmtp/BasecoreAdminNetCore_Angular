using API.Models;
using Microsoft.EntityFrameworkCore.Storage;
using static API.Configurations.DependencyInjectionConfig;
namespace API._Repositories
{
    [DependencyInjection(ServiceLifetime.Scoped)]
    public interface IRepositoryAccessor
    {
        Task<bool> SaveAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        IRepository<Command> Commands { get; }
        IRepository<CommandInFunction> CommandInFunctions { get; }
        IRepository<ActivityLog> ActivityLogs { get; }
        IRepository<Category> Categories { get; }
        IRepository<Comment> Comments { get; }
        IRepository<Function> Functions { get; }
        IRepository<KnowledgeBase> KnowledgeBases { get; }
        IRepository<Label> Labels { get; }
        IRepository<LabelInKnowledgeBase> LabelInKnowledgeBases { get; }
        IRepository<Permission> Permissions { get; }
        IRepository<Report> Reports { get; }
        IRepository<Vote> Votes { get; }
        IRepository<Attachment> Attachments { get; }
    }
}