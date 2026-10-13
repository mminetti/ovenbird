BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013024423_AddAccountCompany'
)
BEGIN
    ALTER TABLE [Account] ADD [CompanyId] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013024423_AddAccountCompany'
)
BEGIN
    CREATE INDEX [IX_Account_CompanyId] ON [Account] ([CompanyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013024423_AddAccountCompany'
)
BEGIN
    ALTER TABLE [Account] ADD CONSTRAINT [FK_Account_Company_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Company] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013024423_AddAccountCompany'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261013024423_AddAccountCompany', N'10.0.12');
END;

COMMIT;
GO

