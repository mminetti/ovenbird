using Core.ServiceDelivery;

namespace Infrastructure.Data.Config.ServiceDelivery;

public class ServiceEventActionConfiguration : BaseEntityTypeConfiguration<ServiceEventAction, long>
{
    public override void Configure(EntityTypeBuilder<ServiceEventAction> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Message)
            .HasColumnType("nvarchar(max)");

        // Cascade: actions are owned by the service event, delete with it.
        builder.HasOne(x => x.ServiceEvent)
            .WithMany(x => x.Actions)
            .HasForeignKey(x => x.ServiceEventId)
            .OnDelete(DeleteBehavior.Cascade);

        // NoAction: protect the referenced lookup row from accidental/cascading deletion.
        builder.HasOne(x => x.ActionType)
            .WithMany()
            .HasForeignKey(x => x.ActionTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced lookup row from accidental/cascading deletion.
        builder.HasOne(x => x.Status)
            .WithMany()
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
