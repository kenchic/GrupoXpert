-- =======================================================================================
-- Módulo:        Identidad
-- Nombre script: Usuarios.sql
-- Propósito:     Creación del esquema Identidad y la tabla Usuarios para el modelo
--                físico de datos del Agregado Raíz Usuario.
-- =======================================================================================

USE [GrupoXpert];
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Identidad')
BEGIN
    EXEC('CREATE SCHEMA [Identidad]');
END
GO

-- Tabla: Usuarios (Aggregate Root)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Identidad].[Usuarios]'))
BEGIN
CREATE TABLE [Identidad].[Usuarios] (
    [Id]                 UNIQUEIDENTIFIER    NOT NULL    CONSTRAINT DF_Usuarios_Id DEFAULT NEWSEQUENTIALID(),
    [NombreUsuario]      NVARCHAR(50)        NOT NULL,
    [HashClave]          NVARCHAR(500)       NOT NULL,
    [Nombre]             NVARCHAR(150)       NOT NULL,
    [Imagen]             NVARCHAR(500)       NULL,
    [EstaActivo]         BIT                 NOT NULL    CONSTRAINT DF_Usuarios_EstaActivo DEFAULT 1,
    [FechaCreacion]      DATETIMEOFFSET      NOT NULL    CONSTRAINT DF_Usuarios_FechaCreacion DEFAULT SYSDATETIMEOFFSET(),
    [UltimoInicioSesion] DATETIMEOFFSET      NULL,

    CONSTRAINT [PK_Usuarios] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [UQ_Usuarios_NombreUsuario] UNIQUE ([NombreUsuario])
);
END
GO

-- Índice para optimizar búsquedas por nombre de usuario en el login
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Usuarios_NombreUsuario_Login' AND object_id = OBJECT_ID('Identidad.Usuarios'))
BEGIN
CREATE NONCLUSTERED INDEX [IX_Usuarios_NombreUsuario_Login]
    ON [Identidad].[Usuarios] ([NombreUsuario])
    INCLUDE ([HashClave], [EstaActivo]);
END
GO

-- Índice para filtrar usuarios activos (útil para listas y reportes)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Usuarios_EstaActivo' AND object_id = OBJECT_ID('Identidad.Usuarios'))
BEGIN
CREATE NONCLUSTERED INDEX [IX_Usuarios_EstaActivo]
    ON [Identidad].[Usuarios] ([EstaActivo])
    WHERE [EstaActivo] = 1;
END
GO
