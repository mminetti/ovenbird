namespace Infrastructure.Data.Config.Market;

internal class MarketConfiguration : IEntityTypeConfiguration<Core.Market.Market>
{
    public void Configure(EntityTypeBuilder<Core.Market.Market> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Identifier)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
    }
}
