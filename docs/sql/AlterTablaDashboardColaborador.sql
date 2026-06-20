USE [GrupoXpert];
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- ALTER: Agregar columnas de ReputacionAcademica a PerfilesColaboradores
-- =============================================
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_SCHEMA = 'Perfil' AND TABLE_NAME = 'PerfilesColaboradores' AND COLUMN_NAME = 'PuntajePromedioReputacion'
)
BEGIN
    ALTER TABLE [Perfil].[PerfilesColaboradores]
        ADD [PuntajePromedioReputacion] DECIMAL(5,2) NOT NULL DEFAULT 0.00;
END
GO

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_SCHEMA = 'Perfil' AND TABLE_NAME = 'PerfilesColaboradores' AND COLUMN_NAME = 'TotalCalificacionesReputacion'
)
BEGIN
    ALTER TABLE [Perfil].[PerfilesColaboradores]
        ADD [TotalCalificacionesReputacion] INT NOT NULL DEFAULT 0;
END
GO

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_SCHEMA = 'Perfil' AND TABLE_NAME = 'PerfilesColaboradores' AND COLUMN_NAME = 'NivelReputacion'
)
BEGIN
    ALTER TABLE [Perfil].[PerfilesColaboradores]
        ADD [NivelReputacion] INT NOT NULL DEFAULT 0;
END
GO

-- =============================================
-- INDEXES: Optimizados para Dashboard Colaborador
-- =============================================

-- SolicitudAcademica: Buscar por AsesorId + Estado (proyectos en curso, solicitudes abiertas)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_SolicitudesAcademicas_AsesorId_Estado' AND object_id = OBJECT_ID(N'[Academia].[SolicitudesAcademicas]'))
    CREATE NONCLUSTERED INDEX [IX_SolicitudesAcademicas_AsesorId_Estado] 
    ON [Academia].[SolicitudesAcademicas] ([AsesorId], [Estado]);
GO

-- SolicitudAcademica: Buscar por Estado (solicitudes pendientes)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_SolicitudesAcademicas_Estado' AND object_id = OBJECT_ID(N'[Academia].[SolicitudesAcademicas]'))
    CREATE NONCLUSTERED INDEX [IX_SolicitudesAcademicas_Estado] 
    ON [Academia].[SolicitudesAcademicas] ([Estado]);
GO

-- SolicitudAcademica: Buscar por ClienteId
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_SolicitudesAcademicas_ClienteId' AND object_id = OBJECT_ID(N'[Academia].[SolicitudesAcademicas]'))
    CREATE NONCLUSTERED INDEX [IX_SolicitudesAcademicas_ClienteId] 
    ON [Academia].[SolicitudesAcademicas] ([ClienteId]);
GO

-- Postulaciones: Buscar por ColaboradorId + Estado (postulaciones pendientes)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Postulaciones_ColaboradorId_Estado' AND object_id = OBJECT_ID(N'[Academia].[Postulaciones]'))
    CREATE NONCLUSTERED INDEX [IX_Postulaciones_ColaboradorId_Estado] 
    ON [Academia].[Postulaciones] ([ColaboradorId], [Estado]);
GO

-- Avances: Contar entregas por AsesorId (verificar si ya existe con otro nombre)
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes i 
    JOIN sys.tables t ON i.object_id = t.object_id 
    WHERE t.name = 'Avances' AND i.name = 'IX_Avances_AsesorId'
)
    CREATE NONCLUSTERED INDEX [IX_Avances_AsesorId] 
    ON [Academia].[Avances] ([AsesorId]);
GO

-- CalificacionesColaborador: Ya existe IX_CalificacionesColaborador_ColaboradorId, verificamos
-- (Presente en la BD original - no necesita crearse)

PRINT 'Script ALTER ejecutado correctamente.';
GO