if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[spxSPProperty_CanView]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[spxSPProperty_CanView]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE    PROCEDURE [dbo].[spxSPProperty_CanView]
	@intUserID		INT,
	@intPropertyID		INT,
	@blnCanView		BIT = 0 OUTPUT
AS

IF EXISTS(
	SELECT 1
	FROM SPServiceProperty AS sp
		INNER JOIN User_SPService AS lnk ON (lnk.SPServiceID = sp.ServiceID AND sp.PropertyID = @intPropertyID)
	WHERE lnk.UserID = @intUserID
)
	SET @blnCanView = 1
ELSE
	SET @blnCanView = 0






GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

