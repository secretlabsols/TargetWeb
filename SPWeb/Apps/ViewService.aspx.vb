Imports System.Configuration.ConfigurationManager
Imports System
Imports System.Text
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library
Imports Target.Web.Apps.AmendReq
Imports Target.Web.Apps.AmendReq.Collections
Imports System.Data.SqlClient
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security
Imports Target.SP.Library

Namespace Apps

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.ViewService
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Views service details
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO  03/12/2007  SPBG-341 - also used as logic error.
    '''     MikeVO  05/10/2007  Ported to use AjaxToolkit Tab control.       
    '''     MikeVO  23/04/2007  SPBG-313 - disabling of "also used as" addresses/contacts.
    '''     MikeVO  08/03/2007  Hide Amend Req buttons based on security item.
    '''     MikeVO  26/01/2007  SPBG-305 - handle when GrossOrSubsidy flag is null.
    '''     MikeVO  09/01/2007  SPBG-301 - fixed various layout issues.
    '''                         Hide Districts tab if not a 2-tier authority.
    '''     MikeVO  18/12/2006  Default dateFrom param given to Subsidies screen to today.
    '''     MikeVO  27/11/2006  Check user is allowed to view the service.
    '''     MikeVO  26/10/2006  Fixes for amendment requests.
    '''     MikeVO  24/10/2006  Various UI fixes.
    ''' 	[paul]	28/09/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class ViewService
        Inherits Target.Web.Apps.BasePage

        Protected WithEvents PropertySelector1 As Target.SP.Web.Apps.UserControls.PropertySelector
        Protected WithEvents managerAddress As Target.SP.Web.Apps.UserControls.AddressContact
        Protected WithEvents emergencyAddress As Target.SP.Web.Apps.UserControls.AddressContact
        Protected WithEvents contractAddress As Target.SP.Web.Apps.UserControls.AddressContact
        Protected WithEvents selfReferralAddress As Target.SP.Web.Apps.UserControls.AddressContact
        Protected WithEvents selfReferralAddress2 As Target.SP.Web.Apps.UserControls.AddressContact

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim strStyle As New StringBuilder
            Dim serviceID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim editMode As Boolean = Convert.ToBoolean(Target.Library.Utils.ToInt32(Request.QueryString("editMode")))
            Dim processedFieldCount As Integer = Target.Library.Utils.ToInt32(Request.QueryString("processedFieldCount"))
            Dim reader As SqlDataReader = Nothing
            Dim msg As ErrorMessage = New ErrorMessage
            Dim bOutputList As Boolean
            Dim bULOutput As Boolean
            Dim strList As New StringBuilder
            Dim strUrl As String
            Dim settings As WebAmendReqDataItemCollection = Nothing
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim managerAddressID As Integer, emergencyAddressID As Integer, contractAddressID As Integer
            Dim selfReferralAddressID As Integer, selfReferralAddress2ID As Integer
            Dim managerContactID As Integer, emergencyContactID As Integer, contractContactID As Integer
            Dim selfReferralContactID As Integer, selfReferralContact2ID As Integer
            Dim emergencyShowSameAs, ContractShowSameAs, selfReferralShowSameAs, selfReferral2ShowSameAs As Boolean
            Dim canView As Boolean, canViewAmendReqs As Boolean
            Dim adminAuthorityType As Byte
            Dim newAlsoUsedAsItem As ListItem

            Const SP_NAME As String = "spxSPService_Fetch"

            Me.InitPage(ConstantsManager.GetConstant("webSecurityItemSPServiceView"), "Service Details")

            ' check user is allowed to view this service
            msg = SPClassesBL.UserCanViewService(Me.DbConnection, user.ExternalUserID, serviceID, canView)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            If Not canView Then Response.Redirect("~/Library/Errors/AccessDenied.aspx")

            strStyle.Append(".label { float:left; width:19em; padding-right:1em; font-weight: bold; }")
            strStyle.Append("ul.list { margin:0em;padding:0em;list-style:none; }")
            strStyle.Append(".content { float:left; width:60%; }")
            Me.AddExtraCssStyle(strStyle.ToString)

            canViewAmendReqs = SecurityBL.UserHasItem(Me.DbConnection, user.ID, ConstantsManager.GetConstant("webSecurityItemAmendReqListRequests"))

            ' get the admin authority
            msg = SPClassesBL.GetAdminAuthorityType(ConnectionStrings("Abacus").ConnectionString, AppSettings("SiteID"), "", "", adminAuthorityType)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' setup provider address control
            With managerAddress
                ' address
                .InitControl(Me.DbConnection, managerAddress.ID, True)
                .Address.EditableDataItemConstant = "SPamendReqDataItemServiceManagerAddress"
                .Address.EditableDataFieldConstant = "SPamendReqDataFieldServiceManagerAddressAddress"
                .Postcode.EditableDataItemConstant = "SPamendReqDataItemServiceManagerAddress"
                .Postcode.EditableDataFieldConstant = "SPamendReqDataFieldServiceManagerAddressPostcode"
                .AdminAuthority.EditableDataItemConstant = "SPamendReqDataItemServiceManagerAddressWard"
                .AdminAuthority.EditableDataFieldConstant = "SPamendReqDataFieldServiceManagerAddressWardAdminAuthority"
                .District.EditableDataItemConstant = "SPamendReqDataItemServiceManagerAddressWard"
                .District.EditableDataFieldConstant = "SPamendReqDataFieldServiceManagerAddressWardDistrict"
                .Ward.EditableDataItemConstant = "SPamendReqDataItemServiceManagerAddressWard"
                .Ward.EditableDataFieldConstant = "SPamendReqDataFieldServiceManagerAddressWardWard"
                .Directions.EditableDataItemConstant = "SPamendReqDataItemServiceManagerAddress"
                .Directions.EditableDataFieldConstant = "SPamendReqDataFieldServiceManagerAddressDirections"
                .DisabledAccess.EditableDataItemConstant = "SPamendReqDataItemServiceManagerAddress"
                .DisabledAccess.EditableDataFieldConstant = "SPamendReqDataFieldServiceManagerAddressDisabledAccess"
                .AlsoUsedAs.EditableDataItemConstant = "SPamendReqDataItemServiceManagerAddress"
                .AlsoUsedAs.EditableDataFieldConstant = "SPamendReqDataFieldServiceManagerAddressAlsoUsedAs"
                .UPRN.EditableDataItemConstant = "SPamendReqDataItemServiceManagerAddressURNs"
                .UPRN.EditableDataFieldConstant = "SPamendReqDataFieldServiceManagerAddressURNsUPRN"
                .USRN.EditableDataItemConstant = "SPamendReqDataItemServiceManagerAddressURNs"
                .USRN.EditableDataFieldConstant = "SPamendReqDataFieldServiceManagerAddressURNsUSRN"
                ' contact
                .ContactType.EditableDataItemConstant = "SPamendReqDataItemServiceManagerAddressContact"
                .ContactType.EditableDataFieldConstant = "SPamendReqDataFieldServiceManagerAddressContactType"
                .ContactOrganisation.EditableDataItemConstant = "SPamendReqDataItemServiceManagerAddressContact"
                .ContactOrganisation.EditableDataFieldConstant = "SPamendReqDataFieldServiceManagerAddressContactOrganisation"
                .ContactTitle.EditableDataItemConstant = "SPamendReqDataItemServiceManagerAddressContact"
                .ContactTitle.EditableDataFieldConstant = "SPamendReqDataFieldServiceManagerAddressContactTitle"
                .ContactForenames.EditableDataItemConstant = "SPamendReqDataItemServiceManagerAddressContact"
                .ContactForenames.EditableDataFieldConstant = "SPamendReqDataFieldServiceManagerAddressContactForenames"
                .ContactSurname.EditableDataItemConstant = "SPamendReqDataItemServiceManagerAddressContact"
                .ContactSurname.EditableDataFieldConstant = "SPamendReqDataFieldServiceManagerAddressContactSurname"
                .ContactPosition.EditableDataItemConstant = "SPamendReqDataItemServiceManagerAddressContact"
                .ContactPosition.EditableDataFieldConstant = "SPamendReqDataFieldServiceManagerAddressContactPosition"
                .ContactTel.EditableDataItemConstant = "SPamendReqDataItemServiceManagerAddressContact"
                .ContactTel.EditableDataFieldConstant = "SPamendReqDataFieldServiceManagerAddressContactTelephoneNo"
                .ContactFax.EditableDataItemConstant = "SPamendReqDataItemServiceManagerAddressContact"
                .ContactFax.EditableDataFieldConstant = "SPamendReqDataFieldServiceManagerAddressContactFaxNo"
                .ContactMobile.EditableDataItemConstant = "SPamendReqDataItemServiceManagerAddressContact"
                .ContactMobile.EditableDataFieldConstant = "SPamendReqDataFieldServiceManagerAddressContactMobileNo"
                .ContactPager.EditableDataItemConstant = "SPamendReqDataItemServiceManagerAddressContact"
                .ContactPager.EditableDataFieldConstant = "SPamendReqDataFieldServiceManagerAddressContactPagerNo"
                .ContactEmail.EditableDataItemConstant = "SPamendReqDataItemServiceManagerAddressContact"
                .ContactEmail.EditableDataFieldConstant = "SPamendReqDataFieldServiceManagerAddressContactEmailAddress"
                .ContactWeb.EditableDataItemConstant = "SPamendReqDataItemServiceManagerAddressContact"
                .ContactWeb.EditableDataFieldConstant = "SPamendReqDataFieldServiceManagerAddressContactWebAddress"
                .ContactAlsoUsedAs.EditableDataItemConstant = "SPamendReqDataItemServiceManagerAddressContact"
                .ContactAlsoUsedAs.EditableDataFieldConstant = "SPamendReqDataFieldServiceManagerAddressContactAlsoUsedAs"
            End With

            With emergencyAddress
                ' address
                .InitControl(Me.DbConnection, emergencyAddress.ID, True)
                .Address.EditableDataItemConstant = "SPamendReqDataItemServiceEmergencyAddress"
                .Address.EditableDataFieldConstant = "SPamendReqDataFieldServiceEmergencyAddressAddress"
                .Postcode.EditableDataItemConstant = "SPamendReqDataItemServiceEmergencyAddress"
                .Postcode.EditableDataFieldConstant = "SPamendReqDataFieldServiceEmergencyAddressPostcode"
                .AdminAuthority.EditableDataItemConstant = "SPamendReqDataItemServiceEmergencyAddressWard"
                .AdminAuthority.EditableDataFieldConstant = "SPamendReqDataFieldServiceEmergencyAddressWardAdminAuthority"
                .District.EditableDataItemConstant = "SPamendReqDataItemServiceEmergencyAddressWard"
                .District.EditableDataFieldConstant = "SPamendReqDataFieldServiceEmergencyAddressWardDistrict"
                .Ward.EditableDataItemConstant = "SPamendReqDataItemServiceEmergencyAddressWard"
                .Ward.EditableDataFieldConstant = "SPamendReqDataFieldServiceEmergencyAddressWardWard"
                .Directions.EditableDataItemConstant = "SPamendReqDataItemServiceEmergencyAddress"
                .Directions.EditableDataFieldConstant = "SPamendReqDataFieldServiceEmergencyAddressDirections"
                .DisabledAccess.EditableDataItemConstant = "SPamendReqDataItemServiceEmergencyAddress"
                .DisabledAccess.EditableDataFieldConstant = "SPamendReqDataFieldServiceEmergencyAddressDisabledAccess"
                .AlsoUsedAs.EditableDataItemConstant = "SPamendReqDataItemServiceEmergencyAddress"
                .AlsoUsedAs.EditableDataFieldConstant = "SPamendReqDataFieldServiceEmergencyAddressAlsoUsedAs"
                .UPRN.EditableDataItemConstant = "SPamendReqDataItemServiceEmergencyAddressURNs"
                .UPRN.EditableDataFieldConstant = "SPamendReqDataFieldServiceEmergencyAddressURNsUPRN"
                .USRN.EditableDataItemConstant = "SPamendReqDataItemServiceEmergencyAddressURNs"
                .USRN.EditableDataFieldConstant = "SPamendReqDataFieldServiceEmergencyAddressURNsUSRN"
                ' contact
                .ContactType.EditableDataItemConstant = "SPamendReqDataItemServiceEmergencyAddressContact"
                .ContactType.EditableDataFieldConstant = "SPamendReqDataFieldServiceEmergencyAddressContactType"
                .ContactOrganisation.EditableDataItemConstant = "SPamendReqDataItemServiceEmergencyAddressContact"
                .ContactOrganisation.EditableDataFieldConstant = "SPamendReqDataFieldServiceEmergencyAddressContactOrganisation"
                .ContactTitle.EditableDataItemConstant = "SPamendReqDataItemServiceEmergencyAddressContact"
                .ContactTitle.EditableDataFieldConstant = "SPamendReqDataFieldServiceEmergencyAddressContactTitle"
                .ContactForenames.EditableDataItemConstant = "SPamendReqDataItemServiceEmergencyAddressContact"
                .ContactForenames.EditableDataFieldConstant = "SPamendReqDataFieldServiceEmergencyAddressContactForenames"
                .ContactSurname.EditableDataItemConstant = "SPamendReqDataItemServiceEmergencyAddressContact"
                .ContactSurname.EditableDataFieldConstant = "SPamendReqDataFieldServiceEmergencyAddressContactSurname"
                .ContactPosition.EditableDataItemConstant = "SPamendReqDataItemServiceEmergencyAddressContact"
                .ContactPosition.EditableDataFieldConstant = "SPamendReqDataFieldServiceEmergencyAddressContactPosition"
                .ContactTel.EditableDataItemConstant = "SPamendReqDataItemServiceEmergencyAddressContact"
                .ContactTel.EditableDataFieldConstant = "SPamendReqDataFieldServiceEmergencyAddressContactTelephoneNo"
                .ContactFax.EditableDataItemConstant = "SPamendReqDataItemServiceEmergencyAddressContact"
                .ContactFax.EditableDataFieldConstant = "SPamendReqDataFieldServiceEmergencyAddressContactFaxNo"
                .ContactMobile.EditableDataItemConstant = "SPamendReqDataItemServiceEmergencyAddressContact"
                .ContactMobile.EditableDataFieldConstant = "SPamendReqDataFieldServiceEmergencyAddressContactMobileNo"
                .ContactPager.EditableDataItemConstant = "SPamendReqDataItemServiceEmergencyAddressContact"
                .ContactPager.EditableDataFieldConstant = "SPamendReqDataFieldServiceEmergencyAddressContactPagerNo"
                .ContactEmail.EditableDataItemConstant = "SPamendReqDataItemServiceEmergencyAddressContact"
                .ContactEmail.EditableDataFieldConstant = "SPamendReqDataFieldServiceEmergencyAddressContactEmailAddress"
                .ContactWeb.EditableDataItemConstant = "SPamendReqDataItemServiceEmergencyAddressContact"
                .ContactWeb.EditableDataFieldConstant = "SPamendReqDataFieldServiceEmergencyAddressContactWebAddress"
                .ContactAlsoUsedAs.EditableDataItemConstant = "SPamendReqDataItemServiceEmergencyAddressContact"
                .ContactAlsoUsedAs.EditableDataFieldConstant = "SPamendReqDataFieldServiceEmergencyAddressContactAlsoUsedAs"
            End With

            With contractAddress
                ' address
                .InitControl(Me.DbConnection, contractAddress.ID, True)
                .Address.EditableDataItemConstant = "SPamendReqDataItemServiceContractAddress"
                .Address.EditableDataFieldConstant = "SPamendReqDataFieldServiceContractAddressAddress"
                .Postcode.EditableDataItemConstant = "SPamendReqDataItemServiceContractAddress"
                .Postcode.EditableDataFieldConstant = "SPamendReqDataFieldServiceContractAddressPostcode"
                .AdminAuthority.EditableDataItemConstant = "SPamendReqDataItemServiceContractAddressWard"
                .AdminAuthority.EditableDataFieldConstant = "SPamendReqDataFieldServiceContractAddressWardAdminAuthority"
                .District.EditableDataItemConstant = "SPamendReqDataItemServiceContractAddressWard"
                .District.EditableDataFieldConstant = "SPamendReqDataFieldServiceContractAddressWardDistrict"
                .Ward.EditableDataItemConstant = "SPamendReqDataItemServiceContractAddressWard"
                .Ward.EditableDataFieldConstant = "SPamendReqDataFieldServiceContractAddressWardWard"
                .Directions.EditableDataItemConstant = "SPamendReqDataItemServiceContractAddress"
                .Directions.EditableDataFieldConstant = "SPamendReqDataFieldServiceContractAddressDirections"
                .DisabledAccess.EditableDataItemConstant = "SPamendReqDataItemServiceContractAddress"
                .DisabledAccess.EditableDataFieldConstant = "SPamendReqDataFieldServiceContractAddressDisabledAccess"
                .AlsoUsedAs.EditableDataItemConstant = "SPamendReqDataItemServiceContractAddress"
                .AlsoUsedAs.EditableDataFieldConstant = "SPamendReqDataFieldServiceContractAddressAlsoUsedAs"
                .UPRN.EditableDataItemConstant = "SPamendReqDataItemServiceContractAddressURNs"
                .UPRN.EditableDataFieldConstant = "SPamendReqDataFieldServiceContractAddressURNsUPRN"
                .USRN.EditableDataItemConstant = "SPamendReqDataItemServiceContractAddressURNs"
                .USRN.EditableDataFieldConstant = "SPamendReqDataFieldServiceContractAddressURNsUSRN"
                ' contact
                .ContactType.EditableDataItemConstant = "SPamendReqDataItemServiceContractAddressContact"
                .ContactType.EditableDataFieldConstant = "SPamendReqDataFieldServiceContractAddressContactType"
                .ContactOrganisation.EditableDataItemConstant = "SPamendReqDataItemServiceContractAddressContact"
                .ContactOrganisation.EditableDataFieldConstant = "SPamendReqDataFieldServiceContractAddressContactOrganisation"
                .ContactTitle.EditableDataItemConstant = "SPamendReqDataItemServiceContractAddressContact"
                .ContactTitle.EditableDataFieldConstant = "SPamendReqDataFieldServiceContractAddressContactTitle"
                .ContactForenames.EditableDataItemConstant = "SPamendReqDataItemServiceContractAddressContact"
                .ContactForenames.EditableDataFieldConstant = "SPamendReqDataFieldServiceContractAddressContactForenames"
                .ContactSurname.EditableDataItemConstant = "SPamendReqDataItemServiceContractAddressContact"
                .ContactSurname.EditableDataFieldConstant = "SPamendReqDataFieldServiceContractAddressContactSurname"
                .ContactPosition.EditableDataItemConstant = "SPamendReqDataItemServiceContractAddressContact"
                .ContactPosition.EditableDataFieldConstant = "SPamendReqDataFieldServiceContractAddressContactPosition"
                .ContactTel.EditableDataItemConstant = "SPamendReqDataItemServiceContractAddressContact"
                .ContactTel.EditableDataFieldConstant = "SPamendReqDataFieldServiceContractAddressContactTelephoneNo"
                .ContactFax.EditableDataItemConstant = "SPamendReqDataItemServiceContractAddressContact"
                .ContactFax.EditableDataFieldConstant = "SPamendReqDataFieldServiceContractAddressContactFaxNo"
                .ContactMobile.EditableDataItemConstant = "SPamendReqDataItemServiceContractAddressContact"
                .ContactMobile.EditableDataFieldConstant = "SPamendReqDataFieldServiceContractAddressContactMobileNo"
                .ContactPager.EditableDataItemConstant = "SPamendReqDataItemServiceContractAddressContact"
                .ContactPager.EditableDataFieldConstant = "SPamendReqDataFieldServiceContractAddressContactPagerNo"
                .ContactEmail.EditableDataItemConstant = "SPamendReqDataItemServiceContractAddressContact"
                .ContactEmail.EditableDataFieldConstant = "SPamendReqDataFieldServiceContractAddressContactEmailAddress"
                .ContactWeb.EditableDataItemConstant = "SPamendReqDataItemServiceContractAddressContact"
                .ContactWeb.EditableDataFieldConstant = "SPamendReqDataFieldServiceContractAddressContactWebAddress"
                .ContactAlsoUsedAs.EditableDataItemConstant = "SPamendReqDataItemServiceContractAddressContact"
                .ContactAlsoUsedAs.EditableDataFieldConstant = "SPamendReqDataFieldServiceContractAddressContactAlsoUsedAs"
            End With

            With selfReferralAddress
                ' address
                .InitControl(Me.DbConnection, selfReferralAddress.ID, True)
                .Address.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddress"
                .Address.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddressAddress"
                .Postcode.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddress"
                .Postcode.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddressPostcode"
                .AdminAuthority.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddressWard"
                .AdminAuthority.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddressWardAdminAuthority"
                .District.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddressWard"
                .District.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddressWardDistrict"
                .Ward.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddressWard"
                .Ward.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddressWardWard"
                .Directions.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddress"
                .Directions.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddressDirections"
                .DisabledAccess.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddress"
                .DisabledAccess.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddressDisabledAccess"
                .AlsoUsedAs.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddress"
                .AlsoUsedAs.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddressAlsoUsedAs"
                .UPRN.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddressURNs"
                .UPRN.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddressURNsUPRN"
                .USRN.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddressURNs"
                .USRN.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddressURNsUSRN"
                ' contact
                .ContactType.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddressContact"
                .ContactType.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddressContactType"
                .ContactOrganisation.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddressContact"
                .ContactOrganisation.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddressContactOrganisation"
                .ContactTitle.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddressContact"
                .ContactTitle.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddressContactTitle"
                .ContactForenames.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddressContact"
                .ContactForenames.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddressContactForenames"
                .ContactSurname.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddressContact"
                .ContactSurname.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddressContactSurname"
                .ContactPosition.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddressContact"
                .ContactPosition.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddressContactPosition"
                .ContactTel.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddressContact"
                .ContactTel.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddressContactTelephoneNo"
                .ContactFax.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddressContact"
                .ContactFax.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddressContactFaxNo"
                .ContactMobile.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddressContact"
                .ContactMobile.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddressContactMobileNo"
                .ContactPager.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddressContact"
                .ContactPager.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddressContactPagerNo"
                .ContactEmail.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddressContact"
                .ContactEmail.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddressContactEmailAddress"
                .ContactWeb.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddressContact"
                .ContactWeb.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddressContactWebAddress"
                .ContactAlsoUsedAs.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddressContact"
                .ContactAlsoUsedAs.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddressContactAlsoUsedAs"
            End With

            With selfReferralAddress2
                ' address
                .InitControl(Me.DbConnection, selfReferralAddress2.ID, True)
                .Address.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddress2"
                .Address.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddress2Address"
                .Postcode.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddress2"
                .Postcode.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddress2Postcode"
                .AdminAuthority.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddress2Ward"
                .AdminAuthority.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddress2WardAdminAuthority"
                .District.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddress2Ward"
                .District.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddress2WardDistrict"
                .Ward.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddress2Ward"
                .Ward.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddress2WardWard"
                .Directions.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddress2"
                .Directions.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddress2Directions"
                .DisabledAccess.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddress2"
                .DisabledAccess.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddress2DisabledAccess"
                .AlsoUsedAs.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddress2"
                .AlsoUsedAs.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddress2AlsoUsedAs"
                .UPRN.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddress2URNs"
                .UPRN.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddress2URNsUPRN"
                .USRN.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddress2URNs"
                .USRN.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddress2URNsUSRN"
                ' contact
                .ContactType.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddress2Contact"
                .ContactType.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddress2ContactType"
                .ContactOrganisation.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddress2Contact"
                .ContactOrganisation.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddress2ContactOrganisation"
                .ContactTitle.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddress2Contact"
                .ContactTitle.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddress2ContactTitle"
                .ContactForenames.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddress2Contact"
                .ContactForenames.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddress2ContactForenames"
                .ContactSurname.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddress2Contact"
                .ContactSurname.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddress2ContactSurname"
                .ContactPosition.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddress2Contact"
                .ContactPosition.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddress2ContactPosition"
                .ContactTel.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddress2Contact"
                .ContactTel.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddress2ContactTelephoneNo"
                .ContactFax.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddress2Contact"
                .ContactFax.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddress2ContactFaxNo"
                .ContactMobile.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddress2Contact"
                .ContactMobile.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddress2ContactMobileNo"
                .ContactPager.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddress2Contact"
                .ContactPager.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddress2ContactPagerNo"
                .ContactEmail.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddress2Contact"
                .ContactEmail.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddress2ContactEmailAddress"
                .ContactWeb.EditableDataItemConstant = "SPamendReqDataItemServiceReferralAddress2Contact"
                .ContactWeb.EditableDataFieldConstant = "SPamendReqDataFieldServiceReferralAddress2ContactWebAddress"
            End With

            ' setup edit buttons
            If canViewAmendReqs Then
                btnEdit.Visible = Not editMode
                btnCancel.Visible = editMode
                btnSubmit.Visible = editMode
                If processedFieldCount > 0 Then
                    lblAmendReq.Visible = True
                End If
            Else
                btnEdit.Visible = False
                btnCancel.Visible = False
                btnSubmit.Visible = False
            End If
            ' enable editable fields
            If Not IsPostBack Then
                If editMode Then
                    ' get settings
                    msg = AmendReqBL.FetchEditableFieldSettings(Me.DbConnection, user.ExternalUserID, settings)
                    If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                    ' enable editable fields
                    AmendReqBL.EnableEditableFields(Page.Controls, settings)
                End If
                Try
                    Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME, False)
                    spParams(0).Value = serviceID

                    ' execute
                    reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams)

                    '1st read is the non-list data.
                    While reader.Read
                        ' setup nav buttons
                        btnProvider.Attributes.Add("onclick", _
                            String.Format("document.location.href='ViewProvider.aspx?id={0}&backUrl={1}';", _
                            reader("ProviderID"), HttpUtility.UrlEncode(Request.Url.ToString())))

                        If Convert.IsDBNull(reader("GrossOrSubsidy")) OrElse reader("GrossOrSubsidy") <> "Subsidy" Then
                            btnSubsidies.Visible = False
                        Else
                            btnSubsidies.Attributes.Add("onclick", _
                                String.Format("document.location.href='ListSubsidies/ListSubsidies.aspx?dateFrom=&providerID={0}&serviceID={1}&currentStep=2';", _
                                reader("ProviderID"), serviceID))
                        End If

                        'Service Details
                        lblProviderName.Text = WebUtils.EncodeOutput(reader("ProviderName"))
                        lblProviderAddress.Text = WebUtils.EncodeOutput(reader("ProviderAddress")).Replace(vbCrLf, "<br />")
                        lblServiceName.Text = WebUtils.EncodeOutput(reader("ServiceName"))
                        lblReference.Text = WebUtils.EncodeOutput(reader("Reference"))
                        lblSPINTLSID.Text = WebUtils.EncodeOutput(reader("SPINTLSID"))
                        txtPublicName.Text = WebUtils.EncodeOutput(reader("PublicName"), txtPublicName.IsReadOnly)
                        txtPublicName.RecordID = serviceID
                        With txtDescription
                            .TextBox.TextMode = TextBoxMode.MultiLine
                            .TextBox.Rows = 5
                            .Text = WebUtils.EncodeOutput(reader("Description"))
                            .RecordID = serviceID
                        End With
                        lblServiceType.Text = WebUtils.EncodeOutput(reader("ServiceType"))
                        If reader("AccElement") Then
                            lblAccElement.Text = "Yes"
                        Else
                            lblAccElement.Text = "No"
                        End If
                        If reader("VATExempt") Then
                            lblVATExempt.Text = "Yes"
                        Else
                            lblVATExempt.Text = "No"
                        End If
                        lblPatch.Text = WebUtils.EncodeOutput(reader("Patch"))
                        If reader("SPService") Then
                            lblSPService.Text = "Yes"
                        Else
                            lblSPService.Text = "No"
                        End If
                        If reader("IncludeInSPLSExtract") Then
                            lblIncludeSPLSExtract.Text = "Yes"
                        Else
                            lblIncludeSPLSExtract.Text = "No"
                        End If
                        If reader("PipelineService") Then
                            lblPipelineService.Text = "Yes"
                        Else
                            lblPipelineService.Text = "No"
                        End If
                        lblCommencementDate.Text = WebUtils.EncodeOutput(reader("ServiceCommencementDate"))
                        lblDecommissionDate.Text = WebUtils.EncodeOutput(reader("DecommissionDate"))
                        lblNoWeeksBasis.Text = WebUtils.EncodeOutput(reader("NumberOfWeeksBasis"))
                        lblUrbanRural.Text = WebUtils.EncodeOutput("")
                        Select Case Convert.ToString(reader("RuralOrUrban"))
                            Case "R"
                                lblUrbanRural.Text = "Rural"
                            Case "U"
                                lblUrbanRural.Text = "Urban"
                        End Select
                        If reader("EmergencyReferral") Then
                            lblEmergencyReferral.Text = "Yes"
                        Else
                            lblEmergencyReferral.Text = "No"
                        End If
                        If reader("AccessToInterpreters") Then
                            lblAccessToInterpreters.Text = "Yes"
                        Else
                            lblAccessToInterpreters.Text = "No"
                        End If
                        If reader("VisualImpairmentSupport") Then
                            lblVisualImpairment.Text = "Yes"
                        Else
                            lblVisualImpairment.Text = "No"
                        End If
                        If reader("HearingImpairmentSupport") Then
                            lblHearingImpairment.Text = "Yes"
                        Else
                            lblHearingImpairment.Text = "No"
                        End If
                        If reader("WaitingListInOperation") Then
                            lblWaitingList.Text = "Yes"
                        Else
                            lblWaitingList.Text = "No"
                        End If
                        lblImpactAssessment.Text = WebUtils.EncodeOutput("")
                        Select Case Convert.ToString(reader("ImpactAssessment"))
                            Case "L"
                                lblImpactAssessment.Text = "Low"
                            Case "M"
                                lblImpactAssessment.Text = "Medium"
                            Case "H"
                                lblImpactAssessment.Text = "High"
                        End Select
                        lblRiskAssessment.Text = WebUtils.EncodeOutput("")
                        Select Case Convert.ToString(reader("RiskAssessment"))
                            Case "L"
                                lblRiskAssessment.Text = "Low"
                            Case "M"
                                lblRiskAssessment.Text = "Medium"
                            Case "H"
                                lblRiskAssessment.Text = "High"
                        End Select
                        'Description Tab
                        lblSupportDuration.Text = WebUtils.EncodeOutput(reader("SupportDuration"))

                        lblServiceDelivery.Text = WebUtils.EncodeOutput("")
                        If Not Convert.IsDBNull(reader("ServiceDelivery1")) Then
                            lblServiceDelivery.Text = WebUtils.EncodeOutput(reader("ServiceDelivery1"))
                        ElseIf Not Convert.IsDBNull(reader("ServiceDelivery2")) Then
                            lblServiceDelivery.Text = WebUtils.EncodeOutput(reader("ServiceDelivery2"))
                        End If
                        lblLevelofService.Text = WebUtils.EncodeOutput(reader("ServiceLevel"))

                        'Availability tab
                        lblPrimaryClientGroup.Text = WebUtils.EncodeOutput(reader("PrimaryClientGroup"))
                        lblSecondaryClientGroup.Text = WebUtils.EncodeOutput(reader("SecondaryClientGroup"))
                        lblCulturalGroup.Text = WebUtils.EncodeOutput(reader("RelevantCulturalGroup"))
                        lblHouseholdUnits.Text = WebUtils.EncodeOutput(reader("HouseholdUnitsAvailable"))

                        'Staff Details Tab
                        lblStandardHours.Text = WebUtils.EncodeOutput(reader("StandardHoursPerWeek"))
                        lblNoPaidManagers.Text = WebUtils.EncodeOutput(reader("NumberPaidManagers"))
                        lblNoPaidFrontLine.Text = WebUtils.EncodeOutput(reader("NumberPaidFrontLineStaff"))
                        lblNoUnpaidManagers.Text = WebUtils.EncodeOutput(reader("NumberUnpaidManagers"))
                        lblNoUnpaidFrontLine.Text = WebUtils.EncodeOutput(reader("NumberUnpaidFrontLineStaff"))

                        'User Involvement Tab
                        If reader("MechanismsInPlace") Then
                            lblMechanismsInPlace.Text = "Yes"
                        Else
                            lblMechanismsInPlace.Text = "No"
                        End If
                        lblConsultationFrequency.Text = WebUtils.EncodeOutput(reader("Consultationfrequency"))
                        If reader("CurrentConsultationStrategy") Then
                            lblConsultationStrategy.Text = "Yes"
                        Else
                            lblConsultationStrategy.Text = "No"
                        End If

                        'HIA Tab
                        lblNoEnquiries.Text = WebUtils.EncodeOutput(reader("HIANumberOfEnquiries"))
                        lblNoHouseholdsAssisted.Text = WebUtils.EncodeOutput(reader("HIAHouseholdAssisted"))
                        lblFeeIncome.Text = WebUtils.EncodeOutput(reader("HIAFeeIncome"))

                        'Contracting Information
                        If reader("ServiceChargeable") Then
                            lblChargeable.Text = "Yes"
                        Else
                            lblChargeable.Text = "No"
                        End If
                        lblExemptionReason.Text = WebUtils.EncodeOutput(reader("ExemptFromChargingreason"))
                        lblGrossOrSubsidy.Text = WebUtils.EncodeOutput(reader("GrossOrSubsidy"))
                        If reader("ODPMDesignatedService") Then
                            lblNationalImportance.Text = "Yes"
                        Else
                            lblNationalImportance.Text = "No"
                        End If
                        lblServiceReviewDate.Text = WebUtils.EncodeOutput(reader("ServiceReviewDate"))
                    End While

                    'Read Lists from recordset

                    'Description Tab - Support Provisions
                    reader.NextResult()
                    bOutputList = False
                    bULOutput = False
                    strList = New StringBuilder
                    litSupportProvisions.Text = WebUtils.EncodeOutput("")
                    If reader.HasRows Then
                        While reader.Read()
                            bOutputList = True
                            If Not Convert.IsDBNull(reader("Value")) AndAlso Not bULOutput Then
                                If bOutputList Then strList.Append("<ul class=""list"">")
                                bULOutput = True
                            End If
                            If Not Convert.IsDBNull(reader("Value")) Then strList.AppendFormat("<li>{0}</li>", reader("Value"))
                        End While
                        If bOutputList Then
                            If bULOutput Then strList.Append("</ul>")
                            litSupportProvisions.Text = strList.ToString()
                        End If
                    End If

                    'Description Tab - Eligible Tasks
                    reader.NextResult()
                    bOutputList = False
                    bULOutput = False
                    strList = New StringBuilder
                    litEligibleTasks.Text = WebUtils.EncodeOutput("")
                    If reader.HasRows Then
                        While reader.Read()
                            bOutputList = True
                            If Not Convert.IsDBNull(reader("Value")) AndAlso Not bULOutput Then
                                If bOutputList Then strList.Append("<ul class=""list"">")
                                bULOutput = True
                            End If
                            If Not Convert.IsDBNull(reader("Value")) Then strList.AppendFormat("<li>{0}</li>", reader("Value"))
                        End While
                        If bOutputList Then
                            If bULOutput Then strList.Append("</ul>")
                            litEligibleTasks.Text = strList.ToString()
                        End If
                    End If

                    'Description Tab - Non Eligible Tasks
                    reader.NextResult()
                    bOutputList = False
                    bULOutput = False
                    strList = New StringBuilder
                    litNonEligibleTasks.Text = WebUtils.EncodeOutput("")
                    If reader.HasRows Then
                        While reader.Read()
                            bOutputList = True
                            If Not Convert.IsDBNull(reader("Value")) AndAlso Not bULOutput Then
                                If bOutputList Then strList.Append("<ul class=""list"">")
                                bULOutput = True
                            End If
                            If Not Convert.IsDBNull(reader("Value")) Then strList.AppendFormat("<li>{0}</li>", reader("Value"))
                        End While
                        If bOutputList Then
                            If bULOutput Then strList.Append("</ul>")
                            litNonEligibleTasks.Text = strList.ToString()
                        End If
                    End If

                    'Description Tab - Supported Languages
                    reader.NextResult()
                    bOutputList = False
                    bULOutput = False
                    strList = New StringBuilder
                    litSupportedLanguages.Text = WebUtils.EncodeOutput("")
                    If reader.HasRows Then
                        While reader.Read()
                            bOutputList = True
                            If Not Convert.IsDBNull(reader("Value")) AndAlso Not bULOutput Then
                                If bOutputList Then strList.Append("<ul class=""list"">")
                                bULOutput = True
                            End If
                            If Not Convert.IsDBNull(reader("Value")) Then strList.AppendFormat("<li>{0}</li>", reader("Value"))
                        End While
                        If bOutputList Then
                            If bULOutput Then strList.Append("</ul>")
                            litSupportedLanguages.Text = strList.ToString()
                        End If
                    End If

                    'Description Tab - Supported Religions
                    reader.NextResult()
                    bOutputList = False
                    bULOutput = False
                    strList = New StringBuilder
                    litSupportedReligions.Text = WebUtils.EncodeOutput("")
                    If reader.HasRows Then
                        While reader.Read()
                            bOutputList = True
                            If Not Convert.IsDBNull(reader("Value")) AndAlso Not bULOutput Then
                                If bOutputList Then strList.Append("<ul class=""list"">")
                                bULOutput = True
                            End If
                            If Not Convert.IsDBNull(reader("Value")) Then strList.AppendFormat("<li>{0}</li>", reader("Value"))
                        End While
                        If bOutputList Then
                            If bULOutput Then strList.Append("</ul>")
                            litSupportedReligions.Text = strList.ToString()
                        End If
                    End If

                    'Description Tab - Referral Routes
                    reader.NextResult()
                    bOutputList = False
                    bULOutput = False
                    strList = New StringBuilder
                    litReferralRoutes.Text = WebUtils.EncodeOutput("")
                    If reader.HasRows Then
                        While reader.Read()
                            bOutputList = True
                            If Not Convert.IsDBNull(reader("Value")) AndAlso Not bULOutput Then
                                If bOutputList Then strList.Append("<ul class=""list"">")
                                bULOutput = True
                            End If
                            If Not Convert.IsDBNull(reader("Value")) Then strList.AppendFormat("<li>{0}</li>", reader("Value"))
                        End While
                        If bOutputList Then
                            If bULOutput Then strList.Append("</ul>")
                            litReferralRoutes.Text = strList.ToString()
                        End If
                    End If

                    'Avalability Tab - Primary Service User Group - Ages Supported
                    reader.NextResult()
                    bOutputList = False
                    bULOutput = False
                    strList = New StringBuilder
                    litPGAgesSupported.Text = WebUtils.EncodeOutput("")
                    If reader.HasRows Then
                        While reader.Read()
                            bOutputList = True
                            If Not Convert.IsDBNull(reader("Value")) AndAlso Not bULOutput Then
                                If bOutputList Then strList.Append("<ul class=""list"">")
                                bULOutput = True
                            End If
                            If Not Convert.IsDBNull(reader("Value")) Then strList.AppendFormat("<li>{0}</li>", reader("Value"))
                        End While
                        If bOutputList Then
                            If bULOutput Then strList.Append("</ul>")
                            litPGAgesSupported.Text = strList.ToString()
                        End If
                    End If

                    'Avalability Tab - Secondary Service User Group - Ages Supported
                    reader.NextResult()
                    bOutputList = False
                    bULOutput = False
                    strList = New StringBuilder
                    litSGAgesSupported.Text = WebUtils.EncodeOutput("")
                    If reader.HasRows Then
                        While reader.Read()
                            bOutputList = True
                            If Not Convert.IsDBNull(reader("Value")) AndAlso Not bULOutput Then
                                If bOutputList Then strList.Append("<ul class=""list"">")
                                bULOutput = True
                            End If
                            If Not Convert.IsDBNull(reader("Value")) Then strList.AppendFormat("<li>{0}</li>", reader("Value"))
                        End While
                        If bOutputList Then
                            If bULOutput Then strList.Append("</ul>")
                            litSGAgesSupported.Text = strList.ToString()
                        End If
                    End If

                    'Avalability Tab - Ethnic Groups Supported
                    reader.NextResult()
                    bOutputList = False
                    bULOutput = False
                    strList = New StringBuilder
                    litEthnicGroups.Text = WebUtils.EncodeOutput("")
                    If reader.HasRows Then
                        While reader.Read()
                            bOutputList = True
                            If Not Convert.IsDBNull(reader("Value")) AndAlso Not bULOutput Then
                                If bOutputList Then strList.Append("<ul class=""list"">")
                                bULOutput = True
                            End If
                            If Not Convert.IsDBNull(reader("Value")) Then strList.AppendFormat("<li>{0}</li>", reader("Value"))
                        End While
                        If bOutputList Then
                            If bULOutput Then strList.Append("</ul>")
                            litEthnicGroups.Text = strList.ToString()
                        End If
                    End If

                    'Avalability Tab - Household Groups
                    reader.NextResult()
                    bOutputList = False
                    bULOutput = False
                    strList = New StringBuilder
                    litHouseholdGroups.Text = WebUtils.EncodeOutput("")
                    If reader.HasRows Then
                        While reader.Read()
                            bOutputList = True
                            If Not Convert.IsDBNull(reader("Value")) AndAlso Not bULOutput Then
                                If bOutputList Then strList.Append("<ul class=""list"">")
                                bULOutput = True
                            End If
                            If Not Convert.IsDBNull(reader("Value")) Then strList.AppendFormat("<li>{0}</li>", reader("Value"))
                        End While
                        If bOutputList Then
                            If bULOutput Then strList.Append("</ul>")
                            litHouseholdGroups.Text = strList.ToString()
                        End If
                    End If

                    'Avalability Tab - Referral Route
                    reader.NextResult()
                    bOutputList = False
                    bULOutput = False
                    strList = New StringBuilder
                    litReferralRoute.Text = WebUtils.EncodeOutput("")
                    If reader.HasRows Then
                        While reader.Read()
                            bOutputList = True
                            If Not Convert.IsDBNull(reader("Value")) AndAlso Not bULOutput Then
                                If bOutputList Then strList.Append("<ul class=""list"">")
                                bULOutput = True
                            End If
                            If Not Convert.IsDBNull(reader("Value")) Then strList.AppendFormat("<li>{0}</li>", reader("Value"))
                        End While
                        If bOutputList Then
                            If bULOutput Then strList.Append("</ul>")
                            litReferralRoute.Text = strList.ToString()
                        End If
                    End If

                    'Avalability Tab - User Exclusions
                    reader.NextResult()
                    bOutputList = False
                    bULOutput = False
                    strList = New StringBuilder
                    litUserExclusions.Text = WebUtils.EncodeOutput("")
                    If reader.HasRows Then
                        While reader.Read()
                            bOutputList = True
                            If Not Convert.IsDBNull(reader("Value")) AndAlso Not bULOutput Then
                                If bOutputList Then strList.Append("<ul class=""list"">")
                                bULOutput = True
                            End If
                            If Not Convert.IsDBNull(reader("Value")) Then strList.AppendFormat("<li>{0}</li>", reader("Value"))
                        End While
                        If bOutputList Then
                            If bULOutput Then strList.Append("</ul>")
                            litUserExclusions.Text = strList.ToString()
                        End If
                    End If

                    'User Involvement Tab - Consultation Methods Used
                    reader.NextResult()
                    bOutputList = False
                    bULOutput = False
                    strList = New StringBuilder
                    litConsultationMethods.Text = WebUtils.EncodeOutput("")
                    If reader.HasRows Then
                        While reader.Read()
                            bOutputList = True
                            If Not Convert.IsDBNull(reader("Value")) AndAlso Not bULOutput Then
                                If bOutputList Then strList.Append("<ul class=""list"">")
                                bULOutput = True
                            End If
                            If Not Convert.IsDBNull(reader("Value")) Then strList.AppendFormat("<li>{0}</li>", reader("Value"))
                        End While
                        If bOutputList Then
                            If bULOutput Then strList.Append("</ul>")
                            litConsultationMethods.Text = strList.ToString()
                        End If
                    End If

                    'Service Description Tab
                    reader.NextResult()
                    If adminAuthorityType = 0 Then
                        rptServiceDistricts.DataSource = reader
                        rptServiceDistricts.DataBind()
                    End If

                    'Populate Address's
                    'Manager Address
                    reader.NextResult()
                    While reader.Read
                        With managerAddress
                            ' address
                            managerAddressID = Target.Library.Utils.ToInt32(reader("AddressID"))
                            .SetAddressIDs(Target.Library.Utils.ToInt32(reader("AddressID")), serviceID)
                            .Address.Text = WebUtils.EncodeOutput(reader("Address"), .Address.IsReadOnly)
                            .Postcode.Text = WebUtils.EncodeOutput(reader("PostCode"), .Postcode.IsReadOnly)
                            .AdminAuthority.Text = WebUtils.EncodeOutput(reader("AdminAuthority"), .AdminAuthority.IsReadOnly)
                            .District.Text = WebUtils.EncodeOutput(reader("District"), .District.IsReadOnly)
                            .Ward.Text = WebUtils.EncodeOutput(reader("Ward"), .Ward.IsReadOnly)
                            .Directions.Text = WebUtils.EncodeOutput(reader("Directions"), .Directions.IsReadOnly)
                            .UPRN.Text = WebUtils.EncodeOutput(reader("UPRN"), .UPRN.IsReadOnly)
                            .USRN.Text = WebUtils.EncodeOutput(reader("USRN"), .USRN.IsReadOnly)

                            .Confidential.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("Confidential")) Then
                                If reader("Confidential") Then
                                    .Confidential.Text = "Yes"
                                Else
                                    .Confidential.Text = "No"
                                End If
                            End If

                            .DisabledAccess.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("DisabledAccess")) Then
                                If .DisabledAccess.IsReadOnly Then
                                    If reader("DisabledAccess") = "Y" Then
                                        .DisabledAccess.Text = "Yes"
                                    ElseIf reader("DisabledAccess") = "N" Then
                                        .DisabledAccess.Text = "No"
                                    End If
                                Else
                                    .DisabledAccess.DropDownList.SelectedValue = reader("DisabledAccess")
                                End If
                            End If

                            ' contact
                            managerContactID = Target.Library.Utils.ToInt32(reader("ContactID"))
                            .SetContactIDs(Target.Library.Utils.ToInt32(reader("ContactID")), serviceID)
                            .ContactType.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("ContactType")) Then
                                If .ContactType.IsReadOnly Then
                                    Select Case Convert.ToString(reader("ContactType"))
                                        Case "P"
                                            .ContactType.Text = "Person"
                                        Case "R"
                                            .ContactType.Text = "Role"
                                        Case "O"
                                            .ContactType.Text = "Organisation"
                                    End Select
                                Else
                                    .ContactType.DropDownList.SelectedValue = reader("ContactType")
                                End If
                            End If
                            .ContactOrganisation.Text = WebUtils.EncodeOutput(reader("Organisation"), .ContactOrganisation.IsReadOnly)
                            .ContactTitle.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("Title")) Then
                                If .ContactTitle.IsReadOnly Then
                                    .ContactTitle.Text = WebUtils.EncodeOutput(reader("Title"))
                                Else
                                    .ContactTitle.DropDownList.SelectedValue = reader("Title")
                                End If
                            End If
                            .ContactForenames.Text = WebUtils.EncodeOutput(reader("FirstNames"), .ContactForenames.IsReadOnly)
                            .ContactSurname.Text = WebUtils.EncodeOutput(reader("Surname"), .ContactSurname.IsReadOnly)
                            .ContactPosition.Text = WebUtils.EncodeOutput(reader("Position"), .ContactPosition.IsReadOnly)
                            .ContactTel.Text = WebUtils.EncodeOutput(reader("TelNo"), .ContactTel.IsReadOnly)
                            .ContactFax.Text = WebUtils.EncodeOutput(reader("FaxNo"), .ContactFax.IsReadOnly)
                            .ContactMobile.Text = WebUtils.EncodeOutput(reader("MobileNo"), .ContactMobile.IsReadOnly)
                            .ContactPager.Text = WebUtils.EncodeOutput(reader("PagerNo"), .ContactPager.IsReadOnly)
                            If Not Convert.IsDBNull(reader("EmailAddress")) AndAlso Convert.ToString(reader("EmailAddress")).Length > 0 Then
                                .ContactEmail.Text = WebUtils.EncodeOutput(reader("EmailAddress"), .ContactEmail.IsReadOnly)
                                .ContactEmail.Link.NavigateUrl = "mailto:" & reader("EmailAddress")
                            End If
                            If Not Convert.IsDBNull(reader("WebAddress")) AndAlso Convert.ToString(reader("WebAddress")).Length > 0 Then
                                strUrl = reader("WebAddress")
                                .ContactWeb.Text = WebUtils.EncodeOutput(strUrl, .ContactWeb.IsReadOnly)
                                .ContactWeb.Link.NavigateUrl = IIf(Not strUrl.StartsWith("http"), "http://", String.Empty) & strUrl
                                .ContactWeb.Link.Attributes.Add("rel", "external")
                            End If

                        End With
                    End While

                    'Emergency Address
                    reader.NextResult()
                    While reader.Read
                        With emergencyAddress
                            ' address
                            emergencyAddressID = Target.Library.Utils.ToInt32(reader("AddressID"))
                            .SetAddressIDs(Target.Library.Utils.ToInt32(reader("AddressID")), serviceID)
                            .Address.Text = WebUtils.EncodeOutput(reader("Address"), .Address.IsReadOnly)
                            .Postcode.Text = WebUtils.EncodeOutput(reader("PostCode"), .Postcode.IsReadOnly)
                            .AdminAuthority.Text = WebUtils.EncodeOutput(reader("AdminAuthority"), .AdminAuthority.IsReadOnly)
                            .District.Text = WebUtils.EncodeOutput(reader("District"), .District.IsReadOnly)
                            .Ward.Text = WebUtils.EncodeOutput(reader("Ward"), .Ward.IsReadOnly)
                            .Directions.Text = WebUtils.EncodeOutput(reader("Directions"), .Directions.IsReadOnly)
                            .UPRN.Text = WebUtils.EncodeOutput(reader("UPRN"), .UPRN.IsReadOnly)
                            .USRN.Text = WebUtils.EncodeOutput(reader("USRN"), .USRN.IsReadOnly)

                            .Confidential.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("Confidential")) Then
                                If reader("Confidential") Then
                                    .Confidential.Text = "Yes"
                                Else
                                    .Confidential.Text = "No"
                                End If
                            End If

                            .DisabledAccess.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("DisabledAccess")) Then
                                If .DisabledAccess.IsReadOnly Then
                                    If reader("DisabledAccess") = "Y" Then
                                        .DisabledAccess.Text = "Yes"
                                    ElseIf reader("DisabledAccess") = "N" Then
                                        .DisabledAccess.Text = "No"
                                    End If
                                Else
                                    .DisabledAccess.DropDownList.SelectedValue = reader("DisabledAccess")
                                End If
                            End If

                            ' contact
                            emergencyContactID = Target.Library.Utils.ToInt32(reader("ContactID"))
                            .SetContactIDs(Target.Library.Utils.ToInt32(reader("ContactID")), serviceID)
                            .ContactType.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("ContactType")) Then
                                If .ContactType.IsReadOnly Then
                                    Select Case Convert.ToString(reader("ContactType"))
                                        Case "P"
                                            .ContactType.Text = "Person"
                                        Case "R"
                                            .ContactType.Text = "Role"
                                        Case "O"
                                            .ContactType.Text = "Organisation"
                                    End Select
                                Else
                                    .ContactType.DropDownList.SelectedValue = reader("ContactType")
                                End If
                            End If
                            .ContactOrganisation.Text = WebUtils.EncodeOutput(reader("Organisation"), .ContactOrganisation.IsReadOnly)
                            .ContactTitle.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("Title")) Then
                                If .ContactTitle.IsReadOnly Then
                                    .ContactTitle.Text = WebUtils.EncodeOutput(reader("Title"))
                                Else
                                    .ContactTitle.DropDownList.SelectedValue = reader("Title")
                                End If
                            End If
                            .ContactForenames.Text = WebUtils.EncodeOutput(reader("FirstNames"), .ContactForenames.IsReadOnly)
                            .ContactSurname.Text = WebUtils.EncodeOutput(reader("Surname"), .ContactSurname.IsReadOnly)
                            .ContactPosition.Text = WebUtils.EncodeOutput(reader("Position"), .ContactPosition.IsReadOnly)
                            .ContactTel.Text = WebUtils.EncodeOutput(reader("TelNo"), .ContactTel.IsReadOnly)
                            .ContactFax.Text = WebUtils.EncodeOutput(reader("FaxNo"), .ContactFax.IsReadOnly)
                            .ContactMobile.Text = WebUtils.EncodeOutput(reader("MobileNo"), .ContactMobile.IsReadOnly)
                            .ContactPager.Text = WebUtils.EncodeOutput(reader("PagerNo"), .ContactPager.IsReadOnly)
                            If Not Convert.IsDBNull(reader("EmailAddress")) AndAlso Convert.ToString(reader("EmailAddress")).Length > 0 Then
                                .ContactEmail.Text = WebUtils.EncodeOutput(reader("EmailAddress"), .ContactEmail.IsReadOnly)
                                .ContactEmail.Link.NavigateUrl = "mailto:" & reader("EmailAddress")
                            End If
                            If Not Convert.IsDBNull(reader("WebAddress")) AndAlso Convert.ToString(reader("WebAddress")).Length > 0 Then
                                strUrl = reader("WebAddress")
                                .ContactWeb.Text = WebUtils.EncodeOutput(strUrl, .ContactWeb.IsReadOnly)
                                .ContactWeb.Link.NavigateUrl = IIf(Not strUrl.StartsWith("http"), "http://", String.Empty) & strUrl
                                .ContactWeb.Link.Attributes.Add("rel", "external")
                            End If

                        End With
                    End While

                    'Contract Address
                    reader.NextResult()
                    While reader.Read
                        With contractAddress
                            ' address
                            contractAddressID = Target.Library.Utils.ToInt32(reader("AddressID"))
                            .SetAddressIDs(Target.Library.Utils.ToInt32(reader("AddressID")), serviceID)
                            .Address.Text = WebUtils.EncodeOutput(reader("Address"), .Address.IsReadOnly)
                            .Postcode.Text = WebUtils.EncodeOutput(reader("PostCode"), .Postcode.IsReadOnly)
                            .AdminAuthority.Text = WebUtils.EncodeOutput(reader("AdminAuthority"), .AdminAuthority.IsReadOnly)
                            .District.Text = WebUtils.EncodeOutput(reader("District"), .District.IsReadOnly)
                            .Ward.Text = WebUtils.EncodeOutput(reader("Ward"), .Ward.IsReadOnly)
                            .Directions.Text = WebUtils.EncodeOutput(reader("Directions"), .Directions.IsReadOnly)
                            .UPRN.Text = WebUtils.EncodeOutput(reader("UPRN"), .UPRN.IsReadOnly)
                            .USRN.Text = WebUtils.EncodeOutput(reader("USRN"), .USRN.IsReadOnly)

                            .Confidential.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("Confidential")) Then
                                If reader("Confidential") Then
                                    .Confidential.Text = "Yes"
                                Else
                                    .Confidential.Text = "No"
                                End If
                            End If

                            .DisabledAccess.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("DisabledAccess")) Then
                                If .DisabledAccess.IsReadOnly Then
                                    If reader("DisabledAccess") = "Y" Then
                                        .DisabledAccess.Text = "Yes"
                                    ElseIf reader("DisabledAccess") = "N" Then
                                        .DisabledAccess.Text = "No"
                                    End If
                                Else
                                    .DisabledAccess.DropDownList.SelectedValue = reader("DisabledAccess")
                                End If
                            End If

                            ' contact
                            contractContactID = Target.Library.Utils.ToInt32(reader("ContactID"))
                            .SetContactIDs(Target.Library.Utils.ToInt32(reader("ContactID")), serviceID)
                            .ContactType.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("ContactType")) Then
                                If .ContactType.IsReadOnly Then
                                    Select Case Convert.ToString(reader("ContactType"))
                                        Case "P"
                                            .ContactType.Text = "Person"
                                        Case "R"
                                            .ContactType.Text = "Role"
                                        Case "O"
                                            .ContactType.Text = "Organisation"
                                    End Select
                                Else
                                    .ContactType.DropDownList.SelectedValue = reader("ContactType")
                                End If
                            End If
                            .ContactOrganisation.Text = WebUtils.EncodeOutput(reader("Organisation"), .ContactOrganisation.IsReadOnly)
                            .ContactTitle.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("Title")) Then
                                If .ContactTitle.IsReadOnly Then
                                    .ContactTitle.Text = WebUtils.EncodeOutput(reader("Title"))
                                Else
                                    .ContactTitle.DropDownList.SelectedValue = reader("Title")
                                End If
                            End If
                            .ContactForenames.Text = WebUtils.EncodeOutput(reader("FirstNames"), .ContactForenames.IsReadOnly)
                            .ContactSurname.Text = WebUtils.EncodeOutput(reader("Surname"), .ContactSurname.IsReadOnly)
                            .ContactPosition.Text = WebUtils.EncodeOutput(reader("Position"), .ContactPosition.IsReadOnly)
                            .ContactTel.Text = WebUtils.EncodeOutput(reader("TelNo"), .ContactTel.IsReadOnly)
                            .ContactFax.Text = WebUtils.EncodeOutput(reader("FaxNo"), .ContactFax.IsReadOnly)
                            .ContactMobile.Text = WebUtils.EncodeOutput(reader("MobileNo"), .ContactMobile.IsReadOnly)
                            .ContactPager.Text = WebUtils.EncodeOutput(reader("PagerNo"), .ContactPager.IsReadOnly)
                            If Not Convert.IsDBNull(reader("EmailAddress")) AndAlso Convert.ToString(reader("EmailAddress")).Length > 0 Then
                                .ContactEmail.Text = WebUtils.EncodeOutput(reader("EmailAddress"), .ContactEmail.IsReadOnly)
                                .ContactEmail.Link.NavigateUrl = "mailto:" & reader("EmailAddress")
                            End If
                            If Not Convert.IsDBNull(reader("WebAddress")) AndAlso Convert.ToString(reader("WebAddress")).Length > 0 Then
                                strUrl = reader("WebAddress")
                                .ContactWeb.Text = WebUtils.EncodeOutput(strUrl, .ContactWeb.IsReadOnly)
                                .ContactWeb.Link.NavigateUrl = IIf(Not strUrl.StartsWith("http"), "http://", String.Empty) & strUrl
                                .ContactWeb.Link.Attributes.Add("rel", "external")
                            End If

                        End With
                    End While

                    'Self Referral Address
                    reader.NextResult()
                    While reader.Read
                        With selfReferralAddress
                            ' address
                            selfReferralAddressID = Target.Library.Utils.ToInt32(reader("AddressID"))
                            .SetAddressIDs(Target.Library.Utils.ToInt32(reader("AddressID")), serviceID)
                            .Address.Text = WebUtils.EncodeOutput(reader("Address"), .Address.IsReadOnly)
                            .Postcode.Text = WebUtils.EncodeOutput(reader("PostCode"), .Postcode.IsReadOnly)
                            .AdminAuthority.Text = WebUtils.EncodeOutput(reader("AdminAuthority"), .AdminAuthority.IsReadOnly)
                            .District.Text = WebUtils.EncodeOutput(reader("District"), .District.IsReadOnly)
                            .Ward.Text = WebUtils.EncodeOutput(reader("Ward"), .Ward.IsReadOnly)
                            .Directions.Text = WebUtils.EncodeOutput(reader("Directions"), .Directions.IsReadOnly)
                            .UPRN.Text = WebUtils.EncodeOutput(reader("UPRN"), .UPRN.IsReadOnly)
                            .USRN.Text = WebUtils.EncodeOutput(reader("USRN"), .USRN.IsReadOnly)

                            .Confidential.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("Confidential")) Then
                                If reader("Confidential") Then
                                    .Confidential.Text = "Yes"
                                Else
                                    .Confidential.Text = "No"
                                End If
                            End If

                            .DisabledAccess.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("DisabledAccess")) Then
                                If .DisabledAccess.IsReadOnly Then
                                    If reader("DisabledAccess") = "Y" Then
                                        .DisabledAccess.Text = "Yes"
                                    ElseIf reader("DisabledAccess") = "N" Then
                                        .DisabledAccess.Text = "No"
                                    End If
                                Else
                                    .DisabledAccess.DropDownList.SelectedValue = reader("DisabledAccess")
                                End If
                            End If

                            ' contact
                            selfReferralContactID = Target.Library.Utils.ToInt32(reader("ContactID"))
                            .SetContactIDs(Target.Library.Utils.ToInt32(reader("ContactID")), serviceID)
                            .ContactType.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("ContactType")) Then
                                If .ContactType.IsReadOnly Then
                                    Select Case Convert.ToString(reader("ContactType"))
                                        Case "P"
                                            .ContactType.Text = "Person"
                                        Case "R"
                                            .ContactType.Text = "Role"
                                        Case "O"
                                            .ContactType.Text = "Organisation"
                                    End Select
                                Else
                                    .ContactType.DropDownList.SelectedValue = reader("ContactType")
                                End If
                            End If
                            .ContactOrganisation.Text = WebUtils.EncodeOutput(reader("Organisation"), .ContactOrganisation.IsReadOnly)
                            .ContactTitle.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("Title")) Then
                                If .ContactTitle.IsReadOnly Then
                                    .ContactTitle.Text = WebUtils.EncodeOutput(reader("Title"))
                                Else
                                    .ContactTitle.DropDownList.SelectedValue = reader("Title")
                                End If
                            End If
                            .ContactForenames.Text = WebUtils.EncodeOutput(reader("FirstNames"), .ContactForenames.IsReadOnly)
                            .ContactSurname.Text = WebUtils.EncodeOutput(reader("Surname"), .ContactSurname.IsReadOnly)
                            .ContactPosition.Text = WebUtils.EncodeOutput(reader("Position"), .ContactPosition.IsReadOnly)
                            .ContactTel.Text = WebUtils.EncodeOutput(reader("TelNo"), .ContactTel.IsReadOnly)
                            .ContactFax.Text = WebUtils.EncodeOutput(reader("FaxNo"), .ContactFax.IsReadOnly)
                            .ContactMobile.Text = WebUtils.EncodeOutput(reader("MobileNo"), .ContactMobile.IsReadOnly)
                            .ContactPager.Text = WebUtils.EncodeOutput(reader("PagerNo"), .ContactPager.IsReadOnly)
                            If Not Convert.IsDBNull(reader("EmailAddress")) AndAlso Convert.ToString(reader("EmailAddress")).Length > 0 Then
                                .ContactEmail.Text = WebUtils.EncodeOutput(reader("EmailAddress"), .ContactEmail.IsReadOnly)
                                .ContactEmail.Link.NavigateUrl = "mailto:" & reader("EmailAddress")
                            End If
                            If Not Convert.IsDBNull(reader("WebAddress")) AndAlso Convert.ToString(reader("WebAddress")).Length > 0 Then
                                strUrl = reader("WebAddress")
                                .ContactWeb.Text = WebUtils.EncodeOutput(strUrl, .ContactWeb.IsReadOnly)
                                .ContactWeb.Link.NavigateUrl = IIf(Not strUrl.StartsWith("http"), "http://", String.Empty) & strUrl
                                .ContactWeb.Link.Attributes.Add("rel", "external")
                            End If

                        End With
                    End While

                    'Self Referral Address 2
                    reader.NextResult()
                    While reader.Read
                        With selfReferralAddress2
                            ' address
                            selfReferralAddress2ID = Target.Library.Utils.ToInt32(reader("AddressID"))
                            .SetAddressIDs(Target.Library.Utils.ToInt32(reader("AddressID")), serviceID)
                            .Address.Text = WebUtils.EncodeOutput(reader("Address"), .Address.IsReadOnly)
                            .Postcode.Text = WebUtils.EncodeOutput(reader("PostCode"), .Postcode.IsReadOnly)
                            .AdminAuthority.Text = WebUtils.EncodeOutput(reader("AdminAuthority"), .AdminAuthority.IsReadOnly)
                            .District.Text = WebUtils.EncodeOutput(reader("District"), .District.IsReadOnly)
                            .Ward.Text = WebUtils.EncodeOutput(reader("Ward"), .Ward.IsReadOnly)
                            .Directions.Text = WebUtils.EncodeOutput(reader("Directions"), .Directions.IsReadOnly)
                            .UPRN.Text = WebUtils.EncodeOutput(reader("UPRN"), .UPRN.IsReadOnly)
                            .USRN.Text = WebUtils.EncodeOutput(reader("USRN"), .USRN.IsReadOnly)

                            .Confidential.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("Confidential")) Then
                                If reader("Confidential") Then
                                    .Confidential.Text = "Yes"
                                Else
                                    .Confidential.Text = "No"
                                End If
                            End If

                            .DisabledAccess.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("DisabledAccess")) Then
                                If .DisabledAccess.IsReadOnly Then
                                    If reader("DisabledAccess") = "Y" Then
                                        .DisabledAccess.Text = "Yes"
                                    ElseIf reader("DisabledAccess") = "N" Then
                                        .DisabledAccess.Text = "No"
                                    End If
                                Else
                                    .DisabledAccess.DropDownList.SelectedValue = reader("DisabledAccess")
                                End If
                            End If

                            ' contact
                            selfReferralContact2ID = Target.Library.Utils.ToInt32(reader("ContactID"))
                            .SetContactIDs(Target.Library.Utils.ToInt32(reader("ContactID")), serviceID)
                            .ContactType.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("ContactType")) Then
                                If .ContactType.IsReadOnly Then
                                    Select Case Convert.ToString(reader("ContactType"))
                                        Case "P"
                                            .ContactType.Text = "Person"
                                        Case "R"
                                            .ContactType.Text = "Role"
                                        Case "O"
                                            .ContactType.Text = "Organisation"
                                    End Select
                                Else
                                    .ContactType.DropDownList.SelectedValue = reader("ContactType")
                                End If
                            End If
                            .ContactOrganisation.Text = WebUtils.EncodeOutput(reader("Organisation"), .ContactOrganisation.IsReadOnly)
                            .ContactTitle.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("Title")) Then
                                If .ContactTitle.IsReadOnly Then
                                    .ContactTitle.Text = WebUtils.EncodeOutput(reader("Title"))
                                Else
                                    .ContactTitle.DropDownList.SelectedValue = reader("Title")
                                End If
                            End If
                            .ContactForenames.Text = WebUtils.EncodeOutput(reader("FirstNames"), .ContactForenames.IsReadOnly)
                            .ContactSurname.Text = WebUtils.EncodeOutput(reader("Surname"), .ContactSurname.IsReadOnly)
                            .ContactPosition.Text = WebUtils.EncodeOutput(reader("Position"), .ContactPosition.IsReadOnly)
                            .ContactTel.Text = WebUtils.EncodeOutput(reader("TelNo"), .ContactTel.IsReadOnly)
                            .ContactFax.Text = WebUtils.EncodeOutput(reader("FaxNo"), .ContactFax.IsReadOnly)
                            .ContactMobile.Text = WebUtils.EncodeOutput(reader("MobileNo"), .ContactMobile.IsReadOnly)
                            .ContactPager.Text = WebUtils.EncodeOutput(reader("PagerNo"), .ContactPager.IsReadOnly)
                            If Not Convert.IsDBNull(reader("EmailAddress")) AndAlso Convert.ToString(reader("EmailAddress")).Length > 0 Then
                                .ContactEmail.Text = WebUtils.EncodeOutput(reader("EmailAddress"), .ContactEmail.IsReadOnly)
                                .ContactEmail.Link.NavigateUrl = "mailto:" & reader("EmailAddress")
                            End If
                            If Not Convert.IsDBNull(reader("WebAddress")) AndAlso Convert.ToString(reader("WebAddress")).Length > 0 Then
                                strUrl = reader("WebAddress")
                                .ContactWeb.Text = WebUtils.EncodeOutput(strUrl, .ContactWeb.IsReadOnly)
                                .ContactWeb.Link.NavigateUrl = IIf(Not strUrl.StartsWith("http"), "http://", String.Empty) & strUrl
                                .ContactWeb.Link.Attributes.Add("rel", "external")
                            End If

                        End With
                    End While

                    'Set Also Used as fields on addresses
                    With managerAddress
                        .AlsoUsedAs.Items.Add(New ListItem("Emergency Address", managerAddressID))
                        .AlsoUsedAs.Items.Add(New ListItem("Contract Address", managerAddressID))
                        .AlsoUsedAs.Items.Add(New ListItem("Self Referral Address", managerAddressID))
                        .AlsoUsedAs.Items.Add(New ListItem("Self Referral Address 2", managerAddressID))
                        If (managerAddressID > 0 AndAlso (managerAddressID = emergencyAddressID)) Then
                            .AlsoUsedAs.Items(0).Selected = True
                            emergencyShowSameAs = True
                        End If
                        If (managerAddressID > 0 AndAlso (managerAddressID = contractAddressID)) Then
                            .AlsoUsedAs.Items(1).Selected = True
                            ContractShowSameAs = True
                        End If
                        If (managerAddressID > 0 AndAlso (managerAddressID = selfReferralAddressID)) Then
                            .AlsoUsedAs.Items(2).Selected = True
                            selfReferralShowSameAs = True
                        End If
                        If (managerAddressID > 0 AndAlso (managerAddressID = selfReferralAddress2ID)) Then
                            .AlsoUsedAs.Items(3).Selected = True
                            selfReferral2ShowSameAs = True
                        End If
                        If .AlsoUsedAs.Items.Count = 0 Then .AlsoUsedAs.Visible = False
                    End With

                    With emergencyAddress
                        If Not emergencyShowSameAs Then
                            If (managerAddressID = 0) Or (contractAddressID <> managerAddressID) Then
                                newAlsoUsedAsItem = New ListItem("Contract Address", emergencyAddressID)
                                .AlsoUsedAs.Items.Add(newAlsoUsedAsItem)
                                If (emergencyAddressID > 0) AndAlso (emergencyAddressID = contractAddressID) Then
                                    newAlsoUsedAsItem.Selected = True
                                    ContractShowSameAs = True
                                End If
                            End If
                            If (managerAddressID = 0) Or (selfReferralAddressID <> managerAddressID) Then
                                newAlsoUsedAsItem = New ListItem("Self Referral Address", emergencyAddressID)
                                .AlsoUsedAs.Items.Add(newAlsoUsedAsItem)
                                If (emergencyAddressID > 0) AndAlso (emergencyAddressID = selfReferralAddressID) Then
                                    newAlsoUsedAsItem.Selected = True
                                    selfReferralShowSameAs = True
                                End If
                            End If
                            If (managerAddressID = 0) Or (selfReferralAddress2ID <> managerAddressID) Then
                                newAlsoUsedAsItem = New ListItem("Self Referral Address 2", emergencyAddressID)
                                .AlsoUsedAs.Items.Add(newAlsoUsedAsItem)
                                If (emergencyAddressID > 0) AndAlso (emergencyAddressID = selfReferralAddress2ID) Then
                                    newAlsoUsedAsItem.Selected = True
                                    selfReferral2ShowSameAs = True
                                End If
                            End If
                        Else
                            If editMode Then .DisableAddressTab()
                        End If
                        If .AlsoUsedAs.Items.Count = 0 Then .AlsoUsedAs.Visible = False
                    End With

                    With contractAddress
                        If Not ContractShowSameAs Then
                            If (managerAddressID = 0) Or (selfReferralAddressID <> managerAddressID) Then
                                If (emergencyAddressID = 0) Or (selfReferralAddressID <> emergencyAddressID) Then
                                    newAlsoUsedAsItem = New ListItem("Self Referral Address", emergencyAddressID)
                                    .AlsoUsedAs.Items.Add(newAlsoUsedAsItem)
                                    If (contractAddressID > 0) AndAlso (contractAddressID = selfReferralAddressID) Then
                                        newAlsoUsedAsItem.Selected = True
                                        selfReferralShowSameAs = True
                                    End If
                                End If
                            End If
                            If (managerAddressID = 0) Or (selfReferralAddress2ID <> managerAddressID) Then
                                If (emergencyAddressID = 0) Or (selfReferralAddress2ID <> emergencyAddressID) Then
                                    newAlsoUsedAsItem = New ListItem("Self Referral Address 2", emergencyAddressID)
                                    .AlsoUsedAs.Items.Add(newAlsoUsedAsItem)
                                    If (contractAddressID > 0) AndAlso (contractAddressID = selfReferralAddress2ID) Then
                                        newAlsoUsedAsItem.Selected = True
                                        selfReferral2ShowSameAs = True
                                    End If
                                End If
                            End If
                        Else
                            If editMode Then .DisableAddressTab()
                        End If
                        If .AlsoUsedAs.Items.Count = 0 Then .AlsoUsedAs.Visible = False
                    End With

                    With selfReferralAddress
                        If Not selfReferralShowSameAs Then
                            If (managerAddressID = 0) Or (selfReferralAddress2ID <> managerAddressID) Then
                                If (emergencyAddressID = 0) Or (selfReferralAddress2ID <> emergencyAddressID) Then
                                    If (contractAddressID = 0) Or (selfReferralAddress2ID <> contractAddressID) Then
                                        newAlsoUsedAsItem = New ListItem("Self Referral Address 2", emergencyAddressID)
                                        .AlsoUsedAs.Items.Add(newAlsoUsedAsItem)
                                        If (selfReferralAddressID > 0) AndAlso (selfReferralAddressID = selfReferralAddress2ID) Then
                                            newAlsoUsedAsItem.Selected = True
                                        End If
                                    End If
                                End If
                            End If
                        Else
                            If editMode Then .DisableAddressTab()
                        End If
                        If .AlsoUsedAs.Items.Count = 0 Then .AlsoUsedAs.Visible = False
                    End With

                    With selfReferralAddress2
                        If selfReferral2ShowSameAs AndAlso editMode Then .DisableAddressTab()
                        .AlsoUsedAs.Visible = False
                    End With

                    'Set Also Used as fields on contacts
                    emergencyShowSameAs = ContractShowSameAs = selfReferralShowSameAs = selfReferral2ShowSameAs = False
                    With managerAddress
                        .ContactAlsoUsedAs.Items.Add(New ListItem("Emergency Contact", managerContactID))
                        .ContactAlsoUsedAs.Items.Add(New ListItem("Contract Contact", managerContactID))
                        .ContactAlsoUsedAs.Items.Add(New ListItem("Self Referral Contact", managerContactID))
                        .ContactAlsoUsedAs.Items.Add(New ListItem("Self Referral Contact 2", managerContactID))
                        If (managerContactID > 0 AndAlso (managerContactID = emergencyContactID)) Then
                            .ContactAlsoUsedAs.Items(0).Selected = True
                            emergencyShowSameAs = True
                        End If
                        If (managerContactID > 0 AndAlso (managerContactID = contractContactID)) Then
                            .ContactAlsoUsedAs.Items(1).Selected = True
                            ContractShowSameAs = True
                        End If
                        If (managerContactID > 0 AndAlso (managerContactID = selfReferralContactID)) Then
                            .ContactAlsoUsedAs.Items(2).Selected = True
                            selfReferralShowSameAs = True
                        End If
                        If (managerContactID > 0 AndAlso (managerContactID = selfReferralContact2ID)) Then
                            .ContactAlsoUsedAs.Items(3).Selected = True
                            selfReferral2ShowSameAs = True
                        End If
                        If .ContactAlsoUsedAs.Items.Count = 0 Then .ContactAlsoUsedAs.Visible = False
                    End With

                    With emergencyAddress
                        If Not emergencyShowSameAs Then
                            If (managerContactID = 0) Or (contractContactID <> managerContactID) Then
                                newAlsoUsedAsItem = New ListItem("Contract Contact", emergencyContactID)
                                .ContactAlsoUsedAs.Items.Add(newAlsoUsedAsItem)
                                If (emergencyContactID > 0) AndAlso (emergencyContactID = contractContactID) Then
                                    newAlsoUsedAsItem.Selected = True
                                    ContractShowSameAs = True
                                End If
                            End If
                            If (managerContactID = 0) Or (selfReferralContactID <> managerContactID) Then
                                newAlsoUsedAsItem = New ListItem("Self Referral Contact", emergencyContactID)
                                .ContactAlsoUsedAs.Items.Add(newAlsoUsedAsItem)
                                If (emergencyContactID > 0) AndAlso (emergencyContactID = selfReferralContactID) Then
                                    newAlsoUsedAsItem.Selected = True
                                    selfReferralShowSameAs = True
                                End If
                            End If
                            If (managerContactID = 0) Or (selfReferralContact2ID <> managerContactID) Then
                                newAlsoUsedAsItem = New ListItem("Self Referral Contact 2", emergencyContactID)
                                .ContactAlsoUsedAs.Items.Add(newAlsoUsedAsItem)
                                If (emergencyContactID > 0) AndAlso (emergencyContactID = selfReferralContact2ID) Then
                                    newAlsoUsedAsItem.Selected = True
                                    selfReferral2ShowSameAs = True
                                End If
                            End If
                        Else
                            If editMode Then .DisableContactTab()
                        End If
                        If .ContactAlsoUsedAs.Items.Count = 0 Then .ContactAlsoUsedAs.Visible = False
                    End With

                    With contractAddress
                        If Not ContractShowSameAs Then
                            If (managerContactID = 0) Or (selfReferralContactID <> managerContactID) Then
                                If (emergencyContactID = 0) Or (selfReferralContactID <> emergencyContactID) Then
                                    newAlsoUsedAsItem = New ListItem("Self Referral Contact", emergencyContactID)
                                    .ContactAlsoUsedAs.Items.Add(newAlsoUsedAsItem)
                                    If (contractContactID > 0) AndAlso (contractContactID = selfReferralContactID) Then
                                        newAlsoUsedAsItem.Selected = True
                                        selfReferralShowSameAs = True
                                    End If
                                End If
                            End If
                            If (managerContactID = 0) Or (selfReferralContact2ID <> managerContactID) Then
                                If (emergencyContactID = 0) Or (selfReferralContact2ID <> emergencyContactID) Then
                                    newAlsoUsedAsItem = New ListItem("Self Referral Contact 2", emergencyContactID)
                                    .ContactAlsoUsedAs.Items.Add(newAlsoUsedAsItem)
                                    If (contractContactID > 0) AndAlso (contractContactID = selfReferralContact2ID) Then
                                        newAlsoUsedAsItem.Selected = True
                                        selfReferral2ShowSameAs = True
                                    End If
                                End If
                            End If
                        Else
                            If editMode Then .DisableContactTab()
                        End If
                        If .ContactAlsoUsedAs.Items.Count = 0 Then .ContactAlsoUsedAs.Visible = False
                    End With

                    With selfReferralAddress
                        If Not selfReferralShowSameAs Then
                            If (managerContactID = 0) Or (selfReferralContact2ID <> managerContactID) Then
                                If (emergencyContactID = 0) Or (selfReferralContact2ID <> emergencyContactID) Then
                                    If (contractContactID = 0) Or (selfReferralContact2ID <> contractContactID) Then
                                        newAlsoUsedAsItem = New ListItem("Self Referral Contact 2", emergencyContactID)
                                        .ContactAlsoUsedAs.Items.Add(newAlsoUsedAsItem)
                                        If (selfReferralContactID > 0) AndAlso (selfReferralContactID = selfReferralContact2ID) Then
                                            newAlsoUsedAsItem.Selected = True
                                        End If
                                    End If
                                End If
                            End If
                        Else
                            If editMode Then .DisableContactTab()
                        End If
                        If .ContactAlsoUsedAs.Items.Count = 0 Then .ContactAlsoUsedAs.Visible = False
                    End With

                    With selfReferralAddress2
                        If selfReferral2ShowSameAs AndAlso editMode Then .DisableContactTab()
                        .ContactAlsoUsedAs.Visible = False
                    End With

                    'Populate Property Tab
                    PropertySelector1.init_Control(serviceID)

                Catch ex As Exception
                    msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME, "ViewService.Page_load")     ' error reading data
                    Target.Library.Web.Utils.DisplayError(msg)
                Finally
                    If Not reader Is Nothing AndAlso Not reader.IsClosed Then reader.Close()
                End Try
            End If
        End Sub

        Private Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click

            Dim msg As ErrorMessage
            Dim settings As WebAmendReqDataItemCollection = Nothing
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim processedFieldCount As Integer
            Dim ServiceID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim backUrl As String = HttpUtility.UrlEncode(Request.QueryString("backUrl"))

            If IsValid Then
                ' get the edtiable field settings
                msg = AmendReqBL.FetchEditableFieldSettings(Me.DbConnection, user.ExternalUserID, settings)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                ' create amendment requests
                msg = AmendReqBL.ProcessEditableFields(Page.Controls, settings, Me.DbConnection, ConnectionStrings("Abacus").ConnectionString, AppSettings("SiteID"), user.ExternalUsername, user.ID, processedFieldCount)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                Response.Redirect(String.Format("ViewService.aspx?id={0}&backUrl={1}&processedFieldCount={2}", ServiceID, backUrl, processedFieldCount))
            End If

        End Sub

    End Class
End Namespace
