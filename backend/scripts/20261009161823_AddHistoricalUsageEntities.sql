BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009161823_AddHistoricalUsageEntities'
)
BEGIN
    CREATE TABLE [Account] (
        [Id] bigint NOT NULL IDENTITY,
        [Identifier] nvarchar(250) NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_Account] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009161823_AddHistoricalUsageEntities'
)
BEGIN
    CREATE TABLE [Commodity] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_Commodity] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009161823_AddHistoricalUsageEntities'
)
BEGIN
    CREATE TABLE [MarketDocumentReferenceType] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_MarketDocumentReferenceType] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009161823_AddHistoricalUsageEntities'
)
BEGIN
    CREATE TABLE [Meter] (
        [Id] bigint NOT NULL IDENTITY,
        [Identifier] nvarchar(250) NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_Meter] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009161823_AddHistoricalUsageEntities'
)
BEGIN
    CREATE TABLE [ServicePoint] (
        [Id] bigint NOT NULL IDENTITY,
        [Identifier] nvarchar(250) NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_ServicePoint] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009161823_AddHistoricalUsageEntities'
)
BEGIN
    CREATE TABLE [UnitOfMeasure] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_UnitOfMeasure] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009161823_AddHistoricalUsageEntities'
)
BEGIN
    CREATE TABLE [MarketDocumentReference] (
        [Id] bigint NOT NULL IDENTITY,
        [MarketDocumentItemId] bigint NOT NULL,
        [ReferenceTypeId] int NOT NULL,
        [ReferenceId] bigint NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_MarketDocumentReference] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MarketDocumentReference_MarketDocumentItem_MarketDocumentItemId] FOREIGN KEY ([MarketDocumentItemId]) REFERENCES [MarketDocumentItem] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_MarketDocumentReference_MarketDocumentReferenceType_ReferenceTypeId] FOREIGN KEY ([ReferenceTypeId]) REFERENCES [MarketDocumentReferenceType] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009161823_AddHistoricalUsageEntities'
)
BEGIN
    CREATE TABLE [HistoricalUsage] (
        [Id] bigint NOT NULL IDENTITY,
        [Identifier] nvarchar(250) NOT NULL,
        [AccountId] bigint NOT NULL,
        [ServicePointId] bigint NOT NULL,
        [MeterId] bigint NULL,
        [PeriodStartDate] date NOT NULL,
        [PeriodEndDate] date NOT NULL,
        [Consumption] float NOT NULL,
        [CommodityId] int NOT NULL,
        [UnitOfMeasureId] int NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_HistoricalUsage] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HistoricalUsage_Account_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [Account] ([Id]),
        CONSTRAINT [FK_HistoricalUsage_Commodity_CommodityId] FOREIGN KEY ([CommodityId]) REFERENCES [Commodity] ([Id]),
        CONSTRAINT [FK_HistoricalUsage_Meter_MeterId] FOREIGN KEY ([MeterId]) REFERENCES [Meter] ([Id]),
        CONSTRAINT [FK_HistoricalUsage_ServicePoint_ServicePointId] FOREIGN KEY ([ServicePointId]) REFERENCES [ServicePoint] ([Id]),
        CONSTRAINT [FK_HistoricalUsage_UnitOfMeasure_UnitOfMeasureId] FOREIGN KEY ([UnitOfMeasureId]) REFERENCES [UnitOfMeasure] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009161823_AddHistoricalUsageEntities'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[Commodity]'))
        SET IDENTITY_INSERT [Commodity] ON;
    EXEC(N'INSERT INTO [Commodity] ([Id], [Name])
    VALUES (1, N''Electricity'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[Commodity]'))
        SET IDENTITY_INSERT [Commodity] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009161823_AddHistoricalUsageEntities'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[MarketDocumentReferenceType]'))
        SET IDENTITY_INSERT [MarketDocumentReferenceType] ON;
    EXEC(N'INSERT INTO [MarketDocumentReferenceType] ([Id], [Name])
    VALUES (1, N''HistoricalUsage''),
    (2, N''Usage''),
    (3, N''Invoice''),
    (4, N''ServiceStart''),
    (5, N''ServiceEnd''),
    (6, N''ServiceRequest'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[MarketDocumentReferenceType]'))
        SET IDENTITY_INSERT [MarketDocumentReferenceType] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009161823_AddHistoricalUsageEntities'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[UnitOfMeasure]'))
        SET IDENTITY_INSERT [UnitOfMeasure] ON;
    EXEC(N'INSERT INTO [UnitOfMeasure] ([Id], [Name])
    VALUES (1, N''Kilowatt Hour'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[UnitOfMeasure]'))
        SET IDENTITY_INSERT [UnitOfMeasure] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009161823_AddHistoricalUsageEntities'
)
BEGIN
    CREATE INDEX [IX_HistoricalUsage_AccountId] ON [HistoricalUsage] ([AccountId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009161823_AddHistoricalUsageEntities'
)
BEGIN
    CREATE INDEX [IX_HistoricalUsage_CommodityId] ON [HistoricalUsage] ([CommodityId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009161823_AddHistoricalUsageEntities'
)
BEGIN
    CREATE INDEX [IX_HistoricalUsage_MeterId] ON [HistoricalUsage] ([MeterId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009161823_AddHistoricalUsageEntities'
)
BEGIN
    CREATE INDEX [IX_HistoricalUsage_ServicePointId] ON [HistoricalUsage] ([ServicePointId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009161823_AddHistoricalUsageEntities'
)
BEGIN
    CREATE INDEX [IX_HistoricalUsage_UnitOfMeasureId] ON [HistoricalUsage] ([UnitOfMeasureId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009161823_AddHistoricalUsageEntities'
)
BEGIN
    CREATE INDEX [IX_MarketDocumentReference_MarketDocumentItemId] ON [MarketDocumentReference] ([MarketDocumentItemId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009161823_AddHistoricalUsageEntities'
)
BEGIN
    CREATE INDEX [IX_MarketDocumentReference_ReferenceTypeId] ON [MarketDocumentReference] ([ReferenceTypeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009161823_AddHistoricalUsageEntities'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261009161823_AddHistoricalUsageEntities', N'10.0.12');
END;

COMMIT;
GO

