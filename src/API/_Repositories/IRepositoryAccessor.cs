using API.Models;
using Microsoft.EntityFrameworkCore.Storage;
using static API.Configurations.DependencyInjectionConfig;
namespace API._Repositories
{
    [DependencyInjection(ServiceLifetime.Scoped)]
    public interface IRepositoryAccessor
    {
        Task<bool> SaveChangesAsync();
        bool SaveChanges();
        Task<IDbContextTransaction> BeginTransactionAsync();
        IRepository<User> Users { get; }
        IRepository<Command> Commands { get; }
        IRepository<CommandInFunction> CommandInFunctions { get; }
        IRepository<ActivityLog> ActivityLogs { get; }
        IRepository<Category> Categories { get; }
        IRepository<Comment> Comments { get; }
        IRepository<Function> Functions { get; }
        IRepository<Forum> Forums { get; }
        IRepository<Label> Labels { get; }
        IRepository<LabelInForum> LabelInForums { get; }
        IRepository<Permission> Permissions { get; }
        IRepository<Report> Reports { get; }
        IRepository<Vote> Votes { get; }
        IRepository<Attachment> Attachments { get; }
        IRepository<SystemLanguage> SystemLanguages { get; }
        IRepository<RefreshToken> RefreshTokens { get; }
    }
}