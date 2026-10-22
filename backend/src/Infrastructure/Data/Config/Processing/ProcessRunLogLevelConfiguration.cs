using Core.Common.Constants;
using Core.Processing;

namespace Infrastructure.Data.Config.Processing;

public class ProcessRunLogLevelConfiguration : IEntityTypeConfiguration<ProcessRunLogLevel>
{
    public void Configure(EntityTypeBuilder<ProcessRunLogLevel> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new ProcessRunLogLevel { Id = Constants.ProcessRunLogLevels.Error, Name = nameof(Constants.ProcessRunLogLevels.Error) },
            new ProcessRunLogLevel { Id = Constants.ProcessRunLogLevels.Warning, Name = nameof(Constants.ProcessRunLogLevels.Warning) },
            new ProcessRunLogLevel { Id = Constants.ProcessRunLogLevels.Info, Name = nameof(Constants.ProcessRunLogLevels.Info) });
    }
}
