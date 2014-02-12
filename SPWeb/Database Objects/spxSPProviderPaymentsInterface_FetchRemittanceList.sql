if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[spxSPProviderPaymentsInterface_FetchRemittanceList]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[spxSPProviderPaymentsInterface_FetchRemittanceList]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


CREATE  PROCEDURE [dbo].[spxSPProviderPaymentsInterface_FetchRemittanceList]
	@intInterfaceLogID	INT
AS

SELECT r.[ID]
FROM SPRemittance AS r
	INNER JOIN SPContractHeader AS c ON (r.ContractID = c.[ID])
WHERE r.ProviderInterfaceLogID = @intInterfaceLogID
AND c.GrossOrSubsidy = 'S'


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

