using Core.Shared;

namespace Infrastructure.Data.Config.Shared;

public class ConnectorConfiguration : BaseEntityTypeConfiguration<Connector, int>
{
    public override void Configure(EntityTypeBuilder<Connector> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Description)
            .HasMaxLength(DataSchemaConstants.DEFAULT_DESCRIPTION_LENGTH);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.ConnectorType)
            .WithMany(x => x.Connectors)
            .HasForeignKey(x => x.ConnectorTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.ConnectorImplementation)
            .WithMany(x => x.Connectors)
            .HasForeignKey(x => x.ConnectorImplementationId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
