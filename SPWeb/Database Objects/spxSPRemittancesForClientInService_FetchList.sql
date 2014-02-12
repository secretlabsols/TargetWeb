if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[spxSPRemittancesForClientInService_FetchList]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[spxSPRemittancesForClientInService_FetchList]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO








CREATE      PROCEDURE [dbo].[spxSPRemittancesForClientInService_FetchList]
	@intUserID			INT,
	@intServiceID			INT = NULL,
	@intClientID			INT,
	@dteDateFrom			DATETIME = NULL,
	@dteDateTo			DATETIME = NULL
AS

SELECT DISTINCT r.[ID] 
FROM SPRemittanceDetail AS rd
	INNER JOIN SPRemittance AS r ON (r.[ID] = rd.RemittanceID)
	INNER JOIN SPSubsidyAgreement AS sub ON (sub.[ID] = rd.RelatedID)
	INNER JOIN SPServiceAgreement AS sa ON (sa.[ID] = sub.ServiceAgreementID)
	INNER JOIN User_SPService AS us ON (us.SPServiceID = sa.ServiceID)
WHERE r.InterfaceLogID IS NOT NULL
AND (@intServiceID IS NULL OR (@intServiceID IS NOT NULL AND sa.ServiceID = @intServiceID))
AND us.UserID = @intUserID
AND sa.ClientID = @intClientID
AND (@dteDateFrom IS NULL OR (@dteDateFrom IS NOT NULL AND DATEDIFF(dd, @dteDateFrom, r.DateTo) >= 0))
AND (@dteDateTo IS NULL OR (@dteDateTo IS NOT NULL AND DATEDIFF(dd, @dteDateTo, r.DateFrom) <= 0))






GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

