Imports System.Collections.Generic
Imports System.Text
Imports Target.Library
Imports Target.Abacus.Web.Apps.UserControls

Namespace Apps.InPlaceSelectors

    ''' <summary>
    ''' Class representing a selector tool for GenericCreditors records
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''   ColinD   04/03/2011 D11874 - Created
    ''' </history>
    Partial Public Class GenericCreditors
        Inherits Target.Web.Apps.BasePage

#Region "Fields"

        ' constants 
        Private Const _JavascriptPath As String = "GenericCreditors.js"
        Private Const _PageTitle As String = "Select Generic Creditor"
        Private Const _QueryStringIdKey As String = "id"
        Private Const _QueryStringControlIdKey As String = "ctrlid"

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets selected id.
        ''' </summary>
        ''' <value>Get selected id.</value>
        Private ReadOnly Property FilterSelectedID() As Integer
            Get
                Return Target.Library.Utils.ToInt32(Request.QueryString(_QueryStringIdKey))
            End Get
        End Property

        ''' <summary>
        ''' Gets control id.
        ''' </summary>
        ''' <value>Get control id.</value>
        Private ReadOnly Property FilterControlID() As String
            Get
                Return Request.QueryString(_QueryStringControlIdKey)
            End Get
        End Property

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

            EnableTimeout = False
            InitPage(Target.Library.Web.ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), _PageTitle)
            JsLinks.Add(_JavascriptPath)

            ' setup the selector control
            With selector
                .FilterSelectedID = FilterSelectedID
                .InitControl(Me)
            End With

        End Sub

        ''' <summary>
        ''' Handles the PreRenderComplete event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            Dim js As New StringBuilder()

            ' add filter properties to page in js format
            js.AppendFormat("parentControlID = '{0}';", FilterControlID)

            Me.ClientScript.RegisterStartupScript(Me.GetType(), _
                                                   "Target.Abacus.Web.Apps.UserControls.GenericCreditors.Startup", _
                                                   Target.Library.Web.Utils.WrapClientScript(js.ToString()))

        End Sub

#End Region

    End Class

End Namespace
