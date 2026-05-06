USE [GrupoXpert];
GO
DECLARE @Columnas NVARCHAR(MAX) = '';
SELECT @Columnas = @Columnas + COLUMN_NAME + ', '
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'Identidad' AND TABLE_NAME = 'Usuarios';
PRINT 'Columnas encontradas: ' + @Columnas;
GO
