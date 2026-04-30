USE [GrupoXpert];
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- 1. Renombrar columna NombreUsuario -> Email si existe
-- El usuario mencionó "NombreCuenta", por si acaso validamos ambos
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Identidad].[Usuarios]') AND name = 'NombreUsuario')
BEGIN
    EXEC sp_rename 'Identidad.Usuarios.NombreUsuario', 'Email', 'COLUMN';
    PRINT 'Columna NombreUsuario renombrada a Email.';
END
ELSE IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Identidad].[Usuarios]') AND name = 'NombreCuenta')
BEGIN
    EXEC sp_rename 'Identidad.Usuarios.NombreCuenta', 'Email', 'COLUMN';
    PRINT 'Columna NombreCuenta renombrada a Email.';
END
GO

-- 2. Asegurar que Email tenga el tamaño correcto (NVARCHAR(254))
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Identidad].[Usuarios]') AND name = 'Email')
BEGIN
    ALTER TABLE [Identidad].[Usuarios] ALTER COLUMN [Email] NVARCHAR(254) NOT NULL;
    PRINT 'Columna Email ajustada a NVARCHAR(254).';
END
GO

-- 3. Agregar columnas de activación si no existen
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Identidad].[Usuarios]') AND name = 'TokenActivacion')
BEGIN
    ALTER TABLE [Identidad].[Usuarios] ADD [TokenActivacion] NVARCHAR(256) NULL;
    PRINT 'Columna TokenActivacion agregada.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Identidad].[Usuarios]') AND name = 'TokenActivacionExpira')
BEGIN
    ALTER TABLE [Identidad].[Usuarios] ADD [TokenActivacionExpira] DATETIMEOFFSET NULL;
    PRINT 'Columna TokenActivacionExpira agregada.';
END
GO

-- 4. Reconstruir índices
-- Eliminar índices viejos relacionados con el login si existen
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'UX_Usuarios_NombreUsuario' AND object_id = OBJECT_ID(N'[Identidad].[Usuarios]'))
    DROP INDEX [UX_Usuarios_NombreUsuario] ON [Identidad].[Usuarios];
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'UX_Usuarios_NombreCuenta' AND object_id = OBJECT_ID(N'[Identidad].[Usuarios]'))
    DROP INDEX [UX_Usuarios_NombreCuenta] ON [Identidad].[Usuarios];
GO

-- Crear/Asegurar índice único para Email
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'UX_Usuarios_Email' AND object_id = OBJECT_ID(N'[Identidad].[Usuarios]'))
    DROP INDEX [UX_Usuarios_Email] ON [Identidad].[Usuarios];
GO
CREATE UNIQUE INDEX [UX_Usuarios_Email] ON [Identidad].[Usuarios] ([Email]);
GO

-- Índice para TokenActivacion
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Usuarios_TokenActivacion' AND object_id = OBJECT_ID(N'[Identidad].[Usuarios]'))
    DROP INDEX [IX_Usuarios_TokenActivacion] ON [Identidad].[Usuarios];
GO
CREATE INDEX [IX_Usuarios_TokenActivacion] ON [Identidad].[Usuarios] ([TokenActivacion]) WHERE [TokenActivacion] IS NOT NULL;
GO

PRINT 'Migración de tabla Usuarios (Identidad) completada exitosamente.';
GO
