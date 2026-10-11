BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011215344_MakeServiceStartItemServiceDateNullable'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ServiceStartItem]') AND [c].[name] = N'ServiceDate');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [ServiceStartItem] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [ServiceStartItem] ALTER COLUMN [ServiceDate] date NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011215344_MakeServiceStartItemServiceDateNullable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261011215344_MakeServiceStartItemServiceDateNullable', N'10.0.12');
END;

COMMIT;
GO

