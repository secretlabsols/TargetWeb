if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[spxServiceUserAddress_Fetch]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[spxServiceUserAddress_Fetch]
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE spxServiceUserAddress_Fetch(@ClientID int, @AddressTypeID int)

AS

SELECT Title, Surname, Address, PostCode, Phone, Relation FROM ClientAddress
WHERE ClientID = @ClientId AND TypeID = @AddressTypeID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

