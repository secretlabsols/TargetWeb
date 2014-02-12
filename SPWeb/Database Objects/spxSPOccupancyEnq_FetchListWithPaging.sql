if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[spxSPOccupancyEnq_FetchListWithPaging]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[spxSPOccupancyEnq_FetchListWithPaging]
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO





CREATE   PROCEDURE [dbo].[spxSPOccupancyEnq_FetchListWithPaging]
	@intUserID			INT,
	@intServiceID			INT,
	@intPropertyID			INT,
	@dteDateFrom			DATETIME = NULL,
	@dteDateTo			DATETIME = NULL,
	@Status				INT = NULL,
	@intCurrentPage 		INT,
	@intPageSize			INT,
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


DECLARE PagingCursor CURSOR DYNAMIC READ_ONLY FOR
	SELECT SUB.ID FROM SPSubsidyAgreement SUB
		INNER JOIN SPServiceAgreement SA ON SA.ID = SUB.ServiceAgreementID
		INNER JOIN ClientDetail CD ON CD.ID = SA.ClientID
	WHERE  SA.ServiceID = @intServiceID
	AND SA.PropertyID = @intPropertyID
	AND (NOT ((SUB.Status & @Status) = 0) OR SUB.Status IS NULL)
	AND (@dteDateFrom IS NULL OR (@dteDateFrom IS NOT NULL AND DATEDIFF(dd, @dteDateFrom, SUB.DateTo) >= 0))
	AND (@dteDateTo IS NULL OR (@dteDateTo IS NOT NULL AND DATEDIFF(dd, @dteDateTo, SUB.DateFrom) <= 0))
	AND (@strListFilterReference IS NULL OR (@strListFilterReference IS NOT NULL AND CD.Reference LIKE @strListFilterReference))
	AND (@strListFilterName IS NULL OR (@strListFilterName IS NOT NULL AND CD.[Name] LIKE @strListFilterName))
	ORDER BY 
		CASE WHEN @intSortColumn = 1 AND @intSortDir = 1 THEN CD.[Name] END ASC,
		CASE WHEN @intSortColumn = 1 AND @intSortDir = 2 THEN CD.[Name] END DESC,
		CASE WHEN @intSortColumn = 2 AND @intSortDir = 1 THEN CONVERT(VARCHAR(8), SUB.DateFrom, 112) END ASC,
		CASE WHEN @intSortColumn = 2 AND @intSortDir = 2 THEN CONVERT(VARCHAR(8), SUB.DateFrom, 112) END DESC

SELECT @intTotalRecords = COUNT(DISTINCT SUB.[ID])
	FROM SPSubsidyAgreement SUB
		INNER JOIN SPServiceAgreement SA ON SA.ID = SUB.ServiceAgreementID
		INNER JOIN ClientDetail CD ON CD.ID = SA.ClientID
	WHERE  SA.ServiceID = @intServiceID
	AND SA.PropertyID = @intPropertyID
	AND (NOT ((SUB.Status & @Status) = 0) OR SUB.Status IS NULL)
	AND (@dteDateFrom IS NULL OR (@dteDateFrom IS NOT NULL AND DATEDIFF(dd, @dteDateFrom, SUB.DateTo) >= 0))
	AND (@dteDateTo IS NULL OR (@dteDateTo IS NOT NULL AND DATEDIFF(dd, @dteDateTo, SUB.DateFrom) <= 0))
	AND (@strListFilterReference IS NULL OR (@strListFilterReference IS NOT NULL AND CD.Reference LIKE @strListFilterReference))
	AND (@strListFilterName IS NULL OR (@strListFilterName IS NOT NULL AND CD.[Name] LIKE @strListFilterName))

SET @intFirst = (@intCurrentPage - 1) * @intPageSize + 1
SET @intPageSizeCheck = @intPageSize

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

SELECT OE.* FROM vwListSPOccupancyEnq OE
INNER JOIN @tblTemp T ON T.SubsidyID = OE.SubsidyID




GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

