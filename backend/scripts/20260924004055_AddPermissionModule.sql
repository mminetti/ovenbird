BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924004055_AddPermissionModule'
)
BEGIN
    ALTER TABLE [Permission] ADD [ModuleId] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924004055_AddPermissionModule'
)
BEGIN
    CREATE TABLE [PermissionModule] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_PermissionModule] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924004055_AddPermissionModule'
)
BEGIN
    EXEC(N'UPDATE [Permission] SET [ModuleId] = 1
    WHERE [Id] = 1;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924004055_AddPermissionModule'
)
BEGIN
    EXEC(N'UPDATE [Permission] SET [ModuleId] = 1
    WHERE [Id] = 2;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924004055_AddPermissionModule'
)
BEGIN
    EXEC(N'UPDATE [Permission] SET [ModuleId] = 1
    WHERE [Id] = 3;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924004055_AddPermissionModule'
)
BEGIN
    EXEC(N'UPDATE [Permission] SET [ModuleId] = 1
    WHERE [Id] = 4;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924004055_AddPermissionModule'
)
BEGIN
    EXEC(N'UPDATE [Permission] SET [ModuleId] = 1
    WHERE [Id] = 5;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924004055_AddPermissionModule'
)
BEGIN
    EXEC(N'UPDATE [Permission] SET [ModuleId] = 1
    WHERE [Id] = 6;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924004055_AddPermissionModule'
)
BEGIN
    EXEC(N'UPDATE [Permission] SET [ModuleId] = 2
    WHERE [Id] = 7;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924004055_AddPermissionModule'
)
BEGIN
    EXEC(N'UPDATE [Permission] SET [ModuleId] = 2
    WHERE [Id] = 8;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924004055_AddPermissionModule'
)
BEGIN
    EXEC(N'UPDATE [Permission] SET [ModuleId] = 2
    WHERE [Id] = 9;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924004055_AddPermissionModule'
)
BEGIN
    EXEC(N'UPDATE [Permission] SET [ModuleId] = 2
    WHERE [Id] = 10;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924004055_AddPermissionModule'
)
BEGIN
    EXEC(N'UPDATE [Permission] SET [ModuleId] = 2
    WHERE [Id] = 11;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924004055_AddPermissionModule'
)
BEGIN
    EXEC(N'UPDATE [Permission] SET [ModuleId] = 2
    WHERE [Id] = 12;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924004055_AddPermissionModule'
)
BEGIN
    EXEC(N'UPDATE [Permission] SET [ModuleId] = 2
    WHERE [Id] = 13;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924004055_AddPermissionModule'
)
BEGIN
    EXEC(N'UPDATE [Permission] SET [ModuleId] = 2
    WHERE [Id] = 14;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924004055_AddPermissionModule'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[PermissionModule]'))
        SET IDENTITY_INSERT [PermissionModule] ON;
    EXEC(N'INSERT INTO [PermissionModule] ([Id], [Name])
    VALUES (1, N''Security''),
    (2, N''Settings'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[PermissionModule]'))
        SET IDENTITY_INSERT [PermissionModule] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924004055_AddPermissionModule'
)
BEGIN
    CREATE INDEX [IX_Permission_ModuleId] ON [Permission] ([ModuleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924004055_AddPermissionModule'
)
BEGIN
    ALTER TABLE [Permission] ADD CONSTRAINT [FK_Permission_PermissionModule_ModuleId] FOREIGN KEY ([ModuleId]) REFERENCES [PermissionModule] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924004055_AddPermissionModule'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260924004055_AddPermissionModule', N'10.0.12');
END;

COMMIT;
GO

