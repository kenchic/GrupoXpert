-- =============================================================================
-- Caso de Uso: Subir Entregable Parcial / Avance de Fase (Colaborador)
-- Tablas: Avances, Comentarios, ArchivosAdjuntos
-- Schema: Academia
-- =============================================================================
USE [GrupoXpert];
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================================================
-- Tabla: [Academia].[Avances]
-- =============================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Academia].[Avances]'))
BEGIN
    CREATE TABLE [Academia].[Avances]
    (
        [Id]            UNIQUEIDENTIFIER    NOT NULL,
        [SolicitudId]   UNIQUEIDENTIFIER    NOT NULL,
        [AsesorId]      UNIQUEIDENTIFIER    NOT NULL,
        [Descripcion]   NVARCHAR(2000)      NOT NULL,
        [NumeroFase]    INT                 NOT NULL,
        [Tipo]          INT                 NOT NULL,
        [Estado]        INT                 NOT NULL DEFAULT 1,
        [FechaSubida]   DATETIMEOFFSET      NOT NULL,

        CONSTRAINT [PK_Avances] PRIMARY KEY CLUSTERED ([Id])
    );
END
GO

-- Índices
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Avances_SolicitudId' AND object_id = OBJECT_ID(N'[Academia].[Avances]'))
    CREATE INDEX [IX_Avances_SolicitudId] ON [Academia].[Avances] ([SolicitudId]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Avances_AsesorId' AND object_id = OBJECT_ID(N'[Academia].[Avances]'))
    CREATE INDEX [IX_Avances_AsesorId] ON [Academia].[Avances] ([AsesorId]);
GO

-- Foreign Keys
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Avances_SolicitudesAcademicas')
    ALTER TABLE [Academia].[Avances]
        ADD CONSTRAINT [FK_Avances_SolicitudesAcademicas]
        FOREIGN KEY ([SolicitudId]) REFERENCES [Academia].[SolicitudesAcademicas] ([Id]);
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Avances_PerfilesColaboradores')
    ALTER TABLE [Academia].[Avances]
        ADD CONSTRAINT [FK_Avances_PerfilesColaboradores]
        FOREIGN KEY ([AsesorId]) REFERENCES [Perfil].[PerfilesColaboradores] ([Id]);
GO


-- =============================================================================
-- Tabla: [Academia].[Comentarios]
-- =============================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Academia].[Comentarios]'))
BEGIN
    CREATE TABLE [Academia].[Comentarios]
    (
        [Id]            UNIQUEIDENTIFIER    NOT NULL,
        [AvanceId]      UNIQUEIDENTIFIER    NOT NULL,
        [AutorId]       UNIQUEIDENTIFIER    NOT NULL,
        [Contenido]     NVARCHAR(4000)      NOT NULL,
        [FechaCreacion] DATETIMEOFFSET      NOT NULL,

        CONSTRAINT [PK_Comentarios] PRIMARY KEY CLUSTERED ([Id])
    );
END
GO

-- Índices
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Comentarios_AvanceId' AND object_id = OBJECT_ID(N'[Academia].[Comentarios]'))
    CREATE INDEX [IX_Comentarios_AvanceId] ON [Academia].[Comentarios] ([AvanceId]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Comentarios_AutorId' AND object_id = OBJECT_ID(N'[Academia].[Comentarios]'))
    CREATE INDEX [IX_Comentarios_AutorId] ON [Academia].[Comentarios] ([AutorId]);
GO

-- Foreign Keys
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Comentarios_Avances')
    ALTER TABLE [Academia].[Comentarios]
        ADD CONSTRAINT [FK_Comentarios_Avances]
        FOREIGN KEY ([AvanceId]) REFERENCES [Academia].[Avances] ([Id])
        ON DELETE CASCADE;
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Comentarios_Usuarios')
    ALTER TABLE [Academia].[Comentarios]
        ADD CONSTRAINT [FK_Comentarios_Usuarios]
        FOREIGN KEY ([AutorId]) REFERENCES [Identidad].[Usuarios] ([Id]);
GO


-- =============================================================================
-- Tabla: [Academia].[ArchivosAdjuntos]
-- =============================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Academia].[ArchivosAdjuntos]'))
BEGIN
    CREATE TABLE [Academia].[ArchivosAdjuntos]
    (
        [Id]             UNIQUEIDENTIFIER   NOT NULL,
        [AvanceId]       UNIQUEIDENTIFIER   NOT NULL,
        [NombreArchivo]  NVARCHAR(500)      NOT NULL,
        [Url]            NVARCHAR(2000)     NOT NULL,
        [TamanioBytes]   BIGINT             NOT NULL,
        [TipoContenido]  NVARCHAR(200)      NOT NULL,
        [FechaSubida]    DATETIMEOFFSET     NOT NULL,

        CONSTRAINT [PK_ArchivosAdjuntos] PRIMARY KEY CLUSTERED ([Id])
    );
END
GO

-- Índices
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ArchivosAdjuntos_AvanceId' AND object_id = OBJECT_ID(N'[Academia].[ArchivosAdjuntos]'))
    CREATE INDEX [IX_ArchivosAdjuntos_AvanceId] ON [Academia].[ArchivosAdjuntos] ([AvanceId]);
GO

-- Foreign Keys
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ArchivosAdjuntos_Avances')
    ALTER TABLE [Academia].[ArchivosAdjuntos]
        ADD CONSTRAINT [FK_ArchivosAdjuntos_Avances]
        FOREIGN KEY ([AvanceId]) REFERENCES [Academia].[Avances] ([Id])
        ON DELETE CASCADE;
GO
