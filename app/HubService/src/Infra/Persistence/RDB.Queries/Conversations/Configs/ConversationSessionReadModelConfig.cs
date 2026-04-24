namespace Insurance.HubService.Infra.Persistence.RDB.Queries.Conversations.Configs;

using Insurance.HubService.Infra.Persistence.RDB.Queries.Conversations.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ConversationSessionReadModelConfig : IEntityTypeConfiguration<ConversationSessionReadModel>
{
    public void Configure(EntityTypeBuilder<ConversationSessionReadModel> builder)
    {
        builder.ToTable("ConversationSessions");

        builder.HasKey(x => x.SessionId);
        builder.Property(x => x.SessionId).HasMaxLength(64);
        builder.Property(x => x.UserId).HasMaxLength(128).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.LastActivityUtc).IsRequired();
    }
}
