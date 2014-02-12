
Imports Target.Library
Imports Target.Web.Apps.Security.Collections

Namespace Apps.Security.Admin

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.Security.Admin.UserList
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Allows users to search for other user accounts.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      08/03/2007  Support for user levels.
    '''     MikeVO      15/11/2006  Support for CanViewOtherExternalUsers security item.
    ''' 	[Mikevo]	??/??/????	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class UserList
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.Users"), "Admin: Security: List Users")

            Dim userID As Integer = Utils.ToInt32(Request.QueryString("userID"))

            selector.InitControl(Me, userID)

        End Sub

    End Class

End Namespace