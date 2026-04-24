namespace Insurance.HubService.Infra.Persistence.RDB.Commands.Conversations.Configs;

using Insurance.HubService.Infra.Persistence.RDB.Commands.Conversations.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ConversationSessionConfig : IEntityTypeConfiguration<ConversationSessionEntity>
{
    public void Configure(EntityTypeBuilder<ConversationSessionEntity> builder)
    {
        builder.ToTable("ConversationSessions");

        builder.HasKey(x => x.SessionId);
        builder.Property(x => x.SessionId).HasMaxLength(64);
        builder.Property(x => x.UserId).HasMaxLength(128).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.LastActivityUtc).IsRequired();

        builder
            .HasMany(x => x.Messages)
            .WithOne(x => x.Session)
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.LastActivityUtc);
    }
}
