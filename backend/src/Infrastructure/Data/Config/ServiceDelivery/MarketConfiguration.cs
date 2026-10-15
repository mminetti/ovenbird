namespace Infrastructure.Data.Config.ServiceDelivery;

internal class MarketConfiguration : BaseEntityTypeConfiguration<Core.ServiceDelivery.Market, int>
{
    public override void Configure(EntityTypeBuilder<Core.ServiceDelivery.Market> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Identifier)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
    }
}
