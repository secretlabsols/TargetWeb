if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[spxSPService_FetchListWithPaging]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[spxSPService_FetchListWithPaging]
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO




CREATE   PROCEDURE [dbo].[spxSPService_FetchListWithPaging]
	@intUserID			INT,
	@intProviderID			INT = NULL,
	@intCurrentPage 		INT OUTPUT,
	@intPageSize			INT,
	@intSelectedServiceID		INT = NULL,
	@strListFilterReference		VARCHAR(50) = NULL,
	@strListFilterName		VARCHAR(50) = NULL,
	@intTotalRecords 		INT = NULL OUTPUT

AS

DECLARE @tblTemp 			TABLE([ID] INT IDENTITY PRIMARY KEY, ServiceID INT)
DECLARE @ServiceID			INT
DECLARE @intPageSizeCheck		INT
DECLARE @intFirst 			INT
DECLARE @blnFoundSelected		BIT
DECLARE @intSelectedPage		INT

DECLARE PagingCursor CURSOR DYNAMIC READ_ONLY FOR
	SELECT S.ID FROM SPService S
		INNER JOIN User_SPService US ON US.SPServiceID = S.ID
	WHERE US.UserID = @intUserID 
	AND (@intProviderID IS NULL OR (@intProviderID IS NOT NULL AND S.ProviderID = @intProviderID))
	AND (@strListFilterReference IS NULL OR (@strListFilterReference IS NOT NULL AND S.Reference LIKE @strListFilterReference))
	AND (@strListFilterName IS NULL OR (@strListFilterName IS NOT NULL AND S.[Name] LIKE @strListFilterName))
	ORDER BY s.Name

-- select the records we want
SELECT @intTotalRecords = COUNT(DISTINCT S.[ID])
	FROM SPService S
		INNER JOIN User_SPService US ON US.SPServiceID = S.ID
	WHERE US.UserID = @intUserID 
	AND (@intProviderID IS NULL OR (@intProviderID IS NOT NULL AND S.ProviderID = @intProviderID))
	AND (@strListFilterReference IS NULL OR (@strListFilterReference IS NOT NULL AND S.Reference LIKE @strListFilterReference))
	AND (@strListFilterName IS NULL OR (@strListFilterName IS NOT NULL AND S.[Name] LIKE @strListFilterName))

-- if we need to select a provider, override the requested page by finding 
-- the first record in the page that contains the requested provider
SET @blnFoundSelected = 0
IF @intSelectedServiceID IS NOT NULL
BEGIN
	SET @intFirst = 1
	SET @intSelectedPage = 1
	SET @intPageSizeCheck = @intPageSize

	OPEN PagingCursor
	FETCH NEXT FROM PagingCursor INTO @ServiceID
	WHILE @intPageSizeCheck > 0 AND @@FETCH_STATUS = 0
	BEGIN
		-- bail out if we find the service
	    	IF @ServiceID = @intSelectedServiceID
		BEGIN
			SET @blnFoundSelected = 1
			SET @intCurrentPage = @intSelectedPage
			BREAK
		END

		FETCH NEXT FROM PagingCursor INTO @ServiceID
		
		SET @intPageSizeCheck = @intPageSizeCheck - 1
		-- if we have reached the end of this page, move the counters onto the next page
		IF @intPageSizeCheck = 0
		BEGIN
			SET @intFirst = @intFirst + @intPageSize
			SET @intSelectedPage = @intSelectedPage + 1
			SET @intPageSizeCheck = @intPageSize
		END
	END
	CLOSE PagingCursor
END

IF @blnFoundSelected = 0
	SET @intFirst = (@intCurrentPage - 1) * @intPageSize + 1
SET @intPageSizeCheck = @intPageSize

-- insert the records we want into the temp table
OPEN PagingCursor
FETCH RELATIVE @intFirst FROM PagingCursor INTO @ServiceID
WHILE @intPageSizeCheck > 0 AND @@FETCH_STATUS = 0
BEGIN
    INSERT @tblTemp(ServiceID) VALUES(@ServiceID)
    FETCH NEXT FROM PagingCursor INTO @ServiceID
    SET @intPageSizeCheck = @intPageSizeCheck - 1
END
CLOSE PagingCursor
DEALLOCATE PagingCursor	

-- select out the results
SELECT S.* FROM vwListSPService S
INNER JOIN @tblTemp T ON T.ServiceID = S.ServiceID
ORDER BY T.ID




GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

