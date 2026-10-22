using Core.Common.Constants;
using Core.Processing;

namespace Infrastructure.Data.Config.Processing;

public class ProcessRunStatusConfiguration : IEntityTypeConfiguration<ProcessRunStatus>
{
    public void Configure(EntityTypeBuilder<ProcessRunStatus> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new ProcessRunStatus { Id = Constants.ProcessRunStatuses.Running, Name = nameof(Constants.ProcessRunStatuses.Running) },
            new ProcessRunStatus { Id = Constants.ProcessRunStatuses.Succeeded, Name = nameof(Constants.ProcessRunStatuses.Succeeded) },
            new ProcessRunStatus { Id = Constants.ProcessRunStatuses.Failed, Name = nameof(Constants.ProcessRunStatuses.Failed) });
    }
}
