USE [GrupoXpert];
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- Agregar columna Tipo si no existe
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Identidad].[Usuarios]') AND name = 'Tipo')
BEGIN
    ALTER TABLE [Identidad].[Usuarios] ADD [Tipo] NVARCHAR(50) NOT NULL DEFAULT 'Estudiante';
    PRINT 'Columna Tipo agregada a Usuarios con valor por defecto Estudiante.';
END
ELSE
BEGIN
    PRINT 'La columna Tipo ya existe en Usuarios.';
END
GO

-- Crear índice para Tipo si no existe
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Usuarios_Tipo' AND object_id = OBJECT_ID(N'[Identidad].[Usuarios]'))
BEGIN
    CREATE INDEX [IX_Usuarios_Tipo] ON [Identidad].[Usuarios] ([Tipo]);
    PRINT 'Índice IX_Usuarios_Tipo creado.';
END
ELSE
BEGIN
    PRINT 'El índice IX_Usuarios_Tipo ya existe.';
END
GO

PRINT 'Migración de columna Tipo completada exitosamente.';
GO
