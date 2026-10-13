using Core.Subscription;

namespace Infrastructure.Data.Config.Subscription;

public class AccountConfiguration : BaseEntityTypeConfiguration<Account, long>
{
    public override void Configure(EntityTypeBuilder<Account> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Identifier)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.ServicePoint)
            .WithMany()
            .HasForeignKey(x => x.ServicePointId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.ServiceStatus)
            .WithMany()
            .HasForeignKey(x => x.ServiceStatusId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.ServiceSubStatus)
            .WithMany()
            .HasForeignKey(x => x.ServiceSubStatusId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.ServiceConnectionStatus)
            .WithMany()
            .HasForeignKey(x => x.ServiceConnectionStatusId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.ServiceStart)
            .WithMany()
            .HasForeignKey(x => x.ServiceStartId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
