BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918155725_ConnectorTypeSeedNameUpdate'
)
BEGIN
    EXEC(N'UPDATE [ConnectorType] SET [Name] = N''FTP''
    WHERE [Id] = 1;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918155725_ConnectorTypeSeedNameUpdate'
)
BEGIN
    EXEC(N'UPDATE [ConnectorType] SET [Name] = N''File Storage''
    WHERE [Id] = 2;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918155725_ConnectorTypeSeedNameUpdate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260918155725_ConnectorTypeSeedNameUpdate', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918162453_MoveConnectorTypeToConnectorImplementation'
)
BEGIN
    ALTER TABLE [Connector] DROP CONSTRAINT [FK_Connector_ConnectorType_ConnectorTypeId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918162453_MoveConnectorTypeToConnectorImplementation'
)
BEGIN
    DROP INDEX [IX_Connector_ConnectorTypeId] ON [Connector];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918162453_MoveConnectorTypeToConnectorImplementation'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Connector]') AND [c].[name] = N'ConnectorTypeId');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [Connector] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [Connector] DROP COLUMN [ConnectorTypeId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918162453_MoveConnectorTypeToConnectorImplementation'
)
BEGIN
    ALTER TABLE [ConnectorImplementation] ADD [ConnectorTypeId] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918162453_MoveConnectorTypeToConnectorImplementation'
)
BEGIN
    ALTER TABLE [ConnectorImplementation] ADD [Identifier] nvarchar(250) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918162453_MoveConnectorTypeToConnectorImplementation'
)
BEGIN
    EXEC(N'UPDATE [ConnectorImplementation] SET [ConnectorTypeId] = 1, [Identifier] = N''FluentFtpService'', [Name] = N''Fluent FTP''
    WHERE [Id] = 1;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918162453_MoveConnectorTypeToConnectorImplementation'
)
BEGIN
    EXEC(N'UPDATE [ConnectorImplementation] SET [ConnectorTypeId] = 1, [Identifier] = N''SshNetSftpService'', [Name] = N''SSH.NET SFTP''
    WHERE [Id] = 2;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918162453_MoveConnectorTypeToConnectorImplementation'
)
BEGIN
    EXEC(N'UPDATE [ConnectorImplementation] SET [ConnectorTypeId] = 1, [Identifier] = N''LocalFileSystemFtpService'', [Name] = N''Local File System FTP''
    WHERE [Id] = 3;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918162453_MoveConnectorTypeToConnectorImplementation'
)
BEGIN
    EXEC(N'UPDATE [ConnectorImplementation] SET [ConnectorTypeId] = 2, [Identifier] = N''AzureBlobFileStorage'', [Name] = N''Azure Blob Storage''
    WHERE [Id] = 4;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918162453_MoveConnectorTypeToConnectorImplementation'
)
BEGIN
    EXEC(N'UPDATE [ConnectorImplementation] SET [ConnectorTypeId] = 2, [Identifier] = N''LocalFileSystemFileStorage'', [Name] = N''Local File System Storage''
    WHERE [Id] = 5;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918162453_MoveConnectorTypeToConnectorImplementation'
)
BEGIN
    CREATE INDEX [IX_ConnectorImplementation_ConnectorTypeId] ON [ConnectorImplementation] ([ConnectorTypeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918162453_MoveConnectorTypeToConnectorImplementation'
)
BEGIN
    ALTER TABLE [ConnectorImplementation] ADD CONSTRAINT [FK_ConnectorImplementation_ConnectorType_ConnectorTypeId] FOREIGN KEY ([ConnectorTypeId]) REFERENCES [ConnectorType] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918162453_MoveConnectorTypeToConnectorImplementation'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260918162453_MoveConnectorTypeToConnectorImplementation', N'10.0.12');
END;

COMMIT;
GO