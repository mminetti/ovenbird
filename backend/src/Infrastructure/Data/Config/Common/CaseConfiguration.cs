using Core.Common;

namespace Infrastructure.Data.Config.Common;

public class CaseConfiguration : BaseEntityTypeConfiguration<Case, long>
{
    public override void Configure(EntityTypeBuilder<Case> builder)
    {
        base.Configure(builder);

        // NoAction: protect the referenced lookup row from accidental/cascading deletion.
        builder.HasOne(x => x.EntityType)
            .WithMany()
            .HasForeignKey(x => x.EntityTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced lookup row from accidental/cascading deletion.
        builder.HasOne(x => x.Status)
            .WithMany()
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: unassigning a deleted user should not take the case with it.
        builder.HasOne(x => x.AssignedToUser)
            .WithMany()
            .HasForeignKey(x => x.AssignedToUserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
