
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Text
Imports System
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps.AmendReq
Imports Target.Web.Apps.AmendReq.Collections
Imports Target.Web.Apps.Security
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.SP.Library

Namespace Apps
    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.ViewProvider
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' View selected providers details
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO  05/10/2007  Ported to use AjaxToolkit Tab control.    
    '''     MikeVO  08/03/2007  Hide Amend Req buttons based on security item.
    '''     MikeVO  07/02/2007  Use SetDropdownListValue() to safely set dropdown values.
    '''     MikeVO  26/01/2007  SPBG-305 - handle when registration detail dates are null.
    '''     MikeVO  27/11/2006  Check user is allowed to view the provider.
    '''     MikeVO  03/10/2006  Further fixes for AddressContact user control AlsoUsedAs logic.
    '''     MikeVO  03/10/2006  Further changes for AddressContact user control.
    '''     MikeVO  03/10/2006  Implemented AddressContact user control.
    '''     MikeVO  02/10/2006  Implemented amendment requests.
    '''     MikeVO  28/09/2006  TabStrip and other formatting fixes.
    ''' 	[paul]	28/09/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class ViewProvider
        Inherits Target.Web.Apps.BasePage

        Private _haveRegistrationDetails As Boolean

        Protected WithEvents providerAddress As Target.SP.Web.Apps.UserControls.AddressContact
        Protected WithEvents contactAddress As Target.SP.Web.Apps.UserControls.AddressContact
        Protected WithEvents billingAddress As Target.SP.Web.Apps.UserControls.AddressContact

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Const SP_NAME As String = "spxProvider_Fetch"

            Dim providerID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim editMode As Boolean = Convert.ToBoolean(Target.Library.Utils.ToInt32(Request.QueryString("editMode")))
            Dim processedFieldCount As Integer = Target.Library.Utils.ToInt32(Request.QueryString("processedFieldCount"))
            Dim msg As ErrorMessage = New ErrorMessage
            Dim reader As SqlDataReader = Nothing
            Dim strProviderType As New StringBuilder
            Dim strEthnicOrigins As New StringBuilder
            Dim strAlsoUsedAs As New StringBuilder
            Dim strUrl As String
            Dim bOutputList As Boolean
            Dim settings As WebAmendReqDataItemCollection = Nothing
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim providerAddressID As Integer, contactAddressID As Integer, billingAddressID As Integer
            Dim providerContactID As Integer, contactContactID As Integer, billingContactID As Integer
            Dim canView As Boolean, canViewAmendReqs As Boolean

            Me.InitPage(ConstantsManager.GetConstant("webSecurityItemSPProviderView"), "Provider Details")

            ' check user is allowed to view this provider
            msg = SPClassesBL.UserCanViewProvider(Me.DbConnection, user.ExternalUserID, providerID, canView)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            If Not canView Then Response.Redirect("~/Library/Errors/AccessDenied.aspx")

            Me.AddExtraCssStyle("label.label { float:left; width:15em; font-weight: bold; }")
            Me.AddExtraCssStyle("ul.list { margin:0em;list-style:none; }")
            Me.AddExtraCssStyle(".content { width:70%; float:left; }")

            canViewAmendReqs = SecurityBL.UserHasItem(Me.DbConnection, user.ID, ConstantsManager.GetConstant("webSecurityItemAmendReqListRequests"))

            ' setup provider address control
            With providerAddress
                ' address
                .InitControl(Me.DbConnection, providerAddress.ID, True)
                .Address.EditableDataItemConstant = "SPamendReqDataItemProviderMainAddress"
                .Address.EditableDataFieldConstant = "SPamendReqDataFieldProviderMainAddressAddress"
                .Postcode.EditableDataItemConstant = "SPamendReqDataItemProviderMainAddress"
                .Postcode.EditableDataFieldConstant = "SPamendReqDataFieldProviderMainAddressPostcode"
                .AdminAuthority.EditableDataItemConstant = "SPamendReqDataItemProviderMainAddressWard"
                .AdminAuthority.EditableDataFieldConstant = "SPamendReqDataFieldProviderMainAddressWardAdminAuthority"
                .District.EditableDataItemConstant = "SPamendReqDataItemProviderMainAddressWard"
                .District.EditableDataFieldConstant = "SPamendReqDataFieldProviderMainAddressWardDistrict"
                .Ward.EditableDataItemConstant = "SPamendReqDataItemProviderMainAddressWard"
                .Ward.EditableDataFieldConstant = "SPamendReqDataItemProviderMainAddressWardWard"
                .Directions.EditableDataItemConstant = "SPamendReqDataItemProviderMainAddress"
                .Directions.EditableDataFieldConstant = "SPamendReqDataFieldProviderMainAddressDirections"
                .DisabledAccess.EditableDataItemConstant = "SPamendReqDataItemProviderMainAddress"
                .DisabledAccess.EditableDataFieldConstant = "SPamendReqDataFieldProviderMainAddressDisabledAccess"
                .AlsoUsedAs.EditableDataItemConstant = "SPamendReqDataItemProviderMainAddress"
                .AlsoUsedAs.EditableDataFieldConstant = "SPamendReqDataFieldProviderMainAddressAlsoUsedAs"
                .UPRN.EditableDataItemConstant = "SPamendReqDataItemProviderMainAddressURNs"
                .UPRN.EditableDataFieldConstant = "SPamendReqDataFieldProviderMainAddressURNsUPRN"
                .USRN.EditableDataItemConstant = "SPamendReqDataItemProviderMainAddressURNs"
                .USRN.EditableDataFieldConstant = "SPamendReqDataFieldProviderMainAddressURNsUSRN"
                ' contact
                .ContactType.EditableDataItemConstant = "SPamendReqDataItemProviderMainAddressContact"
                .ContactType.EditableDataFieldConstant = "SPamendReqDataFieldProviderMainAddressContactType"
                .ContactOrganisation.EditableDataItemConstant = "SPamendReqDataItemProviderMainAddressContact"
                .ContactOrganisation.EditableDataFieldConstant = "SPamendReqDataFieldProviderMainAddressContactOrganisation"
                .ContactTitle.EditableDataItemConstant = "SPamendReqDataItemProviderMainAddressContact"
                .ContactTitle.EditableDataFieldConstant = "SPamendReqDataFieldProviderMainAddressContactTitle"
                .ContactForenames.EditableDataItemConstant = "SPamendReqDataItemProviderMainAddressContact"
                .ContactForenames.EditableDataFieldConstant = "SPamendReqDataFieldProviderMainAddressContactForenames"
                .ContactSurname.EditableDataItemConstant = "SPamendReqDataItemProviderMainAddressContact"
                .ContactSurname.EditableDataFieldConstant = "SPamendReqDataFieldProviderMainAddressContactSurname"
                .ContactPosition.EditableDataItemConstant = "SPamendReqDataItemProviderMainAddressContact"
                .ContactPosition.EditableDataFieldConstant = "SPamendReqDataFieldProviderMainAddressContactPosition"
                .ContactTel.EditableDataItemConstant = "SPamendReqDataItemProviderMainAddressContact"
                .ContactTel.EditableDataFieldConstant = "SPamendReqDataFieldProviderMainAddressContactTelephoneNo"
                .ContactFax.EditableDataItemConstant = "SPamendReqDataItemProviderMainAddressContact"
                .ContactFax.EditableDataFieldConstant = "SPamendReqDataFieldProviderMainAddressContactFaxNo"
                .ContactMobile.EditableDataItemConstant = "SPamendReqDataItemProviderMainAddressContact"
                .ContactMobile.EditableDataFieldConstant = "SPamendReqDataFieldProviderMainAddressContactMobileNo"
                .ContactPager.EditableDataItemConstant = "SPamendReqDataItemProviderMainAddressContact"
                .ContactPager.EditableDataFieldConstant = "SPamendReqDataFieldProviderMainAddressContactPagerNo"
                .ContactEmail.EditableDataItemConstant = "SPamendReqDataItemProviderMainAddressContact"
                .ContactEmail.EditableDataFieldConstant = "SPamendReqDataFieldProviderMainAddressContactEmailAddress"
                .ContactWeb.EditableDataItemConstant = "SPamendReqDataItemProviderMainAddressContact"
                .ContactWeb.EditableDataFieldConstant = "SPamendReqDataFieldProviderMainAddressContactWebAddress"
                .ContactAlsoUsedAs.EditableDataItemConstant = "SPamendReqDataItemProviderMainAddressContact"
                .ContactAlsoUsedAs.EditableDataFieldConstant = "SPamendReqDataFieldProviderMainAddressContactAlsoUsedAs"
            End With

            ' setup contact address control
            With contactAddress
                ' address
                .InitControl(Me.DbConnection, contactAddress.ID, True)
                .Address.EditableDataItemConstant = "SPamendReqDataItemProviderContactAddress"
                .Address.EditableDataFieldConstant = "SPamendReqDataFieldProviderContactAddressAddress"
                .Postcode.EditableDataItemConstant = "SPamendReqDataItemProviderContactAddress"
                .Postcode.EditableDataFieldConstant = "SPamendReqDataFieldProviderContactAddressPostcode"
                .AdminAuthority.EditableDataItemConstant = "SPamendReqDataItemProviderContactAddressWard"
                .AdminAuthority.EditableDataFieldConstant = "SPamendReqDataFieldProviderContactAddressWardAdminAuthority"
                .District.EditableDataItemConstant = "SPamendReqDataItemProviderContactAddressWard"
                .District.EditableDataFieldConstant = "SPamendReqDataFieldProviderContactAddressWardDistrict"
                .Ward.EditableDataItemConstant = "SPamendReqDataItemProviderContactAddressWard"
                .Ward.EditableDataFieldConstant = "SPamendReqDataItemProviderContactAddressWardWard"
                .Directions.EditableDataItemConstant = "SPamendReqDataItemProviderContactAddress"
                .Directions.EditableDataFieldConstant = "SPamendReqDataFieldProviderContactAddressDirections"
                .DisabledAccess.EditableDataItemConstant = "SPamendReqDataItemProviderContactAddress"
                .DisabledAccess.EditableDataFieldConstant = "SPamendReqDataFieldProviderContactAddressDisabledAccess"
                .AlsoUsedAs.EditableDataItemConstant = "SPamendReqDataItemProviderContactAddress"
                .AlsoUsedAs.EditableDataFieldConstant = "SPamendReqDataFieldProviderContactAddressAlsoUsedAs"
                .UPRN.EditableDataItemConstant = "SPamendReqDataItemProviderContactAddressURNs"
                .UPRN.EditableDataFieldConstant = "SPamendReqDataFieldProviderContactAddressURNsUPRN"
                .USRN.EditableDataItemConstant = "SPamendReqDataItemProviderContactAddressURNs"
                .USRN.EditableDataFieldConstant = "SPamendReqDataFieldProviderContactAddressURNsUSRN"
                ' contact
                .ContactType.EditableDataItemConstant = "SPamendReqDataItemProviderContactAddressContact"
                .ContactType.EditableDataFieldConstant = "SPamendReqDataFieldProviderContactAddressContactType"
                .ContactOrganisation.EditableDataItemConstant = "SPamendReqDataItemProviderContactAddressContact"
                .ContactOrganisation.EditableDataFieldConstant = "SPamendReqDataFieldProviderContactAddressContactOrganisation"
                .ContactTitle.EditableDataItemConstant = "SPamendReqDataItemProviderContactAddressContact"
                .ContactTitle.EditableDataFieldConstant = "SPamendReqDataFieldProviderContactAddressContactTitle"
                .ContactForenames.EditableDataItemConstant = "SPamendReqDataItemProviderContactAddressContact"
                .ContactForenames.EditableDataFieldConstant = "SPamendReqDataFieldProviderContactAddressContactForenames"
                .ContactSurname.EditableDataItemConstant = "SPamendReqDataItemProviderContactAddressContact"
                .ContactSurname.EditableDataFieldConstant = "SPamendReqDataFieldProviderContactAddressContactSurname"
                .ContactPosition.EditableDataItemConstant = "SPamendReqDataItemProviderContactAddressContact"
                .ContactPosition.EditableDataFieldConstant = "SPamendReqDataFieldProviderContactAddressContactPosition"
                .ContactTel.EditableDataItemConstant = "SPamendReqDataItemProviderContactAddressContact"
                .ContactTel.EditableDataFieldConstant = "SPamendReqDataFieldProviderContactAddressContactTelephoneNo"
                .ContactFax.EditableDataItemConstant = "SPamendReqDataItemProviderContactAddressContact"
                .ContactFax.EditableDataFieldConstant = "SPamendReqDataFieldProviderContactAddressContactFaxNo"
                .ContactMobile.EditableDataItemConstant = "SPamendReqDataItemProviderContactAddressContact"
                .ContactMobile.EditableDataFieldConstant = "SPamendReqDataFieldProviderContactAddressContactMobileNo"
                .ContactPager.EditableDataItemConstant = "SPamendReqDataItemProviderContactAddressContact"
                .ContactPager.EditableDataFieldConstant = "SPamendReqDataFieldProviderContactAddressContactPagerNo"
                .ContactEmail.EditableDataItemConstant = "SPamendReqDataItemProviderContactAddressContact"
                .ContactEmail.EditableDataFieldConstant = "SPamendReqDataFieldProviderContactAddressContactEmailAddress"
                .ContactWeb.EditableDataItemConstant = "SPamendReqDataItemProviderContactAddressContact"
                .ContactWeb.EditableDataFieldConstant = "SPamendReqDataFieldProviderContactAddressContactWebAddress"
                .ContactAlsoUsedAs.EditableDataItemConstant = "SPamendReqDataItemProviderContactAddressContact"
                .ContactAlsoUsedAs.EditableDataFieldConstant = "SPamendReqDataFieldProviderContactAddressContactAlsoUsedAs"
            End With

            ' setup billing address control
            With billingAddress
                ' address
                .InitControl(Me.DbConnection, billingAddress.ID, False)
                .Address.EditableDataItemConstant = "SPamendReqDataItemProviderBillingAddress"
                .Address.EditableDataFieldConstant = "SPamendReqDataFieldProviderBillingAddressAddress"
                .Postcode.EditableDataItemConstant = "SPamendReqDataItemProviderBillingAddress"
                .Postcode.EditableDataFieldConstant = "SPamendReqDataFieldProviderBillingAddressPostcode"
                .AdminAuthority.EditableDataItemConstant = "SPamendReqDataItemProviderBillingAddressWard"
                .AdminAuthority.EditableDataFieldConstant = "SPamendReqDataFieldProviderBillingAddressWardAdminAuthority"
                .District.EditableDataItemConstant = "SPamendReqDataItemProviderBillingAddressWard"
                .District.EditableDataFieldConstant = "SPamendReqDataFieldProviderBillingAddressWardDistrict"
                .Ward.EditableDataItemConstant = "SPamendReqDataItemProviderBillingAddressWard"
                .Ward.EditableDataFieldConstant = "SPamendReqDataItemProviderBillingAddressWardWard"
                .Directions.EditableDataItemConstant = "SPamendReqDataItemProviderBillingAddress"
                .Directions.EditableDataFieldConstant = "SPamendReqDataFieldProviderBillingAddressDirections"
                .DisabledAccess.EditableDataItemConstant = "SPamendReqDataItemProviderBillingAddress"
                .DisabledAccess.EditableDataFieldConstant = "SPamendReqDataFieldProviderBillingAddressDisabledAccess"
                .UPRN.EditableDataItemConstant = "SPamendReqDataItemProviderBillingAddressURNs"
                .UPRN.EditableDataFieldConstant = "SPamendReqDataFieldProviderBillingAddressURNsUPRN"
                .USRN.EditableDataItemConstant = "SPamendReqDataItemProviderBillingAddressURNs"
                .USRN.EditableDataFieldConstant = "SPamendReqDataFieldProviderBillingAddressURNsUSRN"
                ' contact
                .ContactType.EditableDataItemConstant = "SPamendReqDataItemProviderBillingAddressContact"
                .ContactType.EditableDataFieldConstant = "SPamendReqDataFieldProviderBillingAddressContactType"
                .ContactOrganisation.EditableDataItemConstant = "SPamendReqDataItemProviderBillingAddressContact"
                .ContactOrganisation.EditableDataFieldConstant = "SPamendReqDataFieldProviderBillingAddressContactOrganisation"
                .ContactTitle.EditableDataItemConstant = "SPamendReqDataItemProviderBillingAddressContact"
                .ContactTitle.EditableDataFieldConstant = "SPamendReqDataFieldProviderBillingAddressContactTitle"
                .ContactForenames.EditableDataItemConstant = "SPamendReqDataItemProviderBillingAddressContact"
                .ContactForenames.EditableDataFieldConstant = "SPamendReqDataFieldProviderBillingAddressContactForenames"
                .ContactSurname.EditableDataItemConstant = "SPamendReqDataItemProviderBillingAddressContact"
                .ContactSurname.EditableDataFieldConstant = "SPamendReqDataFieldProviderBillingAddressContactSurname"
                .ContactPosition.EditableDataItemConstant = "SPamendReqDataItemProviderBillingAddressContact"
                .ContactPosition.EditableDataFieldConstant = "SPamendReqDataFieldProviderBillingAddressContactPosition"
                .ContactTel.EditableDataItemConstant = "SPamendReqDataItemProviderBillingAddressContact"
                .ContactTel.EditableDataFieldConstant = "SPamendReqDataFieldProviderBillingAddressContactTelephoneNo"
                .ContactFax.EditableDataItemConstant = "SPamendReqDataItemProviderBillingAddressContact"
                .ContactFax.EditableDataFieldConstant = "SPamendReqDataFieldProviderBillingAddressContactFaxNo"
                .ContactMobile.EditableDataItemConstant = "SPamendReqDataItemProviderBillingAddressContact"
                .ContactMobile.EditableDataFieldConstant = "SPamendReqDataFieldProviderBillingAddressContactMobileNo"
                .ContactPager.EditableDataItemConstant = "SPamendReqDataItemProviderBillingAddressContact"
                .ContactPager.EditableDataFieldConstant = "SPamendReqDataFieldProviderBillingAddressContactPagerNo"
                .ContactEmail.EditableDataItemConstant = "SPamendReqDataItemProviderBillingAddressContact"
                .ContactEmail.EditableDataFieldConstant = "SPamendReqDataFieldProviderBillingAddressContactEmailAddress"
                .ContactWeb.EditableDataItemConstant = "SPamendReqDataItemProviderBillingAddressContact"
                .ContactWeb.EditableDataFieldConstant = "SPamendReqDataFieldProviderBillingAddressContactWebAddress"
            End With

            ' setup edit buttons
            If canViewAmendReqs Then
                btnEdit.Visible = Not editMode
                btnCancel.Visible = editMode
                btnSubmit.Style.Add("float", "left")
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

                ' load the screen
                Try
                    Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME, False)
                    spParams(0).Value = providerID

                    ' execute
                    reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams)

                    While reader.Read()
                        'Populate Provider Details Tab
                        litReference.Text = WebUtils.EncodeOutput(reader("Reference"))
                        txtLocalProviderID.Text = WebUtils.EncodeOutput(reader("LocalProviderID"), txtLocalProviderID.IsReadOnly)
                        txtLocalProviderID.RecordID = providerID
                        litName.Text = WebUtils.EncodeOutput(reader("Name"))
                        txtWebsite.Text = WebUtils.EncodeOutput(reader("ProviderWebsite"), txtLocalProviderID.IsReadOnly)
                        txtWebsite.RecordID = providerID
                        litOrgType.Text = WebUtils.EncodeOutput(reader("OrganisationType"))
                        litCreditorsRef.Text = WebUtils.EncodeOutput(reader("CreditorsReference"))
                        If reader("VATExempt") Then
                            litVATExempt.Text = "Yes"
                        Else
                            litVATExempt.Text = "No"
                        End If
                        litVATNumber.Text = WebUtils.EncodeOutput(reader("VATNumber"))

                        dteYearEndDate.Text = WebUtils.EncodeOutput("", dteYearEndDate.IsReadOnly)
                        If Not Convert.IsDBNull(reader("FinancialYearEnd")) Then dteYearEndDate.Text = Format(reader("FinancialYearEnd"), "dd/MM/yyyy")
                        dteYearEndDate.RecordID = providerID

                        'create un-ordered list using the string builder class
                        If reader("ServiceProvider") Or reader("Landlord") Or reader("AccomodationManager") Then
                            strProviderType.Append("<ul class=""list"">")
                            If reader("ServiceProvider") Then
                                strProviderType.Append("<li>Service Provider</li>")
                            End If
                            If reader("Landlord") Then
                                strProviderType.Append("<li>Landlord</li>")
                            End If
                            If reader("AccomodationManager") Then
                                strProviderType.Append("<li>Accomodation Manager</li>")
                            End If
                            strProviderType.Append("</ul>")
                            litProviderType.Text = strProviderType.ToString
                        End If

                        If reader("AccreditationExemptStatus") Then
                            litAccreditationExempt.Text = "Yes"
                        Else
                            litAccreditationExempt.Text = "No"
                        End If

                        'Provider Address
                        With providerAddress
                            ' address
                            .SetAddressIDs(Target.Library.Utils.ToInt32(reader("ProviderAddressID")), providerID)
                            .Address.Text = WebUtils.EncodeOutput(reader("PAAddress"), .Address.IsReadOnly)
                            .Postcode.Text = WebUtils.EncodeOutput(reader("PAPostCode"), .Postcode.IsReadOnly)
                            .AdminAuthority.Text = WebUtils.EncodeOutput(reader("PAAdminAuthority"), .AdminAuthority.IsReadOnly)
                            .District.Text = WebUtils.EncodeOutput(reader("PADistrict"), .District.IsReadOnly)
                            .Ward.Text = WebUtils.EncodeOutput(reader("PAWard"), .Ward.IsReadOnly)
                            .Directions.Text = WebUtils.EncodeOutput(reader("PADirections"), .Directions.IsReadOnly)
                            .UPRN.Text = WebUtils.EncodeOutput(reader("PAUPRN"), .UPRN.IsReadOnly)
                            .USRN.Text = WebUtils.EncodeOutput(reader("PAUSRN"), .USRN.IsReadOnly)

                            .Confidential.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("PAConfidential")) Then
                                If reader("PAConfidential") Then
                                    .Confidential.Text = "Yes"
                                Else
                                    .Confidential.Text = "No"
                                End If
                            End If

                            .DisabledAccess.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("PADisabledAccess")) Then
                                If .DisabledAccess.IsReadOnly Then
                                    If reader("PADisabledAccess") = "Y" Then
                                        .DisabledAccess.Text = "Yes"
                                    ElseIf reader("PADisabledAccess") = "N" Then
                                        .DisabledAccess.Text = "No"
                                    End If
                                Else
                                    WebUtils.SetDropdownListValue(.DisabledAccess.DropDownList, reader("PADisabledAccess"))
                                End If
                            End If

                            .AlsoUsedAs.Items.Add(New ListItem("Contact Address", Target.Library.Utils.ToInt32(reader("ProviderAddressID"))))
                            If Not Convert.IsDBNull(reader("ContactAddressID")) Then
                                If Target.Library.Utils.ToInt32(reader("ProviderAddressID")) = reader("ContactAddressID") Then
                                    .AlsoUsedAs.Items(0).Selected = True
                                    If editMode Then contactAddress.DisableAddressTab()
                                End If
                            End If
                            .AlsoUsedAs.Items.Add(New ListItem("Billing Address", Target.Library.Utils.ToInt32(reader("ProviderAddressID"))))
                            If Not Convert.IsDBNull(reader("BillingAddressID")) Then
                                If Target.Library.Utils.ToInt32(reader("ProviderAddressID")) = reader("BillingAddressID") Then
                                    .AlsoUsedAs.Items(1).Selected = True
                                    If editMode Then billingAddress.DisableAddressTab()
                                End If
                            End If

                            ' contact
                            .SetContactIDs(Target.Library.Utils.ToInt32(reader("ProviderContactID")), providerID)
                            .ContactType.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("PCContactType")) Then
                                If .ContactType.IsReadOnly Then
                                    Select Case Convert.ToString(reader("PCContactType"))
                                        Case "P"
                                            .ContactType.Text = "Person"
                                        Case "R"
                                            .ContactType.Text = "Role"
                                        Case "O"
                                            .ContactType.Text = "Organisation"
                                    End Select
                                Else
                                    WebUtils.SetDropdownListValue(.ContactType.DropDownList, reader("PCContactType"))
                                End If
                            End If
                            .ContactOrganisation.Text = WebUtils.EncodeOutput(reader("PCOrganisation"), .ContactOrganisation.IsReadOnly)
                            .ContactTitle.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("PCTitle")) Then
                                If .ContactTitle.IsReadOnly Then
                                    .ContactTitle.Text = WebUtils.EncodeOutput(reader("PCTitle"))
                                Else
                                    WebUtils.SetDropdownListValue(.ContactTitle.DropDownList, reader("PCTitle"))
                                End If
                            End If
                            .ContactForenames.Text = WebUtils.EncodeOutput(reader("PCFirstNames"), .ContactForenames.IsReadOnly)
                            .ContactSurname.Text = WebUtils.EncodeOutput(reader("PCSurname"), .ContactSurname.IsReadOnly)
                            .ContactPosition.Text = WebUtils.EncodeOutput(reader("PCPosition"), .ContactPosition.IsReadOnly)
                            .ContactTel.Text = WebUtils.EncodeOutput(reader("PCTelNo"), .ContactTel.IsReadOnly)
                            .ContactFax.Text = WebUtils.EncodeOutput(reader("PCFaxNo"), .ContactFax.IsReadOnly)
                            .ContactMobile.Text = WebUtils.EncodeOutput(reader("PCMobileNo"), .ContactMobile.IsReadOnly)
                            .ContactPager.Text = WebUtils.EncodeOutput(reader("PCPagerNo"), .ContactPager.IsReadOnly)
                            If Not Convert.IsDBNull(reader("PCEmailAddress")) AndAlso Convert.ToString(reader("PCEmailAddress")).Length > 0 Then
                                .ContactEmail.Text = WebUtils.EncodeOutput(reader("PCEmailAddress"), .ContactEmail.IsReadOnly)
                                .ContactEmail.Link.NavigateUrl = "mailto:" & reader("PCEmailAddress")
                            End If
                            If Not Convert.IsDBNull(reader("PCWebAddress")) AndAlso Convert.ToString(reader("PCWebAddress")).Length > 0 Then
                                strUrl = reader("PCWebAddress")
                                .ContactWeb.Text = WebUtils.EncodeOutput(strUrl, .ContactWeb.IsReadOnly)
                                .ContactWeb.Link.NavigateUrl = IIf(Not strUrl.StartsWith("http"), "http://", String.Empty) & strUrl
                                .ContactWeb.Link.Attributes.Add("rel", "external")
                            End If
                            .ContactAlsoUsedAs.Items.Add(New ListItem("Contact Address", Target.Library.Utils.ToInt32(reader("ProviderContactID"))))
                            If Not Convert.IsDBNull(reader("ContactContactID")) Then
                                If Target.Library.Utils.ToInt32(reader("ProviderContactID")) = reader("ContactContactID") Then
                                    .ContactAlsoUsedAs.Items(0).Selected = True
                                    If editMode Then contactAddress.DisableContactTab()
                                End If
                            End If
                            .ContactAlsoUsedAs.Items.Add(New ListItem("Billing Address", Target.Library.Utils.ToInt32(reader("ProviderContactID"))))
                            If Not Convert.IsDBNull(reader("BillingContactID")) Then
                                If Target.Library.Utils.ToInt32(reader("ProviderContactID")) = reader("BillingContactID") Then
                                    .ContactAlsoUsedAs.Items(1).Selected = True
                                    If editMode Then billingAddress.DisableContactTab()
                                End If
                            End If

                        End With

                        ' contact Address
                        With contactAddress
                            ' address
                            .SetAddressIDs(Target.Library.Utils.ToInt32(reader("ContactAddressID")), providerID)
                            .Address.Text = WebUtils.EncodeOutput(reader("CAAddress"), .Address.IsReadOnly)
                            .Postcode.Text = WebUtils.EncodeOutput(reader("CAPostCode"), .Postcode.IsReadOnly)
                            .AdminAuthority.Text = WebUtils.EncodeOutput(reader("CAAdminAuthority"), .AdminAuthority.IsReadOnly)
                            .District.Text = WebUtils.EncodeOutput(reader("CADistrict"), .District.IsReadOnly)
                            .Ward.Text = WebUtils.EncodeOutput(reader("CAWard"), .Ward.IsReadOnly)
                            .Directions.Text = WebUtils.EncodeOutput(reader("CADirections"), .Directions.IsReadOnly)
                            .UPRN.Text = WebUtils.EncodeOutput(reader("CAUPRN"), .UPRN.IsReadOnly)
                            .USRN.Text = WebUtils.EncodeOutput(reader("CAUSRN"), .USRN.IsReadOnly)

                            .Confidential.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("CAConfidential")) Then
                                If reader("CAConfidential") Then
                                    .Confidential.Text = "Yes"
                                Else
                                    .Confidential.Text = "No"
                                End If
                            End If

                            .DisabledAccess.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("CADisabledAccess")) Then
                                If .DisabledAccess.IsReadOnly Then
                                    If reader("CADisabledAccess") = "Y" Then
                                        .DisabledAccess.Text = "Yes"
                                    ElseIf reader("CADisabledAccess") = "N" Then
                                        .DisabledAccess.Text = "No"
                                    End If
                                Else
                                    WebUtils.SetDropdownListValue(.DisabledAccess.DropDownList, reader("CADisabledAccess"))
                                End If
                            End If

                            providerAddressID = Target.Library.Utils.ToInt32(reader("ProviderAddressID"))
                            contactAddressID = Target.Library.Utils.ToInt32(reader("ContactAddressID"))
                            billingAddressID = Target.Library.Utils.ToInt32(reader("BillingAddressID"))
                            If Not (providerAddressID > 0 AndAlso (providerAddressID = contactAddressID)) Then
                                If Not (providerAddressID > 0 AndAlso (providerAddressID = billingAddressID)) Then
                                    .AlsoUsedAs.Items.Add(New ListItem("Billing Address", providerAddressID))
                                    If billingAddressID > 0 Then
                                        If contactAddressID = billingAddressID Then
                                            .AlsoUsedAs.Items(0).Selected = True
                                            If editMode Then billingAddress.DisableAddressTab()
                                        End If
                                    End If
                                End If
                            End If
                            If .AlsoUsedAs.Items.Count = 0 Then .AlsoUsedAs.Visible = False

                            ' contact
                            .SetContactIDs(Target.Library.Utils.ToInt32(reader("ContactContactID")), providerID)
                            .ContactType.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("CCContactType")) Then
                                If .ContactType.IsReadOnly Then
                                    Select Case Convert.ToString(reader("CCContactType"))
                                        Case "P"
                                            .ContactType.Text = "Person"
                                        Case "R"
                                            .ContactType.Text = "Role"
                                        Case "O"
                                            .ContactType.Text = "Organisation"
                                    End Select
                                Else
                                    WebUtils.SetDropdownListValue(.ContactType.DropDownList, reader("CCContactType"))
                                End If
                            End If
                            .ContactOrganisation.Text = WebUtils.EncodeOutput(reader("CCOrganisation"), .ContactOrganisation.IsReadOnly)
                            .ContactTitle.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("CCTitle")) Then
                                If .ContactTitle.IsReadOnly Then
                                    .ContactTitle.Text = WebUtils.EncodeOutput(reader("CCTitle"))
                                Else
                                    WebUtils.SetDropdownListValue(.ContactTitle.DropDownList, reader("CCTitle"))
                                End If
                            End If
                            .ContactForenames.Text = WebUtils.EncodeOutput(reader("CCFirstNames"), .ContactForenames.IsReadOnly)
                            .ContactSurname.Text = WebUtils.EncodeOutput(reader("CCSurname"), .ContactSurname.IsReadOnly)
                            .ContactPosition.Text = WebUtils.EncodeOutput(reader("CCPosition"), .ContactPosition.IsReadOnly)
                            .ContactTel.Text = WebUtils.EncodeOutput(reader("CCTelNo"), .ContactTel.IsReadOnly)
                            .ContactFax.Text = WebUtils.EncodeOutput(reader("CCFaxNo"), .ContactFax.IsReadOnly)
                            .ContactMobile.Text = WebUtils.EncodeOutput(reader("CCMobileNo"), .ContactMobile.IsReadOnly)
                            .ContactPager.Text = WebUtils.EncodeOutput(reader("CCPagerNo"), .ContactPager.IsReadOnly)
                            If Not Convert.IsDBNull(reader("CCEmailAddress")) AndAlso Convert.ToString(reader("CCEmailAddress")).Length > 0 Then
                                .ContactEmail.Text = WebUtils.EncodeOutput(reader("CCEmailAddress"), .ContactEmail.IsReadOnly)
                                .ContactEmail.Link.NavigateUrl = "mailto:" & reader("CCEmailAddress")
                            End If
                            If Not Convert.IsDBNull(reader("CCWebAddress")) AndAlso Convert.ToString(reader("CCWebAddress")).Length > 0 Then
                                strUrl = reader("CCWebAddress")
                                .ContactWeb.Text = WebUtils.EncodeOutput(strUrl, .ContactWeb.IsReadOnly)
                                .ContactWeb.Link.NavigateUrl = IIf(Not strUrl.StartsWith("http"), "http://", String.Empty) & strUrl
                                .ContactWeb.Link.Attributes.Add("rel", "external")
                            End If

                            providerContactID = Target.Library.Utils.ToInt32(reader("ProviderContactID"))
                            contactContactID = Target.Library.Utils.ToInt32(reader("ContactContactID"))
                            billingContactID = Target.Library.Utils.ToInt32(reader("BillingContactID"))
                            If Not (providerContactID > 0 AndAlso (providerContactID = contactContactID)) Then
                                If Not (providerContactID > 0 AndAlso (providerContactID = billingContactID)) Then
                                    .ContactAlsoUsedAs.Items.Add(New ListItem("Billing Contact", providerContactID))
                                    If billingContactID > 0 Then
                                        If contactContactID = billingContactID Then
                                            .ContactAlsoUsedAs.Items(0).Selected = True
                                            If editMode Then billingAddress.DisableContactTab()
                                        End If
                                    End If
                                End If
                            End If
                            If .ContactAlsoUsedAs.Items.Count = 0 Then .ContactAlsoUsedAs.Visible = False

                        End With

                        ' billing Address
                        With billingAddress
                            ' address
                            .SetAddressIDs(Target.Library.Utils.ToInt32(reader("BillingAddressID")), providerID)
                            .Address.Text = WebUtils.EncodeOutput(reader("BAAddress"), .Address.IsReadOnly)
                            .Postcode.Text = WebUtils.EncodeOutput(reader("BAPostCode"), .Postcode.IsReadOnly)
                            .AdminAuthority.Text = WebUtils.EncodeOutput(reader("BAAdminAuthority"), .AdminAuthority.IsReadOnly)
                            .District.Text = WebUtils.EncodeOutput(reader("BADistrict"), .District.IsReadOnly)
                            .Ward.Text = WebUtils.EncodeOutput(reader("BAWard"), .Ward.IsReadOnly)
                            .Directions.Text = WebUtils.EncodeOutput(reader("BADirections"), .Directions.IsReadOnly)
                            .UPRN.Text = WebUtils.EncodeOutput(reader("BAUPRN"), .UPRN.IsReadOnly)
                            .USRN.Text = WebUtils.EncodeOutput(reader("BAUSRN"), .USRN.IsReadOnly)

                            .Confidential.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("BAConfidential")) Then
                                If reader("BAConfidential") Then
                                    .Confidential.Text = "Yes"
                                Else
                                    .Confidential.Text = "No"
                                End If
                            End If

                            .DisabledAccess.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("BADisabledAccess")) Then
                                If .DisabledAccess.IsReadOnly Then
                                    If reader("BADisabledAccess") = "Y" Then
                                        .DisabledAccess.Text = "Yes"
                                    ElseIf reader("BADisabledAccess") = "N" Then
                                        .DisabledAccess.Text = "No"
                                    End If
                                Else
                                    WebUtils.SetDropdownListValue(.DisabledAccess.DropDownList, reader("BADisabledAccess"))
                                End If
                            End If

                            ' contact
                            .SetContactIDs(Target.Library.Utils.ToInt32(reader("BillingContactID")), providerID)
                            .ContactType.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("BCContactType")) Then
                                If .ContactType.IsReadOnly Then
                                    Select Case Convert.ToString(reader("BCContactType"))
                                        Case "P"
                                            .ContactType.Text = "Person"
                                        Case "R"
                                            .ContactType.Text = "Role"
                                        Case "O"
                                            .ContactType.Text = "Organisation"
                                    End Select
                                Else
                                    WebUtils.SetDropdownListValue(.ContactType.DropDownList, reader("BCContactType"))
                                End If
                            End If
                            .ContactOrganisation.Text = WebUtils.EncodeOutput(reader("BCOrganisation"), .ContactOrganisation.IsReadOnly)
                            .ContactTitle.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("BCTitle")) Then
                                If .ContactTitle.IsReadOnly Then
                                    .ContactTitle.Text = WebUtils.EncodeOutput(reader("BCTitle"))
                                Else
                                    WebUtils.SetDropdownListValue(.ContactTitle.DropDownList, reader("BCTitle"))
                                End If
                            End If
                            .ContactForenames.Text = WebUtils.EncodeOutput(reader("BCFirstNames"), .ContactForenames.IsReadOnly)
                            .ContactSurname.Text = WebUtils.EncodeOutput(reader("BCSurname"), .ContactSurname.IsReadOnly)
                            .ContactPosition.Text = WebUtils.EncodeOutput(reader("BCPosition"), .ContactPosition.IsReadOnly)
                            .ContactTel.Text = WebUtils.EncodeOutput(reader("BCTelNo"), .ContactTel.IsReadOnly)
                            .ContactFax.Text = WebUtils.EncodeOutput(reader("BCFaxNo"), .ContactFax.IsReadOnly)
                            .ContactMobile.Text = WebUtils.EncodeOutput(reader("BCMobileNo"), .ContactMobile.IsReadOnly)
                            .ContactPager.Text = WebUtils.EncodeOutput(reader("BCPagerNo"), .ContactPager.IsReadOnly)
                            If Not Convert.IsDBNull(reader("BCEmailAddress")) AndAlso Convert.ToString(reader("BCEmailAddress")).Length > 0 Then
                                .ContactEmail.Text = WebUtils.EncodeOutput(reader("BCEmailAddress"), .ContactEmail.IsReadOnly)
                                .ContactEmail.Link.NavigateUrl = "mailto:" & reader("BCEmailAddress")
                            End If
                            If Not Convert.IsDBNull(reader("BCWebAddress")) AndAlso Convert.ToString(reader("BCWebAddress")).Length > 0 Then
                                strUrl = reader("BCWebAddress")
                                .ContactWeb.Text = WebUtils.EncodeOutput(strUrl, .ContactWeb.IsReadOnly)
                                .ContactWeb.Link.NavigateUrl = IIf(Not strUrl.StartsWith("http"), "http://", String.Empty) & strUrl
                                .ContactWeb.Link.Attributes.Add("rel", "external")
                            End If

                        End With

                    End While

                    'Ethnic origin Tab
                    reader.NextResult()
                    bOutputList = False
                    Dim bULOutput As Boolean = False
                    litEthnicOrigins.Text = WebUtils.EncodeOutput("")
                    If reader.HasRows Then
                        While reader.Read()
                            If Not Convert.IsDBNull(reader("BMESpecialistProvider")) Then
                                If reader("BMESpecialistProvider") Then
                                    litBMESpecific.Text = "Yes"
                                Else
                                    litBMESpecific.Text = "No"
                                End If
                            End If
                            bOutputList = True
                            If Not Convert.IsDBNull(reader("SupportedEthnicOrigin")) AndAlso Not bULOutput Then
                                If bOutputList Then strEthnicOrigins.Append("<ul class=""list"">")
                                bULOutput = True
                            End If
                            If Not Convert.IsDBNull(reader("SupportedEthnicOrigin")) Then strEthnicOrigins.AppendFormat("<li>{0}</li>", reader("SupportedEthnicOrigin"))
                            litCulturalGroup.Text = WebUtils.EncodeOutput(reader("CulturalGroup"))
                        End While
                        If bOutputList Then
                            If bULOutput Then strEthnicOrigins.Append("</ul>")
                            litEthnicOrigins.Text = strEthnicOrigins.ToString()
                        End If
                    End If

                    'Registration Details Tab
                    reader.NextResult()
                    _haveRegistrationDetails = reader.HasRows
                    Repeater1.DataSource = reader
                    Repeater1.DataBind()
                    ' close
                    reader.Close()

                    msg.Success = True

                Catch ex As Exception
                    msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME, "ViewProvider.Page_load")     ' error reading data
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
            Dim providerID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim backUrl As String = HttpUtility.UrlEncode(Request.QueryString("backUrl"))

            If IsValid Then
                ' get the edtiable field settings
                msg = AmendReqBL.FetchEditableFieldSettings(Me.DbConnection, user.ExternalUserID, settings)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                ' create amendment requests
                msg = AmendReqBL.ProcessEditableFields(Page.Controls, settings, Me.DbConnection, ConnectionStrings("Abacus").ConnectionString, AppSettings("SiteID"), user.ExternalUsername, user.ID, processedFieldCount)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                Response.Redirect(String.Format("ViewProvider.aspx?id={0}&backUrl={1}&processedFieldCount={2}", providerID, backUrl, processedFieldCount))
            End If

        End Sub

        Protected Function HaveRegistrationDetails() As Boolean
            Return _haveRegistrationDetails
        End Function

        Protected Function GetRegistrationDate(ByVal theDate As Object) As String
            If Target.Library.Utils.IsDate(theDate) Then
                Return DirectCast(theDate, Date).ToString("dd/MM/yyyy")
            Else
                Return String.Empty
            End If
        End Function

    End Class

End Namespace
