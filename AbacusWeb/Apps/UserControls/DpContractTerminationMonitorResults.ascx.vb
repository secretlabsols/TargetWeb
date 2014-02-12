Imports System.Collections.Generic
Imports System.Text
Imports System.Web.UI.WebControls
Imports Target.Abacus.Library
Imports Target.Abacus.Web.Apps.UserControls.SelectorWizardSteps
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Library.Web.UserControls

Namespace Apps.UserControls

    ''' <summary>
    ''' Class representing a results tool for monitoring terminated dp contracts 
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''   ColinD   08/09/2011 D12161 - Created
    ''' </history>
    Partial Public Class DpContractTerminationMonitorResults
        Inherits System.Web.UI.UserControl

#Region "Fields"

        ' constants
        Private Const _DateFormat As String = "dd/MM/yyyy"
        Private Const _ReportButtonReportsIDKey As String = "AbacusIntranet.WebReport.DirectPaymentsMonitor"
        Private Const _SelectorName As String = "DpContractTerminationMonitorResults"

        ' locals
        Private _FilterContractType As Nullable(Of Boolean) = Nothing
        Private _FilterIsBalanced As Nullable(Of Boolean) = False
        Private _FilterSelectedId As Integer = 0
        Private _FilterTerminationPeriodFrom As Nullable(Of DateTime) = Nothing
        Private _FilterTerminationPeriodTo As Nullable(Of DateTime) = Nothing
        Private _FilterUnderOrOverPayments As Nullable(Of Boolean) = Nothing

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the base page.
        ''' </summary>
        ''' <value>The base page.</value>
        Private ReadOnly Property BasePage() As BasePage
            Get
                Return CType(Me.Page, BasePage)
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the type of the filter contract.
        ''' </summary>
        ''' <value>The type of the filter contract.</value>
        Public Property FilterContractType() As Nullable(Of Boolean)
            Get
                Return _FilterContractType
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                _FilterContractType = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the filter is balanced.
        ''' </summary>
        ''' <value>The filter is balanced.</value>
        Public Property FilterIsBalanced() As Nullable(Of Boolean)
            Get
                Return _FilterIsBalanced
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                _FilterIsBalanced = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the filter selected id.
        ''' </summary>
        ''' <value>The filter selected id.</value>
        Public Property FilterSelectedId() As Integer
            Get
                Return _FilterSelectedId
            End Get
            Set(ByVal value As Integer)
                _FilterSelectedId = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the filter under or over payments.
        ''' </summary>
        ''' <value>The filter under or over payments.</value>
        Public Property FilterUnderOrOverPayments() As Nullable(Of Boolean)
            Get
                Return _FilterUnderOrOverPayments
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                _FilterUnderOrOverPayments = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the filter termination period from.
        ''' </summary>
        ''' <value>The filter termination period from.</value>
        Public Property FilterTerminationPeriodFrom() As Nullable(Of DateTime)
            Get
                Return _FilterTerminationPeriodFrom
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                _FilterTerminationPeriodFrom = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the filter termination period to.
        ''' </summary>
        ''' <value>The filter termination period to.</value>
        Public Property FilterTerminationPeriodTo() As Nullable(Of DateTime)
            Get
                Return _FilterTerminationPeriodTo
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                _FilterTerminationPeriodTo = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the reports button control.
        ''' </summary>
        ''' <value>The reports button control.</value>
        Private ReadOnly Property ReportsButtonControl() As IReportsButton
            Get
                Return CType(DpContractTerminationMonitorResults_btnPrint, IReportsButton)
            End Get
        End Property

        ''' <summary>
        ''' Gets the report button reports ID.
        ''' </summary>
        ''' <value>The report button reports ID.</value>
        Private ReadOnly Property ReportButtonReportsID() As Integer
            Get
                Return Target.Library.Web.ConstantsManager.GetConstant(_ReportButtonReportsIDKey)
            End Get
        End Property

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            SetupJavaScript()
            SetupReportsButton()

        End Sub

        ''' <summary>
        ''' Handles the PreRender event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            Dim js As New StringBuilder()

            js.AppendFormat("{0}_FilterContractType = {1};", _SelectorName, WebUtils.GetBooleanAsJavascriptString(FilterContractType))
            js.AppendFormat("{0}_FilterIsBalanced = {1};", _SelectorName, WebUtils.GetBooleanAsJavascriptString(FilterIsBalanced))
            js.AppendFormat("{0}_SelectedID = {1};", _SelectorName, FilterSelectedId)
            js.AppendFormat("{0}_FilterUnderOrOverPayments = {1};", _SelectorName, WebUtils.GetBooleanAsJavascriptString(FilterUnderOrOverPayments))
            js.AppendFormat("{0}_FilterTerminationPeriodFrom = {1};", _SelectorName, WebUtils.GetDateAsJavascriptString(FilterTerminationPeriodFrom))
            js.AppendFormat("{0}_FilterTerminationPeriodTo = {1};", _SelectorName, WebUtils.GetDateAsJavascriptString(FilterTerminationPeriodTo))

            ' register dynamic script once with client
            BasePage.ClientScript.RegisterStartupScript(Me.GetType(), _
                                                       String.Format("Target.Abacus.Web.Apps.UserControls.{0}.Startup", _SelectorName), _
                                                       Target.Library.Web.Utils.WrapClientScript(js.ToString()))

        End Sub

#End Region

#Region "Functions"

        ''' <summary>
        ''' Set up the java script.
        ''' </summary>
        Private Sub SetupJavaScript()

            ' add date utility JS
            BasePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Date.js"))

            ' add utility js link
            BasePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))

            ' add dialog JS
            BasePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))

            ' add list filter JS
            BasePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))

            ' add reports js
            BasePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/HiddenReportStep.js"))

            ' add the ajax web svc tools
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DPContract))

            ' add in the jquery library
            BasePage.UseJQuery = True

            ' add in the jquery template library
            BasePage.UseJqueryTemplates = True

            ' add page JS
            BasePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath(String.Format("AbacusWeb/Apps/UserControls/{0}.js", _SelectorName)))

        End Sub

        ''' <summary>
        ''' Set up the reports button.
        ''' </summary>
        Private Sub SetupReportsButton()

            ' setup register report setup
            With ReportsButtonControl
                .ReportID = ReportButtonReportsID
                .Position = Target.Library.Web.Controls.SearchableMenu.SearchableMenuPosition.TopLeft
                If FilterContractType.HasValue Then
                    .Parameters.Add("isSds", FilterContractType.Value)
                End If
                If FilterIsBalanced.HasValue Then
                    .Parameters.Add("isBalanced", FilterIsBalanced.Value)
                End If
                If FilterTerminationPeriodFrom.HasValue Then
                    .Parameters.Add("terminatedDateFrom", FilterTerminationPeriodFrom.Value)
                End If
                If FilterTerminationPeriodTo.HasValue Then
                    .Parameters.Add("terminatedDateTo", FilterTerminationPeriodTo.Value)
                End If
                If FilterUnderOrOverPayments.HasValue Then
                    .Parameters.Add("isUnderPaid", FilterUnderOrOverPayments.Value)
                End If
            End With

        End Sub

#End Region

    End Class

End Namespace
