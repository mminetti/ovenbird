namespace Infrastructure.Data.Config.Service;

internal class MarketConfiguration : BaseEntityTypeConfiguration<Core.Service.Market, int>
{
    public override void Configure(EntityTypeBuilder<Core.Service.Market> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Identifier)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
    }
}
