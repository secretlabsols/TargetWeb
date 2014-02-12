if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[spxSPProperty_FetchListWithPaging]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[spxSPProperty_FetchListWithPaging]
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE [dbo].[spxSPProperty_FetchListWithPaging]
	@intUserID			INT,
	@intServiceID			INT,
	@intCurrentPage 		INT,
	@intPageSize			INT,
	@intTotalRecords 		INT = NULL OUTPUT

AS

DECLARE @tblTemp 			TABLE([ID] INT IDENTITY PRIMARY KEY, PropertyID INT)
DECLARE @PropertyID			INT
DECLARE @intPageSizeCheck		INT
DECLARE @intFirst 			INT


DECLARE PagingCursor CURSOR DYNAMIC READ_ONLY FOR
SELECT P.PropertyID FROM SPServiceProperty P
WHERE  P.ServiceID = @intServiceID


SELECT @intTotalRecords = COUNT(DISTINCT P.[ID])
FROM SPServiceProperty P
WHERE P.ServiceID = @intServiceID

SET @intFirst = (@intCurrentPage - 1) * @intPageSize + 1
SET @intPageSizeCheck = @intPageSize

OPEN PagingCursor
FETCH RELATIVE @intFirst FROM PagingCursor INTO @PropertyID
WHILE @intPageSizeCheck > 0 AND @@FETCH_STATUS = 0
BEGIN
    INSERT @tblTemp(PropertyID) VALUES(@PropertyID)
    FETCH NEXT FROM PagingCursor INTO @PropertyID
    SET @intPageSizeCheck = @intPageSizeCheck - 1
END
CLOSE PagingCursor
DEALLOCATE PagingCursor	

SELECT P.* FROM vwListSPProperty P
INNER JOIN @tblTemp T ON T.PropertyID = P.PropertyID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

