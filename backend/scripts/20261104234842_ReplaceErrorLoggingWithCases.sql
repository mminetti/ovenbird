BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104234842_ReplaceErrorLoggingWithCases'
)
BEGIN
    DROP TABLE [MarketDocumentEventItem];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104234842_ReplaceErrorLoggingWithCases'
)
BEGIN
    DROP TABLE [ProcessRunLogItem];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104234842_ReplaceErrorLoggingWithCases'
)
BEGIN
    CREATE TABLE [CaseEntityType] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_CaseEntityType] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104234842_ReplaceErrorLoggingWithCases'
)
BEGIN
    CREATE TABLE [CaseStatus] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_CaseStatus] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104234842_ReplaceErrorLoggingWithCases'
)
BEGIN
    CREATE TABLE [Case] (
        [Id] bigint NOT NULL IDENTITY,
        [EntityTypeId] int NOT NULL,
        [EntityId] bigint NOT NULL,
        [StatusId] int NOT NULL,
        [AssignedToUserId] int NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_Case] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Case_CaseEntityType_EntityTypeId] FOREIGN KEY ([EntityTypeId]) REFERENCES [CaseEntityType] ([Id]),
        CONSTRAINT [FK_Case_CaseStatus_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [CaseStatus] ([Id]),
        CONSTRAINT [FK_Case_User_AssignedToUserId] FOREIGN KEY ([AssignedToUserId]) REFERENCES [User] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104234842_ReplaceErrorLoggingWithCases'
)
BEGIN
    CREATE TABLE [CaseItem] (
        [Id] bigint NOT NULL IDENTITY,
        [CaseId] bigint NOT NULL,
        [SeverityId] int NOT NULL,
        [Field] nvarchar(250) NOT NULL,
        [Message] nvarchar(max) NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_CaseItem] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CaseItem_Case_CaseId] FOREIGN KEY ([CaseId]) REFERENCES [Case] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CaseItem_ErrorSeverity_SeverityId] FOREIGN KEY ([SeverityId]) REFERENCES [ErrorSeverity] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104234842_ReplaceErrorLoggingWithCases'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[CaseEntityType]'))
        SET IDENTITY_INSERT [CaseEntityType] ON;
    EXEC(N'INSERT INTO [CaseEntityType] ([Id], [Name])
    VALUES (1, N''ProcessRun''),
    (2, N''MarketDocumentItem''),
    (3, N''MarketDocument'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[CaseEntityType]'))
        SET IDENTITY_INSERT [CaseEntityType] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104234842_ReplaceErrorLoggingWithCases'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[CaseStatus]'))
        SET IDENTITY_INSERT [CaseStatus] ON;
    EXEC(N'INSERT INTO [CaseStatus] ([Id], [Name])
    VALUES (1, N''Open''),
    (2, N''InProgress''),
    (3, N''Resolved''),
    (4, N''Cancelled'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[CaseStatus]'))
        SET IDENTITY_INSERT [CaseStatus] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104234842_ReplaceErrorLoggingWithCases'
)
BEGIN
    CREATE INDEX [IX_Case_AssignedToUserId] ON [Case] ([AssignedToUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104234842_ReplaceErrorLoggingWithCases'
)
BEGIN
    CREATE INDEX [IX_Case_EntityTypeId] ON [Case] ([EntityTypeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104234842_ReplaceErrorLoggingWithCases'
)
BEGIN
    CREATE INDEX [IX_Case_StatusId] ON [Case] ([StatusId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104234842_ReplaceErrorLoggingWithCases'
)
BEGIN
    CREATE INDEX [IX_CaseItem_CaseId] ON [CaseItem] ([CaseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104234842_ReplaceErrorLoggingWithCases'
)
BEGIN
    CREATE INDEX [IX_CaseItem_SeverityId] ON [CaseItem] ([SeverityId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104234842_ReplaceErrorLoggingWithCases'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261104234842_ReplaceErrorLoggingWithCases', N'10.0.14');
END;

COMMIT;
GO

