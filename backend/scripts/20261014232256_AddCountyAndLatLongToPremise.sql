BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014232256_AddCountyAndLatLongToPremise'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Premise]') AND [c].[name] = N'County');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [Premise] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [Premise] DROP COLUMN [County];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014232256_AddCountyAndLatLongToPremise'
)
BEGIN
    EXEC sp_rename N'[Premise].[Address]', N'AddressLine1', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014232256_AddCountyAndLatLongToPremise'
)
BEGIN
    EXEC sp_rename N'[Premise].[Address2]', N'AddressLine2', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014232256_AddCountyAndLatLongToPremise'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Premise]') AND [c].[name] = N'AddressLine2');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Premise] DROP CONSTRAINT ' + @var1 + ';');
    ALTER TABLE [Premise] ALTER COLUMN [AddressLine2] nvarchar(250) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014232256_AddCountyAndLatLongToPremise'
)
BEGIN
    ALTER TABLE [Premise] ADD [Zip4] nvarchar(250) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014232256_AddCountyAndLatLongToPremise'
)
BEGIN
    ALTER TABLE [Premise] ADD [CountyId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014232256_AddCountyAndLatLongToPremise'
)
BEGIN
    ALTER TABLE [Premise] ADD [Latitude] decimal(9,6) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014232256_AddCountyAndLatLongToPremise'
)
BEGIN
    ALTER TABLE [Premise] ADD [Longitude] decimal(9,6) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014232256_AddCountyAndLatLongToPremise'
)
BEGIN
    CREATE TABLE [County] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_County] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014232256_AddCountyAndLatLongToPremise'
)
BEGIN
    CREATE INDEX [IX_Premise_CountyId] ON [Premise] ([CountyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014232256_AddCountyAndLatLongToPremise'
)
BEGIN
    ALTER TABLE [Premise] ADD CONSTRAINT [FK_Premise_County_CountyId] FOREIGN KEY ([CountyId]) REFERENCES [County] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261014232256_AddCountyAndLatLongToPremise'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261014232256_AddCountyAndLatLongToPremise', N'10.0.13');
END;

COMMIT;
GO

