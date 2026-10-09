using Core.Market;

namespace Infrastructure.Data.Config.Market;

public class MarketDocumentReferenceConfiguration : BaseEntityTypeConfiguration<MarketDocumentReference, long>
{
    public override void Configure(EntityTypeBuilder<MarketDocumentReference> builder)
    {
        base.Configure(builder);

        // Cascade: reference rows are owned by the item, delete with it.
        builder.HasOne(x => x.MarketDocumentItem)
            .WithMany()
            .HasForeignKey(x => x.MarketDocumentItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.ReferenceType)
            .WithMany()
            .HasForeignKey(x => x.ReferenceTypeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
