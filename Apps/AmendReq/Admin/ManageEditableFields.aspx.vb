
Imports System.Text
Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps.AmendReq
Imports Target.Web.Apps.AmendReq.Collections

Namespace Apps.AmendReq.Admin

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.AmendReq.Admin.ManageEditableFields
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Screen to allow admin users to view and change the editable fields settings for different users.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[Mikevo]	20/09/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class ManageEditableFields
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(ConstantsManager.GetConstant("webSecurityItemAmendReqAdminManageEditableFields"), "Manage Editable Fields")

            Dim msg As ErrorMessage
            Dim userList As vwWebAmendReqUserSettingCollection = Nothing
            Dim startupJS As StringBuilder
            Dim userID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("userID"))
            Dim forceDefault As Integer = Target.Library.Utils.ToInt32(Request.QueryString("forceDefault"))

            ' add JS links
            Me.JsLinks.Add("ManageEditableFields.js")

            ' are we deleting any custom settings?
            If userID > 0 AndAlso forceDefault = 1 Then
                msg = AmendReqBL.DeleteCustomEditableFieldSettings(Me.DbConnection, userID)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
            End If

            ' fetch the list of users and their settings
            msg = vwWebAmendReqUserSetting.FetchList(Me.DbConnection, userList)
            If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
            ' load the repeater
            rptExternalUsers.DataSource = userList
            rptExternalUsers.DataBind()

            ' build startup JS
            startupJS = New StringBuilder
            For Each user As vwWebAmendReqUserSetting In userList
                ' select the correct radio button
                startupJS.AppendFormat("GetElement(""{0}{1}"").checked=true;", _
                    IIf(user.DefaultSettings, "rdoDefault", "rdoCustom"), user.ExternalUserID)
                ' enable the edit button if required
                If Not user.DefaultSettings Then
                    startupJS.AppendFormat("GetElement(""btnEdit{0}"").disabled=false;", user.ExternalUserID)
                End If
            Next

            ClientScript.RegisterStartupScript(Me.GetType(), "Target.Web.Apps.AmendReq.Admin..ManageEditableFields.Startup", _
                Target.Library.Web.Utils.WrapClientScript(startupJS.ToString()))

        End Sub

        Protected Function GetName(ByVal name As String) As String
            If name Is Nothing OrElse name.Length = 0 Then Return "[Unknown]"
            Return name
        End Function


    End Class


End Namespace

