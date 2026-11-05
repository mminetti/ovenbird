using Core.Common;

namespace Infrastructure.Data.Config.Common;

public class CaseItemConfiguration : BaseEntityTypeConfiguration<CaseItem, long>
{
    public override void Configure(EntityTypeBuilder<CaseItem> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Field)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Message)
            .HasColumnType("nvarchar(max)");

        // Cascade: items are owned by the case, delete with it.
        builder.HasOne(x => x.Case)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.CaseId)
            .OnDelete(DeleteBehavior.Cascade);

        // NoAction: protect the referenced lookup row from accidental/cascading deletion.
        builder.HasOne(x => x.Severity)
            .WithMany()
            .HasForeignKey(x => x.SeverityId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
