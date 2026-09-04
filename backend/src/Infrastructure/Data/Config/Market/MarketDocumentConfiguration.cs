using Core.Market;

namespace Infrastructure.Data.Config.Market;

public class MarketDocumentConfiguration : BaseEntityTypeConfiguration<MarketDocument, long>
{
    public override void Configure(EntityTypeBuilder<MarketDocument> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Name)
            .HasMaxLength(500);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.Direction)
            .WithMany()
            .HasForeignKey(x => x.DirectionId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.Status)
            .WithMany()
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
