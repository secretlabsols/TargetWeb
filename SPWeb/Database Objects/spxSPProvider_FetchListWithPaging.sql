if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[spxSPProvider_FetchListWithPaging]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[spxSPProvider_FetchListWithPaging]
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO





CREATE   PROCEDURE [dbo].[spxSPProvider_FetchListWithPaging]
	@intUserID			INT,
	@intCurrentPage 		INT OUTPUT,
	@intPageSize			INT,
	@intSelectedProviderID		INT = NULL,
	@strListFilterReference		VARCHAR(50) = NULL,
	@strListFilterName		VARCHAR(50) = NULL,
	@intTotalRecords 		INT = NULL OUTPUT

AS

DECLARE @tblTemp 			TABLE([ID] INT IDENTITY PRIMARY KEY, ProviderID INT)
DECLARE @ProviderID			INT
DECLARE @intPageSizeCheck		INT
DECLARE @intFirst 			INT
DECLARE @blnFoundSelected		BIT
DECLARE @intSelectedPage		INT


DECLARE PagingCursor CURSOR DYNAMIC READ_ONLY FOR
	SELECT P.ID FROM SPProvider P
		INNER JOIN User_SPProvider UP ON UP.SPProviderID = P.ID
	WHERE UP.UserID = @intUserID
	AND (@strListFilterReference IS NULL OR (@strListFilterReference IS NOT NULL AND P.Reference LIKE @strListFilterReference))
	AND (@strListFilterName IS NULL OR (@strListFilterName IS NOT NULL AND P.[Name] LIKE @strListFilterName))
	ORDER BY p.[Name]

-- select the records we want
SELECT @intTotalRecords = COUNT(DISTINCT P.[ID])
	FROM SPProvider P
		INNER JOIN User_SPProvider UP ON UP.SPProviderID = P.ID
	WHERE UP.UserID = @intUserID
	AND (@strListFilterReference IS NULL OR (@strListFilterReference IS NOT NULL AND P.Reference LIKE @strListFilterReference))
	AND (@strListFilterName IS NULL OR (@strListFilterName IS NOT NULL AND P.[Name] LIKE @strListFilterName))

-- if we need to select a provider, override the requested page by finding 
-- the first record in the page that contains the requested provider
SET @blnFoundSelected = 0
IF @intSelectedProviderID IS NOT NULL
BEGIN
	SET @intFirst = 1
	SET @intSelectedPage = 1
	SET @intPageSizeCheck = @intPageSize

	OPEN PagingCursor
	FETCH NEXT FROM PagingCursor INTO @ProviderID
	WHILE @intPageSizeCheck > 0 AND @@FETCH_STATUS = 0
	BEGIN
		-- bail out if we find the provider
	    	IF @ProviderID = @intSelectedProviderID
		BEGIN
			SET @blnFoundSelected = 1
			SET @intCurrentPage = @intSelectedPage
			BREAK
		END

		FETCH NEXT FROM PagingCursor INTO @ProviderID
		
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
FETCH RELATIVE @intFirst FROM PagingCursor INTO @ProviderID
WHILE @intPageSizeCheck > 0 AND @@FETCH_STATUS = 0
BEGIN
    INSERT @tblTemp(ProviderID) VALUES(@ProviderID)
    FETCH NEXT FROM PagingCursor INTO @ProviderID
    SET @intPageSizeCheck = @intPageSizeCheck - 1
END
CLOSE PagingCursor
DEALLOCATE PagingCursor	

-- select out the results
SELECT P.ID AS [ProviderID], P.Reference, P.[Name], A.Address, A.PostCode FROM SPProvider P
INNER JOIN @tblTemp AS tmp ON TMP.ProviderID = P.ID
LEFT OUTER JOIN SPAddress A ON A.ID = P.ProviderAddressID
ORDER BY tmp.[ID]




GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

