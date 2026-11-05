using Core.Common;
using Core.Common.Constants;

namespace Infrastructure.Data.Config.Common;

public class CaseStatusConfiguration : IEntityTypeConfiguration<CaseStatus>
{
    public void Configure(EntityTypeBuilder<CaseStatus> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new CaseStatus { Id = Constants.CaseStatuses.Open, Name = nameof(Constants.CaseStatuses.Open) },
            new CaseStatus { Id = Constants.CaseStatuses.InProgress, Name = nameof(Constants.CaseStatuses.InProgress) },
            new CaseStatus { Id = Constants.CaseStatuses.Resolved, Name = nameof(Constants.CaseStatuses.Resolved) },
            new CaseStatus { Id = Constants.CaseStatuses.Cancelled, Name = nameof(Constants.CaseStatuses.Cancelled) });
    }
}
