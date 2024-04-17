using API.Models;
using API.Models.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace API.Data;

public class DataContext(DbContextOptions options) : IdentityDbContext<User>(options)
{

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<EntityEntry> modified = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified || e.State == EntityState.Added);
        foreach (EntityEntry item in modified)
        {
            if (item.Entity is IDateTracking changedOrAddedItem)
            {
                if (item.State == EntityState.Added)
                {
                    changedOrAddedItem.CreatedDate = DateTime.Now;
                }
                else
                {
                    changedOrAddedItem.UpdatedDate = DateTime.Now;
                }
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<IdentityRole>().Property(x => x.Id).HasMaxLength(50).IsUnicode(false);
        builder.Entity<User>().Property(x => x.Id).HasMaxLength(50).IsUnicode(false);

        builder.Entity<LabelInForum>()
                    .HasKey(c => new { c.LabelId, c.ForumId });

        builder.Entity<Permission>()
                   .HasKey(c => new { c.RoleId, c.FunctionId, c.CommandId });

        builder.Entity<Vote>()
                    .HasKey(c => new { c.ForumId, c.UserId });

        builder.Entity<CommandInFunction>()
                   .HasKey(c => new { c.CommandId, c.FunctionId });

        builder.HasSequence("Forumsequence");
    }

    public DbSet<Command> Commands => Set<Command>();
    public DbSet<CommandInFunction> CommandInFunctions => Set<CommandInFunction>();

    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Function> Functions => Set<Function>();
    public DbSet<Forum> Forums => Set<Forum>();
    public DbSet<Label> Labels => Set<Label>();
    public DbSet<LabelInForum> LabelInForums => Set<LabelInForum>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<Vote> Votes => Set<Vote>();
public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<SystemLanguage> SystemLanguages => Set<SystemLanguage>();
}
