BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016200748_MergeEdiConfigurationTypes'
)
BEGIN
    EXEC(N'DELETE FROM [ConfigurationType]
    WHERE [Id] = 3;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016200748_MergeEdiConfigurationTypes'
)
BEGIN
    EXEC(N'UPDATE [ConfigurationType] SET [Name] = N''EDI''
    WHERE [Id] = 2;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016200748_MergeEdiConfigurationTypes'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261016200748_MergeEdiConfigurationTypes', N'10.0.13');
END;

COMMIT;
GO

