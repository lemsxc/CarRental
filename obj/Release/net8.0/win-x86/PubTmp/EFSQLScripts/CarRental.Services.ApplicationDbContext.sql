IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250326101214_InitialMigration'
)
BEGIN
    CREATE TABLE [Cars] (
        [CarId] int NOT NULL IDENTITY,
        [Brand] nvarchar(50) NOT NULL,
        [Model] nvarchar(50) NOT NULL,
        [Image] nvarchar(max) NOT NULL,
        [RentPrice] real NOT NULL,
        [Category] nvarchar(50) NOT NULL,
        [Transmission] nvarchar(20) NOT NULL,
        [FuelType] nvarchar(20) NOT NULL,
        [FuelLevel] int NULL,
        [PlateNumber] nvarchar(20) NOT NULL,
        [Mileage] int NOT NULL,
        [Condition] nvarchar(50) NOT NULL,
        [Status] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_Cars] PRIMARY KEY ([CarId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250326101214_InitialMigration'
)
BEGIN
    CREATE TABLE [Drivers] (
        [DriverId] int NOT NULL IDENTITY,
        [FirstName] nvarchar(100) NOT NULL,
        [LastName] nvarchar(100) NOT NULL,
        [ContactNumber] nvarchar(15) NOT NULL,
        [Address] nvarchar(100) NOT NULL,
        [LicenseNumber] nvarchar(20) NOT NULL,
        [LicenseExpiry] datetime2 NOT NULL,
        [Status] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_Drivers] PRIMARY KEY ([DriverId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250326101214_InitialMigration'
)
BEGIN
    CREATE TABLE [Users] (
        [UsersId] int NOT NULL IDENTITY,
        [Password] nvarchar(max) NOT NULL,
        [FirstName] nvarchar(max) NOT NULL,
        [LastName] nvarchar(max) NOT NULL,
        [Email] nvarchar(max) NOT NULL,
        [ContactNumber] nvarchar(max) NULL,
        [Address] nvarchar(max) NULL,
        [DriversLicense] nvarchar(max) NULL,
        [Role] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([UsersId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250326101214_InitialMigration'
)
BEGIN
    CREATE TABLE [Feedbacks] (
        [FeedbackId] int NOT NULL IDENTITY,
        [UsersId] int NOT NULL,
        [CarId] int NOT NULL,
        [Rating] int NOT NULL,
        [Review] nvarchar(max) NOT NULL,
        [DateReview] datetime2 NOT NULL,
        CONSTRAINT [PK_Feedbacks] PRIMARY KEY ([FeedbackId]),
        CONSTRAINT [FK_Feedbacks_Cars_CarId] FOREIGN KEY ([CarId]) REFERENCES [Cars] ([CarId]) ON DELETE CASCADE,
        CONSTRAINT [FK_Feedbacks_Users_UsersId] FOREIGN KEY ([UsersId]) REFERENCES [Users] ([UsersId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250326101214_InitialMigration'
)
BEGIN
    CREATE TABLE [Reservations] (
        [ReservationId] int NOT NULL IDENTITY,
        [UsersId] int NOT NULL,
        [CarId] int NOT NULL,
        [TotalAmount] real NOT NULL,
        [ReservedDate] datetime2 NOT NULL,
        [StartDate] datetime2 NOT NULL,
        [EndDate] datetime2 NOT NULL,
        [ReturnDate] datetime2 NULL,
        [Status] nvarchar(max) NOT NULL,
        [DriverId] int NULL,
        CONSTRAINT [PK_Reservations] PRIMARY KEY ([ReservationId]),
        CONSTRAINT [FK_Reservations_Cars_CarId] FOREIGN KEY ([CarId]) REFERENCES [Cars] ([CarId]) ON DELETE CASCADE,
        CONSTRAINT [FK_Reservations_Drivers_DriverId] FOREIGN KEY ([DriverId]) REFERENCES [Drivers] ([DriverId]),
        CONSTRAINT [FK_Reservations_Users_UsersId] FOREIGN KEY ([UsersId]) REFERENCES [Users] ([UsersId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250326101214_InitialMigration'
)
BEGIN
    CREATE TABLE [Payments] (
        [PaymentId] int NOT NULL IDENTITY,
        [ReservationId] int NOT NULL,
        [Amount] real NOT NULL,
        [PaymentMethod] nvarchar(max) NOT NULL,
        [PaymentDate] datetime2 NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Payments] PRIMARY KEY ([PaymentId]),
        CONSTRAINT [FK_Payments_Reservations_ReservationId] FOREIGN KEY ([ReservationId]) REFERENCES [Reservations] ([ReservationId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250326101214_InitialMigration'
)
BEGIN
    CREATE INDEX [IX_Feedbacks_CarId] ON [Feedbacks] ([CarId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250326101214_InitialMigration'
)
BEGIN
    CREATE INDEX [IX_Feedbacks_UsersId] ON [Feedbacks] ([UsersId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250326101214_InitialMigration'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Payments_ReservationId] ON [Payments] ([ReservationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250326101214_InitialMigration'
)
BEGIN
    CREATE INDEX [IX_Reservations_CarId] ON [Reservations] ([CarId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250326101214_InitialMigration'
)
BEGIN
    CREATE INDEX [IX_Reservations_DriverId] ON [Reservations] ([DriverId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250326101214_InitialMigration'
)
BEGIN
    CREATE INDEX [IX_Reservations_UsersId] ON [Reservations] ([UsersId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250326101214_InitialMigration'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250326101214_InitialMigration', N'8.0.13');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250402024016_UserVerfication'
)
BEGIN
    ALTER TABLE [Users] ADD [IsVerified] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250402024016_UserVerfication'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250402024016_UserVerfication', N'8.0.13');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250402031336_VerifyUsers'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'DriversLicense');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Users] DROP COLUMN [DriversLicense];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250402031336_VerifyUsers'
)
BEGIN
    CREATE TABLE [Verifications] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [LicensePath] nvarchar(max) NOT NULL,
        [Status] nvarchar(20) NOT NULL,
        [AdminRemarks] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Verifications] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Verifications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UsersId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250402031336_VerifyUsers'
)
BEGIN
    CREATE INDEX [IX_Verifications_UserId] ON [Verifications] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250402031336_VerifyUsers'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250402031336_VerifyUsers', N'8.0.13');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250404050547_VerificationChanges'
)
BEGIN
    EXEC sp_rename N'[Verifications].[LicensePath]', N'LicenseFrontPath', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250404050547_VerificationChanges'
)
BEGIN
    ALTER TABLE [Verifications] ADD [LicenseBackPath] nvarchar(max) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250404050547_VerificationChanges'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250404050547_VerificationChanges', N'8.0.13');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250407041845_AdminLogs'
)
BEGIN
    CREATE TABLE [Logs] (
        [Id] int NOT NULL IDENTITY,
        [ActionType] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [AdminName] nvarchar(max) NOT NULL,
        [AdminId] int NOT NULL,
        [ActionDate] datetime2 NOT NULL,
        CONSTRAINT [PK_Logs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Logs_Users_AdminId] FOREIGN KEY ([AdminId]) REFERENCES [Users] ([UsersId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250407041845_AdminLogs'
)
BEGIN
    CREATE INDEX [IX_Logs_AdminId] ON [Logs] ([AdminId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250407041845_AdminLogs'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250407041845_AdminLogs', N'8.0.13');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250407080224_DropFeedback'
)
BEGIN
    DROP TABLE [Feedbacks];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250407080224_DropFeedback'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250407080224_DropFeedback', N'8.0.13');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250502222847_UpdateCarSeats'
)
BEGIN
    ALTER TABLE [Cars] ADD [Seats] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250502222847_UpdateCarSeats'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250502222847_UpdateCarSeats', N'8.0.13');
END;
GO

COMMIT;
GO

