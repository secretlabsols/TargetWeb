if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[spxSPSubsidies_FetchListWithPaging]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[spxSPSubsidies_FetchListWithPaging]
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO






CREATE     PROCEDURE [dbo].[spxSPSubsidies_FetchListWithPaging]
	@intCurrentPage 		INT,
	@intPageSize			INT,
	@intServiceID			INT,
	@intStatus			INT,
	@intProviderID			INT = NULL,
	@intClientID			INT = NULL,
	@dteDateFrom			Datetime = NULL,
	@dteDateTo			Datetime = NULL,
	@strListFilterReference		VARCHAR(50) = NULL,
	@strListFilterName		VARCHAR(50) = NULL,
	@intSortColumn			TINYINT = 1,
	@intSortDir			TINYINT = 1,
	@intTotalRecords 		INT = NULL OUTPUT

AS

/* 
	@intSortColumn
	-------------
	1 = client name
	2 = subsidy date from (converted to yyyymmdd format)

	@intSortDir
	----------
	1 = ASC, 2 = DESC
*/

DECLARE @tblTemp 			TABLE([ID] INT IDENTITY PRIMARY KEY, SubsidyID INT)
DECLARE @SubsidyID			INT
DECLARE @intPageSizeCheck		INT
DECLARE @intFirst 			INT
DECLARE @blnFoundSelected		BIT
DECLARE @intSelectedPage		INT

DECLARE PagingCursor CURSOR DYNAMIC READ_ONLY FOR
	SELECT SUB.ID FROM SPSubsidyAgreement SUB
		INNER JOIN SPServiceAgreement SA ON SA.ID = SUB.ServiceAgreementID
		INNER JOIN ClientDetail CD ON CD.ID = SA.ClientID
		INNER JOIN SPService S ON S.ID = SA.ServiceID
		INNER JOIN SPProvider P ON P.ID = S.ProviderID
	WHERE  (@intClientID IS NULL OR (@intClientID IS NOT NULL AND CD.ID = @intClientID)) 
	AND S.ID = @intServiceID
	AND (@intProviderID IS NULL OR (@intProviderID IS NOT NULL AND S.ProviderID = @intProviderID))
	AND (NOT ((SUB.Status & @intStatus) = 0) OR SUB.Status IS NULL)
	AND (@dteDateFrom IS NULL OR (@dteDateFrom IS NOT NULL AND DATEDIFF(dd, @dteDateFrom, SUB.DateTo) >= 0))
	AND (@dteDateTo IS NULL OR (@dteDateTo IS NOT NULL AND DATEDIFF(dd, @dteDateTo, SUB.DateFrom) <= 0))
	AND (@strListFilterReference IS NULL OR (@strListFilterReference IS NOT NULL AND CD.Reference LIKE @strListFilterReference))
	AND (@strListFilterName IS NULL OR (@strListFilterName IS NOT NULL AND CD.[Name] LIKE @strListFilterName))
	ORDER BY 
		CASE WHEN @intSortColumn = 1 AND @intSortDir = 1 THEN CD.[Name] END ASC,
		CASE WHEN @intSortColumn = 1 AND @intSortDir = 2 THEN CD.[Name] END DESC,
		CASE WHEN @intSortColumn = 2 AND @intSortDir = 1 THEN CONVERT(VARCHAR(8), SUB.DateFrom, 112) END ASC,
		CASE WHEN @intSortColumn = 2 AND @intSortDir = 2 THEN CONVERT(VARCHAR(8), SUB.DateFrom, 112) END DESC

-- select the records we want
SELECT @intTotalRecords = COUNT(DISTINCT SUB.[ID])
	FROM SPSubsidyAgreement SUB
		INNER JOIN SPServiceAgreement SA ON SA.ID = SUB.ServiceAgreementID
		INNER JOIN ClientDetail CD ON CD.ID = SA.ClientID
		INNER JOIN SPService S ON S.ID = SA.ServiceID
		INNER JOIN SPProvider P ON P.ID = S.ProviderID
	WHERE  (@intClientID IS NULL OR (@intClientID IS NOT NULL AND CD.ID = @intClientID)) 
	AND S.ID = @intServiceID
	AND (@intProviderID IS NULL OR (@intProviderID IS NOT NULL AND S.ProviderID = @intProviderID))
	AND (NOT ((SUB.Status & @intStatus) = 0) OR SUB.Status IS NULL)
	AND (@dteDateFrom IS NULL OR (@dteDateFrom IS NOT NULL AND DATEDIFF(dd, @dteDateFrom, SUB.DateTo) >= 0))
	AND (@dteDateTo IS NULL OR (@dteDateTo IS NOT NULL AND DATEDIFF(dd, @dteDateTo, SUB.DateFrom) <= 0))
	AND (@strListFilterReference IS NULL OR (@strListFilterReference IS NOT NULL AND CD.Reference LIKE @strListFilterReference))
	AND (@strListFilterName IS NULL OR (@strListFilterName IS NOT NULL AND CD.[Name] LIKE @strListFilterName))

SET @intFirst = (@intCurrentPage - 1) * @intPageSize + 1
SET @intPageSizeCheck = @intPageSize

-- insert the records we want into the temp table
OPEN PagingCursor
FETCH RELATIVE @intFirst FROM PagingCursor INTO @SubsidyID
WHILE @intPageSizeCheck > 0 AND @@FETCH_STATUS = 0
BEGIN
    INSERT @tblTemp(SubsidyID) VALUES(@SubsidyID)
    FETCH NEXT FROM PagingCursor INTO @SubsidyID
    SET @intPageSizeCheck = @intPageSizeCheck - 1
END
CLOSE PagingCursor
DEALLOCATE PagingCursor	

-- select out the results
SELECT S.* FROM vwListSPSubsidies S
	INNER JOIN @tblTemp T ON T.SubsidyID = S.ID
ORDER BY T.ID



GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

