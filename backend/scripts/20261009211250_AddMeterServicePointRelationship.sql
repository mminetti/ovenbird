BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009211250_AddMeterServicePointRelationship'
)
BEGIN
    ALTER TABLE [Meter] ADD [IsActive] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009211250_AddMeterServicePointRelationship'
)
BEGIN
    ALTER TABLE [Meter] ADD [ServicePointId] bigint NOT NULL DEFAULT CAST(0 AS bigint);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009211250_AddMeterServicePointRelationship'
)
BEGIN
    CREATE INDEX [IX_Meter_ServicePointId] ON [Meter] ([ServicePointId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009211250_AddMeterServicePointRelationship'
)
BEGIN
    ALTER TABLE [Meter] ADD CONSTRAINT [FK_Meter_ServicePoint_ServicePointId] FOREIGN KEY ([ServicePointId]) REFERENCES [ServicePoint] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009211250_AddMeterServicePointRelationship'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261009211250_AddMeterServicePointRelationship', N'10.0.12');
END;

COMMIT;
GO

