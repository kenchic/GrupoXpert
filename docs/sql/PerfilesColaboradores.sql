USE [GrupoXpert];
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- 1. Crear esquema si no existe
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Perfil')
BEGIN
    EXEC('CREATE SCHEMA [Perfil]');
END
GO

-- 2. Tabla Principal: PerfilesColaboradores
IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Perfil].[PerfilesColaboradores]'))
BEGIN
    CREATE TABLE [Perfil].[PerfilesColaboradores] (
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [UsuarioId] UNIQUEIDENTIFIER NOT NULL,
        [ValidadoPorAdmin] BIT NOT NULL DEFAULT 0,
        [NivelAcademico] INT NOT NULL,
        [DisponibilidadHorasSemana] INT NOT NULL,
        [CargaAcademicaIdeal] INT NOT NULL,
        [EvaluacionCalidad] DECIMAL(3,1) NOT NULL DEFAULT 0.0,
        CONSTRAINT [PK_PerfilesColaboradores] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    -- Índice único para UsuarioId
    CREATE UNIQUE NONCLUSTERED INDEX [IX_PerfilesColaboradores_UsuarioId] 
    ON [Perfil].[PerfilesColaboradores] ([UsuarioId] ASC);
END
GO

-- 3. Tabla Colección: AreasConocimiento
IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Perfil].[PerfilesColaboradores_AreasConocimiento]'))
BEGIN
    CREATE TABLE [Perfil].[PerfilesColaboradores_AreasConocimiento] (
        [PerfilColaboradorId] UNIQUEIDENTIFIER NOT NULL,
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Area] NVARCHAR(100) NOT NULL,
        CONSTRAINT [PK_PerfilesColaboradores_AreasConocimiento] PRIMARY KEY CLUSTERED ([PerfilColaboradorId] ASC, [Id] ASC),
        CONSTRAINT [FK_PerfilesColaboradores_AreasConocimiento_PerfilColaboradorId] FOREIGN KEY ([PerfilColaboradorId]) REFERENCES [Perfil].[PerfilesColaboradores] ([Id]) ON DELETE CASCADE
    );
END
GO

-- 4. Tabla Colección: TiposTrabajo
IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Perfil].[PerfilesColaboradores_TiposTrabajo]'))
BEGIN
    CREATE TABLE [Perfil].[PerfilesColaboradores_TiposTrabajo] (
        [PerfilColaboradorId] UNIQUEIDENTIFIER NOT NULL,
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Tipo] NVARCHAR(100) NOT NULL,
        CONSTRAINT [PK_PerfilesColaboradores_TiposTrabajo] PRIMARY KEY CLUSTERED ([PerfilColaboradorId] ASC, [Id] ASC),
        CONSTRAINT [FK_PerfilesColaboradores_TiposTrabajo_PerfilColaboradorId] FOREIGN KEY ([PerfilColaboradorId]) REFERENCES [Perfil].[PerfilesColaboradores] ([Id]) ON DELETE CASCADE
    );
END
GO

-- 5. Tabla Colección: Idiomas
IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Perfil].[PerfilesColaboradores_Idiomas]'))
BEGIN
    CREATE TABLE [Perfil].[PerfilesColaboradores_Idiomas] (
        [PerfilColaboradorId] UNIQUEIDENTIFIER NOT NULL,
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Nombre] NVARCHAR(50) NOT NULL,
        CONSTRAINT [PK_PerfilesColaboradores_Idiomas] PRIMARY KEY CLUSTERED ([PerfilColaboradorId] ASC, [Id] ASC),
        CONSTRAINT [FK_PerfilesColaboradores_Idiomas_PerfilColaboradorId] FOREIGN KEY ([PerfilColaboradorId]) REFERENCES [Perfil].[PerfilesColaboradores] ([Id]) ON DELETE CASCADE
    );
END
GO

-- 6. Tabla Colección: NormasCitacion
IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[Perfil].[PerfilesColaboradores_NormasCitacion]'))
BEGIN
    CREATE TABLE [Perfil].[PerfilesColaboradores_NormasCitacion] (
        [PerfilColaboradorId] UNIQUEIDENTIFIER NOT NULL,
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Norma] NVARCHAR(50) NOT NULL,
        CONSTRAINT [PK_PerfilesColaboradores_NormasCitacion] PRIMARY KEY CLUSTERED ([PerfilColaboradorId] ASC, [Id] ASC),
        CONSTRAINT [FK_PerfilesColaboradores_NormasCitacion_PerfilColaboradorId] FOREIGN KEY ([PerfilColaboradorId]) REFERENCES [Perfil].[PerfilesColaboradores] ([Id]) ON DELETE CASCADE
    );
END
GO
