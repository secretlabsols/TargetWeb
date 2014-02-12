Imports System.Collections.Generic
Imports System.Text
Imports Target.Library
Imports Target.Abacus.Web.Apps.UserControls

Namespace Apps.InPlaceSelectors

    ''' <summary>
    ''' Class representing a selector tool for Budget Category records
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''   ColinD   23/11/2010 D11964A - Created
    ''' </history>
    Partial Public Class BudgetCategories
        Inherits Target.Web.Apps.BasePage

#Region "Fields"

        ' constants 
        Private Const _JavascriptPath As String = "BudgetCategories.js"
        Private Const _PageTitle As String = "Select Budget Category"
        Private Const _QueryStringMultiValueToken As String = ","
        Private Const _QueryStringExcludeIdsKey As String = "exIds"
        Private Const _QueryStringIncludeIdsKey As String = "incIds"
        Private Const _QueryStringIncludeServiceTypeIdsKey As String = "incSvcTypIds"
        Private Const _QueryStringRedundantKey As String = "redundant"
        Private Const _QueryStringIdKey As String = "id"
        Private Const _QueryStringControlIdKey As String = "ctrlid"

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets control id i.e. the control to update when selecting an item.
        ''' </summary>
        ''' <value>Get control id.</value>
        Private ReadOnly Property FilterControlID() As String
            Get
                Return Request.QueryString(_QueryStringControlIdKey)
            End Get
        End Property

        ''' <summary>
        ''' Gets the ids to always exclude from the query string.
        ''' </summary>
        ''' <value>The ids to always exclude from the query string.</value>
        Private ReadOnly Property FilterExcludeIds() As List(Of Integer)
            Get
                Return Target.Library.Utils.GetListOfIntegerFromTokenSeparatedString(Request.QueryString(_QueryStringExcludeIdsKey), _QueryStringMultiValueToken)
            End Get
        End Property

        ''' <summary>
        ''' Gets the ids to always include from the query string.
        ''' </summary>
        ''' <value>The ids to always include from the query string.</value>
        Private ReadOnly Property FilterIncludeIds() As List(Of Integer)
            Get
                Return Target.Library.Utils.GetListOfIntegerFromTokenSeparatedString(Request.QueryString(_QueryStringIncludeIdsKey), _QueryStringMultiValueToken)
            End Get
        End Property

        ''' <summary>
        ''' Gets the only service type ids to include from the query string.
        ''' </summary>
        ''' <value>The only service type ids to include from the query string.</value>
        Private ReadOnly Property FilterIncludeServiceTypeIds() As List(Of Integer)
            Get
                Return Target.Library.Utils.GetListOfIntegerFromTokenSeparatedString(Request.QueryString(_QueryStringIncludeServiceTypeIdsKey), _QueryStringMultiValueToken)
            End Get
        End Property

        ''' <summary>
        ''' Gets whether to filter by the Redundant flag.
        ''' </summary>
        ''' <value>Whether to filter by the Redundant flag.</value>
        Private ReadOnly Property FilterRedundant() As TriState
            Get
                Return GetTriStateFromString(Request.QueryString(_QueryStringRedundantKey))
            End Get
        End Property

        ''' <summary>
        ''' Gets selected id.
        ''' </summary>
        ''' <value>Get selected id.</value>
        Private ReadOnly Property FilterSelectedID() As Integer
            Get
                Return Target.Library.Utils.ToInt32(Request.QueryString(_QueryStringIdKey))
            End Get
        End Property

#End Region

#Region "Methods"

        ''' <summary>
        ''' Gets tri state from string.
        ''' </summary>
        ''' <param name="convertee">The convertee.</param>
        ''' <returns></returns>
        Private Function GetTriStateFromString(ByVal convertee As String) As TriState

            Dim triValue As TriState = TriState.UseDefault

            If Not String.IsNullOrEmpty(convertee) AndAlso convertee.Trim().Length > 0 Then
                ' if we have a tri flag in the string then use it

                Select Case convertee.ToLower()

                    Case "true"

                        triValue = TriState.True

                    Case "false"

                        triValue = TriState.False

                End Select

            End If

            Return triValue

        End Function

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
                .FilterExcludeIds = FilterExcludeIds
                .FilterIncludeIds = FilterIncludeIds
                .FilterRedundant = FilterRedundant
                .FilterSelectedID = FilterSelectedID
                .FilterIncludeServiceTypeIds = FilterIncludeServiceTypeIds
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
                                                   "Target.Abacus.Web.Apps.UserControls.BudgetCategories.Startup", _
                                                   Target.Library.Web.Utils.WrapClientScript(js.ToString()))

        End Sub

#End Region

    End Class

End Namespace
