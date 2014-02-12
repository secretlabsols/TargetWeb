Imports System.Collections.Generic
Imports System.Text
Imports System.Web.UI.WebControls
Imports Target.Abacus.Library
Imports Target.Abacus.Web.Apps.UserControls.SelectorWizardSteps
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps

Namespace Apps.UserControls

    ''' <summary>
    ''' Class representing a filter tool for monitoring terminated dp contracts 
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''   ColinD   08/09/2011 D12161 - Created
    ''' </history>
    Partial Public Class DpContractTerminationMonitorFilter
        Inherits System.Web.UI.UserControl

#Region "Fields"

        ' constants
        Private Const _DateFormat As String = "dd/MM/yyyy"
        Private Const _SelectorName As String = "DpContractTerminationMonitorFilter"

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
                Return Target.Library.Utils.ToBoolean(rblContractTypes.SelectedValue)
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                If value.HasValue Then
                    rblContractTypes.SelectedValue = value.ToString().ToLower()
                Else
                    rblContractTypes.SelectedValue = String.Empty
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the filter is balanced.
        ''' </summary>
        ''' <value>The filter is balanced.</value>
        Public Property FilterIsBalanced() As Nullable(Of Boolean)
            Get
                Return Target.Library.Utils.ToBoolean(rblBalanced.SelectedValue)
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                If value.HasValue Then
                    rblBalanced.SelectedValue = value.ToString().ToLower()
                Else
                    rblBalanced.SelectedValue = String.Empty
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the filter under or over payments.
        ''' </summary>
        ''' <value>The filter under or over payments.</value>
        Public Property FilterUnderOrOverPayments() As Nullable(Of Boolean)
            Get
                Return Target.Library.Utils.ToBoolean(rblUnderOrOverPayments.SelectedValue)
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                If value.HasValue Then
                    rblUnderOrOverPayments.SelectedValue = value.ToString().ToLower()
                Else
                    rblUnderOrOverPayments.SelectedValue = String.Empty
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the filter termination period from.
        ''' </summary>
        ''' <value>The filter termination period from.</value>
        Public Property FilterTerminationPeriodFrom() As Nullable(Of DateTime)
            Get
                Return Target.Library.Utils.ToDateTime(dteTerminatedFrom.Value)
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                If value.HasValue Then
                    dteTerminatedFrom.Text = value.Value.ToString(_DateFormat)
                Else
                    dteTerminatedFrom.Text = String.Empty
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the filter termination period to.
        ''' </summary>
        ''' <value>The filter termination period to.</value>
        Public Property FilterTerminationPeriodTo() As Nullable(Of DateTime)
            Get
                Return Target.Library.Utils.ToDateTime(dteTerminatedTo.Value)
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                If value.HasValue Then
                    dteTerminatedTo.Text = value.Value.ToString(_DateFormat)
                Else
                    dteTerminatedTo.Text = String.Empty
                End If
            End Set
        End Property

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ' use jquery on base page
            BasePage.UseJQuery = True

            ' add page JS
            BasePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath(String.Format("AbacusWeb/Apps/UserControls/{0}.js", _SelectorName)))

        End Sub

        ''' <summary>
        ''' Handles the PreRender event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            Dim js As New StringBuilder()

            js.AppendFormat("{0}_QsFilterContractTypeKey = '{1}';", _SelectorName, DpContractTerminationMonitorFilterStep.QsFilterContractTypeKey)
            js.AppendFormat("{0}_QsFilterIsBalancedKey = '{1}';", _SelectorName, DpContractTerminationMonitorFilterStep.QsFilterIsBalancedKey)
            js.AppendFormat("{0}_QsFilterTerminationDateFromKey = '{1}';", _SelectorName, DpContractTerminationMonitorFilterStep.QsFilterTerminationDateFromKey)
            js.AppendFormat("{0}_QsFilterTerminationDateToKey = '{1}';", _SelectorName, DpContractTerminationMonitorFilterStep.QsFilterTerminationDateToKey)
            js.AppendFormat("{0}_QsFilterUnderOrOverPaymentsKey = '{1}';", _SelectorName, DpContractTerminationMonitorFilterStep.QsFilterUnderOrOverPaymentsKey)

            ' register dynamic script once with client
            BasePage.ClientScript.RegisterStartupScript(Me.GetType(), _
                                                       String.Format("Target.Abacus.Web.Apps.UserControls.{0}.Startup", _SelectorName), _
                                                       Target.Library.Web.Utils.WrapClientScript(js.ToString()))

        End Sub

#End Region

    End Class

End Namespace
