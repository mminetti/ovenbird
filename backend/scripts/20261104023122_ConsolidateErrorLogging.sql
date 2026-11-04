BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104023122_ConsolidateErrorLogging'
)
BEGIN
    ALTER TABLE [MarketDocumentEventItem] DROP CONSTRAINT [FK_MarketDocumentEventItem_MarketDocumentEventItemLevel_LevelId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104023122_ConsolidateErrorLogging'
)
BEGIN
    ALTER TABLE [ProcessRunLogItem] DROP CONSTRAINT [FK_ProcessRunLogItem_ProcessRunLogLevel_LevelId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104023122_ConsolidateErrorLogging'
)
BEGIN
    DROP TABLE [MarketDocumentEventItemLevel];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104023122_ConsolidateErrorLogging'
)
BEGIN
    DROP TABLE [ProcessRunLogLevel];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104023122_ConsolidateErrorLogging'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MarketDocumentEvent]') AND [c].[name] = N'Message');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [MarketDocumentEvent] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [MarketDocumentEvent] DROP COLUMN [Message];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104023122_ConsolidateErrorLogging'
)
BEGIN
    EXEC sp_rename N'[ProcessRunLogItem].[LevelId]', N'SeverityId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104023122_ConsolidateErrorLogging'
)
BEGIN
    EXEC sp_rename N'[ProcessRunLogItem].[IX_ProcessRunLogItem_LevelId]', N'IX_ProcessRunLogItem_SeverityId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104023122_ConsolidateErrorLogging'
)
BEGIN
    EXEC sp_rename N'[MarketDocumentEventItem].[LevelId]', N'SeverityId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104023122_ConsolidateErrorLogging'
)
BEGIN
    EXEC sp_rename N'[MarketDocumentEventItem].[IX_MarketDocumentEventItem_LevelId]', N'IX_MarketDocumentEventItem_SeverityId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104023122_ConsolidateErrorLogging'
)
BEGIN
    ALTER TABLE [ProcessRunLogItem] ADD [CreatedBy] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104023122_ConsolidateErrorLogging'
)
BEGIN
    ALTER TABLE [ProcessRunLogItem] ADD [LastModifiedAtUtc] datetimeoffset NOT NULL DEFAULT '0001-01-01T00:00:00.0000000+00:00';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104023122_ConsolidateErrorLogging'
)
BEGIN
    ALTER TABLE [ProcessRunLogItem] ADD [LastModifiedBy] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104023122_ConsolidateErrorLogging'
)
BEGIN
    CREATE TABLE [ErrorSeverity] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_ErrorSeverity] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104023122_ConsolidateErrorLogging'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[ErrorSeverity]'))
        SET IDENTITY_INSERT [ErrorSeverity] ON;
    EXEC(N'INSERT INTO [ErrorSeverity] ([Id], [Name])
    VALUES (1, N''Error''),
    (2, N''Warning''),
    (3, N''Info'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[ErrorSeverity]'))
        SET IDENTITY_INSERT [ErrorSeverity] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104023122_ConsolidateErrorLogging'
)
BEGIN
    ALTER TABLE [MarketDocumentEventItem] ADD CONSTRAINT [FK_MarketDocumentEventItem_ErrorSeverity_SeverityId] FOREIGN KEY ([SeverityId]) REFERENCES [ErrorSeverity] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104023122_ConsolidateErrorLogging'
)
BEGIN
    ALTER TABLE [ProcessRunLogItem] ADD CONSTRAINT [FK_ProcessRunLogItem_ErrorSeverity_SeverityId] FOREIGN KEY ([SeverityId]) REFERENCES [ErrorSeverity] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104023122_ConsolidateErrorLogging'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261104023122_ConsolidateErrorLogging', N'10.0.14');
END;

COMMIT;
GO

