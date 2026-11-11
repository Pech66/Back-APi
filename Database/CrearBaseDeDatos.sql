IF DB_ID(N'LABOFMUTANT_PWA') IS NULL
BEGIN
    CREATE DATABASE [LABOFMUTANT_PWA];
END;
GO
USE [LABOFMUTANT_PWA];
GO
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
    WHERE [MigrationId] = N'20261110041447_InitialPwaSqlServer'
)
BEGIN
    CREATE TABLE [Dispositivos] (
        [Id] varchar(64) NOT NULL,
        [UltimaSecuencia] bigint NOT NULL,
        [ActualizadoUtc] datetimeoffset NOT NULL,
        CONSTRAINT [PK_Dispositivos] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261110041447_InitialPwaSqlServer'
)
BEGIN
    CREATE TABLE [NotasDiario] (
        [DispositivoId] varchar(64) NOT NULL,
        [IdLocal] bigint NOT NULL,
        [Texto] nvarchar(600) NOT NULL,
        [CreadaUnixMs] bigint NOT NULL,
        CONSTRAINT [PK_NotasDiario] PRIMARY KEY ([DispositivoId], [IdLocal]),
        CONSTRAINT [FK_NotasDiario_Dispositivos_DispositivoId] FOREIGN KEY ([DispositivoId]) REFERENCES [Dispositivos] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261110041447_InitialPwaSqlServer'
)
BEGIN
    CREATE TABLE [Perfiles] (
        [DispositivoId] varchar(64) NOT NULL,
        [Nombre] nvarchar(40) NOT NULL,
        CONSTRAINT [PK_Perfiles] PRIMARY KEY ([DispositivoId]),
        CONSTRAINT [FK_Perfiles_Dispositivos_DispositivoId] FOREIGN KEY ([DispositivoId]) REFERENCES [Dispositivos] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261110041447_InitialPwaSqlServer'
)
BEGIN
    CREATE TABLE [Preferencias] (
        [DispositivoId] varchar(64) NOT NULL,
        [Linterna] bit NOT NULL,
        [PestanaGuia] varchar(10) NOT NULL,
        CONSTRAINT [PK_Preferencias] PRIMARY KEY ([DispositivoId]),
        CONSTRAINT [FK_Preferencias_Dispositivos_DispositivoId] FOREIGN KEY ([DispositivoId]) REFERENCES [Dispositivos] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261110041447_InitialPwaSqlServer'
)
BEGIN
    CREATE TABLE [Progresos] (
        [DispositivoId] varchar(64) NOT NULL,
        [Visitas] bigint NOT NULL,
        [PrimeraVisita] bigint NULL,
        [UltimaVisita] bigint NULL,
        [VecesDetectado] bigint NOT NULL,
        [MejorSigilo] bigint NOT NULL,
        [TeclasProbadasJson] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Progresos] PRIMARY KEY ([DispositivoId]),
        CONSTRAINT [FK_Progresos_Dispositivos_DispositivoId] FOREIGN KEY ([DispositivoId]) REFERENCES [Dispositivos] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261110041447_InitialPwaSqlServer'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261110041447_InitialPwaSqlServer', N'8.0.0');
END;
GO

COMMIT;
GO

