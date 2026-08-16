using Core.ContributorAggregate;

namespace Infrastructure.Data.Config;

public class ContributorConfiguration : IEntityTypeConfiguration<Contributor>
{
    public void Configure(EntityTypeBuilder<Contributor> builder)
    {
        builder.Property(entity => entity.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.OwnsOne(builder => builder.PhoneNumber);

        builder.Property(x => x.Status)
            .HasConversion(
                x => x.Value,
                x => ContributorStatus.FromValue(x));
    }
}
