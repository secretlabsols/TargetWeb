Imports System
Imports System.Configuration.ConfigurationManager
Imports System.Text
Imports Target.Library
Imports Target.Library.Web
Imports System.Data.SqlClient
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.AmendReq
Imports Target.Web.Apps.AmendReq.Collections
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.SP.Library

Namespace Apps

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.ViewProperty
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Displays property information.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      08/10/2007  Ported to use AjaxToolkit Tab control.    
    '''     MikeVO      08/03/2007  Hide Amend Req buttons based on security item.
    '''     MikeVO      27/11/2006  Ensure user is allowed to view the property.
    ''' 	[Mikevo]	??/??/????	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class ViewProperty
        Inherits Target.Web.Apps.BasePage

        Protected WithEvents propertyAddress As Target.SP.Web.Apps.UserControls.AddressContact

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim strStyle As New StringBuilder
            Dim propertyID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("pid"))
            Dim editMode As Boolean = Convert.ToBoolean(Target.Library.Utils.ToInt32(Request.QueryString("editMode")))
            Dim processedFieldCount As Integer = Target.Library.Utils.ToInt32(Request.QueryString("processedFieldCount"))
            Dim reader As SqlDataReader = Nothing
            Dim settings As WebAmendReqDataItemCollection = Nothing
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim msg As ErrorMessage = New ErrorMessage
            Dim strUrl As String
            Dim canViewProperty As Boolean, canViewAmendReqs As Boolean

            Const SP_NAME As String = "spxSPProperty_Fetch"

            Me.InitPage(ConstantsManager.GetConstant("webSecurityItemSPPropertyView"), "Property Details")

            ' check to ensure the user is allowed to see the client and the property
            msg = SPClassesBL.UserCanViewProperty(Me.DbConnection, user.ExternalUserID, propertyID, canViewProperty)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            If Not canViewProperty Then Response.Redirect("~/Library/Errors/AccessDenied.aspx")

            strStyle.Append("label.label { float:left; width:19em; padding-right:1em; font-weight: bold; }")
            strStyle.Append("span.label { float:left; width:9em; padding-right:1em; font-weight: bold; }")
            strStyle.Append("ul.list { margin:0em;list-style:none; }")
            strStyle.Append(".content { float:left; width:60%; }")
            strStyle.Append(".content2 { float:left; width:6em; }")
            strStyle.Append(".content3 { float:left; width:9em; }")
            strStyle.Append(".content4 { float:left; width:30em; }")
            Me.AddExtraCssStyle(strStyle.ToString)

            canViewAmendReqs = SecurityBL.UserHasItem(Me.DbConnection, user.ID, ConstantsManager.GetConstant("webSecurityItemAmendReqListRequests"))

            ' setup provider address control
            With propertyAddress
                ' address
                .InitControl(Me.DbConnection, propertyAddress.ID, True)
                .Address.EditableDataItemConstant = "SPamendReqDataItemPropertyAddress"
                .Address.EditableDataFieldConstant = "SPamendReqDataFieldPropertyAddressAddress"
                .Postcode.EditableDataItemConstant = "SPamendReqDataItemPropertyAddress"
                .Postcode.EditableDataFieldConstant = "SPamendReqDataFieldPropertyAddressPostcode"
                .AdminAuthority.EditableDataItemConstant = "SPamendReqDataItemPropertyAddressWard"
                .AdminAuthority.EditableDataFieldConstant = "SPamendReqDataFieldPropertyAddressWardAdminAuthority"
                .District.EditableDataItemConstant = "SPamendReqDataItemPropertyAddressWard"
                .District.EditableDataFieldConstant = "SPamendReqDataFieldPropertyAddressWardDistrict"
                .Ward.EditableDataItemConstant = "SPamendReqDataItemPropertyAddressWard"
                .Ward.EditableDataFieldConstant = "SPamendReqDataFieldPropertyAddressWardWard"
                .Directions.EditableDataItemConstant = "SPamendReqDataItemPropertyAddress"
                .Directions.EditableDataFieldConstant = "SPamendReqDataFieldPropertyAddressDirections"
                .DisabledAccess.EditableDataItemConstant = "SPamendReqDataItemPropertyAddress"
                .DisabledAccess.EditableDataFieldConstant = "SPamendReqDataFieldPropertyAddressDisabledAccess"
                .AlsoUsedAs.EditableDataItemConstant = "SPamendReqDataItemPropertyAddress"
                .AlsoUsedAs.EditableDataFieldConstant = "SPamendReqDataFieldPropertyAddressAlsoUsedAs"
                .UPRN.EditableDataItemConstant = "SPamendReqDataItemPropertyAddressURNs"
                .UPRN.EditableDataFieldConstant = "SPamendReqDataFieldPropertyAddressURNsUPRN"
                .USRN.EditableDataItemConstant = "SPamendReqDataItemPropertyAddressURNs"
                .USRN.EditableDataFieldConstant = "SPamendReqDataFieldPropertyAddressURNsUSRN"
                ' contact
                .ContactType.EditableDataItemConstant = "SPamendReqDataItemPropertyAddressContact"
                .ContactType.EditableDataFieldConstant = "SPamendReqDataFieldPropertyAddressContactType"
                .ContactOrganisation.EditableDataItemConstant = "SPamendReqDataItemPropertyAddressContact"
                .ContactOrganisation.EditableDataFieldConstant = "SPamendReqDataFieldPropertyAddressContactOrganisation"
                .ContactTitle.EditableDataItemConstant = "SPamendReqDataItemPropertyAddressContact"
                .ContactTitle.EditableDataFieldConstant = "SPamendReqDataFieldPropertyAddressContactTitle"
                .ContactForenames.EditableDataItemConstant = "SPamendReqDataItemPropertyAddressContact"
                .ContactForenames.EditableDataFieldConstant = "SPamendReqDataFieldPropertyAddressContactForenames"
                .ContactSurname.EditableDataItemConstant = "SPamendReqDataItemPropertyAddressContact"
                .ContactSurname.EditableDataFieldConstant = "SPamendReqDataFieldPropertyAddressContactSurname"
                .ContactPosition.EditableDataItemConstant = "SPamendReqDataItemPropertyAddressContact"
                .ContactPosition.EditableDataFieldConstant = "SPamendReqDataFieldPropertyAddressContactPosition"
                .ContactTel.EditableDataItemConstant = "SPamendReqDataItemPropertyAddressContact"
                .ContactTel.EditableDataFieldConstant = "SPamendReqDataFieldPropertyAddressContactTelephoneNo"
                .ContactFax.EditableDataItemConstant = "SPamendReqDataItemPropertyAddressContact"
                .ContactFax.EditableDataFieldConstant = "SPamendReqDataFieldPropertyAddressContactFaxNo"
                .ContactMobile.EditableDataItemConstant = "SPamendReqDataItemPropertyAddressContact"
                .ContactMobile.EditableDataFieldConstant = "SPamendReqDataFieldPropertyAddressContactMobileNo"
                .ContactPager.EditableDataItemConstant = "SPamendReqDataItemPropertyAddressContact"
                .ContactPager.EditableDataFieldConstant = "SPamendReqDataFieldPropertyAddressContactPagerNo"
                .ContactEmail.EditableDataItemConstant = "SPamendReqDataItemPropertyAddressContact"
                .ContactEmail.EditableDataFieldConstant = "SPamendReqDataFieldPropertyAddressContactEmailAddress"
                .ContactWeb.EditableDataItemConstant = "SPamendReqDataItemPropertyAddressContact"
                .ContactWeb.EditableDataFieldConstant = "SPamendReqDataFieldPropertyAddressContactWebAddress"
                .ContactAlsoUsedAs.EditableDataItemConstant = "SPamendReqDataItemPropertyAddressContact"
                .ContactAlsoUsedAs.EditableDataFieldConstant = "SPamendReqDataFieldPropertyAddressContactAlsoUsedAs"
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
                    spParams(0).Value = propertyID

                    ' execute
                    reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams)

                    While reader.Read
                        'Property Details Tab
                        lblPropertyName.Text = WebUtils.EncodeOutput(reader("PropertyName"))
                        lblReference.Text = WebUtils.EncodeOutput(reader("Reference"))
                        txtAltReference.Text = WebUtils.EncodeOutput(reader("AltRef"), txtAltReference.IsReadOnly)
                        txtAltReference.RecordID = propertyID
                        lblRegistrationStatus.Text = WebUtils.EncodeOutput(reader("RegistrationStatus"))
                        lblRegistrationDate.Text = WebUtils.EncodeOutput(reader("RegDate"))
                        lblDeRegistered.Text = WebUtils.EncodeOutput("")
                        If Not Convert.IsDBNull(reader("DeRegistered")) Then
                            If reader("DeRegistered") = True Then
                                lblDeRegistered.Text = "Yes"
                            Else
                                lblDeRegistered.Text = "No"
                            End If
                        End If
                        lblFurnishingType.Text = WebUtils.EncodeOutput(reader("FurnishingType"))
                        lblWheelchair.Text = WebUtils.EncodeOutput(reader("WheelChairUnits"))
                        lblMobility.Text = WebUtils.EncodeOutput(reader("MobilityUnits"))
                        lblAdaptions.Text = WebUtils.EncodeOutput(reader("AidsAdaptionsUnits"))
                        lblTotal.Text = WebUtils.EncodeOutput(reader("TotalHouseholdUnits"))
                        'Information Tab
                        If reader("MealsSuppliedStatus") = True Then
                            chkMealsSupplied.CheckBox.Checked = True
                        Else
                            chkMealsSupplied.CheckBox.Checked = False
                        End If
                        chkMealsSupplied.RecordID = propertyID
                        If reader("CanteenOnSiteStatus") = True Then
                            chkCanteen.CheckBox.Checked = True
                        Else
                            chkCanteen.CheckBox.Checked = False
                        End If
                        chkCanteen.RecordID = propertyID
                        If reader("SelfCateringStatus") = True Then
                            chkSelfCatering.CheckBox.Checked = True
                        Else
                            chkSelfCatering.CheckBox.Checked = False
                        End If
                        chkSelfCatering.RecordID = propertyID
                        Shops.Text = WebUtils.EncodeOutput(reader("DistanceToShops"))
                        Shops.RecordID = propertyID
                        TrainStation.Text = WebUtils.EncodeOutput(reader("DistanceToTrainStation"))
                        TrainStation.RecordID = propertyID
                        TownCentre.Text = WebUtils.EncodeOutput(reader("DistanceToTownCentre"))
                        TownCentre.RecordID = propertyID
                        GP.Text = WebUtils.EncodeOutput(reader("DistanceToGP"))
                        GP.RecordID = propertyID
                        SecondarySchool.Text = WebUtils.EncodeOutput(reader("DistanceToSecondarySchool"))
                        SecondarySchool.RecordID = propertyID
                        PostOffice.Text = WebUtils.EncodeOutput(reader("DistanceToPostOffice"))
                        PostOffice.RecordID = propertyID
                        BusStop.Text = WebUtils.EncodeOutput(reader("DistanceToBusStop"))
                        BusStop.RecordID = propertyID
                        SocialCentre.Text = WebUtils.EncodeOutput(reader("DistanceToSocialCentre"))
                        SocialCentre.RecordID = propertyID
                        PrimarySchool.Text = WebUtils.EncodeOutput(reader("DistanceToPrimarySchool"))
                        PrimarySchool.RecordID = propertyID

                        'Location Tab
                        With propertyAddress
                            ' address
                            .SetAddressIDs(Target.Library.Utils.ToInt32(reader("AddressID")), propertyID)
                            .Address.Text = WebUtils.EncodeOutput(reader("Address"), .Address.IsReadOnly)
                            .Postcode.Text = WebUtils.EncodeOutput(reader("PostCode"), .Postcode.IsReadOnly)
                            .AdminAuthority.Text = WebUtils.EncodeOutput(reader("AdminAuthority"), .AdminAuthority.IsReadOnly)
                            .District.Text = WebUtils.EncodeOutput(reader("District"), .District.IsReadOnly)
                            .Ward.Text = WebUtils.EncodeOutput(reader("Ward"), .Ward.IsReadOnly)
                            .Directions.Text = WebUtils.EncodeOutput(reader("Directions"), .Directions.IsReadOnly)
                            .UPRN.Text = WebUtils.EncodeOutput(reader("UPRN"), .UPRN.IsReadOnly)
                            .USRN.Text = WebUtils.EncodeOutput(reader("USRN"), .USRN.IsReadOnly)

                            .Confidential.Text = WebUtils.EncodeOutput("")
                            If Not Convert.IsDBNull(reader("ConfidentialAddress")) Then
                                If reader("ConfidentialAddress") Then
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
                            .AlsoUsedAs.Visible = False

                            ' contact
                            .SetContactIDs(Target.Library.Utils.ToInt32(reader("ContactID")), propertyID)
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
                            .ContactOrganisation.Text = WebUtils.EncodeOutput(reader("OrganisationName"), .ContactOrganisation.IsReadOnly)
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
                            .ContactTel.Text = WebUtils.EncodeOutput(reader("TelephoneNumber"), .ContactTel.IsReadOnly)
                            .ContactFax.Text = WebUtils.EncodeOutput(reader("FaxNumber"), .ContactFax.IsReadOnly)
                            .ContactMobile.Text = WebUtils.EncodeOutput(reader("MobileNumber"), .ContactMobile.IsReadOnly)
                            .ContactPager.Text = WebUtils.EncodeOutput(reader("PagerNumber"), .ContactPager.IsReadOnly)
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
                            .ContactAlsoUsedAs.Visible = False
                        End With

                        'Providers Tab
                        lblServiceProvider.Text = WebUtils.EncodeOutput(reader("ServiceProvider"))
                        lblSPAddress.Text = WebUtils.EncodeOutput(reader("SPAddress")).Replace(vbCrLf, "<br />")
                        lblSPPostCode.Text = WebUtils.EncodeOutput(reader("SPPostCode"))
                        lblAccomManager.Text = WebUtils.EncodeOutput(reader("AccomodationManager"))
                        lblAMAddress.Text = WebUtils.EncodeOutput(reader("AMAddress")).Replace(vbCrLf, "<br />")
                        lblAMPostCode.Text = WebUtils.EncodeOutput(reader("AMPostCode"))
                        lblLandlord.Text = WebUtils.EncodeOutput(reader("Landlord"))
                        lblLAddress.Text = WebUtils.EncodeOutput(reader("LAddress")).Replace(vbCrLf, "<br />")
                        lblLPostCode.Text = WebUtils.EncodeOutput(reader("LPostCode"))
                    End While

                Catch ex As Exception
                    msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME, "ViewProperty.Page_load")     ' error reading data
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
            Dim serviceID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("sid"))
            Dim propertyID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("pid"))

            If IsValid Then
                ' get the edtiable field settings
                msg = AmendReqBL.FetchEditableFieldSettings(Me.DbConnection, user.ExternalUserID, settings)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                ' create amendment requests
                msg = AmendReqBL.ProcessEditableFields(Page.Controls, settings, Me.DbConnection, ConnectionStrings("Abacus").ConnectionString, AppSettings("SiteID"), user.ExternalUsername, user.ID, processedFieldCount)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                Response.Redirect(String.Format("ViewProperty.aspx?pid={0}&sid={1}&processedFieldCount={2}", propertyID, serviceID, processedFieldCount))
            End If

        End Sub

    End Class
End Namespace
