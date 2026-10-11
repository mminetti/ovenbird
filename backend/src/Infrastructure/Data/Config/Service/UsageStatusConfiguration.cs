using Core.Common.Constants;
using Core.Service;

namespace Infrastructure.Data.Config.Service;

public class UsageStatusConfiguration : IEntityTypeConfiguration<UsageStatus>
{
    public void Configure(EntityTypeBuilder<UsageStatus> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new UsageStatus { Id = Constants.UsageStatuses.New, Name = nameof(Constants.UsageStatuses.New) });
    }
}
