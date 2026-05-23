USE [GrupoXpert];
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Academia].[Postulaciones]'))
BEGIN
    CREATE TABLE [Academia].[Postulaciones] (
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [SolicitudId] UNIQUEIDENTIFIER NOT NULL,
        [ColaboradorId] UNIQUEIDENTIFIER NOT NULL,
        [FechaPostulacion] DATETIME2 NOT NULL,
        [Estado] INT NOT NULL,
        CONSTRAINT [PK_Postulaciones] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Postulaciones_SolicitudesAcademicas] FOREIGN KEY ([SolicitudId]) REFERENCES [Academia].[SolicitudesAcademicas] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Postulaciones_PerfilesColaboradores] FOREIGN KEY ([ColaboradorId]) REFERENCES [Perfil].[PerfilesColaboradores] ([Id])
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Academia].[Postulaciones]') AND name = N'IX_Postulaciones_SolicitudId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Postulaciones_SolicitudId] ON [Academia].[Postulaciones] ([SolicitudId] ASC);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Academia].[Postulaciones]') AND name = N'IX_Postulaciones_ColaboradorId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Postulaciones_ColaboradorId] ON [Academia].[Postulaciones] ([ColaboradorId] ASC);
END
GO
