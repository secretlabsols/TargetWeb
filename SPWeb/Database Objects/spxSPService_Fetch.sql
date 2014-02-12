if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[spxSPService_Fetch]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[spxSPService_Fetch]
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO



CREATE  PROCEDURE spxSPService_Fetch(@ServiceID int)

AS

SELECT  P.ID AS 'ProviderID', P.Name AS 'ProviderName',  PA.Address AS 'ProviderAddress', S.Name AS 'ServiceName', S.Reference, 
S.SPINTLSID, S.PublicServiceName AS 'PublicName', S.ServiceDescription AS 'Description',
ST.Value AS 'ServiceType', S.ServiceTypeElement AS 'AccElement', S.VATExempt, 
SPP.Value AS 'Patch', S.SPService, S.IncludeInSPLSExtract, S.PipelineService, 
S.ServiceCommencementDate, S.ServiceDecommissionedDate AS 'DecommissionDate', S.NumberOfWeeksBasis, S.RuralOrUrban,
S.EmergencyReferralStatus AS 'EmergencyReferral', S.AccessToInterpreterStatus AS 'AccessToInterpreters',
S.VisualImpairmentStatus AS 'VisualImpairmentSupport', S.HearingImpairmentStatus AS 'HearingImpairmentSupport',
S.WaitingListInOperation, S.ImpactAssessment, S.RiskAssessment, SD.Value AS 'SupportDuration',
ACT.Value AS 'ServiceDelivery1', DL.Value AS 'ServiceDelivery2',
'ServiceLevel' = CASE S.ServiceLevel WHEN 1 THEN 'Uniform' WHEN 2 THEN 'Banded' WHEN 3 THEN 'Variable' END,
PCG.Value AS 'PrimaryClientGroup', SCG.Value AS 'SecondaryClientGroup', S.RelevantCulturalGroup, 
S.HouseholdUnitsAvailable, S.StandardHoursPerWeek, S.NumberPaidManagers, S.NumberPaidFrontLineStaff, 
S.NumberUnpaidManagers, S.NumberUnPaidFrontLineStaff, S.ConsultWithClientStatus AS 'MechanismsInPlace',
S.ConsultationFrequency, S.CurrentConsultationStrategy, S.HIANumberOfEnquiries, 
S.HIAHouseholdAssisted, S.HIAFeeIncome, S.ServiceChargeable, S.ExemptFromChargingReason,
'GrossOrSubsidy' = CASE S.GrossOrSubsidy WHEN 'S' THEN 'Subsidy' WHEN 'G' THEN 'Gross' END,
S.ODPMDesignatedService, S.ServiceReviewDate FROM SPService S
INNER JOIN SPProvider P ON P.ID = S.ProviderID
INNER JOIN SPServiceType ST ON ST.ID = S.ServiceTypeID
LEFT OUTER JOIN SPAddress PA ON PA.ID = P.ProviderAddressID
LEFT OUTER JOIN SPPatches SPP ON SPP.ID = S.PatchID
LEFT OUTER JOIN SPSupportDuration SD ON SD.ID = S.SupportDurationTypeID
LEFT OUTER JOIN SPAccommType ACT ON ACT.ID = S.AccommodationTypeID
LEFT OUTER JOIN SPDeliveryLocations DL ON DL.ID = S.DeliveryLocationID
LEFT OUTER JOIN SPClientGroups PCG ON PCG.ID = S.PrimaryClientGroupID
LEFT OUTER JOIN SPClientGroups SCG ON SCG.ID = S.SecondaryClientGroupID
WHERE S.ID = @ServiceID

SELECT SP.Value FROM SPSupportProvision SP
INNER JOIN SPServiceSupportProvisions SSP ON SSP.SupportProvisionID = SP.ID
WHERE SSP.ServiceID = @ServiceID

SELECT ET.Value FROM SPEligibleTasks ET
INNER JOIN SPServiceEligibleTasks SPSET ON SPSET.EligibleTaskID = ET.ID
WHERE SPSET.ServiceID = @ServiceID

SELECT ET.Value FROM SPNonEligibleTasks ET
INNER JOIN SPServiceNonEligibleTasks SPSET ON SPSET.NonEligibleTaskID = ET.ID
WHERE SPSET.ServiceID = @ServiceID

SELECT L.Value FROM SPLanguage L
INNER JOIN SPServiceLanguage SL ON SL.LanguageID = L.ID
WHERE SL.ServiceID = @ServiceID

SELECT R.Value FROM SPReligion R
INNER JOIN SPServiceReligion SR ON SR.ReligionID = R.ID
WHERE SR.ServiceID = @ServiceID

SELECT R.Value FROM SPReferralAccessRoutes R
INNER JOIN SPServiceReferralAccessRoutes SRR ON SRR.ReferralAccessRouteID = R.ID
WHERE SRR.ServiceID = @ServiceID

SELECT AR.Value FROM SPCGAgeRanges AR
INNER JOIN SPServiceCGAgeRanges SAR ON SAR.CGAgeRangeID = AR.ID
WHERE SAR.[Primary] = 1 AND SAR.ServiceID = @ServiceID

SELECT AR.Value FROM SPCGAgeRanges AR
INNER JOIN SPServiceCGAgeRanges SAR ON SAR.CGAgeRangeID = AR.ID
WHERE SAR.[Primary] = 0 AND SAR.ServiceID = @ServiceID

SELECT EO.Value FROM SPEthnicOrigin EO
INNER JOIN SPServiceEthnicOrigins SEO ON SEO.EthnicOriginID = EO.ID
WHERE SEO.ServiceID = @ServiceID

SELECT HT.Value FROM SPHouseholdType HT
INNER JOIN SPServiceHouseHoldTypes SHT ON SHT.HouseholdTypeID = HT.ID
WHERE SHT.ServiceID = @ServiceID

SELECT R.Value FROM SPReferralRoutes R
INNER JOIN SPServiceReferralRoutes SRR ON SRR.ReferralRouteID = R.ID
WHERE SRR.ServiceID = @ServiceID


SELECT CGE.Value  FROM SPCGExclusions CGE
INNER JOIN SPServiceCGExclusions SCGE ON SCGE.CGExclusionID = CGE.ID
WHERE SCGE.ServiceID = @ServiceID

SELECT UI.Value FROM SPUserInvolvement UI
INNER JOIN SPServiceUserInvolvement SUI ON SUI.UserInvolvementID = UI.ID
WHERE SUI.ServiceID = @ServiceID

SELECT D.Description AS 'District', CAST(SD.Numerator AS Varchar(5)) + '/' + CAST(SD.Denominator AS Varchar(5)) AS 'Proportion' FROM SPDistrict D
INNER JOIN SPServiceDistricts SD ON SD.DistrictONSCode = D.Value
WHERE SD.ServiceID = @ServiceID

SELECT A.Address AS 'Address', A.PostCode AS 'PostCode', AA.Description AS 'AdminAuthority', 
D.Description AS 'District', W.Description AS 'Ward', A.Directions AS 'Directions', 
A.UPRN AS 'UPRN', A.USRN AS 'USRN', A.ConfidentialAddress AS 'Confidential', A.DisabledAccess AS 'DisabledAccess',
C.TYPE AS 'ContactType', C.OrganisationName AS 'Organisation', C.Title AS 'Title', C.FirstNames AS 'FirstNames',
C.Surname AS 'Surname', C.Position AS 'Position', C.TelephoneNumber AS 'TelNo', C.FaxNumber AS 'FaxNo',
C.MobileNumber AS 'MobileNo', C.PagerNumber AS 'PagerNo', C.EmailAddress AS 'EmailAddress', C.WebAddress AS 'WebAddress',
S.ManagerAddressID AS 'AddressID', S.ManagerContactID AS 'ContactID' FROM SPService S
LEFT OUTER JOIN SPAddress A ON A.ID = S.ManagerAddressID
LEFT OUTER JOIN SPAdminAuthority AA ON AA.Value = A.AAONSCode
LEFT OUTER JOIN SPDistrict D ON D.Value = A.DistrictONSCode
LEFT OUTER JOIN SPWard W ON W.Value = A.WardONSCode
LEFT OUTER JOIN SPContact C ON C.ID = S.ManagerContactID
WHERE S.ID = @ServiceID

SELECT A.Address AS 'Address', A.PostCode AS 'PostCode', AA.Description AS 'AdminAuthority', 
D.Description AS 'District', W.Description AS 'Ward', A.Directions AS 'Directions', 
A.UPRN AS 'UPRN', A.USRN AS 'USRN', A.ConfidentialAddress AS 'Confidential', A.DisabledAccess AS 'DisabledAccess',
C.TYPE AS 'ContactType', C.OrganisationName AS 'Organisation', C.Title AS 'Title', C.FirstNames AS 'FirstNames',
C.Surname AS 'Surname', C.Position AS 'Position', C.TelephoneNumber AS 'TelNo', C.FaxNumber AS 'FaxNo',
C.MobileNumber AS 'MobileNo', C.PagerNumber AS 'PagerNo', C.EmailAddress AS 'EmailAddress', C.WebAddress AS 'WebAddress',
S.EmergencyAddressID AS 'AddressID', S.EmergencyContactID AS 'ContactID' FROM SPService S
LEFT OUTER JOIN SPAddress A ON A.ID = S.EmergencyAddressID
LEFT OUTER JOIN SPAdminAuthority AA ON AA.Value = A.AAONSCode
LEFT OUTER JOIN SPDistrict D ON D.Value = A.DistrictONSCode
LEFT OUTER JOIN SPWard W ON W.Value = A.WardONSCode
LEFT OUTER JOIN SPContact C ON C.ID = S.EmergencyContactID
WHERE S.ID = @ServiceID

SELECT A.Address AS 'Address', A.PostCode AS 'PostCode', AA.Description AS 'AdminAuthority', 
D.Description AS 'District', W.Description AS 'Ward', A.Directions AS 'Directions', 
A.UPRN AS 'UPRN', A.USRN AS 'USRN', A.ConfidentialAddress AS 'Confidential', A.DisabledAccess AS 'DisabledAccess',
C.TYPE AS 'ContactType', C.OrganisationName AS 'Organisation', C.Title AS 'Title', C.FirstNames AS 'FirstNames',
C.Surname AS 'Surname', C.Position AS 'Position', C.TelephoneNumber AS 'TelNo', C.FaxNumber AS 'FaxNo',
C.MobileNumber AS 'MobileNo', C.PagerNumber AS 'PagerNo', C.EmailAddress AS 'EmailAddress', C.WebAddress AS 'WebAddress',
S.ContractAddressID AS 'AddressID', S.ContractContactID AS 'ContactID' FROM SPService S
LEFT OUTER JOIN SPAddress A ON A.ID = S.ContractAddressID
LEFT OUTER JOIN SPAdminAuthority AA ON AA.Value = A.AAONSCode
LEFT OUTER JOIN SPDistrict D ON D.Value = A.DistrictONSCode
LEFT OUTER JOIN SPWard W ON W.Value = A.WardONSCode
LEFT OUTER JOIN SPContact C ON C.ID = S.ContractContactID
WHERE S.ID = @ServiceID

SELECT A.Address AS 'Address', A.PostCode AS 'PostCode', AA.Description AS 'AdminAuthority', 
D.Description AS 'District', W.Description AS 'Ward', A.Directions AS 'Directions', 
A.UPRN AS 'UPRN', A.USRN AS 'USRN', A.ConfidentialAddress AS 'Confidential', A.DisabledAccess AS 'DisabledAccess',
C.TYPE AS 'ContactType', C.OrganisationName AS 'Organisation', C.Title AS 'Title', C.FirstNames AS 'FirstNames',
C.Surname AS 'Surname', C.Position AS 'Position', C.TelephoneNumber AS 'TelNo', C.FaxNumber AS 'FaxNo',
C.MobileNumber AS 'MobileNo', C.PagerNumber AS 'PagerNo', C.EmailAddress AS 'EmailAddress', C.WebAddress AS 'WebAddress',
S.SelfReferralAddressID AS 'AddressID', S.SelfReferralContactID AS 'ContactID' FROM SPService S
LEFT OUTER JOIN SPAddress A ON A.ID = S.SelfReferralAddressID
LEFT OUTER JOIN SPAdminAuthority AA ON AA.Value = A.AAONSCode
LEFT OUTER JOIN SPDistrict D ON D.Value = A.DistrictONSCode
LEFT OUTER JOIN SPWard W ON W.Value = A.WardONSCode
LEFT OUTER JOIN SPContact C ON C.ID = S.SelfReferralContactID
WHERE S.ID = @ServiceID 

SELECT A.Address AS 'Address', A.PostCode AS 'PostCode', AA.Description AS 'AdminAuthority', 
D.Description AS 'District', W.Description AS 'Ward', A.Directions AS 'Directions', 
A.UPRN AS 'UPRN', A.USRN AS 'USRN', A.ConfidentialAddress AS 'Confidential', A.DisabledAccess AS 'DisabledAccess',
C.TYPE AS 'ContactType', C.OrganisationName AS 'Organisation', C.Title AS 'Title', C.FirstNames AS 'FirstNames',
C.Surname AS 'Surname', C.Position AS 'Position', C.TelephoneNumber AS 'TelNo', C.FaxNumber AS 'FaxNo',
C.MobileNumber AS 'MobileNo', C.PagerNumber AS 'PagerNo', C.EmailAddress AS 'EmailAddress', C.WebAddress AS 'WebAddress',
S.SelfReferral2AddressID AS 'AddressID', S.SelfReferral2ContactID AS 'ContactID' FROM SPService S
LEFT OUTER JOIN SPAddress A ON A.ID = S.SelfReferral2AddressID
LEFT OUTER JOIN SPAdminAuthority AA ON AA.Value = A.AAONSCode
LEFT OUTER JOIN SPDistrict D ON D.Value = A.DistrictONSCode
LEFT OUTER JOIN SPWard W ON W.Value = A.WardONSCode
LEFT OUTER JOIN SPContact C ON C.ID = S.SelfReferral2ContactID
WHERE S.ID = @ServiceID


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

