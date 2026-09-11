BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260911204134_RemoveModule'
)
BEGIN
    ALTER TABLE [Permission] DROP CONSTRAINT [FK_Permission_Module_ModuleId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260911204134_RemoveModule'
)
BEGIN
    DROP TABLE [Module];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260911204134_RemoveModule'
)
BEGIN
    DROP INDEX [IX_Permission_ModuleId] ON [Permission];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260911204134_RemoveModule'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Permission]') AND [c].[name] = N'ModuleId');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [Permission] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [Permission] DROP COLUMN [ModuleId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260911204134_RemoveModule'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260911204134_RemoveModule', N'10.0.11');
END;

COMMIT;
GO

