/*
    PTPBank Banking Schema (SQL Server)
    --------------------------------------------------------------
    Fixes included from assessment analysis:
    1) Added AccountStatus and AccountType columns
    2) Added CHECK constraints for amount/date/status/enum values
    3) Added UNIQUE constraints for Client ID number and account number
    4) Added indexes for FK/search columns
    5) Added optimistic concurrency rowversion columns
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- Safe re-run support for development.
IF OBJECT_ID('dbo.Transactions', 'U') IS NOT NULL DROP TABLE dbo.Transactions;
IF OBJECT_ID('dbo.Accounts', 'U') IS NOT NULL DROP TABLE dbo.Accounts;
IF OBJECT_ID('dbo.Clients', 'U') IS NOT NULL DROP TABLE dbo.Clients;
GO

CREATE TABLE dbo.Clients
(
    Code            INT IDENTITY(1,1) NOT NULL,
    Name            VARCHAR(50) NULL,
    Surname         VARCHAR(50) NULL,
    IDNumber        VARCHAR(50) NOT NULL,
    CreatedDateUtc  DATETIME2 NOT NULL CONSTRAINT DF_Clients_CreatedDateUtc DEFAULT SYSUTCDATETIME(),
    ModifiedDateUtc DATETIME2 NOT NULL CONSTRAINT DF_Clients_ModifiedDateUtc DEFAULT SYSUTCDATETIME(),
    RowVersion      ROWVERSION NOT NULL,

    CONSTRAINT PK_Clients PRIMARY KEY CLUSTERED (Code),
    CONSTRAINT UQ_Clients_IDNumber UNIQUE (IDNumber),
    CONSTRAINT CK_Clients_IDNumber_NotBlank CHECK (LEN(LTRIM(RTRIM(IDNumber))) > 0)
);
GO

CREATE INDEX IX_Clients_Surname ON dbo.Clients(Surname);
GO

CREATE TABLE dbo.Accounts
(
    Code               INT IDENTITY(1,1) NOT NULL,
    ClientCode         INT NOT NULL,
    AccountNumber      VARCHAR(50) NOT NULL,
    AccountType        VARCHAR(20) NOT NULL,
    AccountStatus      VARCHAR(20) NOT NULL,
    OutstandingBalance DECIMAL(18,2) NOT NULL CONSTRAINT DF_Accounts_OutstandingBalance DEFAULT (0),
    CreatedDateUtc     DATETIME2 NOT NULL CONSTRAINT DF_Accounts_CreatedDateUtc DEFAULT SYSUTCDATETIME(),
    ModifiedDateUtc    DATETIME2 NOT NULL CONSTRAINT DF_Accounts_ModifiedDateUtc DEFAULT SYSUTCDATETIME(),
    RowVersion         ROWVERSION NOT NULL,

    CONSTRAINT PK_Accounts PRIMARY KEY CLUSTERED (Code),
    CONSTRAINT FK_Accounts_Clients FOREIGN KEY (ClientCode) REFERENCES dbo.Clients(Code) ON DELETE NO ACTION,
    CONSTRAINT UQ_Accounts_AccountNumber UNIQUE (AccountNumber),
    CONSTRAINT CK_Accounts_Number_NotBlank CHECK (LEN(LTRIM(RTRIM(AccountNumber))) > 0),
    CONSTRAINT CK_Accounts_Balance_NonNegative CHECK (OutstandingBalance >= 0),
    CONSTRAINT CK_Accounts_AccountType CHECK (AccountType IN ('Savings', 'Cheque')),
    CONSTRAINT CK_Accounts_AccountStatus CHECK (AccountStatus IN ('Open', 'Closed'))
);
GO

CREATE INDEX IX_Accounts_ClientCode ON dbo.Accounts(ClientCode);
CREATE INDEX IX_Accounts_AccountStatus ON dbo.Accounts(AccountStatus);
CREATE INDEX IX_Accounts_AccountType ON dbo.Accounts(AccountType);
GO

CREATE TABLE dbo.Transactions
(
    Code               INT IDENTITY(1,1) NOT NULL,
    AccountCode        INT NOT NULL,
    TransactionDate    DATETIME2 NOT NULL,
    CaptureDate        DATETIME2 NOT NULL CONSTRAINT DF_Transactions_CaptureDate DEFAULT SYSUTCDATETIME(),
    Amount             DECIMAL(18,2) NOT NULL,
    Description        VARCHAR(100) NOT NULL,
    TransactionType    VARCHAR(20) NOT NULL,
    CreatedDateUtc     DATETIME2 NOT NULL CONSTRAINT DF_Transactions_CreatedDateUtc DEFAULT SYSUTCDATETIME(),
    ModifiedDateUtc    DATETIME2 NOT NULL CONSTRAINT DF_Transactions_ModifiedDateUtc DEFAULT SYSUTCDATETIME(),
    RowVersion         ROWVERSION NOT NULL,

    CONSTRAINT PK_Transactions PRIMARY KEY CLUSTERED (Code),
    CONSTRAINT FK_Transactions_Accounts FOREIGN KEY (AccountCode) REFERENCES dbo.Accounts(Code) ON DELETE NO ACTION,
    CONSTRAINT CK_Transactions_Amount_NotZero CHECK (Amount <> 0),
    CONSTRAINT CK_Transactions_Description_NotBlank CHECK (LEN(LTRIM(RTRIM(Description))) > 0),
    CONSTRAINT CK_Transactions_Date_NotFuture CHECK (TransactionDate <= SYSUTCDATETIME()),
    CONSTRAINT CK_Transactions_TransactionType CHECK (TransactionType IN ('Debit', 'Credit'))
);
GO

CREATE INDEX IX_Transactions_AccountCode ON dbo.Transactions(AccountCode);
CREATE INDEX IX_Transactions_TransactionDate ON dbo.Transactions(TransactionDate DESC);
GO

/* ASP.NET Core Identity tables */
IF OBJECT_ID('dbo.AspNetUserTokens', 'U') IS NOT NULL DROP TABLE dbo.AspNetUserTokens;
IF OBJECT_ID('dbo.AspNetUserRoles', 'U') IS NOT NULL DROP TABLE dbo.AspNetUserRoles;
IF OBJECT_ID('dbo.AspNetUserLogins', 'U') IS NOT NULL DROP TABLE dbo.AspNetUserLogins;
IF OBJECT_ID('dbo.AspNetUserClaims', 'U') IS NOT NULL DROP TABLE dbo.AspNetUserClaims;
IF OBJECT_ID('dbo.AspNetRoleClaims', 'U') IS NOT NULL DROP TABLE dbo.AspNetRoleClaims;
IF OBJECT_ID('dbo.AspNetUsers', 'U') IS NOT NULL DROP TABLE dbo.AspNetUsers;
IF OBJECT_ID('dbo.AspNetRoles', 'U') IS NOT NULL DROP TABLE dbo.AspNetRoles;
GO

CREATE TABLE dbo.AspNetRoles
(
    Id NVARCHAR(450) NOT NULL,
    Name NVARCHAR(256) NULL,
    NormalizedName NVARCHAR(256) NULL,
    ConcurrencyStamp NVARCHAR(MAX) NULL,
    CONSTRAINT PK_AspNetRoles PRIMARY KEY (Id)
);
GO

CREATE TABLE dbo.AspNetUsers
(
    Id NVARCHAR(450) NOT NULL,
    FullName NVARCHAR(MAX) NULL,
    UserName NVARCHAR(256) NULL,
    NormalizedUserName NVARCHAR(256) NULL,
    Email NVARCHAR(256) NULL,
    NormalizedEmail NVARCHAR(256) NULL,
    EmailConfirmed BIT NOT NULL DEFAULT 0,
    PasswordHash NVARCHAR(MAX) NULL,
    SecurityStamp NVARCHAR(MAX) NULL,
    ConcurrencyStamp NVARCHAR(MAX) NULL,
    PhoneNumber NVARCHAR(MAX) NULL,
    PhoneNumberConfirmed BIT NOT NULL DEFAULT 0,
    TwoFactorEnabled BIT NOT NULL DEFAULT 0,
    LockoutEnd DATETIMEOFFSET NULL,
    LockoutEnabled BIT NOT NULL DEFAULT 0,
    AccessFailedCount INT NOT NULL DEFAULT 0,
    CONSTRAINT PK_AspNetUsers PRIMARY KEY (Id)
);
GO

CREATE TABLE dbo.AspNetRoleClaims
(
    Id INT IDENTITY(1,1) NOT NULL,
    RoleId NVARCHAR(450) NOT NULL,
    ClaimType NVARCHAR(MAX) NULL,
    ClaimValue NVARCHAR(MAX) NULL,
    CONSTRAINT PK_AspNetRoleClaims PRIMARY KEY (Id),
    CONSTRAINT FK_AspNetRoleClaims_AspNetRoles FOREIGN KEY (RoleId) REFERENCES dbo.AspNetRoles(Id) ON DELETE CASCADE
);
GO

CREATE TABLE dbo.AspNetUserClaims
(
    Id INT IDENTITY(1,1) NOT NULL,
    UserId NVARCHAR(450) NOT NULL,
    ClaimType NVARCHAR(MAX) NULL,
    ClaimValue NVARCHAR(MAX) NULL,
    CONSTRAINT PK_AspNetUserClaims PRIMARY KEY (Id),
    CONSTRAINT FK_AspNetUserClaims_AspNetUsers FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id) ON DELETE CASCADE
);
GO

CREATE TABLE dbo.AspNetUserLogins
(
    LoginProvider NVARCHAR(450) NOT NULL,
    ProviderKey NVARCHAR(450) NOT NULL,
    ProviderDisplayName NVARCHAR(MAX) NULL,
    UserId NVARCHAR(450) NOT NULL,
    CONSTRAINT PK_AspNetUserLogins PRIMARY KEY (LoginProvider, ProviderKey),
    CONSTRAINT FK_AspNetUserLogins_AspNetUsers FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id) ON DELETE CASCADE
);
GO

CREATE TABLE dbo.AspNetUserRoles
(
    UserId NVARCHAR(450) NOT NULL,
    RoleId NVARCHAR(450) NOT NULL,
    CONSTRAINT PK_AspNetUserRoles PRIMARY KEY (UserId, RoleId),
    CONSTRAINT FK_AspNetUserRoles_AspNetUsers FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id) ON DELETE CASCADE,
    CONSTRAINT FK_AspNetUserRoles_AspNetRoles FOREIGN KEY (RoleId) REFERENCES dbo.AspNetRoles(Id) ON DELETE CASCADE
);
GO

CREATE TABLE dbo.AspNetUserTokens
(
    UserId NVARCHAR(450) NOT NULL,
    LoginProvider NVARCHAR(450) NOT NULL,
    Name NVARCHAR(450) NOT NULL,
    Value NVARCHAR(MAX) NULL,
    CONSTRAINT PK_AspNetUserTokens PRIMARY KEY (UserId, LoginProvider, Name),
    CONSTRAINT FK_AspNetUserTokens_AspNetUsers FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id) ON DELETE CASCADE
);
GO

CREATE INDEX IX_AspNetRoleClaims_RoleId ON dbo.AspNetRoleClaims(RoleId);
CREATE UNIQUE INDEX RoleNameIndex ON dbo.AspNetRoles(NormalizedName) WHERE NormalizedName IS NOT NULL;
CREATE INDEX IX_AspNetUserClaims_UserId ON dbo.AspNetUserClaims(UserId);
CREATE INDEX IX_AspNetUserLogins_UserId ON dbo.AspNetUserLogins(UserId);
CREATE INDEX IX_AspNetUserRoles_RoleId ON dbo.AspNetUserRoles(RoleId);
CREATE INDEX EmailIndex ON dbo.AspNetUsers(NormalizedEmail);
CREATE UNIQUE INDEX UserNameIndex ON dbo.AspNetUsers(NormalizedUserName) WHERE NormalizedUserName IS NOT NULL;
GO

/*
    Seed data
    --------------------------------------------------------------
    Note: UI terminology uses "Persons", but the current persisted
    table name in this schema remains dbo.Clients.
*/

-- Persons seed data (dbo.Clients)
INSERT INTO dbo.Clients (Name, Surname, IDNumber) VALUES
('Thabo',  'Mokoena',  '8601015009081'),
('Lerato', 'Ndlovu',   '9205150702084'),
('Aisha',  'Patel',    '9003120123086'),
('Sipho',  'Khumalo',  '8509095678082'),
('Naledi', 'Seema',    '9502210345088'),
('Johan',  'van Wyk',  '7807305024083'),
('Zanele', 'Dlamini',  '9901040288085'),
('Pieter', 'Botha',    '8804115189080'),
('Karabo', 'Molefe',   '9308260441087'),
('Nomsa',  'Mthembu',  '9706170210089');

-- Account seed data (multiple accounts per person, Savings and Cheque, Open and Closed)
INSERT INTO dbo.Accounts (ClientCode, AccountNumber, AccountType, AccountStatus, OutstandingBalance) VALUES
((SELECT Code FROM dbo.Clients WHERE IDNumber = '8601015009081'), '2026-10001', 'Savings', 'Open',   2450.00),
((SELECT Code FROM dbo.Clients WHERE IDNumber = '8601015009081'), '2026-10002', 'Cheque',  'Open',    860.00),
((SELECT Code FROM dbo.Clients WHERE IDNumber = '9205150702084'), '2026-10003', 'Savings', 'Open',  13000.00),
((SELECT Code FROM dbo.Clients WHERE IDNumber = '9003120123086'), '2026-10004', 'Cheque',  'Open',    420.00),
((SELECT Code FROM dbo.Clients WHERE IDNumber = '9003120123086'), '2026-10005', 'Savings', 'Closed',    0.00),
((SELECT Code FROM dbo.Clients WHERE IDNumber = '8509095678082'), '2026-10006', 'Savings', 'Open',   9850.75),
((SELECT Code FROM dbo.Clients WHERE IDNumber = '9502210345088'), '2026-10007', 'Cheque',  'Open',    120.00),
((SELECT Code FROM dbo.Clients WHERE IDNumber = '9502210345088'), '2026-10008', 'Savings', 'Open',   5600.00),
((SELECT Code FROM dbo.Clients WHERE IDNumber = '7807305024083'), '2026-10009', 'Savings', 'Closed',    0.00),
((SELECT Code FROM dbo.Clients WHERE IDNumber = '9901040288085'), '2026-10010', 'Cheque',  'Open',   2300.50),
((SELECT Code FROM dbo.Clients WHERE IDNumber = '8804115189080'), '2026-10011', 'Savings', 'Open',    750.00),
((SELECT Code FROM dbo.Clients WHERE IDNumber = '8804115189080'), '2026-10012', 'Cheque',  'Open',    150.00),
((SELECT Code FROM dbo.Clients WHERE IDNumber = '9308260441087'), '2026-10013', 'Savings', 'Open',  42000.00),
((SELECT Code FROM dbo.Clients WHERE IDNumber = '9706170210089'), '2026-10014', 'Cheque',  'Open',     50.00),
((SELECT Code FROM dbo.Clients WHERE IDNumber = '9706170210089'), '2026-10015', 'Savings', 'Open',   1000.00);

-- Transaction seed data (credits/debits with realistic chronology and balances)
INSERT INTO dbo.Transactions (AccountCode, TransactionDate, CaptureDate, Amount, Description, TransactionType) VALUES
((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10001'), '2025-11-01T09:00:00', '2025-11-01T09:01:00', 3000.00, 'Opening deposit', 'Credit'),
((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10001'), '2025-12-01T08:30:00', '2025-12-01T08:31:00',  500.00, 'Salary top-up', 'Credit'),
((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10001'), '2025-12-03T17:40:00', '2025-12-03T17:41:00',  750.00, 'Rent payment', 'Debit'),
((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10001'), '2025-12-05T13:10:00', '2025-12-05T13:11:00',  300.00, 'Groceries', 'Debit'),

((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10002'), '2025-11-05T10:00:00', '2025-11-05T10:00:30', 1000.00, 'Opening deposit', 'Credit'),
((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10002'), '2025-11-09T09:15:00', '2025-11-09T09:15:20',  200.00, 'Transfer from savings', 'Credit'),
((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10002'), '2025-11-10T07:20:00', '2025-11-10T07:20:15',  120.00, 'Fuel', 'Debit'),
((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10002'), '2025-11-11T19:10:00', '2025-11-11T19:10:30',  220.00, 'Utilities', 'Debit'),

((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10003'), '2025-10-20T09:00:00', '2025-10-20T09:01:00',10000.00, 'Opening deposit', 'Credit'),
((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10003'), '2025-11-20T09:00:00', '2025-11-20T09:01:00', 4000.00, 'Monthly salary', 'Credit'),
((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10003'), '2025-11-25T15:50:00', '2025-11-25T15:51:00', 1000.00, 'School fees', 'Debit'),

((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10004'), '2025-09-18T11:40:00', '2025-09-18T11:40:20',  700.00, 'Opening deposit', 'Credit'),
((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10004'), '2025-09-21T14:05:00', '2025-09-21T14:05:15',  180.00, 'Airtime and data', 'Debit'),
((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10004'), '2025-09-24T16:45:00', '2025-09-24T16:45:40',  100.00, 'Cash withdrawal', 'Debit'),

((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10005'), '2025-08-12T08:00:00', '2025-08-12T08:00:20',  500.00, 'Opening deposit', 'Credit'),
((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10005'), '2025-10-01T09:30:00', '2025-10-01T09:30:10',  500.00, 'Account closure transfer', 'Debit'),

((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10006'), '2025-06-01T09:00:00', '2025-06-01T09:01:00',12000.00, 'Opening deposit', 'Credit'),
((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10006'), '2025-12-15T12:20:00', '2025-12-15T12:20:30', 2149.25, 'Home loan installment', 'Debit'),

((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10007'), '2025-11-01T09:00:00', '2025-11-01T09:00:30',  600.00, 'Opening deposit', 'Credit'),
((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10007'), '2025-11-03T18:30:00', '2025-11-03T18:30:40',  480.00, 'Groceries', 'Debit'),

((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10008'), '2025-07-01T10:00:00', '2025-07-01T10:00:20', 5000.00, 'Opening deposit', 'Credit'),
((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10008'), '2025-08-01T10:00:00', '2025-08-01T10:00:20', 1000.00, 'Performance bonus', 'Credit'),
((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10008'), '2025-08-05T12:10:00', '2025-08-05T12:10:40',  400.00, 'Appliance purchase', 'Debit'),

((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10009'), '2025-05-02T11:25:00', '2025-05-02T11:25:40',  300.00, 'Opening deposit', 'Credit'),
((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10009'), '2025-06-01T11:25:00', '2025-06-01T11:25:20',  300.00, 'Account close-out', 'Debit'),

((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10010'), '2025-11-14T08:55:00', '2025-11-14T08:55:20', 2000.00, 'Opening deposit', 'Credit'),
((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10010'), '2025-11-18T17:20:00', '2025-11-18T17:20:20',  900.00, 'Freelance income', 'Credit'),
((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10010'), '2025-11-21T19:45:00', '2025-11-21T19:45:30',  599.50, 'Travel booking', 'Debit'),

((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10011'), '2025-10-10T09:10:00', '2025-10-10T09:10:10',  750.00, 'Opening deposit', 'Credit'),

((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10012'), '2025-10-12T08:40:00', '2025-10-12T08:40:15',  500.00, 'Opening deposit', 'Credit'),
((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10012'), '2025-10-16T14:20:00', '2025-10-16T14:20:35',  350.00, 'Insurance premium', 'Debit'),

((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10013'), '2025-04-01T10:00:00', '2025-04-01T10:01:00',40000.00, 'Opening deposit', 'Credit'),
((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10013'), '2025-09-15T10:00:00', '2025-09-15T10:01:00', 5000.00, 'Investment return', 'Credit'),
((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10013'), '2025-10-01T10:00:00', '2025-10-01T10:01:00', 3000.00, 'Vehicle purchase', 'Debit'),

((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10014'), '2025-12-01T09:00:00', '2025-12-01T09:00:30',  350.00, 'Opening deposit', 'Credit'),
((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10014'), '2025-12-04T13:35:00', '2025-12-04T13:35:20',  300.00, 'Card purchase', 'Debit'),

((SELECT Code FROM dbo.Accounts WHERE AccountNumber = '2026-10015'), '2025-11-08T10:05:00', '2025-11-08T10:05:20', 1000.00, 'Opening deposit', 'Credit');
GO
