USE [GrupoXpert];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- 1. Agregar columna Estado
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[Academia].[SolicitudesAcademicas]') 
      AND name = N'Estado'
)
BEGIN
    ALTER TABLE [Academia].[SolicitudesAcademicas]
    ADD [Estado] INT NOT NULL DEFAULT 1;
END
GO

-- 2. Agregar columna AsesorId
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[Academia].[SolicitudesAcademicas]') 
      AND name = N'AsesorId'
)
BEGIN
    ALTER TABLE [Academia].[SolicitudesAcademicas]
    ADD [AsesorId] UNIQUEIDENTIFIER NULL;
END
GO

-- 3. Agregar Llave Foránea hacia Perfil.PerfilesColaboradores
IF NOT EXISTS (
    SELECT * FROM sys.foreign_keys 
    WHERE object_id = OBJECT_ID(N'[Academia].[FK_SolicitudesAcademicas_PerfilesColaboradores]')
)
BEGIN
    ALTER TABLE [Academia].[SolicitudesAcademicas]
    ADD CONSTRAINT [FK_SolicitudesAcademicas_PerfilesColaboradores] 
    FOREIGN KEY ([AsesorId]) REFERENCES [Perfil].[PerfilesColaboradores] ([Id])
    ON DELETE NO ACTION;
END
GO
