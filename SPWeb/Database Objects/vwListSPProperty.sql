if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[vwListSPProperty]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[vwListSPProperty]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE VIEW dbo.vwListSPProperty
AS
SELECT     P.ID AS 'PropertyID', P.Reference, P.AltRef, P.Name, A.Address, A.PostCode, S.Name AS Service
FROM         dbo.SPProperty P INNER JOIN
                      dbo.SPServiceProperty SP ON SP.PropertyID = P.ID INNER JOIN
                      dbo.SPService S ON S.ID = SP.ServiceID LEFT OUTER JOIN
                      dbo.SPAddress A ON A.ID = P.AddressID



GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

