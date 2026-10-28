using Core.ServiceDelivery;

namespace Infrastructure.Data.Config.ServiceDelivery;

public class ServiceEventConfiguration : BaseEntityTypeConfiguration<ServiceEvent, long>
{
    public override void Configure(EntityTypeBuilder<ServiceEvent> builder)
    {
        base.Configure(builder);

        // NoAction: protect the referenced lookup row from accidental/cascading deletion.
        builder.HasOne(x => x.EventType)
            .WithMany()
            .HasForeignKey(x => x.EventTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced lookup row from accidental/cascading deletion.
        builder.HasOne(x => x.ReferenceType)
            .WithMany()
            .HasForeignKey(x => x.ReferenceTypeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
