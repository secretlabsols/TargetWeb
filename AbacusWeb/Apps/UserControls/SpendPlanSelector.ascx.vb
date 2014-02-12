Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps

Namespace Apps.UserControls
    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.UserControls.SpendPlanSelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control to encapsulate the listing and selecting of Spend Plans.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     ColinD  14/03/2011 Updated - Updated method of fetching budget category rate lookup method to use business logic rather than using the parent page settings.
    ''' 	[PaulW]	12/07/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Public Class SpendPlanSelector
        Inherits System.Web.UI.UserControl

#Region "Fields"

        ' locals
        Private _showNewButton As Boolean
        Private _showServiceUserColumns As Boolean
        Private _viewSpendplanInNewWindow As Boolean

        ' Constants
        Private Const _QsCurrentPage As String = "page"

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the current page.
        ''' </summary>
        ''' <value>The current page.</value>
        Private ReadOnly Property CurrentPage() As Integer
            Get
                Dim page As Integer = Target.Library.Utils.ToInt32(Request.QueryString(_QsCurrentPage))
                If page <= 0 Then page = 1
                Return page
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether [show new button].
        ''' </summary>
        ''' <value><c>true</c> if [show new button]; otherwise, <c>false</c>.</value>
        Public ReadOnly Property ShowNewButton() As Boolean
            Get
                Return _showNewButton
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether [show service user columns].
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if [show service user columns]; otherwise, <c>false</c>.
        ''' </value>
        Public ReadOnly Property ShowServiceUserColumns() As Boolean
            Get
                Return _showServiceUserColumns
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether [view spendplan in new window].
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if [view spendplan in new window]; otherwise, <c>false</c>.
        ''' </value>
        Public ReadOnly Property ViewSpendplanInNewWindow() As Boolean
            Get
                Return _viewSpendplanInNewWindow
            End Get
        End Property

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the PreRender event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            SpendPlanSelector_btnNew.Visible = _showNewButton

        End Sub

#End Region

#Region "Methods"

        ''' <summary>
        ''' Inits the control.
        ''' </summary>
        ''' <param name="thePage">The page.</param>
        ''' <param name="SpendPlanSelector_selectedSpendPlanID">The spend plan selector_selected spend plan ID.</param>
        ''' <param name="SpendPlanSelector_selectedClientID">The spend plan selector_selected client ID.</param>
        ''' <param name="filterDateFrom">The filter date from.</param>
        ''' <param name="filterDateTo">The filter date to.</param>
        ''' <param name="showNewButton">if set to <c>true</c> [show new button].</param>
        ''' <param name="showServiceUserColumns">if set to <c>true</c> [show service user columns].</param>
        ''' <param name="viewSpendplanInNewWindow">if set to <c>true</c> [view spendplan in new window].</param>
        Public Sub InitControl(ByVal thePage As BasePage, _
                               ByVal SpendPlanSelector_selectedSpendPlanID As Integer, _
                               ByVal SpendPlanSelector_selectedClientID As Integer, _
                               ByVal filterDateFrom As Date, _
                               ByVal filterDateTo As Date, _
                               ByVal showNewButton As Boolean, _
                               ByVal showServiceUserColumns As Boolean, _
                               ByVal viewSpendplanInNewWindow As Boolean)

            Dim msg As New ErrorMessage()
            Dim js As String
            Dim lookupBudgetCategoryRatesMethod As BudgetCategoryBL.BudgetCategoryRateLookupMethod = BudgetCategoryBL.BudgetCategoryRateLookupMethod.UsingTheDate

            _showNewButton = showNewButton
            _showServiceUserColumns = showServiceUserColumns
            _viewSpendplanInNewWindow = viewSpendplanInNewWindow

            ' add date utility JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/SpendPlanSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.SpendPlans))

            ' get the budget category lookup method
            msg = BudgetCategoryBL.GetBudgetCategoryRateLookupMethod(thePage.DbConnection, lookupBudgetCategoryRatesMethod)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' set the visibility of the latest cost legend dependant on the lookup up method for bc rates
            SpendPlanSelector_divLegendLatestCost.Visible = (lookupBudgetCategoryRatesMethod = BudgetCategoryBL.BudgetCategoryRateLookupMethod.UsingTheDate)

            js = String.Format( _
             "SpendPlanSelector_currentPage={0};SpendPlanSelector_selectedSpendPlanID={1};SpendPlanSelector_selectedClientID={2};SpendPlanSelector_dateFrom={3};SpendPlanSelector_dateTo={4};SpendPlanSelector_showServiceUserColumns={5};SpendPlanSelector_viewSpendplanInNewWindow={6};", _
             CurrentPage, SpendPlanSelector_selectedSpendPlanID, SpendPlanSelector_selectedClientID, _
             IIf(Target.Library.Utils.IsDate(filterDateFrom), WebUtils.GetDateAsJavascriptString(filterDateFrom), "null"), _
             IIf(Target.Library.Utils.IsDate(filterDateTo), WebUtils.GetDateAsJavascriptString(filterDateTo), "null"), _
             _showServiceUserColumns.ToString().ToLower(), _viewSpendplanInNewWindow.ToString().ToLower())

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), _
             "Target.Abacus.Web.Apps.UserControls.SpendPlanSelector.Startup", _
             Target.Library.Web.Utils.WrapClientScript(js) _
            )

        End Sub

#End Region

    End Class
End Namespace
