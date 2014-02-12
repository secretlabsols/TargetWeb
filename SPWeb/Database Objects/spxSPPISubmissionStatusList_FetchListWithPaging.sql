SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE   PROCEDURE [dbo].[spxSPPISubmissionStatusList_FetchListWithPaging]
	@intUserID			INT,
	@intCurrentPage 		INT,
	@intPageSize			INT,
	@intProviderID			INT = NULL,
	@intServiceID			INT = NULL,
	@strFinancialYearFrom		VARCHAR(9) = NULL,
	@strQuarterFrom		VARCHAR(1) = NULL,
	@strFinancialYearTo		VARCHAR(9) = NULL,
	@strQuarterTo			VARCHAR(1) = NULL,
	@intStatus			SMALLINT = NULL,
	@intTotalRecords 		INT = NULL OUTPUT
AS

DECLARE @tblTemp 			TABLE([ID] INT IDENTITY PRIMARY KEY, YRFrom char(4),YRTo char(4), Quarter char(1), SPServiceID INT)
DECLARE @strYrFrom			char(4)
DECLARE @strYrTo			char(4)
DECLARE @strQuarter			char(1)
DECLARE @intSPServiceID		INT
DECLARE @intPageSizeCheck		INT
DECLARE @intFirst 			INT
DECLARE @DateFrom			DATETIME
DECLARE @DateTo			DATETIME

IF @strFinancialYearFrom IS NULL
BEGIN
	SELECT TOP 1 @DateFrom = DateFrom FROM SPQuarter ORDER BY FYFrom, Quarter
END
ELSE
BEGIN
	SELECT @DateFrom = DateFrom FROM SPQuarter WHERE FYFrom  = LEFT(@strFinancialYearFrom, 4) AND Quarter = @strQuarterFrom
END

IF @strFinancialYearTo IS NULL
BEGIN
	SELECT TOP 1 @DateTo = DateTo FROM SPQuarter ORDER BY FYFrom DESC, Quarter DESC
END
ELSE
BEGIN
	SELECT @DateTo = DateTo FROM SPQuarter WHERE FYFrom = LEFT(@strFinancialYearTo, 4) AND Quarter = @strQuarterTo
END


DECLARE PagingCursor CURSOR DYNAMIC READ_ONLY FOR
	SELECT DISTINCT PERIOD.[FYFrom], PERIOD.[FYTo], PERIOD.[Quarter], S.[ID]
	FROM (SELECT FYFrom + '-' + FYTo  AS 'FinancialYear', Quarter, FYFrom, FYTo FROM SPQuarter
		WHERE DateFrom >= @DateFrom AND DateTo <= @DateTo) Period
	CROSS JOIN SPService S
	LEFT OUTER JOIN SPPISubmission PIS ON Period.FinancialYear = PIS.FinancialYear AND Period.Quarter = PIS.Quarter AND PIS.ServiceID = S.ID 
	LEFT OUTER JOIN WebSPPISubmissionQueue PISQ ON Period.FinancialYear = PISQ.FinancialYear AND Period.Quarter = PISQ.Quarter AND PISQ.SPServiceID = S.ID 
	INNER JOIN User_SPService AS US ON US.SPServiceID = S.ID
	INNER JOIN SPProvider P ON P.ID = S.ProviderID
	INNER JOIN User_SPProvider AS UP ON UP.SPProviderID = S.ProviderID
	WHERE UP.UserID = @intUserID
	AND US.UserID = @intUserID
	AND (@intProviderID IS NULL OR (@intProviderID IS NOT NULL AND UP.SPProviderID = @intProviderID))
	AND (@intServiceID IS NULL OR (@intServiceID IS NOT NULL AND S.ID = @intServiceID))
	AND (@intStatus IS NULL OR (@intStatus IS NOT NULL AND CASE WHEN PIS.ID IS NULL AND PISQ.Status IS NULL THEN 0
             						  			       WHEN PIS.ID IS NOT NULL THEN 3
  					  	          			       WHEN PIS.ID IS NULL AND PISQ.Status IS NOT NULL THEN PISQ.Status
 						     			       END  = @intStatus))
	ORDER BY PERIOD.[FYFrom], PERIOD.[FYTo], PERIOD.[Quarter], S.[ID]

-- select the records we want
SELECT DISTINCT @intTotalRecords = COUNT(*)
FROM (SELECT FYFrom + '-' + FYTo  AS 'FinancialYear', Quarter, FYFrom, FYTo FROM SPQuarter
		WHERE DateFrom >= @DateFrom AND DateTo <= @DateTo) Period
	CROSS JOIN SPService S
	LEFT OUTER JOIN SPPISubmission PIS ON Period.FinancialYear = PIS.FinancialYear AND Period.Quarter = PIS.Quarter AND PIS.ServiceID = S.ID 
	LEFT OUTER JOIN WebSPPISubmissionQueue PISQ ON Period.FinancialYear = PISQ.FinancialYear AND Period.Quarter = PISQ.Quarter AND PISQ.SPServiceID = S.ID 
	INNER JOIN User_SPService AS US ON US.SPServiceID = S.ID
	INNER JOIN SPProvider P ON P.ID = S.ProviderID
	INNER JOIN User_SPProvider AS UP ON UP.SPProviderID = S.ProviderID
	WHERE UP.UserID = @intUserID
	AND US.UserID = @intUserID
	AND (@intProviderID IS NULL OR (@intProviderID IS NOT NULL AND UP.SPProviderID = @intProviderID))
	AND (@intServiceID IS NULL OR (@intServiceID IS NOT NULL AND S.ID = @intServiceID))
	AND (@intStatus IS NULL OR (@intStatus IS NOT NULL AND CASE WHEN PIS.ID IS NULL AND PISQ.Status IS NULL THEN 0
             						  			       WHEN PIS.ID IS NOT NULL THEN 3
  					  	          			       WHEN PIS.ID IS NULL AND PISQ.Status IS NOT NULL THEN PISQ.Status
 						     			       END  = @intStatus))


SET @intFirst = (@intCurrentPage - 1) * @intPageSize + 1
SET @intPageSizeCheck = @intPageSize


-- insert the records we want into the temp table
OPEN PagingCursor
FETCH RELATIVE @intFirst FROM PagingCursor INTO  @strYrFrom,  @strYrTo,  @strQuarter, @intSPServiceID
WHILE @intPageSizeCheck > 0 AND @@FETCH_STATUS = 0
BEGIN
    INSERT @tblTemp(YRFrom,YRTo, Quarter, SPServiceID) VALUES(@strYrFrom,  @strYrTo,  @strQuarter, @intSPServiceID)
    FETCH NEXT FROM PagingCursor INTO @strYrFrom,  @strYrTo,  @strQuarter, @intSPServiceID
    SET @intPageSizeCheck = @intPageSizeCheck - 1
END
CLOSE PagingCursor
DEALLOCATE PagingCursor	


-- select out the results

SELECT  PERIOD.FYFrom, PERIOD.Quarter, PERIOD. FYFrom + '-' + PERIOD.FYTo AS 'FinancialYear',
	PERIOD.Quarter,
	'Status' = CASE WHEN PIS.ID IS NULL AND PISQ.Status IS NULL THEN 0
             			 WHEN PIS.ID IS NOT NULL THEN 3
  			WHEN PIS.ID IS NULL AND PISQ.Status IS NOT NULL THEN PISQ.Status
		   END,
	S.Reference AS ServiceReference,
	S.[Name] AS ServiceName,
	P.Reference AS ProviderReference,
	P.[Name]  AS ProviderName,
	PIS.SubmissionDate,  S.ID
FROM  SPQuarter PERIOD
	CROSS JOIN SPService S
	LEFT OUTER JOIN SPPISubmission PIS ON  PERIOD.FYFrom + '-' + PERIOD.FYTo = PIS.FinancialYear AND Period.Quarter = PIS.Quarter AND PIS.ServiceID = S.ID 
	LEFT OUTER JOIN (SELECT SPServiceID, FinancialYear, Quarter, Max(Status) AS Status FROM WEBSPPISubmissionQueue
			GROUP BY SPServiceID, FinancialYear, Quarter) PISQ ON PERIOD.FYFrom + '-' + PERIOD.FYTo = PISQ.FinancialYear 
								AND Period.Quarter = PISQ.Quarter AND PISQ.SPServiceID = S.ID AND PIS.ID IS NULL 
	INNER JOIN SPProvider P ON P.ID = S.ProviderID
	INNER JOIN @tblTemp AS TMP ON CAST(TMP.YRFrom AS INT) = CAST(PERIOD.FYFrom AS INT) AND  CAST(TMP.YRTo AS INT) = CAST(PERIOD.FYTo AS INT) AND CAST(TMP.Quarter AS INT) = CAST(PERIOD.Quarter AS INT) AND TMP.SPServiceID = S.ID

ORDER BY S.ID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

