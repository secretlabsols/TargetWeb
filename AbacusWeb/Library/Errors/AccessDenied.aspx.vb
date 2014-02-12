
Imports System.Configuration.ConfigurationSettings
Imports Target.Library.Web

Namespace Library.Errors

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Library.Errors.AccessDenied
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Page displayed when the user tries to access a resource that they do not 
    '''     have rights to view.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	23/01/2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class AccessDenied
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            InitPage(-1, "Access Denied")
        End Sub

    End Class

End Namespace