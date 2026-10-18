BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018230524_SplitMarketDocumentAsset'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MarketDocument]') AND [c].[name] = N'File');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [MarketDocument] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [MarketDocument] DROP COLUMN [File];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018230524_SplitMarketDocumentAsset'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MarketDocument]') AND [c].[name] = N'Name');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [MarketDocument] DROP CONSTRAINT ' + @var1 + ';');
    ALTER TABLE [MarketDocument] DROP COLUMN [Name];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018230524_SplitMarketDocumentAsset'
)
BEGIN
    ALTER TABLE [MarketDocument] ADD [AssetId] bigint NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018230524_SplitMarketDocumentAsset'
)
BEGIN
    ALTER TABLE [MarketDocument] ADD [Identifier] nvarchar(250) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018230524_SplitMarketDocumentAsset'
)
BEGIN
    CREATE TABLE [Asset] (
        [Id] bigint NOT NULL IDENTITY,
        [FileName] nvarchar(250) NOT NULL,
        [Extension] nvarchar(20) NULL,
        [ContentType] nvarchar(100) NULL,
        [SizeBytes] bigint NULL,
        [Reference] nvarchar(max) NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_Asset] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018230524_SplitMarketDocumentAsset'
)
BEGIN
    CREATE INDEX [IX_MarketDocument_AssetId] ON [MarketDocument] ([AssetId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018230524_SplitMarketDocumentAsset'
)
BEGIN
    ALTER TABLE [MarketDocument] ADD CONSTRAINT [FK_MarketDocument_Asset_AssetId] FOREIGN KEY ([AssetId]) REFERENCES [Asset] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018230524_SplitMarketDocumentAsset'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261018230524_SplitMarketDocumentAsset', N'10.0.13');
END;

COMMIT;
GO

