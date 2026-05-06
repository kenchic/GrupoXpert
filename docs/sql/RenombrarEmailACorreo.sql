USE [GrupoXpert];
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- Renombrar columna Email → Correo si existe
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Identidad].[Usuarios]') AND name = 'Email')
BEGIN
    -- Eliminar índice antiguo si existe
    IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'UX_Usuarios_Email' AND object_id = OBJECT_ID(N'[Identidad].[Usuarios]'))
        DROP INDEX [UX_Usuarios_Email] ON [Identidad].[Usuarios];
    
    EXEC sp_rename 'Identidad.Usuarios.Email', 'Correo', 'COLUMN';
    PRINT 'Columna Email renombrada a Correo.';
END
GO

-- Crear índice único para Correo si no existe
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UX_Usuarios_Correo' AND object_id = OBJECT_ID(N'[Identidad].[Usuarios]'))
BEGIN
    CREATE UNIQUE INDEX [UX_Usuarios_Correo] ON [Identidad].[Usuarios] ([Correo]);
    PRINT 'Índice UX_Usuarios_Correo creado.';
END
GO

PRINT 'Migración de columna Email→Correo completada exitosamente.';
GO
