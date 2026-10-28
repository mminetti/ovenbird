BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028200025_AddServiceEventEntities'
)
BEGIN
    CREATE TABLE [ServiceEventActionType] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_ServiceEventActionType] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028200025_AddServiceEventEntities'
)
BEGIN
    CREATE TABLE [ServiceEventReferenceType] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_ServiceEventReferenceType] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028200025_AddServiceEventEntities'
)
BEGIN
    CREATE TABLE [ServiceEventType] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_ServiceEventType] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028200025_AddServiceEventEntities'
)
BEGIN
    CREATE TABLE [ServiceEvent] (
        [Id] bigint NOT NULL IDENTITY,
        [EventTypeId] int NOT NULL,
        [ReferenceTypeId] int NOT NULL,
        [ReferenceId] bigint NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_ServiceEvent] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ServiceEvent_ServiceEventReferenceType_ReferenceTypeId] FOREIGN KEY ([ReferenceTypeId]) REFERENCES [ServiceEventReferenceType] ([Id]),
        CONSTRAINT [FK_ServiceEvent_ServiceEventType_EventTypeId] FOREIGN KEY ([EventTypeId]) REFERENCES [ServiceEventType] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028200025_AddServiceEventEntities'
)
BEGIN
    CREATE TABLE [ServiceEventAction] (
        [Id] bigint NOT NULL IDENTITY,
        [ServiceEventId] bigint NOT NULL,
        [ActionTypeId] int NOT NULL,
        [StatusId] int NOT NULL,
        [Message] nvarchar(max) NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_ServiceEventAction] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ServiceEventAction_ServiceEventActionType_ActionTypeId] FOREIGN KEY ([ActionTypeId]) REFERENCES [ServiceEventActionType] ([Id]),
        CONSTRAINT [FK_ServiceEventAction_ServiceEvent_ServiceEventId] FOREIGN KEY ([ServiceEventId]) REFERENCES [ServiceEvent] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ServiceEventAction_ServiceOrderStatus_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [ServiceOrderStatus] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028200025_AddServiceEventEntities'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[ServiceEventActionType]'))
        SET IDENTITY_INSERT [ServiceEventActionType] ON;
    EXEC(N'INSERT INTO [ServiceEventActionType] ([Id], [Name])
    VALUES (1, N''UpdateServicePoint''),
    (2, N''SendNotification'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[ServiceEventActionType]'))
        SET IDENTITY_INSERT [ServiceEventActionType] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028200025_AddServiceEventEntities'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[ServiceEventReferenceType]'))
        SET IDENTITY_INSERT [ServiceEventReferenceType] ON;
    EXEC(N'INSERT INTO [ServiceEventReferenceType] ([Id], [Name])
    VALUES (1, N''ServiceStartItem'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[ServiceEventReferenceType]'))
        SET IDENTITY_INSERT [ServiceEventReferenceType] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028200025_AddServiceEventEntities'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[ServiceEventType]'))
        SET IDENTITY_INSERT [ServiceEventType] ON;
    EXEC(N'INSERT INTO [ServiceEventType] ([Id], [Name])
    VALUES (1, N''ServiceStartConfirmed'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[ServiceEventType]'))
        SET IDENTITY_INSERT [ServiceEventType] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028200025_AddServiceEventEntities'
)
BEGIN
    CREATE INDEX [IX_ServiceEvent_EventTypeId] ON [ServiceEvent] ([EventTypeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028200025_AddServiceEventEntities'
)
BEGIN
    CREATE INDEX [IX_ServiceEvent_ReferenceTypeId] ON [ServiceEvent] ([ReferenceTypeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028200025_AddServiceEventEntities'
)
BEGIN
    CREATE INDEX [IX_ServiceEventAction_ActionTypeId] ON [ServiceEventAction] ([ActionTypeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028200025_AddServiceEventEntities'
)
BEGIN
    CREATE INDEX [IX_ServiceEventAction_ServiceEventId] ON [ServiceEventAction] ([ServiceEventId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028200025_AddServiceEventEntities'
)
BEGIN
    CREATE INDEX [IX_ServiceEventAction_StatusId] ON [ServiceEventAction] ([StatusId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028200025_AddServiceEventEntities'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261028200025_AddServiceEventEntities', N'10.0.13');
END;

COMMIT;
GO

