BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914233052_SeedPermissions'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAtUtc', N'CreatedBy', N'Description', N'LastModifiedAtUtc', N'LastModifiedBy', N'Name') AND [object_id] = OBJECT_ID(N'[Permission]'))
        SET IDENTITY_INSERT [Permission] ON;
    EXEC(N'INSERT INTO [Permission] ([Id], [CreatedAtUtc], [CreatedBy], [Description], [LastModifiedAtUtc], [LastModifiedBy], [Name])
    VALUES (-6, ''2026-09-14T00:00:00.0000000+00:00'', NULL, N''Allows creating, updating, and deleting permissions.'', ''2026-09-14T00:00:00.0000000+00:00'', NULL, N''permissions.write''),
    (-5, ''2026-09-14T00:00:00.0000000+00:00'', NULL, N''Allows viewing permissions.'', ''2026-09-14T00:00:00.0000000+00:00'', NULL, N''permissions.read''),
    (-4, ''2026-09-14T00:00:00.0000000+00:00'', NULL, N''Allows creating, updating, and deleting roles.'', ''2026-09-14T00:00:00.0000000+00:00'', NULL, N''roles.write''),
    (-3, ''2026-09-14T00:00:00.0000000+00:00'', NULL, N''Allows viewing roles.'', ''2026-09-14T00:00:00.0000000+00:00'', NULL, N''roles.read''),
    (-2, ''2026-09-14T00:00:00.0000000+00:00'', NULL, N''Allows creating, updating, and deleting users.'', ''2026-09-14T00:00:00.0000000+00:00'', NULL, N''users.write''),
    (-1, ''2026-09-14T00:00:00.0000000+00:00'', NULL, N''Allows viewing users.'', ''2026-09-14T00:00:00.0000000+00:00'', NULL, N''users.read'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAtUtc', N'CreatedBy', N'Description', N'LastModifiedAtUtc', N'LastModifiedBy', N'Name') AND [object_id] = OBJECT_ID(N'[Permission]'))
        SET IDENTITY_INSERT [Permission] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914233052_SeedPermissions'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260914233052_SeedPermissions', N'10.0.12');
END;

COMMIT;
GO

