using Core.ServiceDelivery;

namespace Infrastructure.Data.Config.ServiceDelivery;

public class ServiceStartConfiguration : BaseEntityTypeConfiguration<ServiceStart, long>
{
    public override void Configure(EntityTypeBuilder<ServiceStart> builder)
    {
        base.Configure(builder);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.Service)
            .WithMany(x => x.ServiceStarts)
            .HasForeignKey(x => x.ServiceId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.ServiceStartType)
            .WithMany()
            .HasForeignKey(x => x.ServiceStartTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.Status)
            .WithMany()
            .HasForeignKey(x => x.ServiceOrderStatusId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
