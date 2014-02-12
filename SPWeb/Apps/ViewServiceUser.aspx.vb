
Imports System.Configuration.ConfigurationManager
Imports System.Text
Imports Target.Library.Web
Imports Target.Library
Imports Target.Web.Apps
Imports System.Data.SqlClient
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.AmendReq
Imports Target.Web.Apps.AmendReq.Collections
Imports Target.Web.Apps.Security
Imports Target.SP.Library

Namespace Apps

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.ViewServiceUser
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Allows a user to view the details of a single client.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO  08/03/2007  Hide Amend Req buttons based on security item.
    '''     MikeVO  27/11/2006  Ensure user can view service user.
    '''     MikeVO  24/11/2006  Change to completely remove client address functionality.
    '''                         Display property address if propertyID is present.
    '''     MikeVO  07/11/2006  Fix to setting client title dropdown value.
    '''     MikeVO  27/10/2006  Various UI fixes. Implemented amendment requests.
    ''' 	[paul]	26/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class ViewServiceUser
        Inherits Target.Web.Apps.BasePage

        Protected WithEvents propertyAddress As Target.SP.Web.Apps.UserControls.AddressContact

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim strStyle As New StringBuilder
            Dim reader As SqlDataReader = Nothing
            Dim msg As ErrorMessage = New ErrorMessage
            Dim serviceUserID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim propertyID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("propertyID"))
            Dim editMode As Boolean = Convert.ToBoolean(Target.Library.Utils.ToInt32(Request.QueryString("editMode")))
            Dim processedFieldCount As Integer = Target.Library.Utils.ToInt32(Request.QueryString("processedFieldCount"))
            Dim settings As WebAmendReqDataItemCollection = Nothing
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim strUrl As String
            Dim canViewSU As Boolean, canViewProperty As Boolean, canViewAmendReqs As Boolean

            Const SP_SERVICE_USER As String = "spxServiceUser_Fetch"
            Const SP_NAME_FETCH_TITLES As String = "pr_FetchTitles"
            Const SP_NAME_PROPERTY_FETCH As String = "spxSPProperty_Fetch"

            Me.InitPage(ConstantsManager.GetConstant("webSecurityItemSPServiceUserView"), "Service User Details")

            ' check to ensure the user is allowed to see the client and the property
            msg = SPClassesBL.UserCanViewServiceUser(Me.DbConnection, user.ExternalUserID, serviceUserID, canViewSU)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            msg = SPClassesBL.UserCanViewProperty(Me.DbConnection, user.ExternalUserID, propertyID, canViewProperty)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            If Not canViewSU Or Not canViewProperty Then Response.Redirect("~/Library/Errors/AccessDenied.aspx")

            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))

            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.SP.web.Apps.WebSvc.Clients))

            strStyle.Append("label.label { float:left; width:9.5em; font-weight:bold; }")
            strStyle.Append("span.label { float:left; width:9.5em; padding-right:1em; font-weight:bold; }")
            strStyle.Append(".content { float:left; }")
            strStyle.Append(".content2 { width:80%; float:left;}")
            Me.AddExtraCssStyle(strStyle.ToString)

            canViewAmendReqs = SecurityBL.UserHasItem(Me.DbConnection, user.ID, ConstantsManager.GetConstant("webSecurityItemAmendReqListRequests"))

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

                ' grab the list of titles
                Try
                    reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_FETCH_TITLES)

                    With cboTitle.DropDownList
                        .DataSource = reader
                        .DataTextField = "Description"
                        .DataValueField = "ID"
                        .DataBind()
                        .Items.Insert(0, New ListItem("", ""))
                    End With

                Catch ex As Exception
                    msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_TITLES, "ViewServiceUser.Page_load")   ' could not read data
                    Target.Library.Web.Utils.DisplayError(msg)
                Finally
                    If Not reader Is Nothing Then reader.Close()
                End Try

                'Populate Service User Screen
                Try
                    Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_SERVICE_USER, False)
                    spParams(0).Value = serviceUserID
                    ' execute
                    reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_SERVICE_USER, spParams)

                    While reader.Read
                        lblReference.Text = WebUtils.EncodeOutput(reader("Reference"))
                        If Not Convert.IsDBNull(reader("Title")) Then cboTitle.DropDownList.SelectedValue = reader("TitleID")
                        cboTitle.RecordID = serviceUserID
                        txtFirstName.Text = WebUtils.EncodeOutput(reader("FirstNames"), txtFirstName.IsReadOnly)
                        txtFirstName.RecordID = serviceUserID
                        txtSurname.Text = WebUtils.EncodeOutput(reader("LastName"), txtSurname.IsReadOnly)
                        txtSurname.RecordID = serviceUserID
                        txtDateOfBirth.Text = WebUtils.EncodeOutput(reader("BirthDate"), txtDateOfBirth.IsReadOnly)
                        txtDateOfBirth.RecordID = serviceUserID
                        txtNINo.Text = WebUtils.EncodeOutput(reader("NINO"), txtNINo.IsReadOnly)
                        txtNINo.RecordID = serviceUserID
                    End While

                Catch ex As Exception
                    msg = Target.Library.Utils.CatchError(ex, "E0501", SP_SERVICE_USER, "ViewServiceUser.Page_load")     ' error reading data
                    Target.Library.Web.Utils.DisplayError(msg)
                Finally
                    If Not reader Is Nothing AndAlso Not reader.IsClosed Then reader.Close()
                End Try

                'Address section
                propertyAddress.InitControl(Me.DbConnection, propertyAddress.ID, False)
                If propertyID > 0 Then
                    Try
                        Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME_PROPERTY_FETCH, False)
                        spParams(0).Value = propertyID
                        ' execute
                        reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_PROPERTY_FETCH, spParams)

                        While reader.Read
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
                        End While

                    Catch ex As Exception
                        msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_PROPERTY_FETCH, "ViewServiceUser.Page_load")     ' error reading data
                        Target.Library.Web.Utils.DisplayError(msg)
                    Finally
                        If Not reader Is Nothing AndAlso Not reader.IsClosed Then reader.Close()
                    End Try
                End If

            End If

        End Sub

        Private Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click

            Dim msg As ErrorMessage
            Dim settings As WebAmendReqDataItemCollection = Nothing
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim processedFieldCount As Integer
            Dim serviceUserID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))

            If IsValid Then
                ' get the edtiable field settings
                msg = AmendReqBL.FetchEditableFieldSettings(Me.DbConnection, user.ExternalUserID, settings)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                ' create amendment requests
                msg = AmendReqBL.ProcessEditableFields(Page.Controls, settings, Me.DbConnection, ConnectionStrings("Abacus").ConnectionString, AppSettings("SiteID"), user.ExternalUsername, user.ID, processedFieldCount)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                Response.Redirect(String.Format("ViewServiceUser.aspx?id={0}&processedFieldCount={1}", serviceUserID, processedFieldCount))
            End If

        End Sub

    End Class

End Namespace