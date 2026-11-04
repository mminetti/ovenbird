using Core.Processing;
using Infrastructure.Data.Config.Common;

namespace Infrastructure.Data.Config.Processing;

public class ProcessRunLogItemConfiguration : BaseEntityTypeConfiguration<ProcessRunLogItem, long>
{
    public override void Configure(EntityTypeBuilder<ProcessRunLogItem> builder)
    {
        base.Configure(builder);

        builder.ConfigureErrorLogItem();

        // NoAction: protect the referenced ProcessRun row from accidental/cascading deletion;
        // log items are removed explicitly alongside their run, never implicitly.
        builder.HasOne(x => x.ProcessRun)
            .WithMany(x => x.LogItems)
            .HasForeignKey(x => x.ProcessRunId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
