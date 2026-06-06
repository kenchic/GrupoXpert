-- =============================================================================
-- Caso de Uso: Revisión de Calidad Interna (Admin / Revisor)
-- Schema: Calidad
-- Tabla: RevisionesCalidad
-- =============================================================================
USE [GrupoXpert];
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================================================
-- Esquema: Calidad
-- =============================================================================
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Calidad')
    EXEC('CREATE SCHEMA [Calidad]');
GO

-- =============================================================================
-- Tabla: [Calidad].[RevisionesCalidad]
-- =============================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Calidad].[RevisionesCalidad]'))
BEGIN
    CREATE TABLE [Calidad].[RevisionesCalidad]
    (
        [Id]             UNIQUEIDENTIFIER    NOT NULL,
        [AvanceId]       UNIQUEIDENTIFIER    NOT NULL,
        [RevisorId]      UNIQUEIDENTIFIER    NOT NULL,
        [FechaRevision]  DATETIMEOFFSET      NOT NULL,
        [VistoBueno]     BIT                 NOT NULL DEFAULT 0,
        [Observaciones]  NVARCHAR(1000)      NULL,
        [Estado]         INT                 NOT NULL DEFAULT 1,

        CONSTRAINT [PK_RevisionesCalidad] PRIMARY KEY CLUSTERED ([Id])
    );
END
GO

-- =============================================================================
-- Índices
-- =============================================================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RevisionesCalidad_AvanceId' AND object_id = OBJECT_ID(N'[Calidad].[RevisionesCalidad]'))
    CREATE UNIQUE INDEX [IX_RevisionesCalidad_AvanceId] ON [Calidad].[RevisionesCalidad] ([AvanceId]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RevisionesCalidad_RevisorId' AND object_id = OBJECT_ID(N'[Calidad].[RevisionesCalidad]'))
    CREATE INDEX [IX_RevisionesCalidad_RevisorId] ON [Calidad].[RevisionesCalidad] ([RevisorId]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RevisionesCalidad_Estado_Pendientes' AND object_id = OBJECT_ID(N'[Calidad].[RevisionesCalidad]'))
    CREATE INDEX [IX_RevisionesCalidad_Estado_Pendientes] ON [Calidad].[RevisionesCalidad] ([Estado]) WHERE [Estado] = 1;
GO

-- =============================================================================
-- Foreign Keys
-- =============================================================================
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_RevisionesCalidad_Avances')
    ALTER TABLE [Calidad].[RevisionesCalidad]
        ADD CONSTRAINT [FK_RevisionesCalidad_Avances]
        FOREIGN KEY ([AvanceId]) REFERENCES [Academia].[Avances] ([Id]);
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_RevisionesCalidad_Usuarios')
    ALTER TABLE [Calidad].[RevisionesCalidad]
        ADD CONSTRAINT [FK_RevisionesCalidad_Usuarios]
        FOREIGN KEY ([RevisorId]) REFERENCES [Identidad].[Usuarios] ([Id]);
GO

PRINT 'Migración de tabla RevisionesCalidad (Calidad) completada exitosamente.';
GO
