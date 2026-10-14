BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014163310_MoveUsageAndHistoricalUsageReferenceFromAccountToService'
)
BEGIN
    ALTER TABLE [HistoricalUsage] DROP CONSTRAINT [FK_HistoricalUsage_Account_AccountId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014163310_MoveUsageAndHistoricalUsageReferenceFromAccountToService'
)
BEGIN
    ALTER TABLE [Usage] DROP CONSTRAINT [FK_Usage_Account_AccountId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014163310_MoveUsageAndHistoricalUsageReferenceFromAccountToService'
)
BEGIN
    EXEC sp_rename N'[Usage].[AccountId]', N'ServiceId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014163310_MoveUsageAndHistoricalUsageReferenceFromAccountToService'
)
BEGIN
    EXEC sp_rename N'[Usage].[IX_Usage_AccountId]', N'IX_Usage_ServiceId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014163310_MoveUsageAndHistoricalUsageReferenceFromAccountToService'
)
BEGIN
    EXEC sp_rename N'[HistoricalUsage].[AccountId]', N'ServiceId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014163310_MoveUsageAndHistoricalUsageReferenceFromAccountToService'
)
BEGIN
    EXEC sp_rename N'[HistoricalUsage].[IX_HistoricalUsage_AccountId]', N'IX_HistoricalUsage_ServiceId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014163310_MoveUsageAndHistoricalUsageReferenceFromAccountToService'
)
BEGIN
    ALTER TABLE [HistoricalUsage] ADD CONSTRAINT [FK_HistoricalUsage_Service_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [Service] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014163310_MoveUsageAndHistoricalUsageReferenceFromAccountToService'
)
BEGIN
    ALTER TABLE [Usage] ADD CONSTRAINT [FK_Usage_Service_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [Service] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014163310_MoveUsageAndHistoricalUsageReferenceFromAccountToService'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261014163310_MoveUsageAndHistoricalUsageReferenceFromAccountToService', N'10.0.13');
END;

COMMIT;
GO

