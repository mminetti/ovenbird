BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011011535_AddServiceStartEntities'
)
BEGIN
    CREATE TABLE [ServiceAction] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_ServiceAction] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011011535_AddServiceStartEntities'
)
BEGIN
    CREATE TABLE [ServiceStartType] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_ServiceStartType] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011011535_AddServiceStartEntities'
)
BEGIN
    CREATE TABLE [ServiceStatus] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_ServiceStatus] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011011535_AddServiceStartEntities'
)
BEGIN
    CREATE TABLE [ServiceStart] (
        [Id] bigint NOT NULL IDENTITY,
        [AccountId] bigint NOT NULL,
        [ServiceStartTypeId] int NOT NULL,
        [ServiceDate] date NOT NULL,
        [ServiceStatusId] int NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_ServiceStart] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ServiceStart_Account_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [Account] ([Id]),
        CONSTRAINT [FK_ServiceStart_ServiceStartType_ServiceStartTypeId] FOREIGN KEY ([ServiceStartTypeId]) REFERENCES [ServiceStartType] ([Id]),
        CONSTRAINT [FK_ServiceStart_ServiceStatus_ServiceStatusId] FOREIGN KEY ([ServiceStatusId]) REFERENCES [ServiceStatus] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011011535_AddServiceStartEntities'
)
BEGIN
    CREATE TABLE [ServiceStartItem] (
        [Id] bigint NOT NULL IDENTITY,
        [ServiceStartId] bigint NOT NULL,
        [Identifier] nvarchar(250) NOT NULL,
        [ServiceDate] date NOT NULL,
        [ServiceActionId] int NOT NULL,
        [Description] nvarchar(1000) NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_ServiceStartItem] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ServiceStartItem_ServiceAction_ServiceActionId] FOREIGN KEY ([ServiceActionId]) REFERENCES [ServiceAction] ([Id]),
        CONSTRAINT [FK_ServiceStartItem_ServiceStart_ServiceStartId] FOREIGN KEY ([ServiceStartId]) REFERENCES [ServiceStart] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011011535_AddServiceStartEntities'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[ServiceAction]'))
        SET IDENTITY_INSERT [ServiceAction] ON;
    EXEC(N'INSERT INTO [ServiceAction] ([Id], [Name])
    VALUES (1, N''Initiate''),
    (2, N''Confirm''),
    (3, N''Reject''),
    (4, N''Complete''),
    (5, N''Cancel''),
    (6, N''Unexecute''),
    (7, N''Error''),
    (8, N''Change'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[ServiceAction]'))
        SET IDENTITY_INSERT [ServiceAction] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011011535_AddServiceStartEntities'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[ServiceStartType]'))
        SET IDENTITY_INSERT [ServiceStartType] ON;
    EXEC(N'INSERT INTO [ServiceStartType] ([Id], [Name])
    VALUES (1, N''MoveIn''),
    (2, N''Switch'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[ServiceStartType]'))
        SET IDENTITY_INSERT [ServiceStartType] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011011535_AddServiceStartEntities'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[ServiceStatus]'))
        SET IDENTITY_INSERT [ServiceStatus] ON;
    EXEC(N'INSERT INTO [ServiceStatus] ([Id], [Name])
    VALUES (1, N''New''),
    (2, N''InProgress''),
    (3, N''Scheduled''),
    (4, N''Cancelled''),
    (5, N''Rejected''),
    (6, N''Completed''),
    (7, N''Unexecuted''),
    (8, N''Error'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[ServiceStatus]'))
        SET IDENTITY_INSERT [ServiceStatus] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011011535_AddServiceStartEntities'
)
BEGIN
    CREATE INDEX [IX_ServiceStart_AccountId] ON [ServiceStart] ([AccountId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011011535_AddServiceStartEntities'
)
BEGIN
    CREATE INDEX [IX_ServiceStart_ServiceStartTypeId] ON [ServiceStart] ([ServiceStartTypeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011011535_AddServiceStartEntities'
)
BEGIN
    CREATE INDEX [IX_ServiceStart_ServiceStatusId] ON [ServiceStart] ([ServiceStatusId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011011535_AddServiceStartEntities'
)
BEGIN
    CREATE INDEX [IX_ServiceStartItem_ServiceActionId] ON [ServiceStartItem] ([ServiceActionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011011535_AddServiceStartEntities'
)
BEGIN
    CREATE INDEX [IX_ServiceStartItem_ServiceStartId] ON [ServiceStartItem] ([ServiceStartId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011011535_AddServiceStartEntities'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261011011535_AddServiceStartEntities', N'10.0.12');
END;

COMMIT;
GO

