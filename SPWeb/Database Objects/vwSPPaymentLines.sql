if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[vwSPPaymentLines]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[vwSPPaymentLines]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO






CREATE VIEW dbo.vwSPPaymentLines
 AS
SELECT R.ProviderID AS 'ContractProviderID', R.ContractID, RD.RemittanceID,
    'Remittance_Ref' = R.Reference,
    'Remittance_Date_Created' = R.DateCreated,
    'Batch_Ref' = R.BatchRef,
    'Line_Type' = RD.Type, 
    'Line_Comment' = REPLACE(CAST(RD.Comment AS VARCHAR(8000)), CHAR(13) + CHAR(10), ' ') ,
    'Line_Date_From' = RD.PeriodFrom, 
    'Line_Date_To' = RD.PeriodTo,
    'Line_Ref' = RD.LineReference, 
    'Line_Value' = RD.LineValue, 
    'Line_VAT' = RD.LineVAT,
    'Cost_Centre' =  ISNULL(CR.CostCentre, ''), 
    'Subjective_Code' =  ISNULL(CR.SubjectiveCode, ''),
    'Subsidy_YN' = CASE WHEN RD.Type = 5 THEN 1 ELSE 0 END, 
    'Adjustment_YN' = CASE WHEN RD.Type = 6 OR RD.Type = 7 OR RD.Type = 8 THEN 1 ELSE 0 END, 
    'Adjustment_Reason' = CASE WHEN RD.TYPE = 4 THEN '' ELSE ISNULL(SUBADJDESC.Description, '')END,
    'Adjustment_Class' = CASE WHEN RD.TYPE = 4 THEN '' ELSE ISNULL(SUBADJDESC.Class, '')END,
    'ServiceAgreementID' = CASE WHEN RD.TYPE = 4 THEN 0 ELSE ISNULL(SERAG.ID, 0) END,
    'District' = CASE WHEN RD.TYPE = 4 THEN '' ELSE ISNULL(DIST.Description, '')END,
    'Service_Agreement_Ref' = CASE WHEN RD.TYPE = 4 THEN '' ELSE ISNULL(SERAG.Reference, '') END,
    'Provider_Agreement_Ref' = CASE WHEN RD.TYPE = 4 THEN '' ELSE ISNULL(SERAG.AltReference, '') END,
    'ClientID' = CASE WHEN RD.TYPE = 4 THEN 0 ELSE ISNULL(CD.ID, 0) END,
    'Client_Ref' = CASE WHEN RD.TYPE = 4 THEN '' ELSE ISNULL(CD.Reference, '') END,
    'Client_Surname' = CASE WHEN RD.TYPE = 4 THEN '' ELSE ISNULL(CD.LastName, '') END,
    'Client_Forenames' = CASE WHEN RD.TYPE = 4 THEN '' ELSE ISNULL(CD.FirstNames, '') END,
    'Client_Title' = CASE WHEN RD.TYPE = 4 THEN '' ELSE ISNULL(vwTitle.Description, '') END,
    'Client_NI_Number' = CASE WHEN RD.TYPE = 4 THEN '' ELSE ISNULL(CD.NINO, '') END,
    'Tenancy_YN' = CASE WHEN RD.TYPE = 4 THEN 0 ELSE ISNULL(SERAG.TenancyAgreement, 0) END,
    'Section_117_YN' = CASE WHEN RD.TYPE = 4 THEN 0 ELSE ISNULL(SERAG.Section117, 0) END,
    'Care_Leaver_YN' = CASE WHEN RD.TYPE = 4 THEN 0 ELSE ISNULL(SERAG.CareLeaver, 0) END, 
    'Transitional_Protection_YN' = CASE WHEN RD.TYPE = 4 THEN '' ELSE ISNULL(SERAG.TransitionalProtection, '') END,
    'Cross_Authority' = CASE WHEN RD.TYPE = 4 THEN '' ELSE ISNULL(ADMIN.Description, '') END,
    'PropertyID' = CASE WHEN RD.TYPE = 4 THEN 0 ELSE ISNULL(PROP.ID, 0) END,
    'Property_Ref' = CASE WHEN RD.TYPE = 4 THEN '' ELSE ISNULL(PROP.Reference, '') END,
    'Property_Name' = CASE WHEN RD.TYPE = 4 THEN '' ELSE ISNULL(PROP.Name, '') END,
    'Property_Address' = CASE WHEN RD.TYPE = 4 THEN '' ELSE ISNULL(PROPADDR.Address, '') END,
    'Property_Postcode' = CASE WHEN RD.TYPE = 4 THEN '' ELSE ISNULL(PROPADDR.Postcode, '') END,
    'SubsidyAgreementID' = CASE WHEN RD.TYPE = 4 THEN 0 ELSE ISNULL(SUBAG.ID, 0) END,
    'Subsidy_Weekly' = CASE WHEN RD.TYPE = 4 THEN 0 ELSE ISNULL(SUBAG.Subsidy, 0) END,
    'Subsidy_VAT_Weekly' = CASE WHEN RD.TYPE = 4 THEN 0 ELSE ISNULL(SUBAG.VAT, 0) END,
    'Transitional_HB_YN' = CASE WHEN RD.TYPE = 4 THEN 0 ELSE ISNULL(SUBAG.TransitionalHB, 0) END,
    'HB_Award_YN' = CASE WHEN RD.TYPE = 4 THEN 0 ELSE ISNULL(SUBAG.InReceiptOfHB, 0) END,
    'FC_Assessment_YN' = CASE WHEN RD.TYPE = 4 THEN 0 ELSE ISNULL(SUBAG.FCAssessment, 0)END,
    'Subsidy_Status' = CASE WHEN RD.TYPE = 4 THEN '' ELSE ISNULL(SUBSTAT.Description, '') END,
   'Remittance_Line_Order' = 
	CASE 
	    WHEN RD.Type = 9 THEN '0000000000'
	    WHEN RD.Type = 1 OR RD.Type = 2 OR RD.Type = 3 THEN '0000000001'
	    WHEN RD.Type = 4 THEN '0000000002'
	    WHEN RD.Type = 5 OR RD.Type = 6 OR RD.Type = 7 OR RD.Type = 8 THEN CD.Name
	END,
  'ServiceID' = CASE WHEN RD.TYPE = 4 THEN 0 ELSE ISNULL(SERAG.ServiceID, 0) END
 FROM SPRemittance R
  INNER JOIN SPRemittanceDetail RD ON RD.RemittanceID = R.ID
  INNER JOIN SPContractHeader CH ON R.ContractID = CH.ID
  LEFT OUTER JOIN SPContractRate CR ON RD.ContractRateID = CR.ID
  LEFT OUTER JOIN SPSubsidyAgreement SUBAG ON RD.RelatedID = SUBAG.ID
  LEFT OUTER JOIN SPServiceAgreement SERAG ON SUBAG.ServiceAgreementID = SERAG.ID
  LEFT OUTER JOIN SPServiceDistricts SDIST ON SERAG.ServiceDistrictID = SDIST.ID
  LEFT OUTER JOIN SPDistrict DIST ON SDIST.DistrictONSCode = DIST.[Value]
  LEFT OUTER JOIN ClientDetail CD ON SERAG.ClientID = CD.ID
  LEFT OUTER JOIN vwTitle ON CD.Title = vwTitle.ID
  LEFT OUTER JOIN SPProperty PROP ON SERAG.PropertyID = PROP.ID
  LEFT OUTER JOIN SPSubsidyAdjustment SUBADJ ON SUBADJ.RemittanceDetailID = RD.ID
  LEFT OUTER JOIN SPSubsidyAdjustmentReason SUBADJDESC ON SUBADJ.ReasonID = SUBADJDESC.ID
  LEFT OUTER JOIN SPSubsidyAgreementStatus SUBSTAT ON SUBAG.Status = SUBSTAT.Value
  LEFT OUTER JOIN SPAddress PROPADDR ON PROP.AddressID = PROPADDR.ID
  LEFT OUTER JOIN SPAdminAuthority ADMIN ON ADMIN.ID = SERAG.CrossAuthority













GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

