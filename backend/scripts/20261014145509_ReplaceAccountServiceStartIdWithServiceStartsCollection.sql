BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014145509_ReplaceAccountServiceStartIdWithServiceStartsCollection'
)
BEGIN
    ALTER TABLE [Account] DROP CONSTRAINT [FK_Account_ServiceStart_ServiceStartId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014145509_ReplaceAccountServiceStartIdWithServiceStartsCollection'
)
BEGIN
    DROP INDEX [IX_Account_ServiceStartId] ON [Account];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014145509_ReplaceAccountServiceStartIdWithServiceStartsCollection'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Account]') AND [c].[name] = N'ServiceStartId');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [Account] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [Account] DROP COLUMN [ServiceStartId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014145509_ReplaceAccountServiceStartIdWithServiceStartsCollection'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261014145509_ReplaceAccountServiceStartIdWithServiceStartsCollection', N'10.0.13');
END;

COMMIT;
GO

