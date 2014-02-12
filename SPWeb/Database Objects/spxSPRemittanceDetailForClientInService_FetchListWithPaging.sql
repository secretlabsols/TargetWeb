if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[spxSPRemittanceDetailForClientInService_FetchListWithPaging]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[spxSPRemittanceDetailForClientInService_FetchListWithPaging]
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO









CREATE      PROCEDURE [dbo].[spxSPRemittanceDetailForClientInService_FetchListWithPaging]
	@intUserID			INT,
	@intCurrentPage 		INT,
	@intPageSize			INT,
	@intServiceID			INT = NULL,
	@intClientID			INT,
	@dteDateFrom			DATETIME = NULL,
	@dteDateTo			DATETIME = NULL,
	@intTotalRecords 		INT = NULL OUTPUT
AS

DECLARE @tblTemp 			TABLE([ID] INT IDENTITY PRIMARY KEY, RemittanceDetailID INT)
DECLARE @intRemittanceDetailID		INT
DECLARE @intPageSizeCheck		INT
DECLARE @intFirst 			INT

DECLARE PagingCursor CURSOR DYNAMIC READ_ONLY FOR
	SELECT rd.[ID] 
	FROM SPRemittanceDetail AS rd WITH (INDEX = IX_SPRemittanceDetail)
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
	ORDER BY rd.[ID]

-- select the records we want
SELECT @intTotalRecords = COUNT(*)
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

SET @intFirst = (@intCurrentPage - 1) * @intPageSize + 1
SET @intPageSizeCheck = @intPageSize

-- insert the records we want into the temp table
OPEN PagingCursor
FETCH RELATIVE @intFirst FROM PagingCursor INTO @intRemittanceDetailID
WHILE @intPageSizeCheck > 0 AND @@FETCH_STATUS = 0
BEGIN
    INSERT @tblTemp(RemittanceDetailID) VALUES(@intRemittanceDetailID)
    FETCH NEXT FROM PagingCursor INTO @intRemittanceDetailID
    SET @intPageSizeCheck = @intPageSizeCheck - 1
END
CLOSE PagingCursor
DEALLOCATE PagingCursor	

-- select out the results
SELECT rd.[ID],
	r.Reference,
	rd.Comment,
	rd.LineValue,
	sa.Reference AS OurRef,
	sa.AltReference AS YourRef	
FROM SPRemittanceDetail AS rd
	INNER JOIN @tblTemp AS tmp ON (tmp.RemittanceDetailID = rd.[ID])
	INNER JOIN SPRemittance AS r ON (r.[ID] = rd.RemittanceID)
	LEFT OUTER JOIN SPSubsidyAgreement AS sub ON (sub.[ID] = rd.RelatedID)
	LEFT OUTER JOIN SPServiceAgreement AS sa ON (sa.[ID] = sub.ServiceAgreementID)
ORDER BY tmp.[ID]

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

