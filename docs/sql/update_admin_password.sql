USE [GrupoXpert];
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

UPDATE [Identidad].[Usuarios] 
SET [HashClave] = '$2a$11$wkrb7afYTtBZhPMCsjoK6OSBoB9AbU9uvC1DYKwj3b7bAQXhBsNGS' 
WHERE [Correo] = 'admin@grupoxpert.com';
GO
