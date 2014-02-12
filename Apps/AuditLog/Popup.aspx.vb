
Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps.AuditLog.Controls

Namespace Apps.AuditLog

    ''' <summary>
    ''' Audit logging viewer popup screen.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
 Partial Class Popup
        Inherits Target.Web.Apps.BasePage

#Region "Properties"

        ''' <summary>
        ''' Gets a value indicating whether [query string show commands].
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if [query string show commands]; otherwise, <c>false</c>.
        ''' </value>
        Private ReadOnly Property QueryStringShowCommands() As Boolean
            Get
                Dim show As Nullable(Of Boolean) = Target.Library.Utils.ToBoolean(Request.QueryString("sc"))
                If show.HasValue Then
                    Return show.Value
                Else
                    Return True
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets the web nav menu item ID from the query string.
        ''' </summary>
        Private ReadOnly Property QueryStringWebNavMenuItemID() As Integer
            Get
                Return Target.Library.Utils.ToInt32(Request.QueryString("wnmi"))
            End Get
        End Property

        ''' <summary>
        ''' Gets the web nav menu item ID.
        ''' </summary>
        Private ReadOnly Property WebNavMenuItemID() As Integer
            Get
                If QueryStringWebNavMenuItemID > 0 Then
                    Return QueryStringWebNavMenuItemID
                Else
                    Return Target.Library.Utils.ToInt32(Session(AuditLogging.SESSION_KEY_MENU_ITEM_ID))
                End If
            End Get
        End Property

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False

            ' pull the menu item ID from the session, stored by the std buttons user control
            Me.InitPage(WebNavMenuItemID, "Audit Log")

            ' set whether to display commands i.e. print etc
            Me.divCommands.Visible = QueryStringShowCommands

        End Sub

#End Region

    End Class

End Namespace