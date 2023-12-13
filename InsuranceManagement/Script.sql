IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [AdminAuditLogs] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NULL,
    [ServiceName] nvarchar(max) NULL,
    [MethodName] nvarchar(max) NULL,
    [Parameters] nvarchar(max) NULL,
    [ExecutionTime] datetime2 NOT NULL,
    [ClientIpAddress] nvarchar(max) NULL,
    [Exception] nvarchar(max) NULL,
    [BrowserInfo] nvarchar(max) NULL,
    [Status] bit NOT NULL,
    [AuditStatusId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_AdminAuditLogs] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [AdminRoles] (
    [Id] int NOT NULL IDENTITY,
    [CreationTime] datetime2 NULL,
    [CreatorUserId] bigint NULL,
    [LastModificationTime] datetime2 NULL,
    [LastModifierUserId] bigint NULL,
    [IsDeleted] bit NOT NULL,
    [DeleterUserId] bigint NULL,
    [DeletionTime] datetime2 NULL,
    [Name] nvarchar(max) NULL,
    [DisplayName] nvarchar(max) NULL,
    [Description] nvarchar(max) NULL,
    [ApprovalTime] datetime2 NULL,
    [ApprovalId] bigint NULL,
    CONSTRAINT [PK_AdminRoles] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [ApplicationLogs] (
    [Id] bigint NOT NULL IDENTITY,
    [CreationTime] datetime2 NOT NULL,
    [Thread] nvarchar(max) NULL,
    [Level] nvarchar(max) NULL,
    [Logger] nvarchar(max) NULL,
    [Message] nvarchar(max) NULL,
    [Exception] nvarchar(max) NULL,
    CONSTRAINT [PK_ApplicationLogs] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Brokers] (
    [Id] int NOT NULL IDENTITY,
    [BrokerName] nvarchar(max) NULL,
    [AccountName] nvarchar(max) NULL,
    [Status] nvarchar(max) NULL,
    [CustomerID] nvarchar(max) NULL,
    [AccountNumber] nvarchar(max) NULL,
    [EmailAddress] nvarchar(max) NULL,
    CONSTRAINT [PK_Brokers] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Comments] (
    [ID] int NOT NULL IDENTITY,
    [RequestID] varchar(15) NOT NULL,
    [Serial] int NOT NULL,
    [Action] varchar(20) NOT NULL,
    [Comment] varchar(2000) NOT NULL,
    [CommentDate] datetime2 NOT NULL,
    [CommentBy] varchar(150) NOT NULL,
    CONSTRAINT [PK_Comments] PRIMARY KEY ([ID])
);

GO

CREATE TABLE [EncryptionData] (
    [Id] int NOT NULL IDENTITY,
    [Key] nvarchar(max) NULL,
    [Iv] nvarchar(max) NULL,
    CONSTRAINT [PK_EncryptionData] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [GatewayLog] (
    [Id] int NOT NULL IDENTITY,
    [APIDatetime] datetime2 NOT NULL,
    [Source] nvarchar(max) NULL,
    [Request] nvarchar(max) NULL,
    [Response] nvarchar(max) NULL,
    [TransReference] nvarchar(max) NULL,
    [RequestTime] datetime2 NOT NULL,
    [ResponseTime] datetime2 NOT NULL,
    [Endpoint] nvarchar(max) NULL,
    [APIType] nvarchar(max) NULL,
    CONSTRAINT [PK_GatewayLog] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [InsuranceTable] (
    [ID] bigint NOT NULL IDENTITY,
    [RequestID] varchar(20) NOT NULL,
    [Serial] int NOT NULL,
    [Stage] varchar(50) NOT NULL,
    [RequestDate] datetime2 NOT NULL,
    [RequestType] varchar(10) NULL,
    [RequestByUsername] varchar(50) NOT NULL,
    [RequestByName] varchar(100) NOT NULL,
    [RequestByemail] varchar(150) NOT NULL,
    [ToBeAuthroiziedBy] varchar(50) NOT NULL,
    [AuthorizedDate] datetime2 NULL,
    [AuthorizedByUsername] varchar(50) NULL,
    [AuthorizedByName] varchar(100) NULL,
    [AuthorizedByEmail] varchar(150) NULL,
    [PolicyNo] varchar(50) NULL,
    [PolicyIssuanceDate] datetime2 NULL,
    [PolicyExpiryDate] datetime2 NULL,
    [PolicyCertificate] nvarchar(max) NULL,
    [FileName] nvarchar(max) NULL,
    [ContentType] nvarchar(max) NULL,
    [CertificateRequestDate] datetime2 NOT NULL,
    [CertificateRequestByUsername] varchar(50) NULL,
    [CertificateRequestByName] varchar(100) NULL,
    [CertificateRequestByemail] varchar(150) NULL,
    [CertificateToBeAuthroiziedBy] varchar(50) NULL,
    [CertificateAuthorizedDate] datetime2 NULL,
    [CertificateAuthorizedByUsername] varchar(50) NULL,
    [CertificateAuthorizedByName] varchar(100) NULL,
    [CertificateAuthorizedByEmail] varchar(150) NULL,
    [FEESFTReference] varchar(25) NULL,
    [COMMFTReference] varchar(25) NULL,
    [ErrorMessage] nvarchar(max) NULL,
    [Status] nvarchar(max) NULL,
    CONSTRAINT [PK_InsuranceTable] PRIMARY KEY ([ID])
);

GO

CREATE TABLE [InsuranceTypes] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [Status] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_InsuranceTypes] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [AdminRoleDetails] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] int NOT NULL,
    [MenuOption] nvarchar(max) NULL,
    [RoleDescription] nvarchar(max) NULL,
    [CreationTime] datetime2 NULL,
    [LastModificationTime] datetime2 NULL,
    [DeletionTime] datetime2 NULL,
    [CreatorUserId] bigint NULL,
    [LastModifierUserId] bigint NULL,
    [DeleterUserId] bigint NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_AdminRoleDetails] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AdminRoleDetails_AdminRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AdminRoles] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [Underwriters] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [EmailAddress] nvarchar(max) NULL,
    [Status] nvarchar(max) NULL,
    [BrokerId] int NOT NULL,
    CONSTRAINT [PK_Underwriters] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Underwriters_Brokers_BrokerId] FOREIGN KEY ([BrokerId]) REFERENCES [Brokers] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [FundTransferLookUp] (
    [Id] int NOT NULL IDENTITY,
    [TransactionNarration] nvarchar(max) NULL,
    [TransactionStatus] nvarchar(max) NULL,
    [TransactionRequest] nvarchar(max) NULL,
    [TransactionResponse] nvarchar(max) NULL,
    [RequesstDate] datetime2 NULL,
    [UniqueID] nvarchar(max) NULL,
    [TransactionType] nvarchar(max) NULL,
    [RequestID] nvarchar(max) NULL,
    [InsuranceTableId] bigint NULL,
    CONSTRAINT [PK_FundTransferLookUp] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FundTransferLookUp_InsuranceTable_InsuranceTableId] FOREIGN KEY ([InsuranceTableId]) REFERENCES [InsuranceTable] ([ID]) ON DELETE NO ACTION
);

GO

CREATE TABLE [BrokerInsuranceTypes] (
    [Id] int NOT NULL IDENTITY,
    [BrokerId] int NOT NULL,
    [Name] nvarchar(max) NULL,
    [Status] nvarchar(max) NULL,
    [InsuranceTypeId] int NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_BrokerInsuranceTypes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_BrokerInsuranceTypes_Brokers_BrokerId] FOREIGN KEY ([BrokerId]) REFERENCES [Brokers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_BrokerInsuranceTypes_InsuranceTypes_InsuranceTypeId] FOREIGN KEY ([InsuranceTypeId]) REFERENCES [InsuranceTypes] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [InsuranceSubTypes] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [InsuranceTypeId] int NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_InsuranceSubTypes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_InsuranceSubTypes_InsuranceTypes_InsuranceTypeId] FOREIGN KEY ([InsuranceTypeId]) REFERENCES [InsuranceTypes] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [BrokerSubInsuranceTypes] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [BrokerId] int NOT NULL,
    [BrokerInsuranceTypeId] int NOT NULL,
    [PercentageToBank] decimal(18, 2) NULL,
    [Comment] nvarchar(max) NULL,
    [Status] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_BrokerSubInsuranceTypes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_BrokerSubInsuranceTypes_Brokers_BrokerId] FOREIGN KEY ([BrokerId]) REFERENCES [Brokers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_BrokerSubInsuranceTypes_BrokerInsuranceTypes_BrokerInsuranceTypeId] FOREIGN KEY ([BrokerInsuranceTypeId]) REFERENCES [BrokerInsuranceTypes] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Request] (
    [ID] bigint NOT NULL IDENTITY,
    [RequestID] varchar(15) NOT NULL,
    [AccountNo] varchar(10) NOT NULL,
    [AccountName] varchar(100) NOT NULL,
    [Branchcode] varchar(10) NOT NULL,
    [CustomerID] varchar(10) NOT NULL,
    [CustomerName] varchar(100) NOT NULL,
    [CustomerEmail] varchar(100) NOT NULL,
    [CollateralValue] decimal(18,2) NOT NULL,
    [Premium] decimal(18,2) NOT NULL,
    [ContractID] varchar(20) NULL,
    [BrokerID] int NOT NULL,
    [InsuranceTypeId] int NOT NULL,
    [InsuranceSubTypeID] int NULL,
    [UnderwriterId] int NULL,
    [Status] varchar(20) NOT NULL,
    [ContractMaturityDate] datetime2 NULL,
    CONSTRAINT [PK_Request] PRIMARY KEY ([ID]),
    CONSTRAINT [FK_Request_Brokers_BrokerID] FOREIGN KEY ([BrokerID]) REFERENCES [Brokers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Request_BrokerSubInsuranceTypes_InsuranceSubTypeID] FOREIGN KEY ([InsuranceSubTypeID]) REFERENCES [BrokerSubInsuranceTypes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Request_BrokerInsuranceTypes_InsuranceTypeId] FOREIGN KEY ([InsuranceTypeId]) REFERENCES [BrokerInsuranceTypes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Request_Underwriters_UnderwriterId] FOREIGN KEY ([UnderwriterId]) REFERENCES [Underwriters] ([Id]) ON DELETE NO ACTION
);

GO

CREATE INDEX [IX_AdminRoleDetails_RoleId] ON [AdminRoleDetails] ([RoleId]);

GO

CREATE INDEX [IX_BrokerInsuranceTypes_BrokerId] ON [BrokerInsuranceTypes] ([BrokerId]);

GO

CREATE INDEX [IX_BrokerInsuranceTypes_InsuranceTypeId] ON [BrokerInsuranceTypes] ([InsuranceTypeId]);

GO

CREATE INDEX [IX_BrokerSubInsuranceTypes_BrokerId] ON [BrokerSubInsuranceTypes] ([BrokerId]);

GO

CREATE INDEX [IX_BrokerSubInsuranceTypes_BrokerInsuranceTypeId] ON [BrokerSubInsuranceTypes] ([BrokerInsuranceTypeId]);

GO

CREATE INDEX [IX_FundTransferLookUp_InsuranceTableId] ON [FundTransferLookUp] ([InsuranceTableId]);

GO

CREATE INDEX [IX_InsuranceSubTypes_InsuranceTypeId] ON [InsuranceSubTypes] ([InsuranceTypeId]);

GO

CREATE INDEX [IX_Request_BrokerID] ON [Request] ([BrokerID]);

GO

CREATE INDEX [IX_Request_InsuranceSubTypeID] ON [Request] ([InsuranceSubTypeID]);

GO

CREATE INDEX [IX_Request_InsuranceTypeId] ON [Request] ([InsuranceTypeId]);

GO

CREATE INDEX [IX_Request_UnderwriterId] ON [Request] ([UnderwriterId]);

GO

CREATE INDEX [IX_Underwriters_BrokerId] ON [Underwriters] ([BrokerId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20231018084657_initialcreate', N'2.1.14-servicing-32113');

GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BrokerInsuranceTypes]') AND [c].[name] = N'IsActive');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [BrokerInsuranceTypes] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [BrokerInsuranceTypes] DROP COLUMN [IsActive];

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20231018093831_initialcreate1', N'2.1.14-servicing-32113');

GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BrokerInsuranceTypes]') AND [c].[name] = N'Name');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [BrokerInsuranceTypes] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [BrokerInsuranceTypes] DROP COLUMN [Name];

GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BrokerInsuranceTypes]') AND [c].[name] = N'Status');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [BrokerInsuranceTypes] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [BrokerInsuranceTypes] ALTER COLUMN [Status] nvarchar(max) NOT NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20231018111509_initialcreate2', N'2.1.14-servicing-32113');

GO

ALTER TABLE [Request] ADD [UpdatedPremium] decimal(18,2) NOT NULL DEFAULT 0.0;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20231018153701_initialcreate3', N'2.1.14-servicing-32113');

GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdminAuditLogs]') AND [c].[name] = N'UserId');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [AdminAuditLogs] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [AdminAuditLogs] ALTER COLUMN [UserId] nvarchar(max) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20231025151712_updatedb1', N'2.1.14-servicing-32113');

GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdminAuditLogs]') AND [c].[name] = N'UserId');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [AdminAuditLogs] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [AdminAuditLogs] ALTER COLUMN [UserId] nvarchar(20) NULL;

GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdminAuditLogs]') AND [c].[name] = N'ServiceName');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [AdminAuditLogs] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [AdminAuditLogs] ALTER COLUMN [ServiceName] nvarchar(50) NULL;

GO

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdminAuditLogs]') AND [c].[name] = N'Parameters');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [AdminAuditLogs] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [AdminAuditLogs] ALTER COLUMN [Parameters] nvarchar(300) NULL;

GO

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdminAuditLogs]') AND [c].[name] = N'MethodName');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [AdminAuditLogs] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [AdminAuditLogs] ALTER COLUMN [MethodName] nvarchar(200) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20231025152312_updatedb2', N'2.1.14-servicing-32113');

GO

DECLARE @var8 sysname;
SELECT @var8 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdminAuditLogs]') AND [c].[name] = N'Parameters');
IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [AdminAuditLogs] DROP CONSTRAINT [' + @var8 + '];');
ALTER TABLE [AdminAuditLogs] ALTER COLUMN [Parameters] nvarchar(max) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20231025152401_updatedb3', N'2.1.14-servicing-32113');

GO

DECLARE @var9 sysname;
SELECT @var9 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdminAuditLogs]') AND [c].[name] = N'ServiceName');
IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [AdminAuditLogs] DROP CONSTRAINT [' + @var9 + '];');
ALTER TABLE [AdminAuditLogs] ALTER COLUMN [ServiceName] nvarchar(max) NULL;

GO

DECLARE @var10 sysname;
SELECT @var10 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdminAuditLogs]') AND [c].[name] = N'MethodName');
IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [AdminAuditLogs] DROP CONSTRAINT [' + @var10 + '];');
ALTER TABLE [AdminAuditLogs] ALTER COLUMN [MethodName] nvarchar(max) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20231025152446_updatedb4', N'2.1.14-servicing-32113');

GO

DECLARE @var11 sysname;
SELECT @var11 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdminAuditLogs]') AND [c].[name] = N'UserId');
IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [AdminAuditLogs] DROP CONSTRAINT [' + @var11 + '];');
ALTER TABLE [AdminAuditLogs] ALTER COLUMN [UserId] nvarchar(max) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20231025152553_updatedb5', N'2.1.14-servicing-32113');

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20231106153230_initialCreatetable', N'2.1.14-servicing-32113');

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20231204102804_getname', N'2.1.14-servicing-32113');

GO

