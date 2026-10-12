using Core.Service;

namespace Infrastructure.Data.Config.Service;

public class ServiceStartConfiguration : BaseEntityTypeConfiguration<ServiceStart, long>
{
    public override void Configure(EntityTypeBuilder<ServiceStart> builder)
    {
        base.Configure(builder);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.Account)
            .WithMany()
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.ServiceStartType)
            .WithMany()
            .HasForeignKey(x => x.ServiceStartTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.ServiceStatus)
            .WithMany()
            .HasForeignKey(x => x.ServiceOrderStatusId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
