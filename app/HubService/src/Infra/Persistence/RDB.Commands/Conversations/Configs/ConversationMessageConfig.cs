namespace Insurance.HubService.Infra.Persistence.RDB.Commands.Conversations.Configs;

using Insurance.HubService.Infra.Persistence.RDB.Commands.Conversations.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ConversationMessageConfig : IEntityTypeConfiguration<ConversationMessageEntity>
{
    public void Configure(EntityTypeBuilder<ConversationMessageEntity> builder)
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
