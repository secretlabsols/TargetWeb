
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.Utils
Imports Target.Web.Apps.FileStore

Namespace Apps.FileStore

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.FileStore.FileUploadProgressDialog
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Provides file upload feedback to the user.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      29/11/2006  Added WebSvcUtils.js link.
    ''' 	[Mikevo]	??/??/????	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class FileUploadProgressDialog
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False
            Me.InitPage(ConstantsManager.GetConstant("webSecurityItemFreeView"), "File Upload Progress")

            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(FileUploadProgress))
            ' add progress bar JS
            Me.JsLinks.Add(GetVirtualPath("Library/JavaScript/WebSvcUtils.js"))
            ' add progress bar JS
            Me.JsLinks.Add(GetVirtualPath("Library/JavaScript/ProgressBar.js"))
            ' add utility JS
            Me.JsLinks.Add("FileUploadProgressDialog.js")
            ' add upload ID JS
            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Web.Apps.FileStore.FileUploadProgressDialog.UploadID", _
                Target.Library.Web.Utils.WrapClientScript(String.Format("FileUploadProgressDialog_UploadID='{0}';", Request.QueryString("uploadID"))))

        End Sub

    End Class

End Namespace