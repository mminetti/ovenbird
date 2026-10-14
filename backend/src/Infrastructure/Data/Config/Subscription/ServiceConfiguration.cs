namespace Infrastructure.Data.Config.Subscription;

public class ServiceConfiguration : BaseEntityTypeConfiguration<Core.Subscription.Service, long>
{
    public override void Configure(EntityTypeBuilder<Core.Subscription.Service> builder)
    {
        base.Configure(builder);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.Account)
            .WithOne(x => x.Service)
            .HasForeignKey<Core.Subscription.Service>(x => x.AccountId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.ServicePoint)
            .WithMany()
            .HasForeignKey(x => x.ServicePointId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.Status)
            .WithMany()
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.SubStatus)
            .WithMany()
            .HasForeignKey(x => x.SubStatusId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.ConnectionStatus)
            .WithMany()
            .HasForeignKey(x => x.ConnectionStatusId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
