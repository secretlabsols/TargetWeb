
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.Utils
Imports Target.Web.Apps.FileStore

Namespace Apps.FileStore

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.FileStore.ProgressForm
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Provides file upload feedback to the user.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' 	[Mikevo]	01/12/2006  Created as an alternative to the FileUploadProgressDialog.
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Class ProgressForm
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False
            Me.InitPage(ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "File Upload Progress")

            ' add progress bar JS
            Me.JsLinks.Add(GetVirtualPath("Library/JavaScript/ProgressBar.js"))
            ' add utility JS
            Me.JsLinks.Add("ProgressForm.js")
            ' add Sarissa XML utility library JS
            Me.JsLinks.Add(GetVirtualPath("Library/JavaScript/Sarissa.js"))
            ' add upload ID JS
            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Web.Apps.FileStore.FileUploadProgressDialog.UploadID", _
                Target.Library.Web.Utils.WrapClientScript(String.Format("FileUploadProgressDialog_UploadID='{0}';", Request.QueryString("uploadID"))))

        End Sub

    End Class

End Namespace