BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012180407_AddEdiExportConfigurationType'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[ConfigurationType]'))
        SET IDENTITY_INSERT [ConfigurationType] ON;
    EXEC(N'INSERT INTO [ConfigurationType] ([Id], [Description], [Name])
    VALUES (3, NULL, N''EDI Export'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[ConfigurationType]'))
        SET IDENTITY_INSERT [ConfigurationType] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012180407_AddEdiExportConfigurationType'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261012180407_AddEdiExportConfigurationType', N'10.0.12');
END;

COMMIT;
GO

