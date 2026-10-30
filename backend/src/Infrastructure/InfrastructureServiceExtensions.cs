using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Core.Security;
using Core.Security.Interfaces;
using Core.Security.Services;
using Core.ServiceDelivery;
using Core.Settings;
using Infrastructure.Data;
using Infrastructure.Data.Interceptors;
using Infrastructure.Data.Queries.Auditing;
using Infrastructure.Data.Queries.Common;
using Infrastructure.Data.Queries.DataLists;
using Infrastructure.Data.Queries.Markets;
using Infrastructure.Data.Queries.Security;
using Infrastructure.Data.Queries.Settings;
using Infrastructure.Services.Files;
using Infrastructure.Services.Secrets;
using Infrastructure.Services.ServiceDelivery;
using Infrastructure.Services.ServiceDelivery.BigData;
using Infrastructure.Services.ServiceDelivery.BigData.Builders;
using Infrastructure.Services.ServiceDelivery.BigData.Readers;
using UseCases.Auditing.List;
using UseCases.Common;
using UseCases.Common.DataLists.Get;
using UseCases.Interfaces.Files;
using UseCases.Interfaces.Secrets;
using UseCases.Interfaces.ServiceDelivery;
using UseCases.Market.MarketDocuments.Export.Interfaces;
using UseCases.Market.MarketDocuments.Import.Interfaces;
using UseCases.Market.Markets.List;
using UseCases.Security.Permissions.List;
using UseCases.Security.Roles.List;
using UseCases.Security.Users;
using UseCases.Security.Users.List;
using UseCases.Settings.Companies.List;
using UseCases.Settings.Configurations.List;
using UseCases.Settings.Connectors.List;

namespace Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
      this IServiceCollection services,
      ConfigurationManager config,
      ILogger logger)
    {
        string? connectionString = config.GetConnectionString("db") ?? config.GetConnectionString("DefaultConnection");

        string? readConnectionString = config.GetConnectionString("db") ?? config.GetConnectionString("ReadConnection");

        Guard.Against.Null(connectionString);

        services.AddHybridCache();
        services.AddSingleton(TimeProvider.System);

        services.AddScoped<IUser, SystemUser>();
        services.AddScoped<EventDispatchInterceptor>();
        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<AuditTrailInterceptor>();
        services.AddScoped<IDomainEventDispatcher, WolverineDomainEventDispatcher>();

        // Register write DbContext
        services.AddDbContext<AppDbContext>((provider, options) =>
        {
            var eventDispatchInterceptor = provider.GetRequiredService<EventDispatchInterceptor>();
            var auditableEntityInterceptor = provider.GetRequiredService<AuditableEntityInterceptor>();
            var auditTrailInterceptor = provider.GetRequiredService<AuditTrailInterceptor>();

            options.UseSqlServer(connectionString);
            options.AddInterceptors(eventDispatchInterceptor, auditableEntityInterceptor, auditTrailInterceptor);
        });

        // Register read DbContext
        services.AddDbContext<ReadDbContext>(options =>
        {
            options.UseSqlServer(readConnectionString);
        });

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>))
            .AddScoped(typeof(IReadRepository<>), typeof(EfReadRepository<>))
            .AddSingleton<IQueryPropertyMapper<User>, UserQueryPropertyMapper>()
            .AddSingleton<IQueryPropertyMapper<Role>, RoleQueryPropertyMapper>()
            .AddSingleton<IQueryPropertyMapper<Permission>, PermissionQueryPropertyMapper>()
            .AddSingleton<IQueryPropertyMapper<Connector>, ConnectorQueryPropertyMapper>()
            .AddSingleton<IQueryPropertyMapper<Company>, CompanyQueryPropertyMapper>()
            .AddSingleton<IQueryPropertyMapper<Market>, MarketQueryPropertyMapper>()
            .AddSingleton<IQueryPropertyMapper<Configuration>, ConfigurationQueryPropertyMapper>()
            .AddScoped<IListUsersQueryService, ListUsersQueryService>()
            .AddScoped<IListRolesQueryService, ListRolesQueryService>()
            .AddScoped<IListPermissionsQueryService, ListPermissionsQueryService>()
            .AddScoped<IListConnectorsQueryService, ListConnectorsQueryService>()
            .AddScoped<IListCompaniesQueryService, ListCompaniesQueryService>()
            .AddScoped<IListMarketsQueryService, ListMarketsQueryService>()
            .AddScoped<IListConfigurationsQueryService, ListConfigurationsQueryService>()
            .AddScoped<IListAuditTrailQueryService, ListAuditTrailQueryService>()
            .AddScoped<IGetDataListQueryService, GetDataListQueryService>()
            .AddScoped<IDeleteUserService, DeleteUserService>()
            .AddScoped<IFtpService, FluentFtpService>()
            .AddScoped<IFtpService, SshNetSftpService>()
            .AddScoped<IFtpService, LocalFileSystemFtpService>()
            .AddScoped<IFileStorage, AzureBlobFileStorage>()
            .AddScoped<IFileStorage, LocalFileSystemFileStorage>()
            .AddScoped<IConnectorFtpTransport, ConnectorFtpTransport>()
            .AddScoped<IConnectorFileStorageTransport, ConnectorFileStorageTransport>()
            .AddScoped<IEdiImportHandler, BigDataEdiImportHandler>()
            .AddScoped<EdiImportHandlerResolver>()
            .AddScoped<IEdiExportHandler, BigDataEdiExportHandler>()
            .AddScoped<EdiExportHandlerResolver>()
            .AddScoped<IMarketDocumentParser, BigDataDocumentImportParser>()
            .AddScoped<MarketDocumentParserResolver>()
            .AddScoped<MarketDocumentEntityBuilderResolver>()
            .AddScoped<IMarketDocumentGenerator, BigDataDocumentExportGenerator>()
            .AddScoped<MarketDocumentGeneratorResolver>()
            .AddScoped<MarketDocumentEntityGeneratorResolver>()
            .AddScoped<IBigDataValueMapper, BigDataValueMapper>()
            .AddScoped<IServiceDeliveryLookupService, ServiceDeliveryLookupService>()
            .AddScoped<IUpdateServicePointAction, UpdateServicePointAction>()
            .AddScoped<IUpdateServicePointFromServiceEventAction, UpdateServicePointFromServiceEventAction>()
            .AddScoped<ServicePointUpdateBuilderResolver>()
            .AddScoped<CurrentUserService>()
            .AddScoped<ProcessRunTracker>();

        services.AddKeyedTransient<IFtpService, FluentFtpService>(nameof(FluentFtpService));
        services.AddKeyedTransient<IFtpService, SshNetSftpService>(nameof(SshNetSftpService));
        services.AddKeyedTransient<IFtpService, LocalFileSystemFtpService>(nameof(LocalFileSystemFtpService));
        services.AddKeyedTransient<IFileStorage, AzureBlobFileStorage>(nameof(AzureBlobFileStorage));
        services.AddKeyedTransient<IFileStorage, LocalFileSystemFileStorage>(nameof(LocalFileSystemFileStorage));
        services.AddKeyedTransient<IMarketDocumentTransactionReader, Usage_867_TransactionReader>(Usage_867_TransactionReader.Set);
        services.AddKeyedTransient<IMarketDocumentTransactionReader, Invoice_810_TransactionReader>(Invoice_810_TransactionReader.Set);
        services.AddKeyedTransient<IMarketDocumentTransactionReader, ServiceOrder_650_TransactionReader>(ServiceOrder_650_TransactionReader.Set);
        services.AddKeyedTransient<IMarketDocumentTransactionReader, ServiceRequest_814_TransactionReader>(ServiceRequest_814_TransactionReader.Set);
        services.AddKeyedScoped<IMarketDocumentEntityBuilder, HistoricalUsage_867_02_ImportBuilder>(HistoricalUsage_867_02_ImportBuilder.Key);
        services.AddKeyedScoped<IMarketDocumentEntityBuilder, Usage_867_03_ImportBuilder>(Usage_867_03_ImportBuilder.Key);
        services.AddKeyedScoped<IMarketDocumentEntityBuilder, InitialMeterRead_867_04_ImportBuilder>(InitialMeterRead_867_04_ImportBuilder.Key);
        services.AddKeyedScoped<IMarketDocumentEntityBuilder, SwitchReject_814_02_ImportBuilder>(SwitchReject_814_02_ImportBuilder.Key);
        services.AddKeyedScoped<IMarketDocumentEntityBuilder, EnrollmentNotification_814_05_ImportBuilder>(EnrollmentNotification_814_05_ImportBuilder.Key);
        services.AddKeyedScoped<IServicePointUpdateBuilder, EnrollmentNotification_814_05_ServicePointUpdateBuilder>(EnrollmentNotification_814_05_ImportBuilder.Key);
        services.AddKeyedScoped<IMarketDocumentEntityBuilder, MoveInReject_814_17_ImportBuilder>(MoveInReject_814_17_ImportBuilder.Key);
        services.AddKeyedScoped<IMarketDocumentEntityGenerator, SwitchRequest_814_01_ExportGenerator>(SwitchRequest_814_01_ExportGenerator.Key);
        services.AddKeyedScoped<IMarketDocumentEntityGenerator, MoveInRequest_814_16_ExportGenerator>(MoveInRequest_814_16_ExportGenerator.Key);

        var keyVaultUri = config["KeyVault:Uri"];

        if (!string.IsNullOrWhiteSpace(keyVaultUri))
        {
            services.AddSingleton(new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential()));
        }
        else
        {
            services.AddSingleton<SecretClient>(_ => null!);
            logger.LogWarning("KeyVault:Uri is not configured; secret ConnectorFields will resolve from environment variables only.");
        }

        services.AddScoped<IConnectorFieldSecretResolver, KeyVaultConnectorFieldSecretResolver>();

        logger.LogInformation("{Project} services registered", "Infrastructure");

        return services;
    }
}
