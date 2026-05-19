USE [GrupoXpert];
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Academia')
    EXEC('CREATE SCHEMA [Academia]');
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Academia].[SolicitudesAcademicas]'))
BEGIN
    CREATE TABLE [Academia].[SolicitudesAcademicas] (
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [ClienteId] UNIQUEIDENTIFIER NOT NULL,
        [NivelAcademico] INT NOT NULL,
        [TipoTrabajo] INT NOT NULL,
        [AreaTematica] NVARCHAR(200) NOT NULL,
        [FechaEntrega] DATETIMEOFFSET NOT NULL,
        [NumeroPaginasOPalabras] INT NOT NULL,
        [NormaCitacion] INT NOT NULL,
        [Idioma] INT NOT NULL,
        [FormatoRequerido] NVARCHAR(500) NOT NULL,
        [MaterialBase] NVARCHAR(MAX) NOT NULL,
        [EsUrgente] BIT NOT NULL,
        [EntregaPorFases] BIT NOT NULL,
        CONSTRAINT [PK_SolicitudesAcademicas] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_SolicitudesAcademicas_PerfilesCliente] FOREIGN KEY ([ClienteId]) REFERENCES [Perfil].[PerfilesCliente] ([Id])
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Academia].[SolicitudesAcademicas]') AND name = N'IX_SolicitudesAcademicas_FechaEntrega')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_SolicitudesAcademicas_FechaEntrega] ON [Academia].[SolicitudesAcademicas] ([FechaEntrega] ASC);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Academia].[SolicitudesAcademicas]') AND name = N'IX_SolicitudesAcademicas_AreaTematica')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_SolicitudesAcademicas_AreaTematica] ON [Academia].[SolicitudesAcademicas] ([AreaTematica] ASC);
END
GO
