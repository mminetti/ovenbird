BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015025117_AddUsagePurposeAndMakeServiceDatesNullable'
)
BEGIN
    EXEC(N'DELETE FROM [UsageType]
    WHERE [Id] = 3;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015025117_AddUsagePurposeAndMakeServiceDatesNullable'
)
BEGIN
    ALTER TABLE [Usage] ADD [UsagePurposeId] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015025117_AddUsagePurposeAndMakeServiceDatesNullable'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Service]') AND [c].[name] = N'StartDate');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [Service] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [Service] ALTER COLUMN [StartDate] date NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015025117_AddUsagePurposeAndMakeServiceDatesNullable'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Service]') AND [c].[name] = N'EndDate');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Service] DROP CONSTRAINT ' + @var1 + ';');
    ALTER TABLE [Service] ALTER COLUMN [EndDate] date NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015025117_AddUsagePurposeAndMakeServiceDatesNullable'
)
BEGIN
    CREATE TABLE [UsagePurpose] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_UsagePurpose] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015025117_AddUsagePurposeAndMakeServiceDatesNullable'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[UsagePurpose]'))
        SET IDENTITY_INSERT [UsagePurpose] ON;
    EXEC(N'INSERT INTO [UsagePurpose] ([Id], [Name])
    VALUES (1, N''Original''),
    (2, N''Cancel''),
    (3, N''Replace'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[UsagePurpose]'))
        SET IDENTITY_INSERT [UsagePurpose] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015025117_AddUsagePurposeAndMakeServiceDatesNullable'
)
BEGIN
    EXEC(N'UPDATE [UsageType] SET [Name] = N''NonInterval''
    WHERE [Id] = 1;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015025117_AddUsagePurposeAndMakeServiceDatesNullable'
)
BEGIN
    EXEC(N'UPDATE [UsageType] SET [Name] = N''Interval''
    WHERE [Id] = 2;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015025117_AddUsagePurposeAndMakeServiceDatesNullable'
)
BEGIN
    CREATE INDEX [IX_Usage_UsagePurposeId] ON [Usage] ([UsagePurposeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015025117_AddUsagePurposeAndMakeServiceDatesNullable'
)
BEGIN
    ALTER TABLE [Usage] ADD CONSTRAINT [FK_Usage_UsagePurpose_UsagePurposeId] FOREIGN KEY ([UsagePurposeId]) REFERENCES [UsagePurpose] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015025117_AddUsagePurposeAndMakeServiceDatesNullable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261015025117_AddUsagePurposeAndMakeServiceDatesNullable', N'10.0.13');
END;

COMMIT;
GO

