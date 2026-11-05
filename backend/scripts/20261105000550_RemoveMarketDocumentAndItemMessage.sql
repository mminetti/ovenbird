BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261105000550_RemoveMarketDocumentAndItemMessage'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MarketDocumentItem]') AND [c].[name] = N'Message');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [MarketDocumentItem] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [MarketDocumentItem] DROP COLUMN [Message];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261105000550_RemoveMarketDocumentAndItemMessage'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MarketDocument]') AND [c].[name] = N'Message');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [MarketDocument] DROP CONSTRAINT ' + @var1 + ';');
    ALTER TABLE [MarketDocument] DROP COLUMN [Message];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261105000550_RemoveMarketDocumentAndItemMessage'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261105000550_RemoveMarketDocumentAndItemMessage', N'10.0.14');
END;

COMMIT;
GO

