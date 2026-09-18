BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918185259_ConfigurationTypeDescription'
)
BEGIN
    ALTER TABLE [Configuration] DROP CONSTRAINT [FK_Configuration_ConfigurationType_ConfigurationTypeId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918185259_ConfigurationTypeDescription'
)
BEGIN
    ALTER TABLE [ConfigurationType] DROP CONSTRAINT [PK_ConfigurationType];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918185259_ConfigurationTypeDescription'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ConfigurationType]') AND [c].[name] = N'Id');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [ConfigurationType] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [ConfigurationType] DROP COLUMN [Id];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918185259_ConfigurationTypeDescription'
)
BEGIN
    ALTER TABLE [ConfigurationType] ADD [Id] int NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918185259_ConfigurationTypeDescription'
)
BEGIN
    ALTER TABLE [ConfigurationType] ADD [Description] nvarchar(1000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918185259_ConfigurationTypeDescription'
)
BEGIN
    ALTER TABLE [ConfigurationType] ADD CONSTRAINT [PK_ConfigurationType] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918185259_ConfigurationTypeDescription'
)
BEGIN
    ALTER TABLE [Configuration] ADD CONSTRAINT [FK_Configuration_ConfigurationType_ConfigurationTypeId] FOREIGN KEY ([ConfigurationTypeId]) REFERENCES [ConfigurationType] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918185259_ConfigurationTypeDescription'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[ConfigurationType]'))
        SET IDENTITY_INSERT [ConfigurationType] ON;
    EXEC(N'INSERT INTO [ConfigurationType] ([Id], [Description], [Name])
    VALUES (1, NULL, N''EDI Import'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[ConfigurationType]'))
        SET IDENTITY_INSERT [ConfigurationType] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918185259_ConfigurationTypeDescription'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260918185259_ConfigurationTypeDescription', N'10.0.12');
END;

COMMIT;
GO

