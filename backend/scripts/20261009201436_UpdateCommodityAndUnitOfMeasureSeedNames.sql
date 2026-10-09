BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009201436_UpdateCommodityAndUnitOfMeasureSeedNames'
)
BEGIN
    EXEC(N'UPDATE [Commodity] SET [Name] = N''Electricity''
    WHERE [Id] = 1;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009201436_UpdateCommodityAndUnitOfMeasureSeedNames'
)
BEGIN
    EXEC(N'UPDATE [UnitOfMeasure] SET [Name] = N''Kilowatt Hour''
    WHERE [Id] = 1;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009201436_UpdateCommodityAndUnitOfMeasureSeedNames'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261009201436_UpdateCommodityAndUnitOfMeasureSeedNames', N'10.0.12');
END;

COMMIT;
GO

