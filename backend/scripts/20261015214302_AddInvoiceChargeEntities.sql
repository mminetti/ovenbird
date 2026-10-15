BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015214302_AddInvoiceChargeEntities'
)
BEGIN
    ALTER TABLE [UnitOfMeasure] ADD [Identifier] nvarchar(250) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015214302_AddInvoiceChargeEntities'
)
BEGIN
    CREATE TABLE [InvoiceChargeCode] (
        [Id] int NOT NULL IDENTITY,
        [Identifier] nvarchar(250) NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_InvoiceChargeCode] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015214302_AddInvoiceChargeEntities'
)
BEGIN
    CREATE TABLE [InvoiceChargeType] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_InvoiceChargeType] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015214302_AddInvoiceChargeEntities'
)
BEGIN
    CREATE TABLE [InvoiceCharge] (
        [Id] bigint NOT NULL IDENTITY,
        [InvoiceId] bigint NOT NULL,
        [InvoiceChargeCodeId] int NOT NULL,
        [InvoiceChargeTypeId] int NOT NULL,
        [Quantity] float NOT NULL,
        [ChargeAmount] float NOT NULL,
        [TotalAmount] float NOT NULL,
        [UnitOfMeasureId] int NOT NULL,
        [Description] nvarchar(1000) NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_InvoiceCharge] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_InvoiceCharge_InvoiceChargeCode_InvoiceChargeCodeId] FOREIGN KEY ([InvoiceChargeCodeId]) REFERENCES [InvoiceChargeCode] ([Id]),
        CONSTRAINT [FK_InvoiceCharge_InvoiceChargeType_InvoiceChargeTypeId] FOREIGN KEY ([InvoiceChargeTypeId]) REFERENCES [InvoiceChargeType] ([Id]),
        CONSTRAINT [FK_InvoiceCharge_Invoice_InvoiceId] FOREIGN KEY ([InvoiceId]) REFERENCES [Invoice] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_InvoiceCharge_UnitOfMeasure_UnitOfMeasureId] FOREIGN KEY ([UnitOfMeasureId]) REFERENCES [UnitOfMeasure] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015214302_AddInvoiceChargeEntities'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[InvoiceChargeType]'))
        SET IDENTITY_INSERT [InvoiceChargeType] ON;
    EXEC(N'INSERT INTO [InvoiceChargeType] ([Id], [Name])
    VALUES (1, N''Rate''),
    (2, N''Account''),
    (3, N''B2B'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[InvoiceChargeType]'))
        SET IDENTITY_INSERT [InvoiceChargeType] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015214302_AddInvoiceChargeEntities'
)
BEGIN
    EXEC(N'UPDATE [UnitOfMeasure] SET [Identifier] = N''KH''
    WHERE [Id] = 1;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015214302_AddInvoiceChargeEntities'
)
BEGIN
    CREATE INDEX [IX_InvoiceCharge_InvoiceChargeCodeId] ON [InvoiceCharge] ([InvoiceChargeCodeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015214302_AddInvoiceChargeEntities'
)
BEGIN
    CREATE INDEX [IX_InvoiceCharge_InvoiceChargeTypeId] ON [InvoiceCharge] ([InvoiceChargeTypeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015214302_AddInvoiceChargeEntities'
)
BEGIN
    CREATE INDEX [IX_InvoiceCharge_InvoiceId] ON [InvoiceCharge] ([InvoiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015214302_AddInvoiceChargeEntities'
)
BEGIN
    CREATE INDEX [IX_InvoiceCharge_UnitOfMeasureId] ON [InvoiceCharge] ([UnitOfMeasureId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015214302_AddInvoiceChargeEntities'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261015214302_AddInvoiceChargeEntities', N'10.0.13');
END;

COMMIT;
GO

