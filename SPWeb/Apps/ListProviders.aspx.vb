Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps.Msg
Imports Target.Web.Apps.Security
Namespace Apps
    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.ListProviders
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Used to list out providers that the user is alowed to view
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO  02/10/2006  Moved majority of code into ProviderSelector user control.
    ''' 	[paul]	28/09/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class ListProviders
        Inherits Target.Web.Apps.BasePage

        Protected WithEvents ProviderSelector1 As Target.SP.Web.Apps.UserControls.ProviderSelector

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(ConstantsManager.GetConstant("webSecurityItemSPProviderList"), "Providers")

            ' add page JS
            Me.JsLinks.Add("ListProviders.js")

            ProviderSelector1.InitControl(Me, True, Target.Library.Utils.ToInt32(Request.QueryString("selectedProviderID")))

        End Sub

    End Class
End Namespace
