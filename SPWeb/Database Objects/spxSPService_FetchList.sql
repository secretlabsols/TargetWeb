if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[spxSPService_FetchList]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[spxSPService_FetchList]
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE spxSPService_FetchList(@SPProviderID INT) 

AS

SELECT S.ID AS 'ServiceID', S.Reference, S.Name, S.ServiceDescription AS 'Description', ST.Value AS 'Type' FROM SPService S
INNER JOIN SPServiceType ST ON ST.ID = S.ServiceTypeID
WHERE S.ProviderID = @SPProviderID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

