USE [GrupoXpert];
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- ====================================================================================
-- CASO DE USO: Asignar Solicitud Manualmente (Rol Administrador)
-- SCRIPT DE VERIFICACIÓN Y CONSULTA DE LA ESTRUCTURA DE PERSISTENCIA
-- ====================================================================================

-- 1. Verificar que la columna Estado y AsesorId existen en Academia.SolicitudesAcademicas
IF EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[Academia].[SolicitudesAcademicas]') 
      AND name = N'AsesorId'
) AND EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[Academia].[SolicitudesAcademicas]') 
      AND name = N'Estado'
)
BEGIN
    PRINT '✅ Persistencia Preparada: Las columnas AsesorId y Estado existen correctamente en [Academia].[SolicitudesAcademicas].';
END
ELSE
BEGIN
    PRINT '❌ Error de Persistencia: Faltan columnas en [Academia].[SolicitudesAcademicas].';
END
GO

-- 2. Verificar que existe la Llave Foránea hacia la tabla de Colaboradores (Asesores)
IF EXISTS (
    SELECT 1 FROM sys.foreign_keys 
    WHERE name = N'FK_SolicitudesAcademicas_PerfilesColaboradores'
)
BEGIN
    PRINT '✅ Integridad Referencial Preparada: La llave foránea FK_SolicitudesAcademicas_PerfilesColaboradores existe correctamente.';
END
ELSE
BEGIN
    PRINT '❌ Error de Persistencia: No se encuentra la llave foránea hacia PerfilesColaboradores.';
END
GO

-- 3. Consulta de ejemplo para verificar solicitudes y sus asesores asignados
-- Esta consulta representa cómo se mapearán las solicitudes en los Dashboards del Cliente y Asesor
SELECT 
    s.Id AS SolicitudId,
    s.AreaTematica,
    s.Estado AS EstadoId,
    CASE s.Estado
        WHEN 1 THEN 'Pendiente'
        WHEN 2 THEN 'En Proceso'
        WHEN 3 THEN 'Completada'
        WHEN 4 THEN 'Cancelada'
        WHEN 5 THEN 'Asignada'
        ELSE 'Desconocido'
    END AS EstadoNombre,
    s.AsesorId,
    u.Nombre AS NombreAsesor
FROM [Academia].[SolicitudesAcademicas] s
LEFT JOIN [Perfil].[PerfilesColaboradores] c ON s.AsesorId = c.Id
LEFT JOIN [Identidad].[Usuarios] u ON c.UsuarioId = u.Id;
GO
