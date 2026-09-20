BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918203051_SeedConfigurationPermissions'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[Permission]'))
        SET IDENTITY_INSERT [Permission] ON;
    EXEC(N'INSERT INTO [Permission] ([Id], [Description], [Name])
    VALUES (13, N''Allows viewing configurations.'', N''configurations.read''),
    (14, N''Allows creating, updating, and deleting configurations.'', N''configurations.write'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[Permission]'))
        SET IDENTITY_INSERT [Permission] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918203051_SeedConfigurationPermissions'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260918203051_SeedConfigurationPermissions', N'10.0.12');
END;

COMMIT;
GO