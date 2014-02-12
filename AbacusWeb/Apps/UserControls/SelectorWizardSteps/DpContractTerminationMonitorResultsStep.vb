
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
Imports Target.Abacus.Web.Apps.General.DebtorInvoices
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.Security.Collections


Namespace Apps.UserControls.SelectorWizardSteps

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Intranet
    ''' Class	 : Abacus.Extranet.SelectorWizardSteps.DpContractTerminationMonitorResultsStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Implements the select results step for dp contract termination monitoring.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	ColinD	08/09/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Class DpContractTerminationMonitorResultsStep
        Implements ISelectorWizardStep

#Region "Fields"

        ' locals
        Private _baseUrl As String = String.Empty
        Private _currentStep As Integer
        Private _dbConnection As SqlConnection = Nothing
        Private _description As String = "Please apply filters and then click ""Next""."
        Private _headerLabelWidth As Unit = New Unit(20, UnitType.Em)
        Private _isCurrentStep As Boolean = False
        Private _queryString As NameValueCollection = Nothing
        Private _required As Boolean = False
        Private _stepIndex As Integer

        ' constants
        Private Const _DateFormat As String = "dd/MM/yyyy"
        Private Const QsCurrentStepKey As String = "currentStep"

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets or set the base portion of the current Url, upto the "?" character
        ''' </summary>
        ''' <value></value>
        ''' -----------------------------------------------------------------------------
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' [Mikevo]	06/10/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property BaseUrl() As String Implements ISelectorWizardStep.BaseUrl
            Get
                Return _baseUrl
            End Get
            Set(ByVal Value As String)
                _baseUrl = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets the Javascript to execute before navigating forwards from the step.
        ''' </summary>
        ''' <value></value>
        ''' -----------------------------------------------------------------------------
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' [Mikevo]	06/10/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property BeforeNavigateJS() As String Implements ISelectorWizardStep.BeforeNavigateJS
            Get
                Return "DpContractTerminationMonitorResults_BeforeNavigate()"
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the database connection to use.
        ''' </summary>
        ''' <value></value>
        ''' -----------------------------------------------------------------------------
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' [Mikevo]	06/10/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property DbConnection() As System.Data.SqlClient.SqlConnection Implements ISelectorWizardStep.DbConnection
            Get
                Return _dbConnection
            End Get
            Set(ByVal Value As System.Data.SqlClient.SqlConnection)
                _dbConnection = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets the description text to display for the step.
        ''' </summary>
        ''' <value></value>
        ''' -----------------------------------------------------------------------------
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' MikeVO      26/01/2007  Made read-write.
        ''' [Mikevo]	06/10/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Description() As String Implements ISelectorWizardStep.Description
            Get
                Return _description
            End Get
            Set(ByVal Value As String)
                _description = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the width of the header label.
        ''' </summary>
        ''' <value>The width of the header label.</value>
        Public Property HeaderLabelWidth() As Unit
            Get
                Return _headerLabelWidth
            End Get
            Set(ByVal value As Unit)
                _headerLabelWidth = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets if the step is the current step.
        ''' </summary>
        ''' <value></value>
        ''' -----------------------------------------------------------------------------
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' [Mikevo]	06/10/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property IsCurrentStep() As Boolean Implements ISelectorWizardStep.IsCurrentStep
            Get
                Return _isCurrentStep
            End Get
            Set(ByVal Value As Boolean)
                _isCurrentStep = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets the type of the qs filter contract.
        ''' </summary>
        ''' <value>The type of the qs filter contract.</value>
        Private ReadOnly Property QsFilterContractType() As Nullable(Of Boolean)
            Get
                Return Utils.ToBoolean(_queryString(DpContractTerminationMonitorFilterStep.QsFilterContractTypeKey))
            End Get
        End Property

        ''' <summary>
        ''' Gets the qs is balanced.
        ''' </summary>
        ''' <value>The qs is balanced.</value>
        Private ReadOnly Property QsFilterIsBalanced() As Nullable(Of Boolean)
            Get
                Return Utils.ToBoolean(_queryString(DpContractTerminationMonitorFilterStep.QsFilterIsBalancedKey))
            End Get
        End Property

        ''' <summary>
        ''' Gets the qs filter selected id.
        ''' </summary>
        ''' <value>The qs filter selected id.</value>
        Private ReadOnly Property QsFilterSelectedId() As Integer
            Get
                Return Utils.ToInt32(_queryString(DpContractTerminationMonitorFilterStep.QsFilterSelectedIdKey))
            End Get
        End Property

        ''' <summary>
        ''' Gets the qs filter contracts under over payment.
        ''' </summary>
        ''' <value>The qs filter contracts under over payment.</value>
        Private ReadOnly Property QsFilterContractsUnderOverPayment() As Nullable(Of Boolean)
            Get
                Return Utils.ToBoolean(_queryString(DpContractTerminationMonitorFilterStep.QsFilterUnderOrOverPaymentsKey))
            End Get
        End Property

        ''' <summary>
        ''' Gets the qs filter termination date from.
        ''' </summary>
        ''' <value>The qs filter termination date from.</value>
        Private ReadOnly Property QsFilterTerminationDateFrom() As Nullable(Of DateTime)
            Get
                Return Utils.ToDateTime(_queryString(DpContractTerminationMonitorFilterStep.QsFilterTerminationDateFromKey))
            End Get
        End Property

        ''' <summary>
        ''' Gets the qs filter termination date to.
        ''' </summary>
        ''' <value>The qs filter termination date to.</value>
        Private ReadOnly Property QsFilterTerminationDateTo() As Nullable(Of DateTime)
            Get
                Return Utils.ToDateTime(_queryString(DpContractTerminationMonitorFilterStep.QsFilterTerminationDateToKey))
            End Get
        End Property

        ''' <summary>
        ''' Gets the qs current step.
        ''' </summary>
        ''' <value>The qs current step.</value>
        Private ReadOnly Property QsCurrentStep() As Integer
            Get
                Return Utils.ToInt32(_queryString(QsCurrentStepKey))
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the current querystring values for the step.
        ''' </summary>
        ''' <value></value>
        ''' -----------------------------------------------------------------------------
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' [Mikevo]	06/10/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property QueryString() As System.Collections.Specialized.NameValueCollection Implements ISelectorWizardStep.QueryString
            Get
                Return New NameValueCollection(_queryString)
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                _queryString = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets if the step must be cimpleted before moving onto the next step.
        ''' </summary>
        ''' <value></value>
        ''' -----------------------------------------------------------------------------
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' [Mikevo]	10/10/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Required() As Boolean Implements ISelectorWizardStep.Required
            Get
                Return _required
            End Get
            Set(ByVal Value As Boolean)
                _required = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets where in the series of steps the step appears.
        ''' </summary>
        ''' <value></value>
        ''' -----------------------------------------------------------------------------
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' [Mikevo]	06/10/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property StepIndex() As Integer Implements ISelectorWizardStep.StepIndex
            Get
                Return _stepIndex
            End Get
            Set(ByVal Value As Integer)
                _stepIndex = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets the title text to display for the step.
        ''' </summary>
        ''' <value></value>
        ''' -----------------------------------------------------------------------------
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' [Mikevo]	06/10/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Title() As String Implements ISelectorWizardStep.Title
            Get
                Return "Results"
            End Get
        End Property

#End Region

#Region "Methods"

        ''' <summary>
        ''' Initialises the step.
        ''' </summary>
        ''' <param name="wizard"></param>
        ''' -----------------------------------------------------------------------------
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' [Mikevo]	22/08/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep

        End Sub

        ''' <summary>
        ''' Called during the page PreRender event.
        ''' Can be used to output step specific javascript.
        ''' </summary>
        ''' <param name="wizard">A SelectorWizard instance.</param>
        ''' -----------------------------------------------------------------------------
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' [Mikevo]	06/10/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender


        End Sub

        ''' <summary>
        ''' Renders the header controls relevant to this step.
        ''' </summary>
        ''' <param name="controls">The control collection to add the new controls to.</param>
        ''' <returns></returns>
        ''' -----------------------------------------------------------------------------
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' [Mikevo]	06/10/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls

            Dim msg As ErrorMessage

            msg = New ErrorMessage
            msg.Success = True

            Return msg

        End Function

        ''' <summary>
        ''' Renders the content controls relevant for the step.
        ''' </summary>
        ''' <param name="wizard">A reference to the wizard user control itself.</param>
        ''' <param name="controls">The control collection to add the new controls to.</param>
        ''' <returns></returns>
        ''' -----------------------------------------------------------------------------
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' [Mikevo]	06/10/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

            Dim msg As New ErrorMessage()
            Dim filterControl As DpContractTerminationMonitorResults = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/DpContractTerminationMonitorResults.ascx")

            ' setup properties of control with values from the qs
            With filterControl
                .FilterContractType = QsFilterContractType
                .FilterIsBalanced = QsFilterIsBalanced
                .FilterSelectedId = QsFilterSelectedId
                .FilterTerminationPeriodFrom = QsFilterTerminationDateFrom
                .FilterTerminationPeriodTo = QsFilterTerminationDateTo
                .FilterUnderOrOverPayments = QsFilterContractsUnderOverPayment
            End With

            ' add the control to the collection
            controls.Add(filterControl)

            msg.Success = True

            Return msg

        End Function

#End Region

    End Class

End Namespace