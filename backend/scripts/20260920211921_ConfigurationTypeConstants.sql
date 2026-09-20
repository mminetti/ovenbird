BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920211921_ConfigurationTypeConstants'
)
BEGIN
    EXEC(N'DELETE FROM [ConfigurationType]
    WHERE [Id] = 1;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920211921_ConfigurationTypeConstants'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[ConfigurationType]'))
        SET IDENTITY_INSERT [ConfigurationType] ON;
    EXEC(N'INSERT INTO [ConfigurationType] ([Id], [Description], [Name])
    VALUES (2, NULL, N''EDI Import'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[ConfigurationType]'))
        SET IDENTITY_INSERT [ConfigurationType] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920211921_ConfigurationTypeConstants'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260920211921_ConfigurationTypeConstants', N'10.0.12');
END;

COMMIT;
GO