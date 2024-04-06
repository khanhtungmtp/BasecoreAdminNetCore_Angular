using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore.Storage;
namespace API._Repositories
{
    public class RepositoryAccessor : IRepositoryAccessor
    {
        private readonly DataContext _dbContext;

        public RepositoryAccessor(DataContext dbContext)
        {
            _dbContext = dbContext;
            Functions = new Repository<Function, DataContext>(_dbContext);
            CommandInFunctions = new Repository<CommandInFunction, DataContext>(_dbContext);
            Commands = new Repository<Command, DataContext>(_dbContext);
            Permissions = new Repository<Permission, DataContext>(_dbContext);
            Forums = new Repository<Forum, DataContext>(_dbContext);
            // Categories = new Repository<Category, DataContext>(_dbContext);
        }

        public IRepository<Command> Commands { get; set; } = default!;

        public IRepository<CommandInFunction> CommandInFunctions { get; set; } = default!;

        public IRepository<ActivityLog> ActivityLogs { get; set; } = default!;

        public IRepository<Category> Categories { get; set; } = default!;

        public IRepository<Comment> Comments { get; set; } = default!;

        public IRepository<Function> Functions { get; set; } = default!;

        public IRepository<Forum> Forums { get; set; } = default!;

        public IRepository<Label> Labels { get; set; } = default!;

        public IRepository<LabelInForum> LabelInForums { get; set; } = default!;

        public IRepository<Permission> Permissions { get; set; } = default!;

        public IRepository<Report> Reports { get; set; } = default!;

        public IRepository<Vote> Votes { get; set; } = default!;


        public IRepository<Attachment> Attachments { get; set; } = default!;

        public async Task<bool> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _dbContext.Database.BeginTransactionAsync();
        }
    }
}