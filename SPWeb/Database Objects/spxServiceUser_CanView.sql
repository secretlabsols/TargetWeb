if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[spxServiceUser_CanView]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[spxServiceUser_CanView]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE    PROCEDURE [dbo].[spxServiceUser_CanView]
	@intUserID		INT,
	@intClientID		INT,
	@blnCanView		BIT = 0 OUTPUT
AS

IF EXISTS(
	SELECT 1
	FROM SPService AS s
		INNER JOIN User_SPService AS lnk ON (lnk.SPServiceID = s.[ID])
		INNER JOIN SPServiceAgreement AS sa ON (sa.ServiceID = s.[ID] AND sa.ClientID = @intClientID)
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

