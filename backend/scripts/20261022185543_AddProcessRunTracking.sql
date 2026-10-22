BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261022185543_AddProcessRunTracking'
)
BEGIN
    CREATE TABLE [ProcessRunLogLevel] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_ProcessRunLogLevel] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261022185543_AddProcessRunTracking'
)
BEGIN
    CREATE TABLE [ProcessRunStatus] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_ProcessRunStatus] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261022185543_AddProcessRunTracking'
)
BEGIN
    CREATE TABLE [ProcessType] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_ProcessType] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261022185543_AddProcessRunTracking'
)
BEGIN
    CREATE TABLE [ProcessRun] (
        [Id] bigint NOT NULL IDENTITY,
        [ProcessTypeId] int NOT NULL,
        [StatusId] int NOT NULL,
        [StartedAtUtc] datetimeoffset NOT NULL,
        [CompletedAtUtc] datetimeoffset NULL,
        CONSTRAINT [PK_ProcessRun] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProcessRun_ProcessRunStatus_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [ProcessRunStatus] ([Id]),
        CONSTRAINT [FK_ProcessRun_ProcessType_ProcessTypeId] FOREIGN KEY ([ProcessTypeId]) REFERENCES [ProcessType] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261022185543_AddProcessRunTracking'
)
BEGIN
    CREATE TABLE [ProcessRunLogItem] (
        [Id] bigint NOT NULL IDENTITY,
        [ProcessRunId] bigint NOT NULL,
        [LevelId] int NOT NULL,
        [EntityType] nvarchar(250) NULL,
        [EntityId] nvarchar(250) NULL,
        [Field] nvarchar(250) NULL,
        [Message] nvarchar(max) NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        CONSTRAINT [PK_ProcessRunLogItem] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProcessRunLogItem_ProcessRunLogLevel_LevelId] FOREIGN KEY ([LevelId]) REFERENCES [ProcessRunLogLevel] ([Id]),
        CONSTRAINT [FK_ProcessRunLogItem_ProcessRun_ProcessRunId] FOREIGN KEY ([ProcessRunId]) REFERENCES [ProcessRun] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261022185543_AddProcessRunTracking'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[ProcessRunLogLevel]'))
        SET IDENTITY_INSERT [ProcessRunLogLevel] ON;
    EXEC(N'INSERT INTO [ProcessRunLogLevel] ([Id], [Name])
    VALUES (1, N''Error''),
    (2, N''Warning''),
    (3, N''Info'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[ProcessRunLogLevel]'))
        SET IDENTITY_INSERT [ProcessRunLogLevel] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261022185543_AddProcessRunTracking'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[ProcessRunStatus]'))
        SET IDENTITY_INSERT [ProcessRunStatus] ON;
    EXEC(N'INSERT INTO [ProcessRunStatus] ([Id], [Name])
    VALUES (1, N''Running''),
    (2, N''Succeeded''),
    (3, N''Failed'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[ProcessRunStatus]'))
        SET IDENTITY_INSERT [ProcessRunStatus] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261022185543_AddProcessRunTracking'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[ProcessType]'))
        SET IDENTITY_INSERT [ProcessType] ON;
    EXEC(N'INSERT INTO [ProcessType] ([Id], [Name])
    VALUES (1, N''MarketDocumentImport''),
    (2, N''MarketDocumentExport''),
    (3, N''MarketDocumentItemExport'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[ProcessType]'))
        SET IDENTITY_INSERT [ProcessType] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261022185543_AddProcessRunTracking'
)
BEGIN
    CREATE INDEX [IX_ProcessRun_ProcessTypeId] ON [ProcessRun] ([ProcessTypeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261022185543_AddProcessRunTracking'
)
BEGIN
    CREATE INDEX [IX_ProcessRun_StatusId] ON [ProcessRun] ([StatusId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261022185543_AddProcessRunTracking'
)
BEGIN
    CREATE INDEX [IX_ProcessRunLogItem_EntityType_EntityId] ON [ProcessRunLogItem] ([EntityType], [EntityId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261022185543_AddProcessRunTracking'
)
BEGIN
    CREATE INDEX [IX_ProcessRunLogItem_LevelId] ON [ProcessRunLogItem] ([LevelId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261022185543_AddProcessRunTracking'
)
BEGIN
    CREATE INDEX [IX_ProcessRunLogItem_ProcessRunId] ON [ProcessRunLogItem] ([ProcessRunId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261022185543_AddProcessRunTracking'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261022185543_AddProcessRunTracking', N'10.0.13');
END;

COMMIT;
GO

