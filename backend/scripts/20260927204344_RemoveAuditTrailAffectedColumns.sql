BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260927204344_RemoveAuditTrailAffectedColumns'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AuditTrail]') AND [c].[name] = N'AffectedColumns');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [AuditTrail] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [AuditTrail] DROP COLUMN [AffectedColumns];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260927204344_RemoveAuditTrailAffectedColumns'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260927204344_RemoveAuditTrailAffectedColumns', N'10.0.12');
END;

COMMIT;
GO

