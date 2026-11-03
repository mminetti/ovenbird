BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103013725_ReplaceMarketDocumentReferenceWithMarketDocumentEvent'
)
BEGIN
    DROP TABLE [MarketDocumentReference];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103013725_ReplaceMarketDocumentReferenceWithMarketDocumentEvent'
)
BEGIN
    DROP TABLE [MarketDocumentReferenceType];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103013725_ReplaceMarketDocumentReferenceWithMarketDocumentEvent'
)
BEGIN
    ALTER TABLE [MarketDocumentItem] ADD [Message] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103013725_ReplaceMarketDocumentReferenceWithMarketDocumentEvent'
)
BEGIN
    ALTER TABLE [MarketDocument] ADD [Message] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103013725_ReplaceMarketDocumentReferenceWithMarketDocumentEvent'
)
BEGIN
    CREATE TABLE [MarketDocumentEventItemLevel] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_MarketDocumentEventItemLevel] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103013725_ReplaceMarketDocumentReferenceWithMarketDocumentEvent'
)
BEGIN
    CREATE TABLE [MarketDocumentEventStatus] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_MarketDocumentEventStatus] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103013725_ReplaceMarketDocumentReferenceWithMarketDocumentEvent'
)
BEGIN
    CREATE TABLE [MarketDocumentEventType] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_MarketDocumentEventType] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103013725_ReplaceMarketDocumentReferenceWithMarketDocumentEvent'
)
BEGIN
    CREATE TABLE [MarketDocumentEvent] (
        [Id] bigint NOT NULL IDENTITY,
        [MarketDocumentItemId] bigint NOT NULL,
        [EventTypeId] int NOT NULL,
        [ReferenceId] bigint NULL,
        [StatusId] int NOT NULL,
        [Message] nvarchar(max) NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_MarketDocumentEvent] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MarketDocumentEvent_MarketDocumentEventStatus_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [MarketDocumentEventStatus] ([Id]),
        CONSTRAINT [FK_MarketDocumentEvent_MarketDocumentEventType_EventTypeId] FOREIGN KEY ([EventTypeId]) REFERENCES [MarketDocumentEventType] ([Id]),
        CONSTRAINT [FK_MarketDocumentEvent_MarketDocumentItem_MarketDocumentItemId] FOREIGN KEY ([MarketDocumentItemId]) REFERENCES [MarketDocumentItem] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103013725_ReplaceMarketDocumentReferenceWithMarketDocumentEvent'
)
BEGIN
    CREATE TABLE [MarketDocumentEventItem] (
        [Id] bigint NOT NULL IDENTITY,
        [MarketDocumentEventId] bigint NOT NULL,
        [LevelId] int NOT NULL,
        [Field] nvarchar(250) NOT NULL,
        [Message] nvarchar(max) NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_MarketDocumentEventItem] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MarketDocumentEventItem_MarketDocumentEventItemLevel_LevelId] FOREIGN KEY ([LevelId]) REFERENCES [MarketDocumentEventItemLevel] ([Id]),
        CONSTRAINT [FK_MarketDocumentEventItem_MarketDocumentEvent_MarketDocumentEventId] FOREIGN KEY ([MarketDocumentEventId]) REFERENCES [MarketDocumentEvent] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103013725_ReplaceMarketDocumentReferenceWithMarketDocumentEvent'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[MarketDocumentEventItemLevel]'))
        SET IDENTITY_INSERT [MarketDocumentEventItemLevel] ON;
    EXEC(N'INSERT INTO [MarketDocumentEventItemLevel] ([Id], [Name])
    VALUES (1, N''Error''),
    (2, N''Warning''),
    (3, N''Info'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[MarketDocumentEventItemLevel]'))
        SET IDENTITY_INSERT [MarketDocumentEventItemLevel] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103013725_ReplaceMarketDocumentReferenceWithMarketDocumentEvent'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[MarketDocumentEventStatus]'))
        SET IDENTITY_INSERT [MarketDocumentEventStatus] ON;
    EXEC(N'INSERT INTO [MarketDocumentEventStatus] ([Id], [Name])
    VALUES (1, N''Done''),
    (2, N''Error'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[MarketDocumentEventStatus]'))
        SET IDENTITY_INSERT [MarketDocumentEventStatus] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103013725_ReplaceMarketDocumentReferenceWithMarketDocumentEvent'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[MarketDocumentEventType]'))
        SET IDENTITY_INSERT [MarketDocumentEventType] ON;
    EXEC(N'INSERT INTO [MarketDocumentEventType] ([Id], [Name])
    VALUES (1, N''HistoricalUsage''),
    (2, N''Usage''),
    (3, N''Invoice''),
    (4, N''ServiceStart''),
    (5, N''ServiceEnd''),
    (6, N''ServiceRequest'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[MarketDocumentEventType]'))
        SET IDENTITY_INSERT [MarketDocumentEventType] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103013725_ReplaceMarketDocumentReferenceWithMarketDocumentEvent'
)
BEGIN
    CREATE INDEX [IX_MarketDocumentEvent_EventTypeId] ON [MarketDocumentEvent] ([EventTypeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103013725_ReplaceMarketDocumentReferenceWithMarketDocumentEvent'
)
BEGIN
    CREATE INDEX [IX_MarketDocumentEvent_MarketDocumentItemId] ON [MarketDocumentEvent] ([MarketDocumentItemId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103013725_ReplaceMarketDocumentReferenceWithMarketDocumentEvent'
)
BEGIN
    CREATE INDEX [IX_MarketDocumentEvent_StatusId] ON [MarketDocumentEvent] ([StatusId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103013725_ReplaceMarketDocumentReferenceWithMarketDocumentEvent'
)
BEGIN
    CREATE INDEX [IX_MarketDocumentEventItem_LevelId] ON [MarketDocumentEventItem] ([LevelId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103013725_ReplaceMarketDocumentReferenceWithMarketDocumentEvent'
)
BEGIN
    CREATE INDEX [IX_MarketDocumentEventItem_MarketDocumentEventId] ON [MarketDocumentEventItem] ([MarketDocumentEventId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103013725_ReplaceMarketDocumentReferenceWithMarketDocumentEvent'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261103013725_ReplaceMarketDocumentReferenceWithMarketDocumentEvent', N'10.0.14');
END;

COMMIT;
GO

