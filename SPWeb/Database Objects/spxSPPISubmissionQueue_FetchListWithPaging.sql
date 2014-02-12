SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE   PROCEDURE [dbo].[spxSPPISubmissionQueue_FetchListWithPaging]
	@intUserID			INT,
	@intCurrentPage 		INT,
	@intPageSize			INT,
	@intProviderID			INT = NULL,
	@intServiceID			INT = NULL,
	@strFinancialYear		VARCHAR(9) = NULL,
	@strQuarter			VARCHAR(1) = NULL,
	@intStatus			SMALLINT = NULL,
	@intTotalRecords 		INT = NULL OUTPUT
AS

DECLARE @tblTemp 			TABLE([ID] INT IDENTITY PRIMARY KEY,SubmissionQueueID INT)
DECLARE @intPISubmissionQueueID	INT
DECLARE @intPageSizeCheck		INT
DECLARE @intFirst 			INT

DECLARE PagingCursor CURSOR DYNAMIC READ_ONLY FOR
	SELECT Q.[ID] 
	FROM WebSPPISubmissionQueue AS Q
		INNER JOIN User_SPService AS us ON (us.SPServiceID = Q.SPServiceID)
		INNER JOIN SPService S ON S.ID = us.SPServiceID
		INNER JOIN User_SPProvider AS up ON (up.SPProviderID = S.ProviderID)
	WHERE up.UserID = @intUserID
	AND us.UserID = @intUserID
	AND (@intProviderID IS NULL OR (@intProviderID IS NOT NULL AND up.SPProviderID = @intProviderID))
	AND (@intServiceID IS NULL OR (@intServiceID IS NOT NULL AND Q.SPServiceID = @intServiceID))
	AND (@strFinancialYear IS NULL OR (@strFinancialYear IS NOT NULL AND Q.FinancialYear = @strFinancialYear))
	AND (@strQuarter IS NULL OR (@strQuarter IS NOT NULL AND Q.Quarter = @strQuarter))
	AND (@intStatus IS NULL OR (@intStatus IS NOT NULL AND Q.Status = @intStatus))
	ORDER BY Q.SubmissionDate DESC, Q.[ID] DESC

-- select the records we want
SELECT @intTotalRecords = COUNT(*)
FROM WebSPPISubmissionQueue AS Q
		INNER JOIN User_SPService AS us ON (us.SPServiceID = Q.SPServiceID)
		INNER JOIN SPService S ON S.ID = us.SPServiceID
		INNER JOIN User_SPProvider AS up ON (up.SPProviderID = S.ProviderID)
	WHERE up.UserID = @intUserID
	AND us.UserID = @intUserID
	AND (@intProviderID IS NULL OR (@intProviderID IS NOT NULL AND up.SPProviderID = @intProviderID))
	AND (@intServiceID IS NULL OR (@intServiceID IS NOT NULL AND Q.SPServiceID = @intServiceID))
	AND (@strFinancialYear IS NULL OR (@strFinancialYear IS NOT NULL AND Q.FinancialYear = @strFinancialYear))
	AND (@strQuarter IS NULL OR (@strQuarter IS NOT NULL AND Q.Quarter = @strQuarter))
	AND (@intStatus IS NULL OR (@intStatus IS NOT NULL AND Q.Status = @intStatus))


SET @intFirst = (@intCurrentPage - 1) * @intPageSize + 1
SET @intPageSizeCheck = @intPageSize

-- insert the records we want into the temp table
OPEN PagingCursor
FETCH RELATIVE @intFirst FROM PagingCursor INTO @intPISubmissionQueueID
WHILE @intPageSizeCheck > 0 AND @@FETCH_STATUS = 0
BEGIN
    INSERT @tblTemp(SubmissionQueueID) VALUES(@intPISubmissionQueueID)
    FETCH NEXT FROM PagingCursor INTO @intPISubmissionQueueID
    SET @intPageSizeCheck = @intPageSizeCheck - 1
END
CLOSE PagingCursor
DEALLOCATE PagingCursor	

-- select out the results
SELECT Q.[ID],
	Q.WebFileStoreDataID,
	Q.FinancialYear,
	Q.Quarter,
	Q.Status,
	Q.Comments,
	Q.SubmissionDate,
	Q.ProcessedDate,
	S.Reference AS ServiceReference,
	S.[Name] AS ServiceName,
	P.Reference AS ProviderReference,
	P.[Name]  AS ProviderName,
	U.FullName AS ProcessedByUser,
	WSU.FirstName AS SubmittedFirstName,
	WSU.Surname AS SubmittedSurname,
	WSU.ExternalFullName AS SubmittedExternalFullName
FROM WebSPPISubmissionQueue AS Q
	INNER JOIN @tblTemp AS TMP ON (TMP.SubmissionQueueID = Q.[ID])
	INNER JOIN SPService AS S ON (S.[ID] = Q.SPServiceID)
	INNER JOIN SPProvider P ON P.ID = S.ProviderID
	INNER JOIN WebSecurityUser WSU ON WSU.ID = Q.SubmittedByUserID
	LEFT OUTER JOIN Users U ON U.ID = Q.ProcessedByUserID
ORDER BY TMP.[ID]
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

