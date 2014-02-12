
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.Data.SqlClient
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library.DebtorInvoices
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports System.Text
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.Security.Collections
Imports Target.Abacus.Extranet.Apps.Dom.PaymentSchedules.UserControls


Namespace Apps.Dom.PaymentSchedules

#Region " PaymentSchedulesFilterStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Apps.Dom.PaymentSchedules.PaymentSchedulesFilterStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Class that implements payment schedules criteria filtering for payment schedule enquiries.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' IHS       26/05/2011  D12084 - Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class PaymentSchedulesFilterStep
        Implements ISelectorWizardStep

        Const QS_CURRENTSTEP As String = "currentStep"

        Private _dbConnection As SqlConnection
        Private _currentStep As Integer
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _description As String = "Specify various other filters"
        Private _headerLabelWidth As Unit = New Unit(10, UnitType.Em)
        Private _headerDescWidth As Unit = New Unit(35, UnitType.Em)
        Private _qsParser As WizardScreenParameters

        Public Property HeaderLabelWidth() As Unit
            Get
                Return _headerLabelWidth
            End Get
            Set(ByVal value As Unit)
                _headerLabelWidth = value
            End Set
        End Property

        Public Property HeaderDescWidth() As Unit
            Get
                Return _headerDescWidth
            End Get
            Set(ByVal value As Unit)
                _headerDescWidth = value
            End Set
        End Property

        Public Property Description() As String Implements ISelectorWizardStep.Description
            Get
                Return _description
            End Get
            Set(ByVal value As String)
                _description = value
            End Set
        End Property

        Public Property DbConnection() As System.Data.SqlClient.SqlConnection Implements ISelectorWizardStep.DbConnection
            Get
                Return _dbConnection
            End Get
            Set(ByVal value As System.Data.SqlClient.SqlConnection)
                _dbConnection = value
            End Set
        End Property

        Public Property QueryString() As System.Collections.Specialized.NameValueCollection Implements ISelectorWizardStep.QueryString
            Get
                Return New NameValueCollection(_queryString)
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                _queryString = Value
                _currentStep = Utils.ToInt32(_queryString(QS_CURRENTSTEP))
                _qsParser = New WizardScreenParameters(_queryString)
            End Set
        End Property

        Public Property BaseUrl() As String Implements ISelectorWizardStep.BaseUrl
            Get
                Return _baseUrl
            End Get
            Set(ByVal value As String)
                _baseUrl = value
            End Set
        End Property

        Public Property Required() As Boolean Implements ISelectorWizardStep.Required
            Get
                Return _required
            End Get
            Set(ByVal value As Boolean)
                _required = value
            End Set
        End Property

        Public ReadOnly Property BeforeNavigateJS() As String Implements ISelectorWizardStep.BeforeNavigateJS
            Get
                Return "PaymentSchedulesFilter_BeforeNavigate()"
            End Get
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

            Dim msg As ErrorMessage = New ErrorMessage
            Dim ucPaymentSchedulesFilter As PaymentSchedulesFilter = wizard.LoadControl("~/AbacusExtranet/Apps/Dom/PaymentSchedules/UserControls/PaymentSchedulesFilter.ascx")
            'ucPaymentSchedulesFilter.InitControl(wizard.Page, _qsParser)
            ucPaymentSchedulesFilter.thePage = wizard.Page
            ucPaymentSchedulesFilter.qsParser = _qsParser
            controls.Add(ucPaymentSchedulesFilter)
            msg.Success = True
            Return msg

        End Function

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls

            Dim msg As ErrorMessage
            Dim lbl As Label
            Dim text As Label
            Dim link As HyperLink = New HyperLink
            Dim clearerDiv As HtmlGenericControl
            Dim qs As NameValueCollection

            Me.QueryString = _queryString

            '++ Add the list of ticked "Invoice Types" onto two lines..
            lbl = New Label()
            lbl.Text = "Attributes"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", _headerLabelWidth.ToString())
            controls.Add(lbl)

            text = New Label()
            text.Style.Add("float", "left")
            text.Style.Add("width", _headerDescWidth.ToString())
            text.Text = "Complex" '_qsParser.SelectedVisitBasedDesc

            controls.Add(text)

            ' add the link
            qs = Me.QueryString
            If Me.IsCurrentStep Then
                ' all
                link.Text = "Filter Criteria"
                qs.Remove(WizardScreenParameters.QS_REFERENCE)
                qs.Remove(WizardScreenParameters.QS_PERIOD_FROM)
                qs.Remove(WizardScreenParameters.QS_PERIOD_TO)
                qs.Remove(WizardScreenParameters.QS_VISIT_YES)
                qs.Remove(WizardScreenParameters.QS_VISIT_NO)

                qs.Remove(WizardScreenParameters.QS_PROFORMA_INVOICES_NONE)
                qs.Remove(WizardScreenParameters.QS_PROFORMA_INVOICES_AWAITING)
                qs.Remove(WizardScreenParameters.QS_PROFORMA_INVOICES_VERIFIED)

                qs.Remove(WizardScreenParameters.QS_INVOICES_UNPAID)
                qs.Remove(WizardScreenParameters.QS_INVOICES_SUSPENDED)
                qs.Remove(WizardScreenParameters.QS_INVOICES_AUTHORISED)

                qs.Remove(WizardScreenParameters.QS_VAR_AWAITING)
                qs.Remove(WizardScreenParameters.QS_VAR_VERIFIED)
                qs.Remove(WizardScreenParameters.QS_VAR_DECLINED)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change Filter Criteria"
                qs.Remove(QS_CURRENTSTEP)
                qs.Add(QS_CURRENTSTEP, Me.StepIndex)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            End If
            If Not String.IsNullOrEmpty(link.Text) Then
                link.Style.Add("float", "right")
                controls.Add(link)
            End If

            clearerDiv = New HtmlGenericControl("div")
            clearerDiv.Attributes.Add("class", "clearer")
            controls.Add(clearerDiv)

            msg = New ErrorMessage
            msg.Success = True

            Return msg

        End Function

        Public Property StepIndex() As Integer Implements ISelectorWizardStep.StepIndex
            Get
                Return _stepIndex
            End Get
            Set(ByVal value As Integer)
                _stepIndex = value
            End Set
        End Property

        Public ReadOnly Property Title() As String Implements ISelectorWizardStep.Title
            Get
                Return "Select the filter criterion to filter the results on."
            End Get
        End Property

        Public Property IsCurrentStep() As Boolean Implements ISelectorWizardStep.IsCurrentStep
            Get
                Return _isCurrentStep
            End Get
            Set(ByVal value As Boolean)
                _isCurrentStep = value
            End Set
        End Property

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

        End Sub

        Public Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep

        End Sub

    End Class

#End Region

End Namespace