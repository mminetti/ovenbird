BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015204420_AddInvoiceAndInvoicePaymentEntities'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ServiceStartItem]') AND [c].[name] = N'ServiceDate');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [ServiceStartItem] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [ServiceStartItem] DROP COLUMN [ServiceDate];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015204420_AddInvoiceAndInvoicePaymentEntities'
)
BEGIN
    CREATE TABLE [InvoicePaymentStatus] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_InvoicePaymentStatus] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015204420_AddInvoiceAndInvoicePaymentEntities'
)
BEGIN
    CREATE TABLE [InvoicePurpose] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_InvoicePurpose] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015204420_AddInvoiceAndInvoicePaymentEntities'
)
BEGIN
    CREATE TABLE [InvoiceStatus] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_InvoiceStatus] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015204420_AddInvoiceAndInvoicePaymentEntities'
)
BEGIN
    CREATE TABLE [InvoiceType] (
        [Id] int NOT NULL,
        [Name] nvarchar(250) NOT NULL,
        CONSTRAINT [PK_InvoiceType] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015204420_AddInvoiceAndInvoicePaymentEntities'
)
BEGIN
    CREATE TABLE [InvoicePayment] (
        [Id] bigint NOT NULL IDENTITY,
        [Identifier] nvarchar(250) NOT NULL,
        [UtilityId] int NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [TraceNumber] nvarchar(250) NOT NULL,
        [ReferenceNumber] nvarchar(250) NOT NULL,
        [InvoicePaymentStatusId] int NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_InvoicePayment] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_InvoicePayment_InvoicePaymentStatus_InvoicePaymentStatusId] FOREIGN KEY ([InvoicePaymentStatusId]) REFERENCES [InvoicePaymentStatus] ([Id]),
        CONSTRAINT [FK_InvoicePayment_Utility_UtilityId] FOREIGN KEY ([UtilityId]) REFERENCES [Utility] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015204420_AddInvoiceAndInvoicePaymentEntities'
)
BEGIN
    CREATE TABLE [Invoice] (
        [Id] bigint NOT NULL IDENTITY,
        [Identifier] nvarchar(250) NOT NULL,
        [ServiceId] bigint NOT NULL,
        [ServicePointId] bigint NOT NULL,
        [InvoicePurposeId] int NOT NULL,
        [InvoiceTypeId] int NOT NULL,
        [OriginalInvoiceId] bigint NULL,
        [PeriodStartDate] date NOT NULL,
        [PeriodEndDate] date NOT NULL,
        [IsFinal] bit NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [DueDate] date NOT NULL,
        [InvoiceStatusId] int NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(500) NULL,
        [LastModifiedAtUtc] datetimeoffset NOT NULL,
        [LastModifiedBy] nvarchar(500) NULL,
        CONSTRAINT [PK_Invoice] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Invoice_InvoicePurpose_InvoicePurposeId] FOREIGN KEY ([InvoicePurposeId]) REFERENCES [InvoicePurpose] ([Id]),
        CONSTRAINT [FK_Invoice_InvoiceStatus_InvoiceStatusId] FOREIGN KEY ([InvoiceStatusId]) REFERENCES [InvoiceStatus] ([Id]),
        CONSTRAINT [FK_Invoice_InvoiceType_InvoiceTypeId] FOREIGN KEY ([InvoiceTypeId]) REFERENCES [InvoiceType] ([Id]),
        CONSTRAINT [FK_Invoice_Invoice_OriginalInvoiceId] FOREIGN KEY ([OriginalInvoiceId]) REFERENCES [Invoice] ([Id]),
        CONSTRAINT [FK_Invoice_ServicePoint_ServicePointId] FOREIGN KEY ([ServicePointId]) REFERENCES [ServicePoint] ([Id]),
        CONSTRAINT [FK_Invoice_Service_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [Service] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015204420_AddInvoiceAndInvoicePaymentEntities'
)
BEGIN
    CREATE TABLE [InvoicePaymentInvoice] (
        [InvoicePaymentId] bigint NOT NULL,
        [InvoiceId] bigint NOT NULL,
        CONSTRAINT [PK_InvoicePaymentInvoice] PRIMARY KEY ([InvoicePaymentId], [InvoiceId]),
        CONSTRAINT [FK_InvoicePaymentInvoice_InvoicePayment_InvoicePaymentId] FOREIGN KEY ([InvoicePaymentId]) REFERENCES [InvoicePayment] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_InvoicePaymentInvoice_Invoice_InvoiceId] FOREIGN KEY ([InvoiceId]) REFERENCES [Invoice] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015204420_AddInvoiceAndInvoicePaymentEntities'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[InvoicePurpose]'))
        SET IDENTITY_INSERT [InvoicePurpose] ON;
    EXEC(N'INSERT INTO [InvoicePurpose] ([Id], [Name])
    VALUES (1, N''Original''),
    (2, N''Cancel''),
    (3, N''Replace'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[InvoicePurpose]'))
        SET IDENTITY_INSERT [InvoicePurpose] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015204420_AddInvoiceAndInvoicePaymentEntities'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[InvoiceType]'))
        SET IDENTITY_INSERT [InvoiceType] ON;
    EXEC(N'INSERT INTO [InvoiceType] ([Id], [Name])
    VALUES (1, N''Month''),
    (2, N''Final''),
    (3, N''ChangeNotice''),
    (4, N''BalanceDue''),
    (5, N''SecondNotice''),
    (6, N''Tampering''),
    (7, N''Charges'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[InvoiceType]'))
        SET IDENTITY_INSERT [InvoiceType] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015204420_AddInvoiceAndInvoicePaymentEntities'
)
BEGIN
    CREATE INDEX [IX_Invoice_InvoicePurposeId] ON [Invoice] ([InvoicePurposeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015204420_AddInvoiceAndInvoicePaymentEntities'
)
BEGIN
    CREATE INDEX [IX_Invoice_InvoiceStatusId] ON [Invoice] ([InvoiceStatusId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015204420_AddInvoiceAndInvoicePaymentEntities'
)
BEGIN
    CREATE INDEX [IX_Invoice_InvoiceTypeId] ON [Invoice] ([InvoiceTypeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015204420_AddInvoiceAndInvoicePaymentEntities'
)
BEGIN
    CREATE INDEX [IX_Invoice_OriginalInvoiceId] ON [Invoice] ([OriginalInvoiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015204420_AddInvoiceAndInvoicePaymentEntities'
)
BEGIN
    CREATE INDEX [IX_Invoice_ServiceId] ON [Invoice] ([ServiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015204420_AddInvoiceAndInvoicePaymentEntities'
)
BEGIN
    CREATE INDEX [IX_Invoice_ServicePointId] ON [Invoice] ([ServicePointId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015204420_AddInvoiceAndInvoicePaymentEntities'
)
BEGIN
    CREATE INDEX [IX_InvoicePayment_InvoicePaymentStatusId] ON [InvoicePayment] ([InvoicePaymentStatusId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015204420_AddInvoiceAndInvoicePaymentEntities'
)
BEGIN
    CREATE INDEX [IX_InvoicePayment_UtilityId] ON [InvoicePayment] ([UtilityId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015204420_AddInvoiceAndInvoicePaymentEntities'
)
BEGIN
    CREATE INDEX [IX_InvoicePaymentInvoice_InvoiceId] ON [InvoicePaymentInvoice] ([InvoiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261015204420_AddInvoiceAndInvoicePaymentEntities'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261015204420_AddInvoiceAndInvoicePaymentEntities', N'10.0.13');
END;

COMMIT;
GO

