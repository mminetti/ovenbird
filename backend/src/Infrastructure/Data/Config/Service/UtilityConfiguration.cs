using Core.Service;

namespace Infrastructure.Data.Config.Service;

public class UtilityConfiguration : IEntityTypeConfiguration<Utility>
{
    public void Configure(EntityTypeBuilder<Utility> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.Market)
            .WithMany()
            .HasForeignKey(x => x.MarketId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
