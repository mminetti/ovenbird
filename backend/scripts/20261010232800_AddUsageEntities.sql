BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261010232800_AddUsageEntities'
)
BEGIN
    CREATE TABLE [UsageStatus] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_UsageStatus] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261010232800_AddUsageEntities'
)
BEGIN
    CREATE TABLE [UsageType] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_UsageType] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261010232800_AddUsageEntities'
)
BEGIN
    CREATE TABLE [Usage] (
        [Id] bigint NOT NULL IDENTITY,
        [UsageTypeId] int NOT NULL,
        [OriginalUsageId] bigint NULL,
        [IsFinal] bit NOT NULL,
        [StartRead] int NOT NULL,
        [EndRead] int NOT NULL,
        [UsageStatusId] int NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        [Identifier] nvarchar(250) NOT NULL,
        [AccountId] bigint NOT NULL,
        [ServicePointId] bigint NOT NULL,
        [MeterId] bigint NULL,
        [PeriodStartDate] date NOT NULL,
        [PeriodEndDate] date NOT NULL,
        [Consumption] float NOT NULL,
        [CommodityId] int NOT NULL,
        [UnitOfMeasureId] int NOT NULL,
        CONSTRAINT [PK_Usage] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Usage_Account_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [Account] ([Id]),
        CONSTRAINT [FK_Usage_Commodity_CommodityId] FOREIGN KEY ([CommodityId]) REFERENCES [Commodity] ([Id]),
        CONSTRAINT [FK_Usage_Meter_MeterId] FOREIGN KEY ([MeterId]) REFERENCES [Meter] ([Id]),
        CONSTRAINT [FK_Usage_ServicePoint_ServicePointId] FOREIGN KEY ([ServicePointId]) REFERENCES [ServicePoint] ([Id]),
        CONSTRAINT [FK_Usage_UnitOfMeasure_UnitOfMeasureId] FOREIGN KEY ([UnitOfMeasureId]) REFERENCES [UnitOfMeasure] ([Id]),
        CONSTRAINT [FK_Usage_UsageStatus_UsageStatusId] FOREIGN KEY ([UsageStatusId]) REFERENCES [UsageStatus] ([Id]),
        CONSTRAINT [FK_Usage_UsageType_UsageTypeId] FOREIGN KEY ([UsageTypeId]) REFERENCES [UsageType] ([Id]),
        CONSTRAINT [FK_Usage_Usage_OriginalUsageId] FOREIGN KEY ([OriginalUsageId]) REFERENCES [Usage] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261010232800_AddUsageEntities'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[UsageStatus]'))
        SET IDENTITY_INSERT [UsageStatus] ON;
    EXEC(N'INSERT INTO [UsageStatus] ([Id], [Name])
    VALUES (1, N''New'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[UsageStatus]'))
        SET IDENTITY_INSERT [UsageStatus] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261010232800_AddUsageEntities'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[UsageType]'))
        SET IDENTITY_INSERT [UsageType] ON;
    EXEC(N'INSERT INTO [UsageType] ([Id], [Name])
    VALUES (1, N''Original''),
    (2, N''Cancel''),
    (3, N''Replace'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[UsageType]'))
        SET IDENTITY_INSERT [UsageType] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261010232800_AddUsageEntities'
)
BEGIN
    CREATE INDEX [IX_Usage_AccountId] ON [Usage] ([AccountId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261010232800_AddUsageEntities'
)
BEGIN
    CREATE INDEX [IX_Usage_CommodityId] ON [Usage] ([CommodityId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261010232800_AddUsageEntities'
)
BEGIN
    CREATE INDEX [IX_Usage_MeterId] ON [Usage] ([MeterId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261010232800_AddUsageEntities'
)
BEGIN
    CREATE INDEX [IX_Usage_OriginalUsageId] ON [Usage] ([OriginalUsageId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261010232800_AddUsageEntities'
)
BEGIN
    CREATE INDEX [IX_Usage_ServicePointId] ON [Usage] ([ServicePointId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261010232800_AddUsageEntities'
)
BEGIN
    CREATE INDEX [IX_Usage_UnitOfMeasureId] ON [Usage] ([UnitOfMeasureId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261010232800_AddUsageEntities'
)
BEGIN
    CREATE INDEX [IX_Usage_UsageStatusId] ON [Usage] ([UsageStatusId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261010232800_AddUsageEntities'
)
BEGIN
    CREATE INDEX [IX_Usage_UsageTypeId] ON [Usage] ([UsageTypeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261010232800_AddUsageEntities'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261010232800_AddUsageEntities', N'10.0.12');
END;

COMMIT;
GO

