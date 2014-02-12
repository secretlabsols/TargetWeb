if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[spxSPProperty_Fetch]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[spxSPProperty_Fetch]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE spxSPProperty_Fetch(@PropertyID int)

AS

SELECT P.ID AS 'PropertyID', P.Name AS 'PropertyName', P.Reference AS 'Reference', P.AltRef,
RS.Value AS 'RegistrationStatus', P.RegDate, P.DeRegistered, FT.Value AS 'FurnishingType',
P.WheelChairUnits, P.MobilityUnits, P.AidsAdaptionsUnits, P.TotalHouseholdUnits, 
P.MealsSuppliedStatus, P.CanteenOnSiteStatus, P.SelfCateringStatus, P.DistanceToShops, 
P.DistanceToPostOffice, P.DistanceToTrainStation, P.DistanceToBusStop,  P.DistanceToTownCentre, 
P.DistanceToSocialCentre, P.DistanceToGP, P.DistanceToPrimarySchool, P.DistanceToSecondarySchool,
A.Address,  A.PostCode,  A.ConfidentialAddress,  A.DisabledAccess,  A.Directions,  A.UPRN,  A.USRN,
AA.Description AS 'AdminAuthority', D.Description AS 'District', W.Description AS 'Ward',
C.Type AS 'ContactType', C.Title, C.FirstNames, C.Surname, C.Position, C.OrganisationName, C.TelephoneNumber,
C.FaxNumber, C.MobileNumber, C.PagerNumber, C.EmailAddress, C.WebAddress,
SP.Name AS 'ServiceProvider', SPA.Address AS 'SPAddress', SPA.PostCode AS 'SPPostCode',
AM.Name AS 'AccomodationManager', AMA.Address AS 'AMAddress', AMA.PostCode AS 'AMPostCode',
L.Name AS 'Landlord', LA.Address AS 'LAddress', LA.PostCode AS 'LPostCode', P.AddressID, P.ContactID
FROM SPProperty P
LEFT OUTER  JOIN SPRegistrationStatus RS ON RS.ID = P.RegStatusID
LEFT OUTER JOIN SPFurnishingType FT ON FT.ID = P.FurnishingTypeID
LEFT OUTER JOIN SPAddress A ON A.ID = P.AddressID
LEFT OUTER JOIN SPWard W ON W.Value = A.WardONSCode
LEFT OUTER JOIN SPAdminAuthority AA ON AA.Value = A.AAONSCode
LEFT OUTER JOIN SPDistrict D ON D.Value = A.DistrictONSCode
LEFT OUTER JOIN SPContact C ON C.ID = P.ContactID
LEFT OUTER JOIN SPProvider SP ON SP.ID = P.ProviderID
LEFT OUTER JOIN SPAddress SPA ON SPA.ID = SP.ProviderAddressID
LEFT OUTER JOIN SPProvider AM ON AM.ID = P.AccomManagerID
LEFT OUTER JOIN SPAddress AMA ON AMA.ID = AM.ProviderAddressID
LEFT OUTER JOIN SPProvider L ON L.ID = P.LandlordID
LEFT OUTER JOIN SPAddress LA ON LA.ID = L.ProviderAddressID
WHERE P.ID = @PropertyID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

