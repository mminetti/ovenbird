BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007205618_AddMarketDocumentItem'
)
BEGIN
    CREATE TABLE [MarketDocumentItemStatus] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_MarketDocumentItemStatus] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007205618_AddMarketDocumentItem'
)
BEGIN
    CREATE TABLE [MarketDocumentItem] (
        [Id] bigint NOT NULL IDENTITY,
        [MarketDocumentId] bigint NULL,
        [MarketDocumentItemStatusId] int NOT NULL,
        [Set] nvarchar(250) NOT NULL,
        [SubSet] nvarchar(250) NOT NULL,
        [Purpose] nvarchar(250) NOT NULL,
        [SubPurpose] nvarchar(250) NOT NULL,
        [TrackingNumber] nvarchar(250) NOT NULL,
        [OriginalTrackingNumber] nvarchar(250) NULL,
        [ServicePointIdentifier] nvarchar(250) NOT NULL,
        [Raw] nvarchar(max) NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_MarketDocumentItem] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MarketDocumentItem_MarketDocumentItemStatus_MarketDocumentItemStatusId] FOREIGN KEY ([MarketDocumentItemStatusId]) REFERENCES [MarketDocumentItemStatus] ([Id]),
        CONSTRAINT [FK_MarketDocumentItem_MarketDocument_MarketDocumentId] FOREIGN KEY ([MarketDocumentId]) REFERENCES [MarketDocument] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007205618_AddMarketDocumentItem'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[MarketDocumentItemStatus]'))
        SET IDENTITY_INSERT [MarketDocumentItemStatus] ON;
    EXEC(N'INSERT INTO [MarketDocumentItemStatus] ([Id], [Name])
    VALUES (1, N''New''),
    (2, N''Done''),
    (3, N''Error'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[MarketDocumentItemStatus]'))
        SET IDENTITY_INSERT [MarketDocumentItemStatus] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007205618_AddMarketDocumentItem'
)
BEGIN
    CREATE INDEX [IX_MarketDocumentItem_MarketDocumentId] ON [MarketDocumentItem] ([MarketDocumentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007205618_AddMarketDocumentItem'
)
BEGIN
    CREATE INDEX [IX_MarketDocumentItem_MarketDocumentItemStatusId] ON [MarketDocumentItem] ([MarketDocumentItemStatusId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007205618_AddMarketDocumentItem'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261007205618_AddMarketDocumentItem', N'10.0.12');
END;

COMMIT;
GO

