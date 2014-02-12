
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.Controls

Namespace Apps.Msg.Admin

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Admin.ManageDistLists
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Allows a user to create, edit and delete messaging distribution lists.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' 	[Mikevo]	06/09/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class ManageDistLists
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.MessagingAdmin.DistributionLists"), "Manage Distribution Lists")

            Dim msg As ErrorMessage
            Dim deleteID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("deleteID"))

            ' add page JS link
            Me.JsLinks.Add("ManageDistLists.js")
            ' add msg common JS link
            Me.JsLinks.Add("../MsgShared.js")
            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add AJAX code for web service
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.Msg.WebSvc.Messaging))

            ' are we deleting?
            If deleteID > 0 Then
                msg = MsgBL.DeleteDistributionList(Me.DbConnection, deleteID)
            End If

        End Sub

        Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click

            Dim distListID As Integer
            Dim msg As ErrorMessage

            If Page.IsValid Then
                ' save the dist list
                distListID = Target.Library.Utils.ToInt32(Request.Form("cboDistLists"))
                msg = MsgBL.SaveDistributionList(Me.DbConnection, distListID, txtName.Text, txtRecipients.Value)
                If Not msg.Success Then
                    Target.Library.Web.Utils.DisplayError(msg)
                Else
                    ' load the list
                    Me.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Web.Apps.Msg.Admin.ManageDistLists.Startup", _
                        Target.Library.Web.Utils.WrapClientScript(String.Format("onLoadDistListID={0};", distListID)))
                End If
            End If

        End Sub

    End Class

End Namespace


