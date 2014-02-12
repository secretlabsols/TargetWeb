if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[spxSPProviderPaymentsInterfaceHistory_FetchListWithPaging]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[spxSPProviderPaymentsInterfaceHistory_FetchListWithPaging]
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



CREATE  PROCEDURE [dbo].[spxSPProviderPaymentsInterfaceHistory_FetchListWithPaging]
	@intCurrentPage 		INT,
	@intPageSize			INT,
	@intUserID			INT,
	@intProviderID			INT,
	@dteDateFrom			DATETIME = NULL,
	@dteDateTo			DATETIME = NULL,
	@intTotalRecords 		INT = NULL OUTPUT
AS

DECLARE @tblTemp 			TABLE([ID] INT IDENTITY PRIMARY KEY, ProviderInterfaceLogID INT)
DECLARE @intProviderInterfaceLogID	INT
DECLARE @intPageSizeCheck		INT
DECLARE @intFirst 			INT

-- select the records we want
DECLARE PagingCursor CURSOR DYNAMIC READ_ONLY FOR
	SELECT l.[ID] 
	FROM SPProviderInterfaceLog AS l
		INNER JOIN User_SPProvider AS up ON (up.SPProviderID = l.ProviderID)
	WHERE up.UserID = @intUserID
	AND l.ProviderID = @intProviderID
	AND (@dteDateFrom IS NULL OR (@dteDateFrom IS NOT NULL AND DATEDIFF(dd, @dteDateFrom, l.DateFrom) >= 0))
	AND (@dteDateTo IS NULL OR (@dteDateTo IS NOT NULL AND DATEDIFF(dd, @dteDateTo, l.DateTo) <= 0))
	ORDER BY l.FileNumber DESC

SELECT @intTotalRecords = COUNT(*)
FROM SPProviderInterfaceLog AS l
	INNER JOIN User_SPProvider AS up ON (up.SPProviderID = l.ProviderID)
WHERE up.UserID = @intUserID
AND l.ProviderID = @intProviderID
AND (@dteDateFrom IS NULL OR (@dteDateFrom IS NOT NULL AND DATEDIFF(dd, @dteDateFrom, l.DateFrom) >= 0))
AND (@dteDateTo IS NULL OR (@dteDateTo IS NOT NULL AND DATEDIFF(dd, @dteDateTo, l.DateTo) <= 0))

SET @intFirst = (@intCurrentPage - 1) * @intPageSize + 1
SET @intPageSizeCheck = @intPageSize

-- insert the records we want into the temp table
OPEN PagingCursor
FETCH RELATIVE @intFirst FROM PagingCursor INTO @intProviderInterfaceLogID
WHILE @intPageSizeCheck > 0 AND @@FETCH_STATUS = 0
BEGIN
    INSERT @tblTemp(ProviderInterfaceLogID) VALUES(@intProviderInterfaceLogID)
    FETCH NEXT FROM PagingCursor INTO @intProviderInterfaceLogID
    SET @intPageSizeCheck = @intPageSizeCheck - 1
END
CLOSE PagingCursor
DEALLOCATE PagingCursor	

-- select out the results
SELECT l.[ID],
	l.FileNumber,
	l.DateFrom,
	l.DateTo,
	l.RemittanceCount,
	l.TotalValue
FROM SPProviderInterfaceLog AS l
	INNER JOIN @tblTemp AS tmp ON (tmp.ProviderInterfaceLogID = l.[ID])
ORDER BY tmp.[ID]
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

