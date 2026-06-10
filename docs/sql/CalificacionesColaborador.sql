-- =============================================================================
-- Caso de Uso: Calificar Servicio y Colaborador
-- Tabla: [Calificacion].[CalificacionesColaborador]
-- ===========================================================================
-- Cuando un Avance alcanza estado Liberado (4), la SolicitudAcademica
-- transiciona a estado Liberacion (6) y el Cliente puede calificar al
-- Colaborador con 1-5 estrellas y una observacion opcional.
-- =============================================================================
USE [GrupoXpert];
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================================================
-- Schema: [Calificacion]
-- =============================================================================
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Calificacion')
    EXEC('CREATE SCHEMA [Calificacion]');
GO

-- =============================================================================
-- Tabla: [Calificacion].[CalificacionesColaborador]
-- =============================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Calificacion].[CalificacionesColaborador]'))
BEGIN
    CREATE TABLE [Calificacion].[CalificacionesColaborador]
    (
        [Id]                UNIQUEIDENTIFIER    NOT NULL,
        [SolicitudId]       UNIQUEIDENTIFIER    NOT NULL,
        [ClienteId]         UNIQUEIDENTIFIER    NOT NULL,
        [ColaboradorId]     UNIQUEIDENTIFIER    NOT NULL,
        [Puntaje]           INT                 NOT NULL,
        [Observacion]       NVARCHAR(1000)      NULL,
        [FechaCalificacion] DATETIMEOFFSET      NOT NULL,

        CONSTRAINT [PK_CalificacionesColaborador] PRIMARY KEY CLUSTERED ([Id] ASC),

        CONSTRAINT [CK_CalificacionesColaborador_Puntaje]
            CHECK ([Puntaje] BETWEEN 1 AND 5),

        CONSTRAINT [UQ_CalificacionesColaborador_Solicitud]
            UNIQUE ([SolicitudId]),

        CONSTRAINT [FK_CalificacionesColaborador_SolicitudesAcademicas]
            FOREIGN KEY ([SolicitudId])
            REFERENCES [Academia].[SolicitudesAcademicas] ([Id]),

        CONSTRAINT [FK_CalificacionesColaborador_PerfilesCliente]
            FOREIGN KEY ([ClienteId])
            REFERENCES [Perfil].[PerfilesCliente] ([Id]),

        CONSTRAINT [FK_CalificacionesColaborador_PerfilesColaborador]
            FOREIGN KEY ([ColaboradorId])
            REFERENCES [Perfil].[PerfilesColaboradores] ([Id])
    );
END
GO

-- =============================================================================
-- Indices: [Calificacion].[CalificacionesColaborador]
-- =============================================================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CalificacionesColaborador_ClienteId' AND object_id = OBJECT_ID(N'[Calificacion].[CalificacionesColaborador]'))
    CREATE NONCLUSTERED INDEX [IX_CalificacionesColaborador_ClienteId]
        ON [Calificacion].[CalificacionesColaborador] ([ClienteId] ASC);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CalificacionesColaborador_ColaboradorId' AND object_id = OBJECT_ID(N'[Calificacion].[CalificacionesColaborador]'))
    CREATE NONCLUSTERED INDEX [IX_CalificacionesColaborador_ColaboradorId]
        ON [Calificacion].[CalificacionesColaborador] ([ColaboradorId] ASC);
GO

-- =============================================================================
-- MIGRACION: Si la tabla ya existe con FKs a Usuarios, corregir a PerfilCliente/Colaborador
-- =============================================================================
IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Calificacion].[CalificacionesColaborador]'))
BEGIN
    -- Eliminar FK viejas si existen (FK a Usuarios)
    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_CalificacionesColaborador_Usuarios_Cliente' AND parent_object_id = OBJECT_ID(N'[Calificacion].[CalificacionesColaborador]'))
        ALTER TABLE [Calificacion].[CalificacionesColaborador] DROP CONSTRAINT [FK_CalificacionesColaborador_Usuarios_Cliente];

    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_CalificacionesColaborador_Usuarios_Colaborador' AND parent_object_id = OBJECT_ID(N'[Calificacion].[CalificacionesColaborador]'))
        ALTER TABLE [Calificacion].[CalificacionesColaborador] DROP CONSTRAINT [FK_CalificacionesColaborador_Usuarios_Colaborador];

    -- Crear FKs nuevas si no existen
    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_CalificacionesColaborador_PerfilesCliente' AND parent_object_id = OBJECT_ID(N'[Calificacion].[CalificacionesColaborador]'))
        ALTER TABLE [Calificacion].[CalificacionesColaborador] ADD CONSTRAINT [FK_CalificacionesColaborador_PerfilesCliente]
            FOREIGN KEY ([ClienteId]) REFERENCES [Perfil].[PerfilesCliente] ([Id]);

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_CalificacionesColaborador_PerfilesColaborador' AND parent_object_id = OBJECT_ID(N'[Calificacion].[CalificacionesColaborador]'))
        ALTER TABLE [Calificacion].[CalificacionesColaborador] ADD CONSTRAINT [FK_CalificacionesColaborador_PerfilesColaborador]
            FOREIGN KEY ([ColaboradorId]) REFERENCES [Perfil].[PerfilesColaboradores] ([Id]);
END
GO