BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014132107_AddServicePointLookupEntities'
)
BEGIN
    ALTER TABLE [ServicePoint] DROP CONSTRAINT [FK_ServicePoint_PremiseType_PremiseTypeId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014132107_AddServicePointLookupEntities'
)
BEGIN
    DROP TABLE [PremiseType];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014132107_AddServicePointLookupEntities'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ServicePoint]') AND [c].[name] = N'Cycle');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [ServicePoint] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [ServicePoint] DROP COLUMN [Cycle];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014132107_AddServicePointLookupEntities'
)
BEGIN
    EXEC sp_rename N'[ServicePoint].[PremiseTypeId]', N'ReadCycleId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014132107_AddServicePointLookupEntities'
)
BEGIN
    EXEC sp_rename N'[ServicePoint].[IX_ServicePoint_PremiseTypeId]', N'IX_ServicePoint_ReadCycleId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014132107_AddServicePointLookupEntities'
)
BEGIN
    DROP INDEX [IX_Usage_AccountId] ON [Usage];
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Usage]') AND [c].[name] = N'AccountId');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Usage] DROP CONSTRAINT ' + @var1 + ';');
    EXEC(N'UPDATE [Usage] SET [AccountId] = CAST(0 AS bigint) WHERE [AccountId] IS NULL');
    ALTER TABLE [Usage] ALTER COLUMN [AccountId] bigint NOT NULL;
    ALTER TABLE [Usage] ADD DEFAULT CAST(0 AS bigint) FOR [AccountId];
    CREATE INDEX [IX_Usage_AccountId] ON [Usage] ([AccountId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014132107_AddServicePointLookupEntities'
)
BEGIN
    DROP INDEX [IX_ServiceStart_AccountId] ON [ServiceStart];
    DECLARE @var2 nvarchar(max);
    SELECT @var2 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ServiceStart]') AND [c].[name] = N'AccountId');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [ServiceStart] DROP CONSTRAINT ' + @var2 + ';');
    EXEC(N'UPDATE [ServiceStart] SET [AccountId] = CAST(0 AS bigint) WHERE [AccountId] IS NULL');
    ALTER TABLE [ServiceStart] ALTER COLUMN [AccountId] bigint NOT NULL;
    ALTER TABLE [ServiceStart] ADD DEFAULT CAST(0 AS bigint) FOR [AccountId];
    CREATE INDEX [IX_ServiceStart_AccountId] ON [ServiceStart] ([AccountId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014132107_AddServicePointLookupEntities'
)
BEGIN
    ALTER TABLE [ServicePoint] ADD [CustomerClassId] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014132107_AddServicePointLookupEntities'
)
BEGIN
    ALTER TABLE [ServicePoint] ADD [IsAmsMeter] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014132107_AddServicePointLookupEntities'
)
BEGIN
    ALTER TABLE [ServicePoint] ADD [LoadProfileId] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014132107_AddServicePointLookupEntities'
)
BEGIN
    DROP INDEX [IX_HistoricalUsage_AccountId] ON [HistoricalUsage];
    DECLARE @var3 nvarchar(max);
    SELECT @var3 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[HistoricalUsage]') AND [c].[name] = N'AccountId');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [HistoricalUsage] DROP CONSTRAINT ' + @var3 + ';');
    EXEC(N'UPDATE [HistoricalUsage] SET [AccountId] = CAST(0 AS bigint) WHERE [AccountId] IS NULL');
    ALTER TABLE [HistoricalUsage] ALTER COLUMN [AccountId] bigint NOT NULL;
    ALTER TABLE [HistoricalUsage] ADD DEFAULT CAST(0 AS bigint) FOR [AccountId];
    CREATE INDEX [IX_HistoricalUsage_AccountId] ON [HistoricalUsage] ([AccountId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014132107_AddServicePointLookupEntities'
)
BEGIN
    CREATE TABLE [CustomerClass] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_CustomerClass] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014132107_AddServicePointLookupEntities'
)
BEGIN
    CREATE TABLE [LoadProfile] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_LoadProfile] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014132107_AddServicePointLookupEntities'
)
BEGIN
    CREATE TABLE [ReadCycle] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_ReadCycle] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014132107_AddServicePointLookupEntities'
)
BEGIN
    CREATE INDEX [IX_ServicePoint_CustomerClassId] ON [ServicePoint] ([CustomerClassId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014132107_AddServicePointLookupEntities'
)
BEGIN
    CREATE INDEX [IX_ServicePoint_LoadProfileId] ON [ServicePoint] ([LoadProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014132107_AddServicePointLookupEntities'
)
BEGIN
    ALTER TABLE [ServicePoint] ADD CONSTRAINT [FK_ServicePoint_CustomerClass_CustomerClassId] FOREIGN KEY ([CustomerClassId]) REFERENCES [CustomerClass] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014132107_AddServicePointLookupEntities'
)
BEGIN
    ALTER TABLE [ServicePoint] ADD CONSTRAINT [FK_ServicePoint_LoadProfile_LoadProfileId] FOREIGN KEY ([LoadProfileId]) REFERENCES [LoadProfile] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014132107_AddServicePointLookupEntities'
)
BEGIN
    ALTER TABLE [ServicePoint] ADD CONSTRAINT [FK_ServicePoint_ReadCycle_ReadCycleId] FOREIGN KEY ([ReadCycleId]) REFERENCES [ReadCycle] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014132107_AddServicePointLookupEntities'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261014132107_AddServicePointLookupEntities', N'10.0.13');
END;

COMMIT;
GO

