using Core.ServiceDelivery;

namespace Infrastructure.Data.Config.ServiceDelivery;

public class ServiceStartItemConfiguration : BaseEntityTypeConfiguration<ServiceStartItem, long>
{
    public override void Configure(EntityTypeBuilder<ServiceStartItem> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Identifier)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Description)
            .HasMaxLength(DataSchemaConstants.DEFAULT_DESCRIPTION_LENGTH);

        // Cascade: items are owned by the service start, delete with it.
        builder.HasOne(x => x.ServiceStart)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.ServiceStartId)
            .OnDelete(DeleteBehavior.Cascade);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.ServiceAction)
            .WithMany()
            .HasForeignKey(x => x.ServiceActionId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
