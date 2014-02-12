SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[spxSPRemittance_CanView]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[spxSPRemittance_CanView]
GO




CREATE   PROCEDURE [dbo].[spxSPRemittance_CanView]
	@intUserID		INT,
	@intSPRemittanceID	INT,
	@blnCanView		BIT = 0 OUTPUT
AS

IF EXISTS(
	SELECT r.[ID]
	FROM SPRemittance AS r
		INNER JOIN User_SPService AS us ON (us.SPServiceID = r.ServiceID)
	WHERE r.InterfaceLogID IS NOT NULL
	AND r.[ID] = @intSPRemittanceID
	AND us.UserID = @intUserID
)
BEGIN
	SET @blnCanView = 1
END
ELSE
BEGIN
	SET @blnCanView = 0
END




GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

