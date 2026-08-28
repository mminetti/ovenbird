using Core.Shared;

namespace Infrastructure.Data.Config.Shared;

public class ConnectorFieldConfiguration : BaseEntityTypeConfiguration<ConnectorField, int>
{
    public override void Configure(EntityTypeBuilder<ConnectorField> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Value)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasOne(x => x.Connector)
            .WithMany(x => x.ConnectorFields)
            .HasForeignKey(x => x.ConnectorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
