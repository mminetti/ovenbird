namespace Infrastructure.Data.Config;

public class ModuleConfiguration : BaseEntityTypeConfiguration<Core.Security.Module, int>
{
    public override void Configure(EntityTypeBuilder<Core.Security.Module> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
    }
}
