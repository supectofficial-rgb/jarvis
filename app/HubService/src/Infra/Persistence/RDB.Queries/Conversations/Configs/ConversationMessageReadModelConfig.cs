namespace Insurance.HubService.Infra.Persistence.RDB.Queries.Conversations.Configs;

using Insurance.HubService.Infra.Persistence.RDB.Queries.Conversations.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ConversationMessageReadModelConfig : IEntityTypeConfiguration<ConversationMessageReadModel>
{
    public void Configure(EntityTypeBuilder<ConversationMessageReadModel> builder)
    {
        builder.ToTable("ConversationMessages");

        builder.HasKey(x => x.MessageId);
        builder.Property(x => x.MessageId).HasMaxLength(64);
        builder.Property(x => x.SessionId).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Content).HasMaxLength(4000).IsRequired();
        builder.Property(x => x.Role).IsRequired();
        builder.Property(x => x.TimestampUtc).IsRequired();

        builder.HasIndex(x => new { x.SessionId, x.TimestampUtc });
    }
}
