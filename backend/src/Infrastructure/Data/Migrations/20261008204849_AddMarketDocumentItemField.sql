BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008204849_AddMarketDocumentItemField'
)
BEGIN
    CREATE TABLE [MarketDocumentItemField] (
        [Id] bigint NOT NULL IDENTITY,
        [MarketDocumentItemId] bigint NOT NULL,
        [FieldName] nvarchar(250) NOT NULL,
        [FieldValue] nvarchar(1000) NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_MarketDocumentItemField] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MarketDocumentItemField_MarketDocumentItem_MarketDocumentItemId] FOREIGN KEY ([MarketDocumentItemId]) REFERENCES [MarketDocumentItem] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008204849_AddMarketDocumentItemField'
)
BEGIN
    CREATE INDEX [IX_MarketDocumentItemField_MarketDocumentItemId] ON [MarketDocumentItemField] ([MarketDocumentItemId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008204849_AddMarketDocumentItemField'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261008204849_AddMarketDocumentItemField', N'10.0.12');
END;

COMMIT;
GO

