BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013013322_AddUtilityIdentifier'
)
BEGIN
    ALTER TABLE [Utility] ADD [Identifier] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013013322_AddUtilityIdentifier'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261013013322_AddUtilityIdentifier', N'10.0.12');
END;

COMMIT;
GO

