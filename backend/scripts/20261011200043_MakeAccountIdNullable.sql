BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011200043_MakeAccountIdNullable'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Usage]') AND [c].[name] = N'AccountId');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [Usage] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [Usage] ALTER COLUMN [AccountId] bigint NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011200043_MakeAccountIdNullable'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ServiceStart]') AND [c].[name] = N'AccountId');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [ServiceStart] DROP CONSTRAINT ' + @var1 + ';');
    ALTER TABLE [ServiceStart] ALTER COLUMN [AccountId] bigint NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011200043_MakeAccountIdNullable'
)
BEGIN
    DECLARE @var2 nvarchar(max);
    SELECT @var2 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[HistoricalUsage]') AND [c].[name] = N'AccountId');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [HistoricalUsage] DROP CONSTRAINT ' + @var2 + ';');
    ALTER TABLE [HistoricalUsage] ALTER COLUMN [AccountId] bigint NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011200043_MakeAccountIdNullable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261011200043_MakeAccountIdNullable', N'10.0.12');
END;

COMMIT;
GO

