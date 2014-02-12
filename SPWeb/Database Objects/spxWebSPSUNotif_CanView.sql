if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[spxWebSPSUNotif_CanView]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[spxWebSPSUNotif_CanView]
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO




CREATE    PROCEDURE [dbo].[spxWebSPSUNotif_CanView]
	@intUserID		INT,
	@intWebSUNotifID	INT,
	@blnCanView		BIT = 0 OUTPUT
AS

IF EXISTS(
	SELECT 1 
	FROM WebSPSUNotif AS n
		INNER JOIN WebSecurityUser AS usr ON (usr.[ID] = n.RequestedByUserID)
	WHERE n.[ID] = @intWebSUNotifID 
	AND usr.ExternalUserID = @intUserID
)
	SET @blnCanView = 1
ELSE
	SET @blnCanView = 0




GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

