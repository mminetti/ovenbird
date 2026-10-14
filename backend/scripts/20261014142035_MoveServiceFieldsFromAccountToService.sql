BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014142035_MoveServiceFieldsFromAccountToService'
)
BEGIN
    ALTER TABLE [Account] DROP CONSTRAINT [FK_Account_ServiceConnectionStatus_ServiceConnectionStatusId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014142035_MoveServiceFieldsFromAccountToService'
)
BEGIN
    ALTER TABLE [Account] DROP CONSTRAINT [FK_Account_ServiceStatus_ServiceStatusId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014142035_MoveServiceFieldsFromAccountToService'
)
BEGIN
    ALTER TABLE [Account] DROP CONSTRAINT [FK_Account_ServiceSubStatus_ServiceSubStatusId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014142035_MoveServiceFieldsFromAccountToService'
)
BEGIN
    DROP INDEX [IX_Account_ServiceConnectionStatusId] ON [Account];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014142035_MoveServiceFieldsFromAccountToService'
)
BEGIN
    DROP INDEX [IX_Account_ServiceStatusId] ON [Account];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014142035_MoveServiceFieldsFromAccountToService'
)
BEGIN
    DROP INDEX [IX_Account_ServiceSubStatusId] ON [Account];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014142035_MoveServiceFieldsFromAccountToService'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Account]') AND [c].[name] = N'IsServiceLocked');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [Account] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [Account] DROP COLUMN [IsServiceLocked];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014142035_MoveServiceFieldsFromAccountToService'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Account]') AND [c].[name] = N'IsServiceProtected');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Account] DROP CONSTRAINT ' + @var1 + ';');
    ALTER TABLE [Account] DROP COLUMN [IsServiceProtected];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014142035_MoveServiceFieldsFromAccountToService'
)
BEGIN
    DECLARE @var2 nvarchar(max);
    SELECT @var2 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Account]') AND [c].[name] = N'ServiceConnectionStatusId');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Account] DROP CONSTRAINT ' + @var2 + ';');
    ALTER TABLE [Account] DROP COLUMN [ServiceConnectionStatusId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014142035_MoveServiceFieldsFromAccountToService'
)
BEGIN
    DECLARE @var3 nvarchar(max);
    SELECT @var3 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Account]') AND [c].[name] = N'ServiceEndDate');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Account] DROP CONSTRAINT ' + @var3 + ';');
    ALTER TABLE [Account] DROP COLUMN [ServiceEndDate];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014142035_MoveServiceFieldsFromAccountToService'
)
BEGIN
    DECLARE @var4 nvarchar(max);
    SELECT @var4 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Account]') AND [c].[name] = N'ServiceStartDate');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Account] DROP CONSTRAINT ' + @var4 + ';');
    ALTER TABLE [Account] DROP COLUMN [ServiceStartDate];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014142035_MoveServiceFieldsFromAccountToService'
)
BEGIN
    DECLARE @var5 nvarchar(max);
    SELECT @var5 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Account]') AND [c].[name] = N'ServiceStatusId');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Account] DROP CONSTRAINT ' + @var5 + ';');
    ALTER TABLE [Account] DROP COLUMN [ServiceStatusId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014142035_MoveServiceFieldsFromAccountToService'
)
BEGIN
    DECLARE @var6 nvarchar(max);
    SELECT @var6 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Account]') AND [c].[name] = N'ServiceSubStatusId');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [Account] DROP CONSTRAINT ' + @var6 + ';');
    ALTER TABLE [Account] DROP COLUMN [ServiceSubStatusId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014142035_MoveServiceFieldsFromAccountToService'
)
BEGIN
    CREATE TABLE [Service] (
        [Id] bigint NOT NULL IDENTITY,
        [AccountId] bigint NOT NULL,
        [StartDate] date NOT NULL,
        [EndDate] date NOT NULL,
        [StatusId] int NOT NULL,
        [SubStatusId] int NOT NULL,
        [ConnectionStatusId] int NOT NULL,
        [IsLocked] bit NOT NULL,
        [IsProtected] bit NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_Service] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Service_Account_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [Account] ([Id]),
        CONSTRAINT [FK_Service_ServiceConnectionStatus_ConnectionStatusId] FOREIGN KEY ([ConnectionStatusId]) REFERENCES [ServiceConnectionStatus] ([Id]),
        CONSTRAINT [FK_Service_ServiceStatus_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [ServiceStatus] ([Id]),
        CONSTRAINT [FK_Service_ServiceSubStatus_SubStatusId] FOREIGN KEY ([SubStatusId]) REFERENCES [ServiceSubStatus] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014142035_MoveServiceFieldsFromAccountToService'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Service_AccountId] ON [Service] ([AccountId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014142035_MoveServiceFieldsFromAccountToService'
)
BEGIN
    CREATE INDEX [IX_Service_ConnectionStatusId] ON [Service] ([ConnectionStatusId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014142035_MoveServiceFieldsFromAccountToService'
)
BEGIN
    CREATE INDEX [IX_Service_StatusId] ON [Service] ([StatusId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014142035_MoveServiceFieldsFromAccountToService'
)
BEGIN
    CREATE INDEX [IX_Service_SubStatusId] ON [Service] ([SubStatusId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014142035_MoveServiceFieldsFromAccountToService'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261014142035_MoveServiceFieldsFromAccountToService', N'10.0.13');
END;

COMMIT;
GO

