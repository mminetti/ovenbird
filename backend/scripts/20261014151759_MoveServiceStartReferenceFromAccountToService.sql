BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014151759_MoveServiceStartReferenceFromAccountToService'
)
BEGIN
    ALTER TABLE [ServiceStart] DROP CONSTRAINT [FK_ServiceStart_Account_AccountId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014151759_MoveServiceStartReferenceFromAccountToService'
)
BEGIN
    EXEC sp_rename N'[ServiceStart].[AccountId]', N'ServiceId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014151759_MoveServiceStartReferenceFromAccountToService'
)
BEGIN
    EXEC sp_rename N'[ServiceStart].[IX_ServiceStart_AccountId]', N'IX_ServiceStart_ServiceId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014151759_MoveServiceStartReferenceFromAccountToService'
)
BEGIN
    ALTER TABLE [ServiceStart] ADD CONSTRAINT [FK_ServiceStart_Service_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [Service] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014151759_MoveServiceStartReferenceFromAccountToService'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261014151759_MoveServiceStartReferenceFromAccountToService', N'10.0.13');
END;

COMMIT;
GO

