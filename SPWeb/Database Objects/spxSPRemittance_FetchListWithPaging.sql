if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[spxSPRemittance_FetchListWithPaging]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[spxSPRemittance_FetchListWithPaging]
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO







CREATE    PROCEDURE [dbo].[spxSPRemittance_FetchListWithPaging]
	@intUserID			INT,
	@intCurrentPage 		INT,
	@intPageSize			INT,
	@intProviderID			INT = NULL,
	@intServiceID			INT = NULL,
	@dteDateFrom			DATETIME = NULL,
	@dteDateTo			DATETIME = NULL,
	@intTotalRecords 		INT = NULL OUTPUT
AS

DECLARE @tblTemp 			TABLE([ID] INT IDENTITY PRIMARY KEY,RemittanceID INT)
DECLARE @intRemittanceID		INT
DECLARE @intPageSizeCheck		INT
DECLARE @intFirst 			INT

DECLARE PagingCursor CURSOR DYNAMIC READ_ONLY FOR
	SELECT r.[ID] 
	FROM SPRemittance AS r
		INNER JOIN User_SPProvider AS up ON (up.SPProviderID = r.ProviderID)
		INNER JOIN User_SPService AS us ON (us.SPServiceID = r.ServiceID)
	WHERE r.InterfaceLogID IS NOT NULL
	AND up.UserID = @intUserID
	AND us.UserID = @intUserID
	AND (@intProviderID IS NULL OR (@intProviderID IS NOT NULL AND r.ProviderID = @intProviderID))
	AND (@intServiceID IS NULL OR (@intServiceID IS NOT NULL AND r.ServiceID = @intServiceID))
	AND (@dteDateFrom IS NULL OR (@dteDateFrom IS NOT NULL AND DATEDIFF(dd, @dteDateFrom, r.DateTo) >= 0))
	AND (@dteDateTo IS NULL OR (@dteDateTo IS NOT NULL AND DATEDIFF(dd, @dteDateTo, r.DateFrom) <= 0))
	ORDER BY r.DateCreated DESC, r.[ID] DESC

-- select the records we want
SELECT @intTotalRecords = COUNT(*)
FROM SPRemittance AS r
	INNER JOIN User_SPProvider AS up ON (up.SPProviderID = r.ProviderID)
	INNER JOIN User_SPService AS us ON (us.SPServiceID = r.ServiceID)
WHERE r.InterfaceLogID IS NOT NULL
AND up.UserID = @intUserID
AND us.UserID = @intUserID
AND (@intProviderID IS NULL OR (@intProviderID IS NOT NULL AND r.ProviderID = @intProviderID))
AND (@intServiceID IS NULL OR (@intServiceID IS NOT NULL AND r.ServiceID = @intServiceID))
AND (@dteDateFrom IS NULL OR (@dteDateFrom IS NOT NULL AND DATEDIFF(dd, @dteDateFrom, r.DateTo) >= 0))
AND (@dteDateTo IS NULL OR (@dteDateTo IS NOT NULL AND DATEDIFF(dd, @dteDateTo, r.DateFrom) <= 0))

SET @intFirst = (@intCurrentPage - 1) * @intPageSize + 1
SET @intPageSizeCheck = @intPageSize

-- insert the records we want into the temp table
OPEN PagingCursor
FETCH RELATIVE @intFirst FROM PagingCursor INTO @intRemittanceID
WHILE @intPageSizeCheck > 0 AND @@FETCH_STATUS = 0
BEGIN
    INSERT @tblTemp(RemittanceID) VALUES(@intRemittanceID)
    FETCH NEXT FROM PagingCursor INTO @intRemittanceID
    SET @intPageSizeCheck = @intPageSizeCheck - 1
END
CLOSE PagingCursor
DEALLOCATE PagingCursor	

-- select out the results
SELECT r.[ID],
	r.Reference,
	s.Reference AS ServiceReference,
	s.[Name] AS ServiceName,
	r.DateFrom,
	r.DateTo,
	r.TotalValue,
	r.DateCreated
FROM SPRemittance AS r
	INNER JOIN @tblTemp AS tmp ON (tmp.RemittanceID = r.[ID])
	INNER JOIN SPService AS s ON (s.[ID] = r.ServiceID)
ORDER BY tmp.[ID]




GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

