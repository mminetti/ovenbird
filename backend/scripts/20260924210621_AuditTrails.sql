BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924210621_AuditTrails'
)
BEGIN
    CREATE TABLE [AuditTrail] (
        [Id] bigint NOT NULL IDENTITY,
        [EntityType] nvarchar(250) NOT NULL,
        [EntityId] nvarchar(250) NOT NULL,
        [Action] int NOT NULL,
        [UserId] nvarchar(250) NULL,
        [TimestampUtc] datetimeoffset NOT NULL,
        [OldValues] nvarchar(max) NULL,
        [NewValues] nvarchar(max) NULL,
        [AffectedColumns] nvarchar(max) NULL,
        CONSTRAINT [PK_AuditTrail] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924210621_AuditTrails'
)
BEGIN
    CREATE TABLE [AuditTrailReference] (
        [Id] int NOT NULL IDENTITY,
        [ReferencedEntityType] nvarchar(250) NOT NULL,
        [ReferencedEntityId] nvarchar(250) NOT NULL,
        [AuditTrailId] bigint NOT NULL,
        CONSTRAINT [PK_AuditTrailReference] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AuditTrailReference_AuditTrail_AuditTrailId] FOREIGN KEY ([AuditTrailId]) REFERENCES [AuditTrail] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924210621_AuditTrails'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'ModuleId', N'Name') AND [object_id] = OBJECT_ID(N'[Permission]'))
        SET IDENTITY_INSERT [Permission] ON;
    EXEC(N'INSERT INTO [Permission] ([Id], [Description], [ModuleId], [Name])
    VALUES (15, N''Allows viewing the audit trail.'', 1, N''audit.trails.read'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'ModuleId', N'Name') AND [object_id] = OBJECT_ID(N'[Permission]'))
        SET IDENTITY_INSERT [Permission] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924210621_AuditTrails'
)
BEGIN
    CREATE INDEX [IX_AuditTrail_EntityType_EntityId] ON [AuditTrail] ([EntityType], [EntityId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924210621_AuditTrails'
)
BEGIN
    CREATE INDEX [IX_AuditTrailReference_AuditTrailId] ON [AuditTrailReference] ([AuditTrailId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924210621_AuditTrails'
)
BEGIN
    CREATE INDEX [IX_AuditTrailReference_ReferencedEntityType_ReferencedEntityId] ON [AuditTrailReference] ([ReferencedEntityType], [ReferencedEntityId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924210621_AuditTrails'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260924210621_AuditTrails', N'10.0.12');
END;

COMMIT;
GO