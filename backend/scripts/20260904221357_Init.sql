IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE TABLE [ConfigurationType] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_ConfigurationType] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE TABLE [ConnectorImplementation] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(250) NOT NULL,
        [Description] nvarchar(1000) NULL,
        CONSTRAINT [PK_ConnectorImplementation] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE TABLE [ConnectorType] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(250) NOT NULL,
        [Description] nvarchar(1000) NULL,
        CONSTRAINT [PK_ConnectorType] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE TABLE [Market] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        [Identifier] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_Market] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE TABLE [MarketDocumentDirection] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_MarketDocumentDirection] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE TABLE [MarketDocumentStatus] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_MarketDocumentStatus] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE TABLE [Module] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(250) NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_Module] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE TABLE [Role] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(250) NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_Role] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE TABLE [User] (
        [Id] int NOT NULL IDENTITY,
        [ExternalIdentifier] nvarchar(1000) NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        [Email] nvarchar(250) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_User] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE TABLE [Connector] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(250) NOT NULL,
        [Description] nvarchar(1000) NULL,
        [ConnectorTypeId] int NOT NULL,
        [ConnectorImplementationId] int NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_Connector] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Connector_ConnectorImplementation_ConnectorImplementationId] FOREIGN KEY ([ConnectorImplementationId]) REFERENCES [ConnectorImplementation] ([Id]),
        CONSTRAINT [FK_Connector_ConnectorType_ConnectorTypeId] FOREIGN KEY ([ConnectorTypeId]) REFERENCES [ConnectorType] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE TABLE [Company] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(250) NOT NULL,
        [MarketId] int NOT NULL,
        [TimeZoneId] nvarchar(250) NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_Company] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Company_Market_MarketId] FOREIGN KEY ([MarketId]) REFERENCES [Market] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE TABLE [Permission] (
        [Id] int NOT NULL IDENTITY,
        [ModuleId] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        [Description] nvarchar(1000) NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_Permission] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Permission_Module_ModuleId] FOREIGN KEY ([ModuleId]) REFERENCES [Module] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE TABLE [UserRole] (
        [RoleId] int NOT NULL,
        [UserId] int NOT NULL,
        CONSTRAINT [PK_UserRole] PRIMARY KEY ([RoleId], [UserId]),
        CONSTRAINT [FK_UserRole_Role_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Role] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserRole_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE TABLE [ConnectorField] (
        [Id] int NOT NULL IDENTITY,
        [ConnectorId] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        [Value] nvarchar(250) NULL,
        [IsSecret] bit NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_ConnectorField] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ConnectorField_Connector_ConnectorId] FOREIGN KEY ([ConnectorId]) REFERENCES [Connector] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE TABLE [Configuration] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(250) NOT NULL,
        [Description] nvarchar(1000) NULL,
        [ConfigurationTypeId] int NOT NULL,
        [CompanyId] int NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_Configuration] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Configuration_Company_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Company] ([Id]),
        CONSTRAINT [FK_Configuration_ConfigurationType_ConfigurationTypeId] FOREIGN KEY ([ConfigurationTypeId]) REFERENCES [ConfigurationType] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE TABLE [MarketDocument] (
        [Id] bigint NOT NULL IDENTITY,
        [Name] nvarchar(500) NOT NULL,
        [File] nvarchar(max) NOT NULL,
        [DirectionId] int NOT NULL,
        [CompanyId] int NOT NULL,
        [StatusId] int NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_MarketDocument] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MarketDocument_Company_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Company] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_MarketDocument_MarketDocumentDirection_DirectionId] FOREIGN KEY ([DirectionId]) REFERENCES [MarketDocumentDirection] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_MarketDocument_MarketDocumentStatus_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [MarketDocumentStatus] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE TABLE [RolePermission] (
        [PermissionId] int NOT NULL,
        [RoleId] int NOT NULL,
        CONSTRAINT [PK_RolePermission] PRIMARY KEY ([PermissionId], [RoleId]),
        CONSTRAINT [FK_RolePermission_Permission_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [Permission] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RolePermission_Role_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Role] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE TABLE [ConfigurationConnector] (
        [ConfigurationId] int NOT NULL,
        [ConnectorId] int NOT NULL,
        CONSTRAINT [PK_ConfigurationConnector] PRIMARY KEY ([ConfigurationId], [ConnectorId]),
        CONSTRAINT [FK_ConfigurationConnector_Configuration_ConfigurationId] FOREIGN KEY ([ConfigurationId]) REFERENCES [Configuration] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ConfigurationConnector_Connector_ConnectorId] FOREIGN KEY ([ConnectorId]) REFERENCES [Connector] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE TABLE [ConfigurationField] (
        [Id] int NOT NULL IDENTITY,
        [ConfigurationId] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        [Value] nvarchar(250) NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_ConfigurationField] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ConfigurationField_Configuration_ConfigurationId] FOREIGN KEY ([ConfigurationId]) REFERENCES [Configuration] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[MarketDocumentDirection]'))
        SET IDENTITY_INSERT [MarketDocumentDirection] ON;
    EXEC(N'INSERT INTO [MarketDocumentDirection] ([Id], [Name])
    VALUES (1, N''Inbound''),
    (2, N''Outbound'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[MarketDocumentDirection]'))
        SET IDENTITY_INSERT [MarketDocumentDirection] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[MarketDocumentStatus]'))
        SET IDENTITY_INSERT [MarketDocumentStatus] ON;
    EXEC(N'INSERT INTO [MarketDocumentStatus] ([Id], [Name])
    VALUES (1, N''New''),
    (2, N''Done''),
    (3, N''Error'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[MarketDocumentStatus]'))
        SET IDENTITY_INSERT [MarketDocumentStatus] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE INDEX [IX_Company_MarketId] ON [Company] ([MarketId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE INDEX [IX_Configuration_CompanyId] ON [Configuration] ([CompanyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE INDEX [IX_Configuration_ConfigurationTypeId] ON [Configuration] ([ConfigurationTypeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE INDEX [IX_ConfigurationConnector_ConnectorId] ON [ConfigurationConnector] ([ConnectorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE INDEX [IX_ConfigurationField_ConfigurationId] ON [ConfigurationField] ([ConfigurationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE INDEX [IX_Connector_ConnectorImplementationId] ON [Connector] ([ConnectorImplementationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE INDEX [IX_Connector_ConnectorTypeId] ON [Connector] ([ConnectorTypeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE INDEX [IX_ConnectorField_ConnectorId] ON [ConnectorField] ([ConnectorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE INDEX [IX_MarketDocument_CompanyId] ON [MarketDocument] ([CompanyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE INDEX [IX_MarketDocument_DirectionId] ON [MarketDocument] ([DirectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE INDEX [IX_MarketDocument_StatusId] ON [MarketDocument] ([StatusId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE INDEX [IX_Permission_ModuleId] ON [Permission] ([ModuleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE INDEX [IX_RolePermission_RoleId] ON [RolePermission] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE UNIQUE INDEX [IX_User_ExternalIdentifier] ON [User] ([ExternalIdentifier]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    CREATE INDEX [IX_UserRole_UserId] ON [UserRole] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904221357_Init'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260904221357_Init', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904224523_DocumentMarketDocumentDeleteBehavior'
)
BEGIN
    ALTER TABLE [MarketDocument] DROP CONSTRAINT [FK_MarketDocument_Company_CompanyId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904224523_DocumentMarketDocumentDeleteBehavior'
)
BEGIN
    ALTER TABLE [MarketDocument] DROP CONSTRAINT [FK_MarketDocument_MarketDocumentDirection_DirectionId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904224523_DocumentMarketDocumentDeleteBehavior'
)
BEGIN
    ALTER TABLE [MarketDocument] DROP CONSTRAINT [FK_MarketDocument_MarketDocumentStatus_StatusId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904224523_DocumentMarketDocumentDeleteBehavior'
)
BEGIN
    ALTER TABLE [MarketDocument] ADD CONSTRAINT [FK_MarketDocument_Company_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Company] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904224523_DocumentMarketDocumentDeleteBehavior'
)
BEGIN
    ALTER TABLE [MarketDocument] ADD CONSTRAINT [FK_MarketDocument_MarketDocumentDirection_DirectionId] FOREIGN KEY ([DirectionId]) REFERENCES [MarketDocumentDirection] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904224523_DocumentMarketDocumentDeleteBehavior'
)
BEGIN
    ALTER TABLE [MarketDocument] ADD CONSTRAINT [FK_MarketDocument_MarketDocumentStatus_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [MarketDocumentStatus] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904224523_DocumentMarketDocumentDeleteBehavior'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260904224523_DocumentMarketDocumentDeleteBehavior', N'10.0.11');
END;

COMMIT;
GO