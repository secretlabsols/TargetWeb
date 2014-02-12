if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[pr_GetIsOffline]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[pr_GetIsOffline]
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE  PROCEDURE [dbo].[pr_GetIsOffline] 
AS

RETURN (SELECT IsOffline FROM SystemInfo)


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

