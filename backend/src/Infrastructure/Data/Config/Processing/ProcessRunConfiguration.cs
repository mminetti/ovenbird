using Core.Processing;

namespace Infrastructure.Data.Config.Processing;

public class ProcessRunConfiguration : IEntityTypeConfiguration<ProcessRun>
{
    public void Configure(EntityTypeBuilder<ProcessRun> builder)
    {
        // NoAction: protect the referenced lookup row from accidental/cascading deletion.
        builder.HasOne(x => x.ProcessType)
            .WithMany()
            .HasForeignKey(x => x.ProcessTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced lookup row from accidental/cascading deletion.
        builder.HasOne(x => x.Status)
            .WithMany()
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
