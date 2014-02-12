if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[pr_FetchSPCouncilDetails]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[pr_FetchSPCouncilDetails]
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO



CREATE  PROCEDURE pr_FetchSPCouncilDetails
AS
SELECT TOP 1
   si.SiteName AS [Council Name], a.Address AS [Council Address], a.PostCode AS [Council Postcode],
   c.Title AS [Title], c.Firstnames AS [FirstName], c.Surname AS [Surname], c.TelephoneNumber AS [Council Phone],
   c.FaxNumber AS [Council Fax], c.EmailAddress AS [Council Email], aa.Description AS [Council Admin Authority]
FROM SystemInfo si
LEFT OUTER JOIN SPAddress a ON a.ID = si.SPCouncilAddressID
LEFT OUTER JOIN SPContact c ON c.ID = si.SPCouncilContactID
LEFT OUTER JOIN SPAdminAuthority aa ON aa.Value = a.AAONSCode


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

