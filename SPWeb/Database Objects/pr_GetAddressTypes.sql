if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[pr_GetAddressTypes]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)਍ഀ
drop procedure [dbo].[pr_GetAddressTypes]਍ഀ
GO਍ഀ
਍ഀ
SET QUOTED_IDENTIFIER OFF ਍ഀ
GO਍ഀ
SET ANSI_NULLS OFF ਍ഀ
GO਍ഀ
਍ഀ
CREATE PROCEDURE pr_GetAddressTypes਍ഀ
਍ഀ
AS਍ഀ
਍ഀ
SELECT * FROM vwAddressType਍ഀ
GO਍ഀ
SET QUOTED_IDENTIFIER OFF ਍ഀ
GO਍ഀ
SET ANSI_NULLS ON ਍ഀ
GO਍ഀ
਍ഀ
