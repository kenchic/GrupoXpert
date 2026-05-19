USE [GrupoXpert];
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- ====================================================================================
-- CASO DE USO: Asignar Solicitud Manualmente (Rol Administrador)
-- SCRIPT DE SEEDING: Registrar Perfil de Colaborador para Usuario de Prueba
-- ====================================================================================

-- 1. Insertar PerfilColaborador
IF NOT EXISTS (
    SELECT 1 FROM [Perfil].[PerfilesColaboradores] 
    WHERE [UsuarioId] = '4E13D5B6-5997-439F-AF6D-F1FFF52023D1'
)
BEGIN
    INSERT INTO [Perfil].[PerfilesColaboradores] (
        [Id], 
        [UsuarioId], 
        [ValidadoPorAdmin], 
        [NivelAcademico], 
        [DisponibilidadHorasSemana], 
        [CargaAcademicaIdeal], 
        [EvaluacionCalidad]
    ) VALUES (
        '7E13D5B6-5997-439F-AF6D-F1FFF52023D1', -- PerfilColaborador.Id
        '4E13D5B6-5997-439F-AF6D-F1FFF52023D1', -- Usuario.Id
        1,                                      -- ValidadoPorAdmin
        3,                                      -- NivelAcademico (Maestría)
        40,                                     -- DisponibilidadHorasSemana
        5,                                      -- CargaAcademicaIdeal
        5.0                                     -- EvaluacionCalidad
    );
    PRINT '✅ Registro de PerfilColaborador insertado exitosamente para el asesor.';
END
ELSE
BEGIN
    PRINT '✅ El PerfilColaborador ya existe para el asesor.';
END
GO

-- 2. Insertar Areas de Conocimiento base (Inteligencia Artificial, Sistemas)
IF EXISTS (
    SELECT 1 FROM [Perfil].[PerfilesColaboradores] 
    WHERE [Id] = '7E13D5B6-5997-439F-AF6D-F1FFF52023D1'
)
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM [Perfil].[PerfilesColaboradores_AreasConocimiento]
        WHERE [PerfilColaboradorId] = '7E13D5B6-5997-439F-AF6D-F1FFF52023D1' AND [Area] = N'Inteligencia Artificial'
    )
    BEGIN
        INSERT INTO [Perfil].[PerfilesColaboradores_AreasConocimiento] ([PerfilColaboradorId], [Area])
        VALUES ('7E13D5B6-5997-439F-AF6D-F1FFF52023D1', N'Inteligencia Artificial');
    END

    IF NOT EXISTS (
        SELECT 1 FROM [Perfil].[PerfilesColaboradores_AreasConocimiento]
        WHERE [PerfilColaboradorId] = '7E13D5B6-5997-439F-AF6D-F1FFF52023D1' AND [Area] = N'Ingeniería de Sistemas'
    )
    BEGIN
        INSERT INTO [Perfil].[PerfilesColaboradores_AreasConocimiento] ([PerfilColaboradorId], [Area])
        VALUES ('7E13D5B6-5997-439F-AF6D-F1FFF52023D1', N'Ingeniería de Sistemas');
    END

    PRINT '✅ Áreas de conocimiento iniciales validadas.';
END
GO
