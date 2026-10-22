using Core.Processing;

namespace Infrastructure.Data.Config.Processing;

public class ProcessRunLogItemConfiguration : IEntityTypeConfiguration<ProcessRunLogItem>
{
    public void Configure(EntityTypeBuilder<ProcessRunLogItem> builder)
    {
        builder.Property(x => x.EntityType)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.EntityId)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Field)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Message)
            .HasColumnType("nvarchar(max)");

        builder.HasIndex(x => new { x.EntityType, x.EntityId });

        // NoAction: protect the referenced ProcessRun row from accidental/cascading deletion;
        // log items are removed explicitly alongside their run, never implicitly.
        builder.HasOne(x => x.ProcessRun)
            .WithMany(x => x.LogItems)
            .HasForeignKey(x => x.ProcessRunId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced lookup row from accidental/cascading deletion.
        builder.HasOne(x => x.Level)
            .WithMany()
            .HasForeignKey(x => x.LevelId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
