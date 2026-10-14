BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014150912_MoveServicePointFromAccountToService'
)
BEGIN
    ALTER TABLE [Account] DROP CONSTRAINT [FK_Account_ServicePoint_ServicePointId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014150912_MoveServicePointFromAccountToService'
)
BEGIN
    DROP INDEX [IX_Account_ServicePointId] ON [Account];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014150912_MoveServicePointFromAccountToService'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Account]') AND [c].[name] = N'ServicePointId');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [Account] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [Account] DROP COLUMN [ServicePointId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014150912_MoveServicePointFromAccountToService'
)
BEGIN
    ALTER TABLE [Service] ADD [ServicePointId] bigint NOT NULL DEFAULT CAST(0 AS bigint);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014150912_MoveServicePointFromAccountToService'
)
BEGIN
    CREATE INDEX [IX_Service_ServicePointId] ON [Service] ([ServicePointId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014150912_MoveServicePointFromAccountToService'
)
BEGIN
    ALTER TABLE [Service] ADD CONSTRAINT [FK_Service_ServicePoint_ServicePointId] FOREIGN KEY ([ServicePointId]) REFERENCES [ServicePoint] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014150912_MoveServicePointFromAccountToService'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261014150912_MoveServicePointFromAccountToService', N'10.0.13');
END;

COMMIT;
GO

