if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[vwListSPOccupancyEnq]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[vwListSPOccupancyEnq]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE VIEW dbo.vwListSPOccupancyEnq
AS

SELECT CD.Reference AS 'ServiceUserReference', CD.Name AS 'ServiceUserName', SUB.DateFrom, 
SUB.DateTo, SUB.Subsidy, SA.HBReference, SA.THBServiceLevel AS 'Level',
(SELECT TOP 1 SUCD.UnitCost FROM SPServiceUnitCost SUC
	INNER JOIN SPServiceUnitCostDetail SUCD ON SUCD.ServiceUnitCostID = SUC.ID
	WHERE ServiceID = SA.ServiceID AND DateFrom <= SUB.DateFrom AND SUCD.THBServiceLevel = SA.THBServiceLevel ORDER BY DateFrom DESC) AS 'UnitCost',
SA.AltReference AS 'ProviderReference', SUB.Status, SUB.ID AS 'SubsidyID', SA.ClientID AS 'ServiceUserID'
FROM dbo.SPSubsidyAgreement SUB
INNER JOIN ClientDetail CD ON CD.ID = SUB.ClientID
INNER JOIN SPServiceAgreement SA ON SA.ID = SUB.ServiceAgreementID








GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

