using Core.Common;
using Core.Common.Constants;

namespace Infrastructure.Data.Config.Common;

public class ErrorSeverityConfiguration : IEntityTypeConfiguration<ErrorSeverity>
{
    public void Configure(EntityTypeBuilder<ErrorSeverity> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new ErrorSeverity { Id = Constants.ErrorSeverities.Error, Name = nameof(Constants.ErrorSeverities.Error) },
            new ErrorSeverity { Id = Constants.ErrorSeverities.Warning, Name = nameof(Constants.ErrorSeverities.Warning) },
            new ErrorSeverity { Id = Constants.ErrorSeverities.Info, Name = nameof(Constants.ErrorSeverities.Info) });
    }
}
