if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[spxServiceUser_Fetch]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[spxServiceUser_Fetch]
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO




CREATE  PROCEDURE spxServiceUser_Fetch(@ClientID int)

AS

SELECT CD.Reference, T.Description AS 'Title', CD.FirstNames, CD.LastName, 
CD.BirthDate, CD.NINO, T.ID AS 'TitleID'
FROM ClientDetail CD
LEFT OUTER JOIN Lookup T ON T.ID = CD.Title
WHERE CD.ID = @ClientID


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

