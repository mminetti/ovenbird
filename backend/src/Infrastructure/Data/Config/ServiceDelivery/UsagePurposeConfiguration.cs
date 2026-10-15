using Core.Common.Constants;
using Core.ServiceDelivery;

namespace Infrastructure.Data.Config.ServiceDelivery;

public class UsagePurposeConfiguration : IEntityTypeConfiguration<UsagePurpose>
{
    public void Configure(EntityTypeBuilder<UsagePurpose> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new UsagePurpose { Id = Constants.UsagePurposes.Original, Name = nameof(Constants.UsagePurposes.Original) },
            new UsagePurpose { Id = Constants.UsagePurposes.Cancel, Name = nameof(Constants.UsagePurposes.Cancel) },
            new UsagePurpose { Id = Constants.UsagePurposes.Replace, Name = nameof(Constants.UsagePurposes.Replace) });
    }
}
