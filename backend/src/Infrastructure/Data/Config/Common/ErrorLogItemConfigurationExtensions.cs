using Core.Common;

namespace Infrastructure.Data.Config.Common;

public static class ErrorLogItemConfigurationExtensions
{
    public static void ConfigureErrorLogItem<TItem>(this EntityTypeBuilder<TItem> builder)
        where TItem : ErrorLogItem
    {
        builder.Property(x => x.Field)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Message)
            .HasColumnType("nvarchar(max)");

        // NoAction: protect the referenced lookup row from accidental/cascading deletion.
        builder.HasOne(x => x.Severity)
            .WithMany()
            .HasForeignKey(x => x.SeverityId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
