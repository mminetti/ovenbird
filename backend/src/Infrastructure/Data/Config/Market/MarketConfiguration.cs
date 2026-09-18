namespace Infrastructure.Data.Config.Market;

internal class MarketConfiguration : BaseEntityTypeConfiguration<Core.Market.Market, int>
{
    public override void Configure(EntityTypeBuilder<Core.Market.Market> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Identifier)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
    }
}
