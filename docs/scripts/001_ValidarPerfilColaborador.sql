-- =============================================
-- Script: 001_ValidarPerfilColaborador.sql
-- Descripción: Agrega la columna EstadoVerificacion a la tabla Usuarios
-- Contexto: IDENTIDAD (GrupoXpert)
-- Date: 2026-05-07
-- =============================================

USE GrupoXpert;
GO

-- 1. Crear el tipo enum como un CHECK CONSTRAINT o usar un TINYINT
-- Estados: 1=Pendiente, 2=EnRevision, 3=Aprobado, 4=Rechazado

-- 2. Agregar columna EstadoVerificacion a la tabla Usuarios
IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID('Usuarios') 
    AND name = 'EstadoVerificacion'
)
BEGIN
    ALTER TABLE Usuarios
    ADD EstadoVerificacion TINYINT NOT NULL 
    CONSTRAINT DF_Usuarios_EstadoVerificacion DEFAULT 1; -- 1 = Pendiente por defecto

    -- Agregar constraint para validar valores permitidos
    ALTER TABLE Usuarios
    ADD CONSTRAINT CK_Usuarios_EstadoVerificacion 
    CHECK (EstadoVerificacion IN (1, 2, 3, 4));

    PRINT 'Columna EstadoVerificacion agregada exitosamente a la tabla Usuarios.';
END
ELSE
BEGIN
    PRINT 'La columna EstadoVerificacion ya existe en la tabla Usuarios.';
END
GO

-- 3. Actualizar registros existentes basado en el tipo de usuario
-- Los Asesores (Tipo = 2) que no estén aprobados quedan como Pendientes (1)
-- Los Asesores aprobados quedan como Aprobados (3)
-- Estudiantes y Administradores quedan como Aprobados (3) automáticamente
UPDATE Usuarios
SET EstadoVerificacion = 
    CASE 
        WHEN Tipo = 2 AND EstaAprobado = 0 THEN 1 -- Pendiente
        WHEN Tipo = 2 AND EstaAprobado = 1 THEN 3 -- Aprobado
        ELSE 3 -- Aprobado (Estudiantes y Administradores)
    END
WHERE EstadoVerificacion IS NULL OR EstadoVerificacion = 0;
GO

-- 4. Crear índice para mejorar consultas por EstadoVerificacion
IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'IX_Usuarios_EstadoVerificacion'
)
BEGIN
    CREATE INDEX IX_Usuarios_EstadoVerificacion 
    ON Usuarios(EstadoVerificacion) 
    INCLUDE (Tipo, EstaAprobado, FechaCreacion);
    
    PRINT 'Índice IX_Usuarios_EstadoVerificacion creado exitosamente.';
END
GO

-- 5. Verificación de datos
SELECT 
    Id,
    Nombre,
    Correo,
    Tipo,
    EstaAprobado,
    EstadoVerificacion,
    CASE EstadoVerificacion
        WHEN 1 THEN 'Pendiente'
        WHEN 2 THEN 'En Revisión'
        WHEN 3 THEN 'Aprobado'
        WHEN 4 THEN 'Rechazado'
    END AS EstadoVerificacionTexto
FROM Usuarios
WHERE Tipo = 2 -- Solo Asesores
ORDER BY FechaCreacion DESC;
GO

PRINT 'Script 001_ValidarPerfilColaborador.sql ejecutado exitosamente.';
GO
