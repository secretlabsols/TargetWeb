
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
    ''' Class	 : Abacus.Extranet.SelectorWizardSteps.DpContractTerminationMonitorFilterStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Implements the select filter step for dp contract termination monitoring.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	ColinD	08/09/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Class DpContractTerminationMonitorFilterStep
        Implements ISelectorWizardStep

#Region "Fields"

        ' locals
        Private _baseUrl As String = String.Empty
        Private _dbConnection As SqlConnection = Nothing
        Private _description As String = "Please apply filters and then click ""Next""."
        Private _headerLabelWidth As Unit = New Unit(20, UnitType.Em)
        Private _isCurrentStep As Boolean = False
        Private _queryString As NameValueCollection = Nothing
        Private _required As Boolean = False
        Private _stepIndex As Integer

        ' constants
        Private Const _DateFormat As String = "dd/MM/yyyy"
        Public Const QsFilterContractTypeKey As String = "fct"
        Public Const QsFilterIsBalancedKey As String = "fib"
        Public Const QsFilterUnderOrOverPaymentsKey As String = "fuop"
        Public Const QsFilterTerminationDateFromKey As String = "ftdf"
        Public Const QsFilterTerminationDateToKey As String = "ftdt"
        Public Const QsFilterSelectedIdKey As String = "dpcID"
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
                Return "DpContractTerminationMonitorFilter_BeforeNavigate()"
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
                Return Utils.ToBoolean(_queryString(QsFilterContractTypeKey))
            End Get
        End Property

        ''' <summary>
        ''' Gets the qs is balanced.
        ''' </summary>
        ''' <value>The qs is balanced.</value>
        Private ReadOnly Property QsFilterIsBalanced() As Nullable(Of Boolean)
            Get
                If _queryString.AllKeys.Contains(QsFilterIsBalancedKey) = False Then
                    Return False
                Else
                    Return Utils.ToBoolean(_queryString(QsFilterIsBalancedKey))
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets the qs filter selected id.
        ''' </summary>
        ''' <value>The qs filter selected id.</value>
        Private ReadOnly Property QsFilterSelectedId() As Integer
            Get
                Return Utils.ToInt32(_queryString(QsFilterSelectedIdKey))
            End Get
        End Property

        ''' <summary>
        ''' Gets the qs filter contracts under over payment.
        ''' </summary>
        ''' <value>The qs filter contracts under over payment.</value>
        Private ReadOnly Property QsFilterContractsUnderOverPayment() As Nullable(Of Boolean)
            Get
                Return Utils.ToBoolean(_queryString(QsFilterUnderOrOverPaymentsKey))
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
                Return "Apply Filters"
            End Get
        End Property

#End Region

#Region "Methods"

        ''' <summary>
        ''' Adds a header control line.
        ''' </summary>
        ''' <param name="controls">The controls.</param>
        ''' <param name="keyText">The key text.</param>
        ''' <param name="valueText">The value text.</param>
        ''' <param name="changeLinkText">The change link text.</param>
        ''' <param name="changeLinkUrl">The change link URL.</param>
        Public Sub AddHeaderControlLine(ByRef controls As System.Web.UI.ControlCollection, _
                                       ByVal keyText As String, _
                                       ByVal valueText As String, _
                                       ByVal changeLinkText As String, _
                                       ByVal changeLinkUrl As String)

            Dim keyLabel As New Label()
            Dim valueLabel As New Label()
            Dim changeLink As New HyperLink()
            Dim container As New HtmlGenericControl("div")

            With container
                .Style.Add("width", "100%")
            End With

            With keyLabel
                .Text = keyText
                .Style.Add("float", "left")
                .Style.Add("font-weight", "bold")
                .Style.Add("width", _headerLabelWidth.ToString())
                .Style.Add("display", "inline")
                container.Controls.Add(keyLabel)
            End With

            With valueLabel
                .Text = valueText
                .Style.Add("float", "left")
                .Style.Add("display", "inline")
                container.Controls.Add(valueLabel)
            End With

            With changeLink
                .Text = changeLinkText
                .NavigateUrl = changeLinkUrl
                .Style.Add("float", "right")
                .Style.Add("display", "inline")
                container.Controls.Add(changeLink)
            End With

            controls.Add(container)
            Dim ltr As New LiteralControl()
            ltr.Text = "<div class='clearer' />"
            controls.Add(ltr)

        End Sub

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

            Dim contractTypeKeyText As String = String.Empty
            Dim contractTypeKeyLabel As String = String.Empty
            Dim contractTypeChangeLinkText As String = String.Empty
            Dim contractTypeChangeLinkUrl As String = String.Empty
            Dim contractTypeQs As NameValueCollection = Nothing
            Dim underOverPaymentsKeyText As String = String.Empty
            Dim underOverPaymentsKeyLabel As String = String.Empty
            Dim underOverPaymentsChangeLinkText As String = String.Empty
            Dim underOverPaymentsChangeLinkUrl As String = String.Empty
            Dim underOverPaymentsQs As NameValueCollection = Nothing
            Dim balancedKeyText As String = String.Empty
            Dim balancedKeyLabel As String = String.Empty
            Dim balancedChangeLinkText As String = String.Empty
            Dim balancedChangeLinkUrl As String = String.Empty
            Dim balancedQs As NameValueCollection = Nothing
            Dim terminationPeriodKeyText As String = String.Empty
            Dim terminationPeriodKeyLabel As String = String.Empty
            Dim terminationPeriodChangeLinkText As String = String.Empty
            Dim terminationPeriodChangeLinkUrl As String = String.Empty
            Dim terminationPeriodQs As NameValueCollection = Nothing

            ' set labels
            contractTypeKeyLabel = "Contract Type"
            underOverPaymentsKeyLabel = "Under/Over Payments"
            balancedKeyLabel = "Balanced"
            terminationPeriodKeyLabel = "Termination Period"

            ' setup contract type label value
            If QsFilterContractType.HasValue = False Then
                contractTypeKeyText = "Do not filter by this item"
            ElseIf QsFilterContractType.Value = True Then
                contractTypeKeyText = "Show SDS contracts"
            Else
                contractTypeKeyText = "Show non-SDS contracts"
            End If

            ' setup under over payments label value
            If QsFilterContractsUnderOverPayment.HasValue = False Then
                underOverPaymentsKeyText = "Do not filter by this item"
            ElseIf QsFilterContractsUnderOverPayment.Value = True Then
                underOverPaymentsKeyText = "Show under-paid contracts"
            Else
                underOverPaymentsKeyText = "Show over-paid contracts"
            End If

            ' setup balanced label value
            If QsFilterIsBalanced.HasValue = False Then
                balancedKeyText = "Do not filter by this item"
            ElseIf QsFilterIsBalanced.Value = True Then
                balancedKeyText = "Show contracts that have been balanced"
            Else
                balancedKeyText = "Show contracts that have not been balanced"
            End If

            ' setup balanced label value
            If QsFilterTerminationDateFrom.HasValue = False AndAlso QsFilterTerminationDateTo.HasValue = False Then
                terminationPeriodKeyText = "Do not filter by this item"
            ElseIf QsFilterTerminationDateFrom.HasValue = True AndAlso QsFilterTerminationDateTo.HasValue = False Then
                terminationPeriodKeyText = String.Format("From {0}", QsFilterTerminationDateFrom.Value.ToString(_DateFormat))
            ElseIf QsFilterTerminationDateTo.HasValue = True AndAlso QsFilterTerminationDateFrom.HasValue = False Then
                terminationPeriodKeyText = String.Format("To {0}", QsFilterTerminationDateTo.Value.ToString(_DateFormat))
            Else
                terminationPeriodKeyText = String.Format("From {0} To {1}", QsFilterTerminationDateFrom.Value.ToString(_DateFormat), QsFilterTerminationDateTo.Value.ToString(_DateFormat))
            End If

            ' add query string items into relevant collections
            contractTypeQs = Me.QueryString
            underOverPaymentsQs = Me.QueryString
            balancedQs = Me.QueryString
            terminationPeriodQs = Me.QueryString

            If Me.IsCurrentStep Then

                ' setup contract types link, for other step
                If QsFilterContractType.HasValue = False Then
                    contractTypeChangeLinkText = ""
                Else
                    contractTypeChangeLinkText = "Show All"
                    contractTypeQs(QsFilterContractTypeKey) = ""
                End If
                contractTypeChangeLinkUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(contractTypeQs))

                ' setup under over payments link, for other step
                If QsFilterContractsUnderOverPayment.HasValue = False Then
                    underOverPaymentsChangeLinkText = ""
                Else
                    underOverPaymentsChangeLinkText = "Show All"
                    underOverPaymentsQs(QsFilterUnderOrOverPaymentsKey) = ""
                End If
                underOverPaymentsChangeLinkUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(underOverPaymentsQs))

                ' setup balanced link, for other step
                If QsFilterIsBalanced.HasValue = False Then
                    balancedChangeLinkText = ""
                Else
                    balancedChangeLinkText = "Show All"
                    balancedQs(QsFilterIsBalancedKey) = ""
                End If
                balancedChangeLinkUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(balancedQs))

                ' setup periods link, for other step
                If QsFilterTerminationDateFrom.HasValue = False AndAlso QsFilterTerminationDateTo.HasValue = False Then
                    terminationPeriodChangeLinkText = ""
                Else
                    terminationPeriodChangeLinkText = "Show All"
                    terminationPeriodQs(QsFilterTerminationDateFromKey) = ""
                    terminationPeriodQs(QsFilterTerminationDateToKey) = ""
                End If
                terminationPeriodChangeLinkUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(terminationPeriodQs))

            ElseIf QsCurrentStep > Me.StepIndex Then

                ' setup contract types link, for current step
                contractTypeChangeLinkText = "Change contract type filter"
                contractTypeQs.Remove(QsCurrentStepKey)
                contractTypeQs.Add(QsCurrentStepKey, Me.StepIndex)
                contractTypeChangeLinkUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(contractTypeQs))

                ' setup under over payments link, for current step
                underOverPaymentsChangeLinkText = "Change payments filter"
                underOverPaymentsQs.Remove(QsCurrentStepKey)
                underOverPaymentsQs.Add(QsCurrentStepKey, Me.StepIndex)
                underOverPaymentsChangeLinkUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(underOverPaymentsQs))

                ' setup balabced link, for current step
                balancedChangeLinkText = "Change balanced filter"
                balancedQs.Remove(QsCurrentStepKey)
                balancedQs.Add(QsCurrentStepKey, Me.StepIndex)
                balancedChangeLinkUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(balancedQs))

                ' setup periods link, for current step
                terminationPeriodChangeLinkText = "Change period filter"
                terminationPeriodQs.Remove(QsCurrentStepKey)
                terminationPeriodQs.Add(QsCurrentStepKey, Me.StepIndex)
                terminationPeriodChangeLinkUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(terminationPeriodQs))

            End If

            ' add lines
            AddHeaderControlLine(controls, contractTypeKeyLabel, contractTypeKeyText, contractTypeChangeLinkText, contractTypeChangeLinkUrl)
            AddHeaderControlLine(controls, underOverPaymentsKeyLabel, underOverPaymentsKeyText, underOverPaymentsChangeLinkText, underOverPaymentsChangeLinkUrl)
            AddHeaderControlLine(controls, balancedKeyLabel, balancedKeyText, balancedChangeLinkText, balancedChangeLinkUrl)
            AddHeaderControlLine(controls, terminationPeriodKeyLabel, terminationPeriodKeyText, terminationPeriodChangeLinkText, terminationPeriodChangeLinkUrl)

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
            Dim filterControl As DpContractTerminationMonitorFilter = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/DpContractTerminationMonitorFilter.ascx")

            ' setup properties of control with values from the qs
            With filterControl
                .FilterContractType = QsFilterContractType
                .FilterIsBalanced = QsFilterIsBalanced
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