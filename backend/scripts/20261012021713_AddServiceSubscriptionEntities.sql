BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [ServiceStart] DROP CONSTRAINT [FK_ServiceStart_ServiceStatus_ServiceStatusId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    EXEC(N'DELETE FROM [ServiceStatus]
    WHERE [Id] = 4;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    EXEC(N'DELETE FROM [ServiceStatus]
    WHERE [Id] = 5;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    EXEC(N'DELETE FROM [ServiceStatus]
    WHERE [Id] = 6;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    EXEC(N'DELETE FROM [ServiceStatus]
    WHERE [Id] = 7;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    EXEC(N'DELETE FROM [ServiceStatus]
    WHERE [Id] = 8;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    EXEC sp_rename N'[ServiceStart].[ServiceStatusId]', N'ServiceOrderStatusId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    EXEC sp_rename N'[ServiceStart].[IX_ServiceStart_ServiceStatusId]', N'IX_ServiceStart_ServiceOrderStatusId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [ServicePoint] ADD [CommodityId] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [ServicePoint] ADD [Cycle] nvarchar(250) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [ServicePoint] ADD [IsCriticalCare] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [ServicePoint] ADD [MarketId] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [ServicePoint] ADD [PremiseId] bigint NOT NULL DEFAULT CAST(0 AS bigint);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [ServicePoint] ADD [PremiseTypeId] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [ServicePoint] ADD [UtilityId] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [Account] ADD [Balance] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [Account] ADD [CustomerId] bigint NOT NULL DEFAULT CAST(0 AS bigint);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [Account] ADD [IsServiceLocked] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [Account] ADD [IsServiceProtected] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [Account] ADD [ServiceConnectionStatusId] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [Account] ADD [ServiceEndDate] date NOT NULL DEFAULT '0001-01-01';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [Account] ADD [ServicePointId] bigint NOT NULL DEFAULT CAST(0 AS bigint);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [Account] ADD [ServiceStartDate] date NOT NULL DEFAULT '0001-01-01';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [Account] ADD [ServiceStartId] bigint NOT NULL DEFAULT CAST(0 AS bigint);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [Account] ADD [ServiceStatusId] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [Account] ADD [ServiceSubStatusId] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    CREATE TABLE [Customer] (
        [Id] bigint NOT NULL IDENTITY,
        [Identifier] nvarchar(250) NOT NULL,
        [FirstName] nvarchar(250) NOT NULL,
        [LastName] nvarchar(250) NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_Customer] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    CREATE TABLE [Premise] (
        [Id] bigint NOT NULL IDENTITY,
        [Address] nvarchar(250) NOT NULL,
        [Address2] nvarchar(250) NOT NULL,
        [City] nvarchar(250) NOT NULL,
        [State] nvarchar(250) NOT NULL,
        [Zip] nvarchar(250) NOT NULL,
        [County] nvarchar(250) NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_Premise] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    CREATE TABLE [PremiseType] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_PremiseType] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    CREATE TABLE [ServiceConnectionStatus] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_ServiceConnectionStatus] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    CREATE TABLE [ServiceOrderStatus] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_ServiceOrderStatus] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    CREATE TABLE [ServiceSubStatus] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_ServiceSubStatus] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    CREATE TABLE [Utility] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(250) NOT NULL,
        [MarketId] int NOT NULL,
        CONSTRAINT [PK_Utility] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Utility_Market_MarketId] FOREIGN KEY ([MarketId]) REFERENCES [Market] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[ServiceConnectionStatus]'))
        SET IDENTITY_INSERT [ServiceConnectionStatus] ON;
    EXEC(N'INSERT INTO [ServiceConnectionStatus] ([Id], [Name])
    VALUES (1, N''PendingDisconnection''),
    (2, N''Disconnected''),
    (3, N''PendingConnection''),
    (4, N''Connected'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[ServiceConnectionStatus]'))
        SET IDENTITY_INSERT [ServiceConnectionStatus] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[ServiceOrderStatus]'))
        SET IDENTITY_INSERT [ServiceOrderStatus] ON;
    EXEC(N'INSERT INTO [ServiceOrderStatus] ([Id], [Name])
    VALUES (1, N''New''),
    (2, N''InProgress''),
    (3, N''Scheduled''),
    (4, N''Cancelled''),
    (5, N''Rejected''),
    (6, N''Completed''),
    (7, N''Unexecuted''),
    (8, N''Error'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[ServiceOrderStatus]'))
        SET IDENTITY_INSERT [ServiceOrderStatus] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    EXEC(N'UPDATE [ServiceStatus] SET [Name] = N''Pending''
    WHERE [Id] = 1;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    EXEC(N'UPDATE [ServiceStatus] SET [Name] = N''On''
    WHERE [Id] = 2;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    EXEC(N'UPDATE [ServiceStatus] SET [Name] = N''Off''
    WHERE [Id] = 3;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[ServiceSubStatus]'))
        SET IDENTITY_INSERT [ServiceSubStatus] ON;
    EXEC(N'INSERT INTO [ServiceSubStatus] ([Id], [Name])
    VALUES (1, N''PendingStart''),
    (2, N''Active''),
    (3, N''PendingEnd''),
    (4, N''Ended''),
    (5, N''Closed'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[ServiceSubStatus]'))
        SET IDENTITY_INSERT [ServiceSubStatus] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    CREATE INDEX [IX_ServicePoint_CommodityId] ON [ServicePoint] ([CommodityId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    CREATE INDEX [IX_ServicePoint_MarketId] ON [ServicePoint] ([MarketId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    CREATE INDEX [IX_ServicePoint_PremiseId] ON [ServicePoint] ([PremiseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    CREATE INDEX [IX_ServicePoint_PremiseTypeId] ON [ServicePoint] ([PremiseTypeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    CREATE INDEX [IX_ServicePoint_UtilityId] ON [ServicePoint] ([UtilityId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    CREATE INDEX [IX_Account_CustomerId] ON [Account] ([CustomerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    CREATE INDEX [IX_Account_ServiceConnectionStatusId] ON [Account] ([ServiceConnectionStatusId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    CREATE INDEX [IX_Account_ServicePointId] ON [Account] ([ServicePointId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    CREATE INDEX [IX_Account_ServiceStartId] ON [Account] ([ServiceStartId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    CREATE INDEX [IX_Account_ServiceStatusId] ON [Account] ([ServiceStatusId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    CREATE INDEX [IX_Account_ServiceSubStatusId] ON [Account] ([ServiceSubStatusId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    CREATE INDEX [IX_Utility_MarketId] ON [Utility] ([MarketId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [Account] ADD CONSTRAINT [FK_Account_Customer_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customer] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [Account] ADD CONSTRAINT [FK_Account_ServiceConnectionStatus_ServiceConnectionStatusId] FOREIGN KEY ([ServiceConnectionStatusId]) REFERENCES [ServiceConnectionStatus] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [Account] ADD CONSTRAINT [FK_Account_ServicePoint_ServicePointId] FOREIGN KEY ([ServicePointId]) REFERENCES [ServicePoint] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [Account] ADD CONSTRAINT [FK_Account_ServiceStart_ServiceStartId] FOREIGN KEY ([ServiceStartId]) REFERENCES [ServiceStart] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [Account] ADD CONSTRAINT [FK_Account_ServiceStatus_ServiceStatusId] FOREIGN KEY ([ServiceStatusId]) REFERENCES [ServiceStatus] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [Account] ADD CONSTRAINT [FK_Account_ServiceSubStatus_ServiceSubStatusId] FOREIGN KEY ([ServiceSubStatusId]) REFERENCES [ServiceSubStatus] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [ServicePoint] ADD CONSTRAINT [FK_ServicePoint_Commodity_CommodityId] FOREIGN KEY ([CommodityId]) REFERENCES [Commodity] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [ServicePoint] ADD CONSTRAINT [FK_ServicePoint_Market_MarketId] FOREIGN KEY ([MarketId]) REFERENCES [Market] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [ServicePoint] ADD CONSTRAINT [FK_ServicePoint_PremiseType_PremiseTypeId] FOREIGN KEY ([PremiseTypeId]) REFERENCES [PremiseType] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [ServicePoint] ADD CONSTRAINT [FK_ServicePoint_Premise_PremiseId] FOREIGN KEY ([PremiseId]) REFERENCES [Premise] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [ServicePoint] ADD CONSTRAINT [FK_ServicePoint_Utility_UtilityId] FOREIGN KEY ([UtilityId]) REFERENCES [Utility] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    ALTER TABLE [ServiceStart] ADD CONSTRAINT [FK_ServiceStart_ServiceOrderStatus_ServiceOrderStatusId] FOREIGN KEY ([ServiceOrderStatusId]) REFERENCES [ServiceOrderStatus] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012021713_AddServiceSubscriptionEntities'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261012021713_AddServiceSubscriptionEntities', N'10.0.12');
END;

COMMIT;
GO

