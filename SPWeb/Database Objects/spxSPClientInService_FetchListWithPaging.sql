if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[spxSPClientInService_FetchListWithPaging]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[spxSPClientInService_FetchListWithPaging]
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO








CREATE      PROCEDURE [dbo].[spxSPClientInService_FetchListWithPaging]
	@intUserID			INT,
	@intProviderID			INT = NULL,
	@intServiceID			INT = NULL,
	@intCurrentPage 		INT OUTPUT,
	@intPageSize			INT,
	@intSelectedClientID		INT = NULL,
	@strListFilterReference		VARCHAR(50) = NULL,
	@strListFilterName		VARCHAR(50) = NULL,
	@intTotalRecords 		INT = NULL OUTPUT
AS

DECLARE @tblTemp 			TABLE([ID] INT IDENTITY PRIMARY KEY, ClientID INT, ClientName VARCHAR(30))
DECLARE @intClientID			INT
DECLARE @intClientName			VARCHAR(30)
DECLARE @intPageSizeCheck		INT
DECLARE @intFirst 			INT
DECLARE @blnFoundSelected		BIT
DECLARE @intSelectedPage		INT

-- select the records we want
DECLARE PagingCursor CURSOR DYNAMIC READ_ONLY FOR
	SELECT DISTINCT cd.[ID], cd.[Name]
	FROM ClientDetail AS cd
		INNER JOIN SPServiceAgreement AS sa ON (sa.ClientID = cd.[ID])
		INNER JOIN User_SPService AS us ON (us.SPServiceID = sa.ServiceID)
		INNER JOIN SPService AS s ON (s.[ID] = sa.ServiceID)
		INNER JOIN SPProvider AS p ON (p.[ID] = s.ProviderID)
		INNER JOIN User_SPProvider AS up ON (up.SPProviderID = p.[ID])
	WHERE (@intServiceID IS NULL OR (@intServiceID IS NOT NULL AND sa.ServiceID = @intServiceID))
	AND (@intProviderID IS NULL OR (@intProviderID IS NOT NULL AND p.[ID] = @intProviderID))
	AND us.UserID = @intUserID
	AND up.UserID = @intUserID
	AND (@strListFilterReference IS NULL OR (@strListFilterReference IS NOT NULL AND cd.Reference LIKE @strListFilterReference))
	AND (@strListFilterName IS NULL OR (@strListFilterName IS NOT NULL AND cd.[Name] LIKE @strListFilterName))
	ORDER BY cd.[Name]

SELECT @intTotalRecords = COUNT(DISTINCT cd.[ID])
FROM ClientDetail AS cd
	INNER JOIN SPServiceAgreement AS sa ON (sa.ClientID = cd.[ID])
	INNER JOIN User_SPService AS us ON (us.SPServiceID = sa.ServiceID)
	INNER JOIN SPService AS s ON (s.[ID] = sa.ServiceID)
	INNER JOIN SPProvider AS p ON (p.[ID] = s.ProviderID)
	INNER JOIN User_SPProvider AS up ON (up.SPProviderID = p.[ID])
WHERE (@intServiceID IS NULL OR (@intServiceID IS NOT NULL AND sa.ServiceID = @intServiceID))
AND (@intProviderID IS NULL OR (@intProviderID IS NOT NULL AND p.[ID] = @intProviderID))
AND us.UserID = @intUserID
AND up.UserID = @intUserID
AND (@strListFilterReference IS NULL OR (@strListFilterReference IS NOT NULL AND cd.Reference LIKE @strListFilterReference))
AND (@strListFilterName IS NULL OR (@strListFilterName IS NOT NULL AND (cd.FirstNames LIKE @strListFilterName OR cd.LastName LIKE @strListFilterName)))

-- if we need to select a client, override the requested page by finding 
-- the first record in the page that contains the requested client
SET @blnFoundSelected = 0
IF @intSelectedClientID IS NOT NULL
BEGIN
	SET @intFirst = 1
	SET @intSelectedPage = 1
	SET @intPageSizeCheck = @intPageSize

	OPEN PagingCursor
	FETCH NEXT FROM PagingCursor INTO @intClientID, @intClientName
	WHILE @intPageSizeCheck > 0 AND @@FETCH_STATUS = 0
	BEGIN
		-- bail out if we find the client
	    	IF @intClientID = @intSelectedClientID
		BEGIN
			SET @blnFoundSelected = 1
			SET @intCurrentPage = @intSelectedPage
			BREAK
		END

		FETCH NEXT FROM PagingCursor INTO @intClientID, @intClientName
		
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
FETCH RELATIVE @intFirst FROM PagingCursor INTO @intClientID, @intClientName
WHILE @intPageSizeCheck > 0 AND @@FETCH_STATUS = 0
BEGIN
    INSERT @tblTemp(ClientID, ClientName) VALUES(@intClientID, @intClientName)
    FETCH NEXT FROM PagingCursor INTO @intClientID, @intClientName
    SET @intPageSizeCheck = @intPageSizeCheck - 1
END
CLOSE PagingCursor
DEALLOCATE PagingCursor	

-- select out the result
SELECT cd.[ID],
	cd.Reference,
	cd.[Name],
	cd.NINO,
	cd.BirthDate
FROM ClientDetail AS cd
	INNER JOIN @tblTemp AS tmp ON (tmp.ClientID = cd.[ID])
ORDER BY tmp.[ID]






GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

