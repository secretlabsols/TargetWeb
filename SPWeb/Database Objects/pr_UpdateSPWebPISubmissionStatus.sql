SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE pr_UpdateSPWebPISubmissionStatus(@ID int, @NewStatus tinyint, @Comment text, @UserName varchar(12))

AS

UPDATE WebSPPISubmissionQueue SET Status = @NewStatus, Comments = @Comment, 
		ProcessedByUserID = (SELECT ID FROM Users WHERE Name = @UserName), ProcessedDate = GetDate()  WHERE ID = @ID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

