BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918200253_MarketCrud'
)
BEGIN
    ALTER TABLE [Company] DROP CONSTRAINT [FK_Company_Market_MarketId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918200253_MarketCrud'
)
BEGIN
    ALTER TABLE [Market] DROP CONSTRAINT [PK_Market];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918200253_MarketCrud'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Market]') AND [c].[name] = N'Id');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [Market] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [Market] DROP COLUMN [Id];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918200253_MarketCrud'
)
BEGIN
    ALTER TABLE [Market] ADD [Id] int NOT NULL IDENTITY;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918200253_MarketCrud'
)
BEGIN
    ALTER TABLE [Market] ADD CONSTRAINT [PK_Market] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918200253_MarketCrud'
)
BEGIN
    ALTER TABLE [Company] ADD CONSTRAINT [FK_Company_Market_MarketId] FOREIGN KEY ([MarketId]) REFERENCES [Market] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918200253_MarketCrud'
)
BEGIN
    ALTER TABLE [Market] ADD [CreatedAtUtc] datetimeoffset NOT NULL DEFAULT '0001-01-01T00:00:00.0000000+00:00';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918200253_MarketCrud'
)
BEGIN
    ALTER TABLE [Market] ADD [CreatedBy] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918200253_MarketCrud'
)
BEGIN
    ALTER TABLE [Market] ADD [LastModifiedAtUtc] datetimeoffset NOT NULL DEFAULT '0001-01-01T00:00:00.0000000+00:00';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918200253_MarketCrud'
)
BEGIN
    ALTER TABLE [Market] ADD [LastModifiedBy] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918200253_MarketCrud'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[Permission]'))
        SET IDENTITY_INSERT [Permission] ON;
    EXEC(N'INSERT INTO [Permission] ([Id], [Description], [Name])
    VALUES (11, N''Allows viewing markets.'', N''markets.read''),
    (12, N''Allows creating, updating, and deleting markets.'', N''markets.write'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[Permission]'))
        SET IDENTITY_INSERT [Permission] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918200253_MarketCrud'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260918200253_MarketCrud', N'10.0.12');
END;

COMMIT;
GO

