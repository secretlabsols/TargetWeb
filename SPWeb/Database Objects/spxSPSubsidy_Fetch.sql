if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[spxSPSubsidy_Fetch]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[spxSPSubsidy_Fetch]
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO




CREATE  PROCEDURE spxSPSubsidy_Fetch(@SubsidyID INT)

AS

SELECT P.Reference AS 'ProviderReference', P.Name AS 'ProviderName',
S.Reference AS 'ServiceReference', S.Name AS 'ServiceName', PC.ID AS 'PrimaryClientID', PC.Reference AS 'PrimaryServiceUserRef',
PC.Name AS 'PrimaryServiceUserName', SC.Reference AS 'SecondaryServiceUserRef', SC.ID AS 'SecondaryClientID',
SC.Name AS 'SecondaryServiceUserName', SUB.DateFrom, SUB.DateTo, SER.Description AS 'EndReason',
SUB.ReviewDate, SA.AltReference AS 'SAProviderReference', SUB.Subsidy, SUB.VAT, SUB.ClientContribution,
SUB.THBServiceLevel, (SELECT TOP 1 SUCD.UnitCost FROM SPServiceUnitCost SUC
	INNER JOIN SPServiceUnitCostDetail SUCD ON SUCD.ServiceUnitCostID = SUC.ID
	WHERE ServiceID = SA.ServiceID AND DateFrom <= SUB.DateFrom AND SUCD.THBServiceLevel = SA.THBServiceLevel 
	ORDER BY DateFrom DESC) AS 'UnitCost', SUB.Status, SUB.TransitionalHB, SUB.FCApplicationMade,
SA.HBReference, SUB.HBAppliedOn, SUB.HBAwardStatusDate, HBAS.Description AS 'HBStatus', SUB.HBAwardStatusDate,
SUB.HBDPWaiver, SA.FCReference, SUB.FCAppliedOn, FCAS.Description AS 'FCStatus', 
SUB.FCAssessmentStatusDate, SUB.SubsidyEndReasonID, SUB.ServiceAgreementID,
PC.FirstNames AS 'PrimaryServiceUserFirstNames', PC.LastName AS 'PrimaryServiceUserLastName',
SC.FirstNames AS 'SecondaryServiceUserFirstNames', SC.LastName AS 'SecondaryServiceUserLastName',
P.[ID] AS 'ProviderID', s.[ID] AS 'ServiceID', SA.PropertyID
FROM SPSubsidyAgreement SUB
INNER JOIN SPServiceAgreement SA ON SA.ID = SUB.ServiceAgreementID
INNER JOIN SPService S ON S.ID = SA.ServiceID
INNER JOIN SPProvider P ON S.ProviderID = P.ID
INNER JOIN ClientDetail PC ON PC.ID = SA.ClientID
LEFT OUTER JOIN ClientDetail SC ON SC.ID = SA.SecondaryClientID
LEFT OUTER JOIN SPSubsidyEndReason SER ON SER.ID = SUB.SubsidyEndReasonID
LEFT OUTER JOIN SPHBAwardStatus HBAS ON HBAS.ID = SUB.HBAwardStatusID
LEFT OUTER JOIN SPFCAssessmentStatus FCAS ON FCAS.ID = SUB.FCAssessmentStatusID
WHERE SUB.ID = @SubsidyID



GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

