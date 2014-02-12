
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.Controls
Imports Target.Web.Apps.Security

Namespace Apps.Msg

    ''' <summary>
    ''' Screen to allow a user to select recipient(s) for the message.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Class SelectRecipients
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Dim msgPassingType As MessagePassingType
            Dim msg As ErrorMessage

            Me.EnableTimeout = False
            Me.InitPage(ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.Messaging"), "Select Recipients")

            ' add page JS link
            Me.JsLinks.Add("SelectRecipients.js")
            ' add msg common JS link
            Me.JsLinks.Add("MsgShared.js")
            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add AJAX code for security web service
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.Security.WebSvc.Security))
            ' add AJAX code for msg web service
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.Msg.WebSvc.Messaging))

            msg = MsgBL.GetMessagePassingType(Me.Settings, msgPassingType)
            If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)

            Me.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Web.Apps.Msg.SelectRecipients", _
                Target.Library.Web.Utils.WrapClientScript(String.Format("msgPassingType={0};", Convert.ToByte(msgPassingType))))

        End Sub

    End Class

End Namespace