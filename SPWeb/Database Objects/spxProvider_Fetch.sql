if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[spxProvider_Fetch]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)਍ഀ
drop procedure [dbo].[spxProvider_Fetch]਍ഀ
GO਍ഀ
਍ഀ
SET QUOTED_IDENTIFIER ON ਍ഀ
GO਍ഀ
SET ANSI_NULLS OFF ਍ഀ
GO਍ഀ
਍ഀ
CREATE PROCEDURE dbo.spxProvider_Fetch (@ProviderID int)਍ഀ
਍ഀ
AS਍ഀ
਍ഀ
SELECT P.ID, P.Reference, P.LocalProviderID, P.Name, P.ProviderWebsite, OT.Value AS 'OrganisationType',਍ഀ
P.CreditorsReference, P.VATExempt, P.VATNumber, P.FinancialYearEnd, ਍ഀ
P.ServiceProvider, P.Landlord, P.AccomodationManager, AccreditationExemptStatus,਍ഀ
P.ProviderAddressID, P.ContactAddressID, P.BillingAddressID,਍ഀ
P.ProviderContactID,  P.ContactContactID, BillingContactID,਍ഀ
PA.Address AS 'PAAddress', PA.PostCode AS 'PAPostCode', PAA.Description AS 'PAAdminAuthority', ਍ഀ
PAD.Description AS 'PADistrict', PAW.Description AS 'PAWard', PA.Directions AS 'PADirections', ਍ഀ
PA.UPRN AS 'PAUPRN', PA.USRN AS 'PAUSRN', PA.ConfidentialAddress AS 'PAConfidential', PA.DisabledAccess AS 'PADisabledAccess',਍ഀ
CA.Address AS 'CAAddress', CA.PostCode AS 'CAPostCode', CAA.Description AS 'CAAdminAuthority', ਍ഀ
CAD.Description AS 'CADistrict', CAW.Description AS 'CAWard', CA.Directions AS 'CADirections', ਍ഀ
CA.UPRN AS 'CAUPRN', CA.USRN AS 'CAUSRN', CA.ConfidentialAddress AS 'CAConfidential', CA.DisabledAccess AS 'CADisabledAccess',਍ഀ
BA.Address AS 'BAAddress', BA.PostCode AS 'BAPostCode', BAA.Description AS 'BAAdminAuthority', ਍ഀ
BAD.Description AS 'BADistrict', BAW.Description AS 'BAWard', BA.Directions AS 'BADirections', ਍ഀ
BA.UPRN AS 'BAUPRN', BA.USRN AS 'BAUSRN', BA.ConfidentialAddress AS 'BAConfidential', BA.DisabledAccess AS 'BADisabledAccess',਍ഀ
PC.TYPE AS 'PCContactType', PC.OrganisationName AS 'PCOrganisation', PC.Title AS 'PCTitle', PC.FirstNames AS 'PCFirstNames',਍ഀ
PC.Surname AS 'PCSurname', PC.Position AS 'PCPosition', PC.TelephoneNumber AS 'PCTelNo', PC.FaxNumber AS 'PCFaxNo',਍ഀ
PC.MobileNumber AS 'PCMobileNo', PC.PagerNumber AS 'PCPagerNo', PC.EmailAddress AS 'PCEmailAddress', PC.WebAddress AS 'PCWebAddress',਍ഀ
CC.TYPE AS 'CCContactType', CC.OrganisationName AS 'CCOrganisation', CC.Title AS 'CCTitle', CC.FirstNames AS 'CCFirstNames',਍ഀ
CC.Surname AS 'CCSurname', CC.Position AS 'CCPosition', CC.TelephoneNumber AS 'CCTelNo', CC.FaxNumber AS 'CCFaxNo',਍ഀ
CC.MobileNumber AS 'CCMobileNo', CC.PagerNumber AS 'CCPagerNo', CC.EmailAddress AS 'CCEmailAddress', CC.WebAddress AS 'CCWebAddress',਍ഀ
BC.TYPE AS 'BCContactType', BC.OrganisationName AS 'BCOrganisation', BC.Title AS 'BCTitle', BC.FirstNames AS 'BCFirstNames',਍ഀ
BC.Surname AS 'BCSurname', BC.Position AS 'BCPosition', BC.TelephoneNumber AS 'BCTelNo', BC.FaxNumber AS 'BCFaxNo',਍ഀ
BC.MobileNumber AS 'BCMobileNo', BC.PagerNumber AS 'BCPagerNo', BC.EmailAddress AS 'BCEmailAddress', BC.WebAddress AS 'BCWebAddress'਍ഀ
FROM dbo.SPProvider P਍ഀ
LEFT OUTER JOIN SPProviderOrganisationType OT ON P.ProviderOrganisationTypeID = OT.ID਍ഀ
LEFT OUTER JOIN SPAddress PA ON PA.ID = P.ProviderAddressID਍ഀ
LEFT OUTER JOIN SPAddress CA ON CA.ID = P.ContactAddressID਍ഀ
LEFT OUTER JOIN SPAddress BA ON BA.ID = P.BillingAddressID਍ഀ
LEFT OUTER JOIN SPAdminAuthority PAA ON PAA.Value = PA.AAONSCode਍ഀ
LEFT OUTER JOIN SPAdminAuthority CAA ON CAA.Value = CA.AAONSCode਍ഀ
LEFT OUTER JOIN SPAdminAuthority BAA ON BAA.Value = BA.AAONSCode਍ഀ
LEFT OUTER JOIN SPDistrict PAD ON PAD.Value = PA.DistrictONSCode਍ഀ
LEFT OUTER JOIN SPDistrict CAD ON CAD.Value = CA.DistrictONSCode਍ഀ
LEFT OUTER JOIN SPDistrict BAD ON BAD.Value = BA.DistrictONSCode਍ഀ
LEFT OUTER JOIN SPWard PAW ON PAW.Value = PA.WardONSCode਍ഀ
LEFT OUTER JOIN SPWard CAW ON CAW.Value = CA.WardONSCode਍ഀ
LEFT OUTER JOIN SPWard BAW ON BAW.Value = BA.WardONSCode਍ഀ
LEFT OUTER JOIN SPContact PC ON PC.ID = P.ProviderContactID਍ഀ
LEFT OUTER JOIN SPContact CC ON CC.ID = P.ContactContactID਍ഀ
LEFT OUTER JOIN SPContact BC ON BC.ID = P.BillingContactID਍ഀ
WHERE P.ID = @ProviderID਍ഀ
਍ഀ
਍ഀ
SELECT P.BMESpecialistProvider, P.CulturalGroup, EO.Value AS 'SupportedEthnicOrigin' FROM SPProvider P਍ഀ
LEFT OUTER JOIN SPProviderEthnicOrigin PEO ON P.ID = PEO.ProviderID਍ഀ
LEFT OUTER JOIN SPEthnicOrigin EO ON EO.ID = PEO.EthnicOriginID਍ഀ
WHERE P.ID = @ProviderID਍ഀ
਍ഀ
SELECT RT.Value AS 'RegType', RegNumber, RegDate FROM SPProviderRegistrationDetails RD਍ഀ
INNER JOIN SPRegistrationTypes RT ON RT.ID = RD.RegTypeID਍ഀ
WHERE RD.ProviderID = @ProviderID਍ഀ
GO਍ഀ
SET QUOTED_IDENTIFIER OFF ਍ഀ
GO਍ഀ
SET ANSI_NULLS ON ਍ഀ
GO਍ഀ
਍ഀ
