USE [GrupoXpert];
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- SCHEMAS
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Identidad')
    EXEC('CREATE SCHEMA [Identidad]');
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Perfil')
    EXEC('CREATE SCHEMA [Perfil]');
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Academia')
    EXEC('CREATE SCHEMA [Academia]');
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Calidad')
    EXEC('CREATE SCHEMA [Calidad]');
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Calificacion')
    EXEC('CREATE SCHEMA [Calificacion]');
GO

-- =============================================
-- TABLE: Identidad.Usuarios
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Identidad].[Usuarios]'))
BEGIN
    CREATE TABLE [Identidad].[Usuarios] (
        [Id]                    UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
        [Correo]               NVARCHAR(256)       NOT NULL,
        [ClaveHash]            NVARCHAR(500)       NOT NULL,
        [Nombre]               NVARCHAR(150)       NOT NULL,
        [Imagen]              NVARCHAR(500)       NULL,
        [Tipo]                INT                  NOT NULL DEFAULT 1,
        [EstaActivo]           BIT                  NOT NULL DEFAULT 0,
        [TokenActivacion]      NVARCHAR(200)       NULL,
        [TokenActivacionExpira] DATETIMEOFFSET     NULL,
        [FechaCreacion]        DATETIMEOFFSET       NOT NULL DEFAULT SYSDATETIMEOFFSET(),
        [UltimoInicioSesion]   DATETIMEOFFSET       NULL,
        [EstaAprobado]          BIT                  NOT NULL DEFAULT 0,
        [FechaAprobacion]      DATETIMEOFFSET       NULL,
        [AprobadoPorId]        UNIQUEIDENTIFIER    NULL,
        [EstadoVerificacion]   INT                  NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Identidad_Usuarios] PRIMARY KEY ([Id])
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Identidad_Usuarios_Correo' AND object_id = OBJECT_ID(N'[Identidad].[Usuarios]'))
    CREATE UNIQUE INDEX [IX_Identidad_Usuarios_Correo] ON [Identidad].[Usuarios] ([Correo]);
GO

-- =============================================
-- TABLE: Perfil.PerfilesCliente
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Perfil].[PerfilesCliente]'))
BEGIN
    CREATE TABLE [Perfil].[PerfilesCliente] (
        [Id]                    UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
        [UsuarioId]            UNIQUEIDENTIFIER    NOT NULL,
        CONSTRAINT [PK_Perfil_PerfilesCliente] PRIMARY KEY ([Id])
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Perfil_PerfilesCliente_UsuarioId' AND object_id = OBJECT_ID(N'[Perfil].[PerfilesCliente]'))
    CREATE UNIQUE INDEX [IX_Perfil_PerfilesCliente_UsuarioId] ON [Perfil].[PerfilesCliente] ([UsuarioId]);
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Perfil_PerfilesCliente_Identidad_Usuarios_UsuarioId')
    ALTER TABLE [Perfil].[PerfilesCliente] WITH NOCHECK
        ADD CONSTRAINT [FK_Perfil_PerfilesCliente_Identidad_Usuarios_UsuarioId]
        FOREIGN KEY ([UsuarioId]) REFERENCES [Identidad].[Usuarios] ([Id]) ON DELETE CASCADE;
GO

-- =============================================
-- TABLE: Perfil.PerfilesColaboradores
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Perfil].[PerfilesColaboradores]'))
BEGIN
    CREATE TABLE [Perfil].[PerfilesColaboradores] (
        [Id]                            UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
        [UsuarioId]                    UNIQUEIDENTIFIER    NOT NULL,
        [ValidadoPorAdmin]             BIT                  NOT NULL DEFAULT 0,
        [NivelAcademico]               INT                  NOT NULL DEFAULT 0,
        [DisponibilidadHorasSemana]    INT                  NOT NULL DEFAULT 0,
        [CargaAcademicaIdeal]          INT                  NOT NULL DEFAULT 0,
        [EvaluacionCalidad]            DECIMAL(3,1)         NOT NULL DEFAULT 0.0,
        [PuntajePromedioReputacion]    DECIMAL(5,2)         NOT NULL DEFAULT 0.00,
        [TotalCalificacionesReputacion] INT                 NOT NULL DEFAULT 0,
        [NivelReputacion]              INT                  NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Perfil_PerfilesColaboradores] PRIMARY KEY ([Id])
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Perfil_PerfilesColaboradores_UsuarioId' AND object_id = OBJECT_ID(N'[Perfil].[PerfilesColaboradores]'))
    CREATE UNIQUE INDEX [IX_Perfil_PerfilesColaboradores_UsuarioId] ON [Perfil].[PerfilesColaboradores] ([UsuarioId]);
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Perfil_PerfilesColaboradores_Identidad_Usuarios_UsuarioId')
    ALTER TABLE [Perfil].[PerfilesColaboradores] WITH NOCHECK
        ADD CONSTRAINT [FK_Perfil_PerfilesColaboradores_Identidad_Usuarios_UsuarioId]
        FOREIGN KEY ([UsuarioId]) REFERENCES [Identidad].[Usuarios] ([Id]) ON DELETE CASCADE;
GO

-- =============================================
-- TABLE: Perfil.PerfilesColaboradores_AreasConocimiento
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Perfil].[PerfilesColaboradores_AreasConocimiento]'))
BEGIN
    CREATE TABLE [Perfil].[PerfilesColaboradores_AreasConocimiento] (
        [PerfilColaboradorId]  UNIQUEIDENTIFIER    NOT NULL,
        [Id]                   INT IDENTITY(1,1)   NOT NULL,
        [Area]                NVARCHAR(100)       NOT NULL,
        CONSTRAINT [PK_Perfil_AreasConocimiento] PRIMARY KEY ([PerfilColaboradorId], [Id])
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Perfil_AreasConocimiento_PerfilesColaboradores')
    ALTER TABLE [Perfil].[PerfilesColaboradores_AreasConocimiento] WITH NOCHECK
        ADD CONSTRAINT [FK_Perfil_AreasConocimiento_PerfilesColaboradores]
        FOREIGN KEY ([PerfilColaboradorId]) REFERENCES [Perfil].[PerfilesColaboradores] ([Id]) ON DELETE CASCADE;
GO

-- =============================================
-- TABLE: Perfil.PerfilesColaboradores_TiposTrabajo
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Perfil].[PerfilesColaboradores_TiposTrabajo]'))
BEGIN
    CREATE TABLE [Perfil].[PerfilesColaboradores_TiposTrabajo] (
        [PerfilColaboradorId]  UNIQUEIDENTIFIER    NOT NULL,
        [Id]                   INT IDENTITY(1,1)   NOT NULL,
        [Tipo]                NVARCHAR(100)       NOT NULL,
        CONSTRAINT [PK_Perfil_TiposTrabajo] PRIMARY KEY ([PerfilColaboradorId], [Id])
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Perfil_TiposTrabajo_PerfilesColaboradores')
    ALTER TABLE [Perfil].[PerfilesColaboradores_TiposTrabajo] WITH NOCHECK
        ADD CONSTRAINT [FK_Perfil_TiposTrabajo_PerfilesColaboradores]
        FOREIGN KEY ([PerfilColaboradorId]) REFERENCES [Perfil].[PerfilesColaboradores] ([Id]) ON DELETE CASCADE;
GO

-- =============================================
-- TABLE: Perfil.PerfilesColaboradores_Idiomas
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Perfil].[PerfilesColaboradores_Idiomas]'))
BEGIN
    CREATE TABLE [Perfil].[PerfilesColaboradores_Idiomas] (
        [PerfilColaboradorId]  UNIQUEIDENTIFIER    NOT NULL,
        [Id]                   INT IDENTITY(1,1)   NOT NULL,
        [Nombre]             NVARCHAR(50)        NOT NULL,
        CONSTRAINT [PK_Perfil_Idiomas] PRIMARY KEY ([PerfilColaboradorId], [Id])
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Perfil_Idiomas_PerfilesColaboradores')
    ALTER TABLE [Perfil].[PerfilesColaboradores_Idiomas] WITH NOCHECK
        ADD CONSTRAINT [FK_Perfil_Idiomas_PerfilesColaboradores]
        FOREIGN KEY ([PerfilColaboradorId]) REFERENCES [Perfil].[PerfilesColaboradores] ([Id]) ON DELETE CASCADE;
GO

-- =============================================
-- TABLE: Perfil.PerfilesColaboradores_NormasCitacion
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Perfil].[PerfilesColaboradores_NormasCitacion]'))
BEGIN
    CREATE TABLE [Perfil].[PerfilesColaboradores_NormasCitacion] (
        [PerfilColaboradorId]  UNIQUEIDENTIFIER    NOT NULL,
        [Id]                   INT IDENTITY(1,1)   NOT NULL,
        [Norma]              NVARCHAR(50)        NOT NULL,
        CONSTRAINT [PK_Perfil_NormasCitacion] PRIMARY KEY ([PerfilColaboradorId], [Id])
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Perfil_NormasCitacion_PerfilesColaboradores')
    ALTER TABLE [Perfil].[PerfilesColaboradores_NormasCitacion] WITH NOCHECK
        ADD CONSTRAINT [FK_Perfil_NormasCitacion_PerfilesColaboradores]
        FOREIGN KEY ([PerfilColaboradorId]) REFERENCES [Perfil].[PerfilesColaboradores] ([Id]) ON DELETE CASCADE;
GO

-- =============================================
-- TABLE: Academia.SolicitudesAcademicas
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Academia].[SolicitudesAcademicas]'))
BEGIN
    CREATE TABLE [Academia].[SolicitudesAcademicas] (
        [Id]                       UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
        [ClienteId]               UNIQUEIDENTIFIER    NOT NULL,
        [NivelAcademico]          INT                  NOT NULL DEFAULT 0,
        [TipoTrabajo]            INT                  NOT NULL DEFAULT 0,
        [AreaTematica]           NVARCHAR(200)       NOT NULL,
        [FechaEntrega]           DATETIMEOFFSET       NOT NULL,
        [NumeroPaginasOPalabras]  INT                  NOT NULL,
        [NormaCitacion]          INT                  NOT NULL DEFAULT 0,
        [Idioma]                 INT                  NOT NULL DEFAULT 0,
        [FormatoRequerido]       NVARCHAR(500)       NOT NULL,
        [MaterialBase]           NVARCHAR(MAX)       NOT NULL,
        [EsUrgente]              BIT                  NOT NULL DEFAULT 0,
        [EntregaPorFases]        BIT                  NOT NULL DEFAULT 0,
        [Estado]                 INT                  NOT NULL DEFAULT 1,
        [AsesorId]               UNIQUEIDENTIFIER    NULL,
        CONSTRAINT [PK_Academia_SolicitudesAcademicas] PRIMARY KEY ([Id])
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Academia_SolicitudesAcademicas_ClienteId' AND object_id = OBJECT_ID(N'[Academia].[SolicitudesAcademicas]'))
    CREATE INDEX [IX_Academia_SolicitudesAcademicas_ClienteId] ON [Academia].[SolicitudesAcademicas] ([ClienteId]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Academia_SolicitudesAcademicas_AsesorId_Estado' AND object_id = OBJECT_ID(N'[Academia].[SolicitudesAcademicas]'))
    CREATE INDEX [IX_Academia_SolicitudesAcademicas_AsesorId_Estado] ON [Academia].[SolicitudesAcademicas] ([AsesorId], [Estado]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Academia_SolicitudesAcademicas_Estado' AND object_id = OBJECT_ID(N'[Academia].[SolicitudesAcademicas]'))
    CREATE INDEX [IX_Academia_SolicitudesAcademicas_Estado] ON [Academia].[SolicitudesAcademicas] ([Estado]);
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Academia_SolicitudesAcademicas_Perfil_PerfilesColaboradores_AsesorId')
    ALTER TABLE [Academia].[SolicitudesAcademicas] WITH NOCHECK
        ADD CONSTRAINT [FK_Academia_SolicitudesAcademicas_Perfil_PerfilesColaboradores_AsesorId]
        FOREIGN KEY ([AsesorId]) REFERENCES [Perfil].[PerfilesColaboradores] ([Id]) ON DELETE NO ACTION;
GO

-- =============================================
-- TABLE: Academia.Postulaciones
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Academia].[Postulaciones]'))
BEGIN
    CREATE TABLE [Academia].[Postulaciones] (
        [Id]                UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
        [SolicitudId]       UNIQUEIDENTIFIER    NOT NULL,
        [ColaboradorId]     UNIQUEIDENTIFIER    NOT NULL,
        [FechaPostulacion]  DATETIMEOFFSET       NOT NULL DEFAULT SYSDATETIMEOFFSET(),
        [Estado]            INT                  NOT NULL DEFAULT 1,
        CONSTRAINT [PK_Academia_Postulaciones] PRIMARY KEY ([Id])
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Academia_Postulaciones_SolicitudId' AND object_id = OBJECT_ID(N'[Academia].[Postulaciones]'))
    CREATE INDEX [IX_Academia_Postulaciones_SolicitudId] ON [Academia].[Postulaciones] ([SolicitudId]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Academia_Postulaciones_ColaboradorId_Estado' AND object_id = OBJECT_ID(N'[Academia].[Postulaciones]'))
    CREATE INDEX [IX_Academia_Postulaciones_ColaboradorId_Estado] ON [Academia].[Postulaciones] ([ColaboradorId], [Estado]);
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Academia_Postulaciones_Academia_SolicitudesAcademicas_SolicitudId')
    ALTER TABLE [Academia].[Postulaciones] WITH NOCHECK
        ADD CONSTRAINT [FK_Academia_Postulaciones_Academia_SolicitudesAcademicas_SolicitudId]
        FOREIGN KEY ([SolicitudId]) REFERENCES [Academia].[SolicitudesAcademicas] ([Id]) ON DELETE CASCADE;
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Academia_Postulaciones_Perfil_PerfilesColaboradores_ColaboradorId')
    ALTER TABLE [Academia].[Postulaciones] WITH NOCHECK
        ADD CONSTRAINT [FK_Academia_Postulaciones_Perfil_PerfilesColaboradores_ColaboradorId]
        FOREIGN KEY ([ColaboradorId]) REFERENCES [Perfil].[PerfilesColaboradores] ([Id]) ON DELETE NO ACTION;
GO

-- =============================================
-- TABLE: Academia.Avances
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Academia].[Avances]'))
BEGIN
    CREATE TABLE [Academia].[Avances] (
        [Id]                UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
        [SolicitudId]       UNIQUEIDENTIFIER    NOT NULL,
        [AsesorId]          UNIQUEIDENTIFIER    NOT NULL,
        [Descripcion]       NVARCHAR(2000)      NOT NULL,
        [NumeroFase]        INT                  NOT NULL,
        [Tipo]              INT                  NOT NULL DEFAULT 1,
        [Estado]            INT                  NOT NULL DEFAULT 1,
        [FechaSubida]       DATETIMEOFFSET       NOT NULL DEFAULT SYSDATETIMEOFFSET(),
        CONSTRAINT [PK_Academia_Avances] PRIMARY KEY ([Id])
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Academia_Avances_SolicitudId' AND object_id = OBJECT_ID(N'[Academia].[Avances]'))
    CREATE INDEX [IX_Academia_Avances_SolicitudId] ON [Academia].[Avances] ([SolicitudId]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Academia_Avances_AsesorId' AND object_id = OBJECT_ID(N'[Academia].[Avances]'))
    CREATE INDEX [IX_Academia_Avances_AsesorId] ON [Academia].[Avances] ([AsesorId]);
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Academia_Avances_Academia_SolicitudesAcademicas_SolicitudId')
    ALTER TABLE [Academia].[Avances] WITH NOCHECK
        ADD CONSTRAINT [FK_Academia_Avances_Academia_SolicitudesAcademicas_SolicitudId]
        FOREIGN KEY ([SolicitudId]) REFERENCES [Academia].[SolicitudesAcademicas] ([Id]) ON DELETE NO ACTION;
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Academia_Avances_Perfil_PerfilesColaboradores_AsesorId')
    ALTER TABLE [Academia].[Avances] WITH NOCHECK
        ADD CONSTRAINT [FK_Academia_Avances_Perfil_PerfilesColaboradores_AsesorId]
        FOREIGN KEY ([AsesorId]) REFERENCES [Perfil].[PerfilesColaboradores] ([Id]) ON DELETE NO ACTION;
GO

-- =============================================
-- TABLE: Academia.Comentarios
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Academia].[Comentarios]'))
BEGIN
    CREATE TABLE [Academia].[Comentarios] (
        [Id]            UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
        [AvanceId]      UNIQUEIDENTIFIER    NOT NULL,
        [AutorId]       UNIQUEIDENTIFIER    NOT NULL,
        [Contenido]    NVARCHAR(2000)      NOT NULL,
        [FechaCreacion] DATETIMEOFFSET       NOT NULL DEFAULT SYSDATETIMEOFFSET(),
        CONSTRAINT [PK_Academia_Comentarios] PRIMARY KEY ([Id])
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Academia_Comentarios_AvanceId' AND object_id = OBJECT_ID(N'[Academia].[Comentarios]'))
    CREATE INDEX [IX_Academia_Comentarios_AvanceId] ON [Academia].[Comentarios] ([AvanceId]);
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Academia_Comentarios_Academia_Avances_AvanceId')
    ALTER TABLE [Academia].[Comentarios] WITH NOCHECK
        ADD CONSTRAINT [FK_Academia_Comentarios_Academia_Avances_AvanceId]
        FOREIGN KEY ([AvanceId]) REFERENCES [Academia].[Avances] ([Id]) ON DELETE CASCADE;
GO

-- =============================================
-- TABLE: Academia.ArchivosAdjuntos
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Academia].[ArchivosAdjuntos]'))
BEGIN
    CREATE TABLE [Academia].[ArchivosAdjuntos] (
        [Id]              UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
        [AvanceId]        UNIQUEIDENTIFIER    NOT NULL,
        [NombreArchivo]   NVARCHAR(200)       NOT NULL,
        [Url]            NVARCHAR(500)       NOT NULL,
        [TamanioBytes]    BIGINT               NOT NULL,
        [TipoContenido]   NVARCHAR(100)       NOT NULL,
        [FechaSubida]     DATETIMEOFFSET       NOT NULL DEFAULT SYSDATETIMEOFFSET(),
        CONSTRAINT [PK_Academia_ArchivosAdjuntos] PRIMARY KEY ([Id])
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Academia_ArchivosAdjuntos_AvanceId' AND object_id = OBJECT_ID(N'[Academia].[ArchivosAdjuntos]'))
    CREATE INDEX [IX_Academia_ArchivosAdjuntos_AvanceId] ON [Academia].[ArchivosAdjuntos] ([AvanceId]);
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Academia_ArchivosAdjuntos_Academia_Avances_AvanceId')
    ALTER TABLE [Academia].[ArchivosAdjuntos] WITH NOCHECK
        ADD CONSTRAINT [FK_Academia_ArchivosAdjuntos_Academia_Avances_AvanceId]
        FOREIGN KEY ([AvanceId]) REFERENCES [Academia].[Avances] ([Id]) ON DELETE CASCADE;
GO

-- =============================================
-- TABLE: Calidad.RevisionesCalidad
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Calidad].[RevisionesCalidad]'))
BEGIN
    CREATE TABLE [Calidad].[RevisionesCalidad] (
        [Id]                UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
        [AvanceId]          UNIQUEIDENTIFIER    NOT NULL,
        [RevisorId]         UNIQUEIDENTIFIER    NOT NULL,
        [FechaRevision]     DATETIMEOFFSET       NOT NULL DEFAULT SYSDATETIMEOFFSET(),
        [VistoBueno]        BIT                  NOT NULL DEFAULT 0,
        [Observaciones]     NVARCHAR(2000)      NULL,
        [Estado]            INT                  NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Calidad_RevisionesCalidad] PRIMARY KEY ([Id])
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Calidad_RevisionesCalidad_AvanceId' AND object_id = OBJECT_ID(N'[Calidad].[RevisionesCalidad]'))
    CREATE INDEX [IX_Calidad_RevisionesCalidad_AvanceId] ON [Calidad].[RevisionesCalidad] ([AvanceId]);
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Calidad_RevisionesCalidad_Academia_Avances_AvanceId')
    ALTER TABLE [Calidad].[RevisionesCalidad] WITH NOCHECK
        ADD CONSTRAINT [FK_Calidad_RevisionesCalidad_Academia_Avances_AvanceId]
        FOREIGN KEY ([AvanceId]) REFERENCES [Academia].[Avances] ([Id]) ON DELETE NO ACTION;
GO

-- =============================================
-- TABLE: Calificacion.CalificacionesColaborador
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Calificacion].[CalificacionesColaborador]'))
BEGIN
    CREATE TABLE [Calificacion].[CalificacionesColaborador] (
        [Id]                 UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
        [SolicitudId]        UNIQUEIDENTIFIER    NOT NULL,
        [ClienteId]          UNIQUEIDENTIFIER    NOT NULL,
        [ColaboradorId]      UNIQUEIDENTIFIER    NOT NULL,
        [Puntaje]            INT                  NOT NULL,
        [Observacion]        NVARCHAR(1000)      NULL,
        [FechaCalificacion]  DATETIMEOFFSET       NOT NULL DEFAULT SYSDATETIMEOFFSET(),
        CONSTRAINT [PK_Calificacion_CalificacionesColaborador] PRIMARY KEY ([Id])
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Calificacion_CalificacionesColaborador_SolicitudId' AND object_id = OBJECT_ID(N'[Calificacion].[CalificacionesColaborador]'))
    CREATE UNIQUE INDEX [IX_Calificacion_CalificacionesColaborador_SolicitudId] ON [Calificacion].[CalificacionesColaborador] ([SolicitudId]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Calificacion_CalificacionesColaborador_ColaboradorId' AND object_id = OBJECT_ID(N'[Calificacion].[CalificacionesColaborador]'))
    CREATE INDEX [IX_Calificacion_CalificacionesColaborador_ColaboradorId] ON [Calificacion].[CalificacionesColaborador] ([ColaboradorId]);
GO

-- =============================================
-- APPLICATION USER: grupoxpert
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'grupoxpert')
BEGIN
    CREATE LOGIN [grupoxpert] WITH PASSWORD = 'Admin123*', DEFAULT_DATABASE = [GrupoXpert], CHECK_POLICY = OFF;
END
GO

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'grupoxpert')
BEGIN
    CREATE USER [grupoxpert] FOR LOGIN [grupoxpert];
END
GO

ALTER ROLE db_owner ADD MEMBER [grupoxpert];
GO