
Imports System.Data.SqlClient
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps.Navigation
Imports Target.Web.Apps.Navigation.Collections
Imports Target.Web.Apps.Security.Collections
Imports Target.Web.Apps.Security.PasswordPolicyChecker

Namespace Apps.Security.Admin

    ''' <summary>
    ''' Allows admin users to enable/disable password exception lists, remove individual words 
    '''	from exception lists and add new words to the custom list.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MikeVO  17/04/2009  A4WA#5388 - fixes after internal testing.
    ''' MikeVO  20/03/2009  D11535 - created.
    ''' </history>
    Partial Class PasswordExceptions
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.PasswordExceptions"), "Admin: Security: Manage Password Exceptions")

            selector.InitControl(Me, 0)

        End Sub

    End Class

End Namespace