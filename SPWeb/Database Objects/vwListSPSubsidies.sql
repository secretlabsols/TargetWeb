if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[vwListSPSubsidies]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[vwListSPSubsidies]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE VIEW dbo.vwListSPSubsidies
AS

SELECT SUB.ID, S.ID AS 'ServiceID', P.ID AS 'ProviderID', CD.ID AS 'ClientID',
S.Reference AS 'ServiceReference', S.Name AS 'ServiceName', P.Reference AS 'ProviderReference', P.Name AS 'ProviderName',
CD.Reference AS 'ServiceUserReference', CD.Name AS 'ServiceUserName', SUB.DateFrom, SUB.DateTo, SUB.Subsidy,
(SELECT TOP 1 SUCD.UnitCost FROM SPServiceUnitCost SUC
	INNER JOIN SPServiceUnitCostDetail SUCD ON SUCD.ServiceUnitCostID = SUC.ID
	WHERE ServiceID = SA.ServiceID AND DateFrom <= SUB.DateFrom AND SUCD.THBServiceLevel = SA.THBServiceLevel 
	ORDER BY DateFrom DESC) AS 'UnitCost',  
SA.HBReference, SA.AltReference AS 'ProviderRef', SA.THBServiceLevel AS 'Level', SUB.Status
FROM SPSubsidyAgreement SUB
INNER JOIN SPServiceAgreement SA ON SA.ID = SUB.ServiceAgreementID
INNER JOIN ClientDetail CD ON CD.ID = SA.ClientID
INNER JOIN SPService S ON S.ID = SA.ServiceID
INNER JOIN SPProvider P ON P.ID = S.ProviderID






GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

