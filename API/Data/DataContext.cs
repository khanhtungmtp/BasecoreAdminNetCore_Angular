using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext : IdentityDbContext<User>
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<IdentityRole>().Property(x => x.Id).HasMaxLength(50).IsUnicode(false);
        builder.Entity<User>().Property(x => x.Id).HasMaxLength(50).IsUnicode(false);

        builder.Entity<LabelInKnowledgeBase>()
                    .HasKey(c => new { c.LabelId, c.KnowledgeBaseId });

        builder.Entity<Permission>()
                   .HasKey(c => new { c.RoleId, c.FunctionId, c.CommandId });

        builder.Entity<Vote>()
                    .HasKey(c => new { c.KnowledgeBaseId, c.UserId });

        builder.Entity<CommandInFunction>()
                   .HasKey(c => new { c.CommandId, c.FunctionId });

        builder.HasSequence("KnowledgeBaseSequence");
    }

    public DbSet<Command> Commands => Set<Command>();
    public DbSet<CommandInFunction> CommandInFunctions => Set<CommandInFunction>();

    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Function> Functions => Set<Function>();
    public DbSet<KnowledgeBase> KnowledgeBases => Set<KnowledgeBase>();
    public DbSet<Label> Labels => Set<Label>();
    public DbSet<LabelInKnowledgeBase> LabelInKnowledgeBases => Set<LabelInKnowledgeBase>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<Vote> Votes => Set<Vote>();

    public DbSet<Attachment> Attachments => Set<Attachment>();
}
