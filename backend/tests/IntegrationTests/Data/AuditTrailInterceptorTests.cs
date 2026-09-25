using System.Text.Json;
using Core.Common;
using Core.Market;
using Core.Security;
using Core.Shared;
using Infrastructure.Data.Queries.Common;

namespace IntegrationTests.Data;

public class AuditTrailInterceptorTests : BaseEfRepoTestFixture
{
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task CreateUpdateDeleteCompany_ProducesAuditRows()
    {
        var market = await CreateMarketAsync();

        var company = new Company { Name = "Acme", MarketId = market.Id, TimeZoneId = "UTC" };
        _dbContext.Add(company);
        await _dbContext.SaveChangesAsync(Ct);

        var createRow = await SingleAuditRowAsync(nameof(Company), company.Id.ToString(), AuditAction.Create);
        createRow.OldValues.ShouldBeNull();
        createRow.NewValues.ShouldNotBeNull();
        createRow.NewValues!.ShouldContain("\"Name\":\"Acme\"");
        createRow.NewValues!.ShouldNotContain("CreatedAtUtc");
        createRow.NewValues!.ShouldNotContain("LastModifiedBy");
        createRow.AffectedColumns.ShouldBeNull();
        createRow.References.ShouldBeEmpty();
        createRow.UserId.ShouldBe("test-user");

        company.Name = "Acme Corp";
        await _dbContext.SaveChangesAsync(Ct);

        var updateRow = await SingleAuditRowAsync(nameof(Company), company.Id.ToString(), AuditAction.Update);
        updateRow.OldValues!.ShouldContain("\"Name\":\"Acme\"");
        updateRow.NewValues!.ShouldContain("\"Name\":\"Acme Corp\"");

        // Only Name was actually changed; repository.UpdateAsync's call to DbContext.Update
        // marks every scalar property IsModified, so this guards against that leaking through
        // as spurious affected columns (e.g. MarketId, TimeZoneId, which were never touched).
        JsonSerializer.Deserialize<string[]>(updateRow.AffectedColumns!).ShouldBe(["Name"]);

        var companyId = company.Id;
        _dbContext.Remove(company);
        await _dbContext.SaveChangesAsync(Ct);

        var deleteRow = await SingleAuditRowAsync(nameof(Company), companyId.ToString(), AuditAction.Delete);
        deleteRow.NewValues.ShouldBeNull();
        deleteRow.OldValues!.ShouldContain("\"Name\":\"Acme Corp\"");
    }

    [Fact]
    public async Task CreateUpdateDeleteMarketDocument_ProducesNoAuditRows()
    {
        // MarketDocument extends AuditableEntityBase (gets CreatedBy/LastModifiedBy stamps)
        // but does not implement IAudited, so it must not show up in the audit trail.
        var market = await CreateMarketAsync();
        var company = await CreateCompanyAsync(market.Id);

        var direction = new MarketDocumentDirection { Name = "Inbound" };
        var status = new MarketDocumentStatus { Name = "Active" };
        _dbContext.AddRange(direction, status);
        await _dbContext.SaveChangesAsync(Ct);

        var document = new MarketDocument
        {
            Name = "Doc",
            File = "doc.pdf",
            DirectionId = direction.Id,
            CompanyId = company.Id,
            StatusId = status.Id
        };
        _dbContext.Add(document);
        await _dbContext.SaveChangesAsync(Ct);

        document.Name = "Doc Updated";
        await _dbContext.SaveChangesAsync(Ct);

        _dbContext.Remove(document);
        await _dbContext.SaveChangesAsync(Ct);

        var rows = await _dbContext.Set<AuditTrail>()
            .Where(a => a.EntityType == nameof(MarketDocument))
            .ToListAsync(Ct);

        rows.ShouldBeEmpty();
    }

    [Fact]
    public async Task AddingRemovingRoleFromUser_ProducesUserRoleAuditRowsWithBothReferences()
    {
        var role = new Role { Name = "Admin", Description = "Admin role" };
        _dbContext.Add(role);
        await _dbContext.SaveChangesAsync(Ct);

        var user = new User
        {
            ExternalIdentifier = Guid.NewGuid().ToString(),
            Name = "User",
            Email = "user@test.com",
            IsActive = true
        };
        _dbContext.Add(user);
        await _dbContext.SaveChangesAsync(Ct);

        // Join table key order is (RoleId, UserId) - see UserConfiguration.HasKey.
        var joinEntityId = $"{role.Id}:{user.Id}";

        user.AddRole(role);
        await _dbContext.SaveChangesAsync(Ct);

        var addRow = await SingleAuditRowAsync("UserRole", joinEntityId, AuditAction.Create);
        addRow.References
            .Select(r => (r.ReferencedEntityType, r.ReferencedEntityId))
            .ShouldBe([(nameof(User), user.Id.ToString()), (nameof(Role), role.Id.ToString())], ignoreOrder: true);

        user.RemoveRole(role.Id);
        await _dbContext.SaveChangesAsync(Ct);

        var removeRow = await SingleAuditRowAsync("UserRole", joinEntityId, AuditAction.Delete);
        removeRow.References.Count.ShouldBe(2);
    }

    [Fact]
    public async Task AddingRemovingPermissionFromRole_ProducesRolePermissionAuditRowsWithBothReferences()
    {
        var permission = new Permission
        {
            Id = 1000,
            Name = "test.permission",
            ModuleId = UseCases.Common.Constants.PermissionModules.Security,
            Description = "Test permission"
        };
        _dbContext.Add(permission);
        await _dbContext.SaveChangesAsync(Ct);

        var role = new Role { Name = "Editor", Description = "Editor role" };
        _dbContext.Add(role);
        await _dbContext.SaveChangesAsync(Ct);

        // Join table key order is (PermissionId, RoleId) - see RoleConfiguration.HasKey.
        var joinEntityId = $"{permission.Id}:{role.Id}";

        role.AddPermission(permission);
        await _dbContext.SaveChangesAsync(Ct);

        var addRow = await SingleAuditRowAsync("RolePermission", joinEntityId, AuditAction.Create);
        addRow.References
            .Select(r => (r.ReferencedEntityType, r.ReferencedEntityId))
            .ShouldBe([(nameof(Role), role.Id.ToString()), (nameof(Permission), permission.Id.ToString())], ignoreOrder: true);

        role.RemovePermission(permission.Id);
        await _dbContext.SaveChangesAsync(Ct);

        await SingleAuditRowAsync("RolePermission", joinEntityId, AuditAction.Delete);
    }

    [Fact]
    public async Task AddingUpdatingConfigurationField_RollsUpToConfigurationAndIsQueryable()
    {
        var configurationType = new ConfigurationType { Name = "Type" };
        _dbContext.Add(configurationType);
        await _dbContext.SaveChangesAsync(Ct);

        var configuration = new Configuration { Name = "Config", ConfigurationTypeId = configurationType.Id };
        _dbContext.Add(configuration);
        await _dbContext.SaveChangesAsync(Ct);

        var field = new ConfigurationField { ConfigurationId = configuration.Id, Name = "Field", Value = "1" };
        _dbContext.Add(field);
        await _dbContext.SaveChangesAsync(Ct);

        var createRow = await SingleAuditRowAsync(nameof(ConfigurationField), field.Id.ToString(), AuditAction.Create);
        var reference = createRow.References.ShouldHaveSingleItem();
        reference.ReferencedEntityType.ShouldBe(nameof(Configuration));
        reference.ReferencedEntityId.ShouldBe(configuration.Id.ToString());

        field.Value = "2";
        await _dbContext.SaveChangesAsync(Ct);

        // Configuration itself was never directly modified by either save above (it does get
        // its own Create row from when it was created, hence filtering on Action here).
        (await _dbContext.Set<AuditTrail>().CountAsync(a => a.EntityType == nameof(Configuration) && a.Action == AuditAction.Update, Ct))
            .ShouldBe(0);

        var queryService = new ListAuditTrailQueryService(GetReadDbContext());
        var history = await queryService.ListAsync(nameof(Configuration), configuration.Id.ToString(), 1, 10, Ct);

        // Configuration's own Create row, plus the ConfigurationField Create and Update rows
        // that reference it.
        history.TotalCount.ShouldBe(3);
        history.Items.ShouldContain(x => x.EntityType == nameof(ConfigurationField) && x.EntityId == field.Id.ToString() && x.Action == AuditAction.Create);
        history.Items.ShouldContain(x => x.EntityType == nameof(ConfigurationField) && x.EntityId == field.Id.ToString() && x.Action == AuditAction.Update);
    }

    [Fact]
    public async Task AddingUpdatingConnectorField_RollsUpToConnectorAndIsQueryable()
    {
        var connectorType = new ConnectorType { Name = "Type" };
        _dbContext.Add(connectorType);
        await _dbContext.SaveChangesAsync(Ct);

        var connectorImplementation = new ConnectorImplementation
        {
            Name = "Implementation",
            Identifier = "impl",
            ConnectorTypeId = connectorType.Id
        };
        _dbContext.Add(connectorImplementation);
        await _dbContext.SaveChangesAsync(Ct);

        var connector = new Connector { Name = "Connector", ConnectorImplementationId = connectorImplementation.Id };
        _dbContext.Add(connector);
        await _dbContext.SaveChangesAsync(Ct);

        var field = new ConnectorField { ConnectorId = connector.Id, Name = "Field", Value = "1" };
        _dbContext.Add(field);
        await _dbContext.SaveChangesAsync(Ct);

        var createRow = await SingleAuditRowAsync(nameof(ConnectorField), field.Id.ToString(), AuditAction.Create);
        var reference = createRow.References.ShouldHaveSingleItem();
        reference.ReferencedEntityType.ShouldBe(nameof(Connector));
        reference.ReferencedEntityId.ShouldBe(connector.Id.ToString());

        var queryService = new ListAuditTrailQueryService(GetReadDbContext());
        var history = await queryService.ListAsync(nameof(Connector), connector.Id.ToString(), 1, 10, Ct);

        history.Items.ShouldContain(x => x.EntityType == nameof(ConnectorField) && x.EntityId == field.Id.ToString());
    }

    private async Task<Market> CreateMarketAsync()
    {
        var market = new Market { Name = "Market", Identifier = Guid.NewGuid().ToString() };
        _dbContext.Add(market);
        await _dbContext.SaveChangesAsync(Ct);
        return market;
    }

    private async Task<Company> CreateCompanyAsync(int marketId)
    {
        var company = new Company { Name = "Company", MarketId = marketId, TimeZoneId = "UTC" };
        _dbContext.Add(company);
        await _dbContext.SaveChangesAsync(Ct);
        return company;
    }

    private async Task<AuditTrail> SingleAuditRowAsync(string entityType, string entityId, AuditAction action)
    {
        var rows = await _dbContext.Set<AuditTrail>()
            .Where(a => a.EntityType == entityType && a.EntityId == entityId && a.Action == action)
            .ToListAsync(Ct);

        rows.Count.ShouldBe(1);
        return rows[0];
    }
}
