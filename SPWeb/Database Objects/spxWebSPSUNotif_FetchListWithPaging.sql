if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[spxWebSPSUNotif_FetchListWithPaging]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[spxWebSPSUNotif_FetchListWithPaging]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO






CREATE    PROCEDURE [dbo].[spxWebSPSUNotif_FetchListWithPaging]
	@dteFrom			DATETIME = NULL,
	@dteTo				DATETIME = NULL,
	@intStatus			TINYINT = NULL,
	@intWebSecurityExternalUserID	INT = NULL,
	@intCurrentPage 		INT,
	@intPageSize			INT,
	@strListFilterReference		VARCHAR(50) = NULL,
	@strListFilterServiceUser	VARCHAR(50) = NULL,
	@intTotalRecords 		INT = NULL OUTPUT
AS

DECLARE @intFirst 			INT
DECLARE @tblTemp 			TABLE([ID] INT IDENTITY PRIMARY KEY, NotifID INT)
DECLARE @intPageSizeCheck		INT
DECLARE @intNotifID			INT

DECLARE PagingCursor CURSOR DYNAMIC READ_ONLY FOR
	SELECT n.[ID]
	FROM WebSPSUNotif AS n
		INNER JOIN WebSecurityUser AS reqBy ON (reqBy.[ID] = n.RequestedByUserID)
	WHERE (@dteFrom IS NULL OR (@dteFrom IS NOT NULL AND DATEDIFF(dd, @dteFrom, n.CreatedDate) >= 0))
	AND (@dteTo IS NULL OR (@dteTo IS NOT NULL AND DATEDIFF(dd, @dteTo, n.CreatedDate) <= 0))
	AND (@intStatus IS NULL OR (@intStatus IS NOT NULL AND n.StatusID = @intStatus))
	AND (@intWebSecurityExternalUserID IS NULL OR (@intWebSecurityExternalUserID IS NOT NULL AND reqBy.ExternalUserID = @intWebSecurityExternalUserID))
	AND (@strListFilterReference IS NULL OR (@strListFilterReference IS NOT NULL AND n.[ID] LIKE @strListFilterReference))
	AND (@strListFilterServiceUser IS NULL OR (@strListFilterServiceUser IS NOT NULL AND (n.PrimaryFirstNames LIKE @strListFilterServiceUser OR n.PrimaryLastName LIKE @strListFilterServiceUser)))
	ORDER BY n.[ID] DESC

-- select out total record count in output param
SELECT @intTotalRecords = COUNT(*)
FROM WebSPSUNotif AS n
	INNER JOIN WebSecurityUser AS reqBy ON (reqBy.[ID] = n.RequestedByUserID)
WHERE (@dteFrom IS NULL OR (@dteFrom IS NOT NULL AND DATEDIFF(dd, @dteFrom, n.CreatedDate) >= 0))
AND (@dteTo IS NULL OR (@dteTo IS NOT NULL AND DATEDIFF(dd, @dteTo, n.CreatedDate) <= 0))
AND (@intStatus IS NULL OR (@intStatus IS NOT NULL AND n.StatusID = @intStatus))
AND (@intWebSecurityExternalUserID IS NULL OR (@intWebSecurityExternalUserID IS NOT NULL AND reqBy.ExternalUserID = @intWebSecurityExternalUserID))
AND (@strListFilterReference IS NULL OR (@strListFilterReference IS NOT NULL AND n.[ID] LIKE @strListFilterReference))
AND (@strListFilterServiceUser IS NULL OR (@strListFilterServiceUser IS NOT NULL AND (n.PrimaryFirstNames LIKE @strListFilterServiceUser OR n.PrimaryLastName LIKE @strListFilterServiceUser)))

-- set local vars
SET @intFirst = (@intCurrentPage - 1) * @intPageSize + 1
SET @intPageSizeCheck = @intPageSize	

-- insert all relevant records into temp table
OPEN PagingCursor
FETCH RELATIVE @intFirst FROM PagingCursor INTO @intNotifID
WHILE @intPageSizeCheck > 0 AND @@FETCH_STATUS = 0
BEGIN
    INSERT @tblTemp(NotifID) VALUES(@intNotifID)
    FETCH NEXT FROM PagingCursor INTO @intNotifID
    SET @intPageSizeCheck = @intPageSizeCheck - 1
END
CLOSE PagingCursor
DEALLOCATE PagingCursor

-- select out the notifs
SELECT n.[ID],
	n.PrimaryFirstNames AS FirstNames,
	n.PrimaryLastName AS LastName,
	n.TypeID,
	n.CreatedDate,
	n.SubmittedDate,
	n.CompletedDate,
	n.StatusID,
	n.SPSubsidyAgreementID
FROM WebSPSUNotif AS n
	INNER JOIN @tblTemp AS tmp ON (tmp.NotifID = n.[ID])
ORDER BY tmp.[ID]


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

