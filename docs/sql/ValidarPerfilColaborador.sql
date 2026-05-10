USE [GrupoXpert];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- 1. Agregar columna EstaAprobado
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Identidad].[Usuarios]') AND name = 'EstaAprobado')
BEGIN
    ALTER TABLE [Identidad].[Usuarios] ADD [EstaAprobado] BIT NOT NULL CONSTRAINT DF_Usuarios_EstaAprobado DEFAULT 0;
    PRINT 'Columna EstaAprobado agregada.';
END
GO

-- 2. Agregar columna FechaAprobacion
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Identidad].[Usuarios]') AND name = 'FechaAprobacion')
BEGIN
    ALTER TABLE [Identidad].[Usuarios] ADD [FechaAprobacion] DATETIMEOFFSET NULL;
    PRINT 'Columna FechaAprobacion agregada.';
END
GO

-- 3. Agregar columna AprobadoPorId con FK
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Identidad].[Usuarios]') AND name = 'AprobadoPorId')
BEGIN
    ALTER TABLE [Identidad].[Usuarios] ADD [AprobadoPorId] UNIQUEIDENTIFIER NULL;
    
    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Usuarios_AprobadoPor')
    BEGIN
        ALTER TABLE [Identidad].[Usuarios] ADD CONSTRAINT FK_Usuarios_AprobadoPor 
            FOREIGN KEY ([AprobadoPorId]) REFERENCES [Identidad].[Usuarios]([Id]);
    END
    PRINT 'Columna AprobadoPorId con FK agregada.';
END
GO

-- 4. Actualizar registros existentes: Estudiantes se auto-aprueban
-- Nota: La columna Tipo es NVARCHAR en este ambiente.
UPDATE [Identidad].[Usuarios] SET [EstaAprobado] = 1 WHERE [Tipo] = 'Estudiante';
GO

-- 5. Índice filtrado para consultas de asesores pendientes
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Usuarios_Tipo_EstaAprobado' AND object_id = OBJECT_ID(N'[Identidad].[Usuarios]'))
BEGIN
    CREATE INDEX [IX_Usuarios_Tipo_EstaAprobado] 
        ON [Identidad].[Usuarios] ([Tipo], [EstaAprobado]) 
        INCLUDE ([Nombre], [Correo], [FechaCreacion]);
    PRINT 'Índice IX_Usuarios_Tipo_EstaAprobado creado.';
END
GO

-- 6. Insertar usuario Administrador semilla (si no existe)
IF NOT EXISTS (SELECT 1 FROM [Identidad].[Usuarios] WHERE [Tipo] = 'Administrador')
BEGIN
    INSERT INTO [Identidad].[Usuarios] (
        [Id], [Correo], [Nombre], [HashClave], [Imagen], [Tipo],
        [EstaActivo], [EstaAprobado], [FechaCreacion]
    ) VALUES (
        NEWID(), 'admin@grupoxpert.com', 'Administrador GrupoXpert',
        '$2a$11$placeholder_hash_cambiar', NULL, 'Administrador',
        1, 1, SYSDATETIMEOFFSET()
    );
    PRINT 'Usuario Administrador semilla insertado.';
END
GO

PRINT 'Migración Validar Perfil Colaborador completada.';
GO
