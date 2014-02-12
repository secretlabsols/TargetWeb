SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE pr_FetchSPPISubmissionBatchWEB(@FinancialYear as varchar(12), @Quarter int, @ProviderID int, @ServiceID int, @Status int)

AS

SELECT S.Name AS 'ServiceName', Q.* FROM WebSPPISubmissionQueue Q
INNER JOIN WebFileStoreData WFSD ON WFSD.ID = Q.WebFileStoreDataID
INNER JOIN SPService S ON S.ID = Q.SPServiceID 
INNER JOIN SPProvider P ON P.ID = S.ProviderID
WHERE Q.FinancialYear = @FinancialYear
AND Q.Quarter = @Quarter
AND ((P.ID = @ProviderID) OR (@ProviderID = 0))
AND ((S.ID = @ServiceID) OR (@ServiceID = 0))
AND Q.Status = @Status
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

