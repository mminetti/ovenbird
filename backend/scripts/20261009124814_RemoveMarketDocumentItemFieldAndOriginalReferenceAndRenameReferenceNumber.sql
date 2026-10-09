BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009124814_RemoveMarketDocumentItemFieldAndOriginalReferenceAndRenameReferenceNumber'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MarketDocumentItem]') AND [c].[name] = N'OriginalTrackingNumber');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [MarketDocumentItem] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [MarketDocumentItem] DROP COLUMN [OriginalTrackingNumber];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009124814_RemoveMarketDocumentItemFieldAndOriginalReferenceAndRenameReferenceNumber'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MarketDocumentItem]') AND [c].[name] = N'Purpose');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [MarketDocumentItem] DROP CONSTRAINT ' + @var1 + ';');
    ALTER TABLE [MarketDocumentItem] DROP COLUMN [Purpose];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009124814_RemoveMarketDocumentItemFieldAndOriginalReferenceAndRenameReferenceNumber'
)
BEGIN
    DECLARE @var2 nvarchar(max);
    SELECT @var2 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MarketDocumentItem]') AND [c].[name] = N'SubPurpose');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [MarketDocumentItem] DROP CONSTRAINT ' + @var2 + ';');
    ALTER TABLE [MarketDocumentItem] DROP COLUMN [SubPurpose];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009124814_RemoveMarketDocumentItemFieldAndOriginalReferenceAndRenameReferenceNumber'
)
BEGIN
    EXEC sp_rename N'[MarketDocumentItem].[TrackingNumber]', N'ReferenceNumber', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009124814_RemoveMarketDocumentItemFieldAndOriginalReferenceAndRenameReferenceNumber'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261009124814_RemoveMarketDocumentItemFieldAndOriginalReferenceAndRenameReferenceNumber', N'10.0.12');
END;

COMMIT;
GO

