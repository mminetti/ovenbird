BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260910115423_FixExternalIdentifierMaxLength'
)
BEGIN
    DROP INDEX [IX_User_ExternalIdentifier] ON [User];
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[User]') AND [c].[name] = N'ExternalIdentifier');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [User] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [User] ALTER COLUMN [ExternalIdentifier] nvarchar(250) NOT NULL;
    CREATE UNIQUE INDEX [IX_User_ExternalIdentifier] ON [User] ([ExternalIdentifier]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260910115423_FixExternalIdentifierMaxLength'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260910115423_FixExternalIdentifierMaxLength', N'10.0.11');
END;

COMMIT;
GO

