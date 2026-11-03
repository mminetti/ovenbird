BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103230519_RemoveProcessRunLogItemEntityFields'
)
BEGIN
    DROP INDEX [IX_ProcessRunLogItem_EntityType_EntityId] ON [ProcessRunLogItem];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103230519_RemoveProcessRunLogItemEntityFields'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProcessRunLogItem]') AND [c].[name] = N'EntityId');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [ProcessRunLogItem] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [ProcessRunLogItem] DROP COLUMN [EntityId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103230519_RemoveProcessRunLogItemEntityFields'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProcessRunLogItem]') AND [c].[name] = N'EntityType');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [ProcessRunLogItem] DROP CONSTRAINT ' + @var1 + ';');
    ALTER TABLE [ProcessRunLogItem] DROP COLUMN [EntityType];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103230519_RemoveProcessRunLogItemEntityFields'
)
BEGIN
    DECLARE @var2 nvarchar(max);
    SELECT @var2 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProcessRunLogItem]') AND [c].[name] = N'Field');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [ProcessRunLogItem] DROP CONSTRAINT ' + @var2 + ';');
    EXEC(N'UPDATE [ProcessRunLogItem] SET [Field] = N'''' WHERE [Field] IS NULL');
    ALTER TABLE [ProcessRunLogItem] ALTER COLUMN [Field] nvarchar(250) NOT NULL;
    ALTER TABLE [ProcessRunLogItem] ADD DEFAULT N'' FOR [Field];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103230519_RemoveProcessRunLogItemEntityFields'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261103230519_RemoveProcessRunLogItemEntityFields', N'10.0.14');
END;

COMMIT;
GO

