/*********************************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[pr_AddExtendedProperty]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[pr_AddExtendedProperty]
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE  PROCEDURE [dbo].[pr_AddExtendedProperty]
	@name		SYSNAME,
	@value		SQL_VARIANT = NULL,
	@level1type	VARCHAR(128) = NULL,
	@level1name	SYSNAME = NULL,
	@level2type	VARCHAR(128) = NULL,
	@level2name	SYSNAME = NULL
AS

DECLARE @level0type	VARCHAR(128)
DECLARE @level0name	SYSNAME
SET @level0type = 'user'
SET @level0name = 'dbo'

-- if the property we are looking for already exists, drop it
IF (EXISTS (SELECT * FROM ::fn_listextendedproperty (@name, @level0type, @level0name, @level1type, @level1name, @level2type, @level2name)))
BEGIN 
	EXEC sp_dropextendedproperty @name=@name,   
					@level0type=@level0type, 
					@level0name=@level0name, 
					@level1type=@level1type,   
					@level1name=@level1name, 
					@level2type=@level2type,   
					@level2name=@level2name
END

-- create?
IF @value IS NOT NULL
BEGIN
	EXEC sp_addextendedproperty @name=@name,
					@value=@value,
					@level0type=@level0type, 
					@level0name=@level0name, 
					@level1type=@level1type,   
					@level1name=@level1name, 
					@level2type=@level2type,   
					@level2name=@level2name
END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
/*********************************************************************************************************/

-- ApplicationSetting
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [ApplicationSetting], N'column', [ApplicationID]
GO

-- AuditLogIDLookup
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [AuditLogIDLookup], N'column', [ApplicationID]
GO











-- vwWebEmailSenderMessage_WebEmailSenderRecipient
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'view', [vwWebEmailSenderMessage_WebEmailSenderRecipient], N'column', [MessageID]
GO
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'view', [vwWebEmailSenderMessage_WebEmailSenderRecipient], N'column', [Status]
GO

-- vwWebMsgDistribListMember
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'view', [vwWebMsgDistribListMember], N'column', [WebMsgDistribListID]
GO

-- vwWebSecurityUser_WebNavMenuItem
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'view', [vwWebSecurityUser_WebNavMenuItem], N'column', [WebSecurityUserID]
GO
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'view', [vwWebSecurityUser_WebNavMenuItem], N'column', [WebNavMenuItemVisibility]
GO

-- vwWebSecurityUser_WebSecurityItem
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'view', [vwWebSecurityUser_WebSecurityItem], N'column', [WebSecurityUserID]
GO
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'view', [vwWebSecurityUser_WebSecurityItem], N'column', [WebSecurityItemID]
GO

-- WebAmendReqDataField
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebAmendReqDataField], N'column', [WebAmendReqDataItemID]
GO

-- WebAmendReqDataItem
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebAmendReqDataItem], N'column', [EditMode]
GO
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebAmendReqDataItem], N'column', [Entity]
GO

-- WebAmendReqDataItem_WebSecurityUser
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebAmendReqDataItem_WebSecurityUser], N'column', [WebSecurityExternalUserID]
GO

-- WebCMSFolder
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebCMSFolder], N'column', [ParentFolderID]
GO

-- WebCMSPage
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebCMSPage], N'column', [WebCMSFolderID]
GO

-- WebEmailSenderRecipient
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebEmailSenderRecipient], N'column', [Status]
GO
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebEmailSenderRecipient], N'column', [WebEmailSenderMessageID]
GO

-- WebFileStoreFile
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebFileStoreFile], N'column', [Height]
GO
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebFileStoreFile], N'column', [WebFileStoreFolderID]
GO
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebFileStoreFile], N'column', [Width]
GO

-- WebFileStoreFolder
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebFileStoreFolder], N'column', [ParentFolderID]
GO

-- WebFileStoreImageVersion
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebFileStoreImageVersion], N'column', [Height]
GO
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebFileStoreImageVersion], N'column', [WebFileStoreFileID]
GO
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebFileStoreImageVersion], N'column', [Width]
GO

-- WebMsgConversation_WebMsgLabel
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebMsgConversation_WebMsgLabel], N'column', [WebMsgConversationID]
GO
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebMsgConversation_WebMsgLabel], N'column', [WebMsgLabelID]
GO

-- WebMsgLabel
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebMsgLabel], N'column', [WebSecurityCompanyID]
GO

-- WebMsgMessage
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebMsgMessage], N'column', [WebMsgConversationID]
GO

-- WebMsgMessage_WebFileStoreData
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebMsgMessage_WebFileStoreData], N'column', [WebMsgMessageID]
GO

-- WebMsgReadStatus
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebMsgReadStatus], N'column', [MsgID]
GO
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebMsgReadStatus], N'column', [UserID]
GO

-- WebNavMenuItem
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebNavMenuItem], N'column', [ParentID]
GO

-- WebNavMenuItem_WebSecurityItem
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebNavMenuItem_WebSecurityItem], N'column', [WebNavMenuItemID]
GO
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebNavMenuItem_WebSecurityItem], N'column', [WebSecurityItemID]
GO

-- WebSecurityArea_WebSecurityRole
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebSecurityArea_WebSecurityRole], N'column', [WebSecurityAreaID]
GO
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebSecurityArea_WebSecurityRole], N'column', [WebSecurityRoleID]
GO

-- WebSecurityCompany
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebSecurityCompany], N'column', [Status]
GO
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebSecurityCompany], N'column', [Name]
GO

-- WebSecurityCompanyOffice
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebSecurityCompanyOffice], N'column', [WebSecurityCompanyID]
GO

-- WebSecurityItem_WebSecurityArea
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebSecurityItem_WebSecurityArea], N'column', [WebSecurityAreaID]
GO
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebSecurityItem_WebSecurityArea], N'column', [WebSecurityItemID]
GO

-- WebSecurityRole
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebSecurityRole], N'column', [Name]
GO

-- WebSecurityUser
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebSecurityUser], N'column', [Email]
GO
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebSecurityUser], N'column', [Status]
GO
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebSecurityUser], N'column', [WebSecurityCompanyID]
GO
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebSecurityUser], N'column', [WebSecurityCompanyOfficeID]
GO
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebSecurityUser], N'column', [ExternalUserID]
GO
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebSecurityUser], N'column', [ClonedFromUserID]
GO
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebSecurityUser], N'column', [ApplicationID]
GO

-- WebSecurityUser_WebSecurityRole
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebSecurityUser_WebSecurityRole], N'column', [WebSecurityRoleID]
GO
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebSecurityUser_WebSecurityRole], N'column', [WebSecurityUserID]
GO

-- WebSecurityUserPasswordHistory
EXEC pr_AddExtendedProperty N'MS_Description', N'FetchListFilter', N'table', [WebSecurityUserPasswordHistory], N'column', [WebSecurityUserID]
GO


/*********************************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[pr_AddExtendedProperty]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[pr_AddExtendedProperty]
GO
/*********************************************************************************************************/