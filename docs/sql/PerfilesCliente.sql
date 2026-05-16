USE [GrupoXpert];
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- Crear esquema Perfil si no existe
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Perfil')
    EXEC('CREATE SCHEMA [Perfil]');
GO

-- Crear tabla PerfilesCliente
IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Perfil].[PerfilesCliente]'))
BEGIN
    CREATE TABLE [Perfil].[PerfilesCliente] (
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [UsuarioId] UNIQUEIDENTIFIER NOT NULL,
        [NivelAcademico] INT NOT NULL DEFAULT 0,
        [UrgenciaEntrega] INT NOT NULL DEFAULT 0,
        [Telefono] NVARCHAR(50) NULL,
        CONSTRAINT [PK_PerfilesCliente] PRIMARY KEY ([Id])
    );
END
GO

-- Crear tabla PerfilClienteAreasInteres para la colección de strings
IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Perfil].[PerfilClienteAreasInteres]'))
BEGIN
    CREATE TABLE [Perfil].[PerfilClienteAreasInteres] (
        [PerfilClienteId] UNIQUEIDENTIFIER NOT NULL,
        [Area] NVARCHAR(200) NOT NULL,
        CONSTRAINT [PK_PerfilClienteAreasInteres] PRIMARY KEY ([PerfilClienteId], [Area]),
        CONSTRAINT [FK_PerfilClienteAreasInteres_PerfilesCliente] FOREIGN KEY ([PerfilClienteId]) REFERENCES [Perfil].[PerfilesCliente] ([Id]) ON DELETE CASCADE
    );
END
GO
