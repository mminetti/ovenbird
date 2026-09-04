using Core.Shared;

namespace Infrastructure.Data.Config.Shared;

public class CompanyConfiguration : BaseEntityTypeConfiguration<Company, int>
{
    public override void Configure(EntityTypeBuilder<Company> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.TimeZoneId)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.Market)
            .WithMany(x => x.Companies)
            .HasForeignKey(x => x.MarketId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
