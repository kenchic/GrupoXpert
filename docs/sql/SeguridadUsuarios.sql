USE [GrupoXpert];
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- 1. Asegurar esquema
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Identidad')
    EXEC('CREATE SCHEMA [Identidad]');
GO

-- 2. Asegurar tabla (si no existe)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Identidad].[Usuarios]'))
BEGIN
    CREATE TABLE [Identidad].[Usuarios] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [Correo] NVARCHAR(254) NOT NULL,
        [HashClave] NVARCHAR(500) NOT NULL,
        [Nombre] NVARCHAR(150) NOT NULL,
        [Imagen] NVARCHAR(500) NULL,
        [Tipo] NVARCHAR(50) NOT NULL DEFAULT 'Estudiante',
        [EstaActivo] BIT NOT NULL DEFAULT 0,
        [FechaCreacion] DATETIMEOFFSET NOT NULL,
        [UltimoInicioSesion] DATETIMEOFFSET NULL,
        [TokenActivacion] NVARCHAR(256) NULL,
        [TokenActivacionExpira] DATETIMEOFFSET NULL
    );

    CREATE UNIQUE INDEX [UX_Usuarios_Correo] ON [Identidad].[Usuarios] ([Correo]);
    CREATE INDEX [IX_Usuarios_Tipo] ON [Identidad].[Usuarios] ([Tipo]);
END
GO

-- 3. Asegurar permisos para el usuario de la aplicación
IF EXISTS (SELECT * FROM sys.database_principals WHERE name = 'grupoxpert')
BEGIN
    GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::[Identidad] TO [grupoxpert];
END
GO
