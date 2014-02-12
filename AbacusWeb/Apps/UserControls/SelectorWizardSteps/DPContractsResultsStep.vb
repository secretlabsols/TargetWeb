
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

#Region " DPContractsResultsStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.SelectorWizardSteps.DPContractsResultsStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Implements the DP Contracts results wizard step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MoTahir 24/10/2012  D12399 Copy Function For Direct Payment Contracts.
    '''     MikeVO  23/05/2011  SDS issue #707 - defaulting date from.
    '''     Waqas   14/02/2011  D12009 Updated the SDS feature on result step
    '''     MoTahir 20/09/2010  D11814 Service User Enquiry
    ''' 	JohnF	27/07/2010	Created (D11801)
    '''     ColinD  06/08/2010  Updated (D11802) - Added ShowCreatePaymentsButton property
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Class DPContractsResultsStep
        Implements ISelectorWizardStep

        Const QS_BUDGETHOLDERID As String = "bhID"
        Const QS_CLIENTID As String = "clientID"
        Const QS_CONTRACTID As String = "dpcID"
        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_ISSDS As String = "isSDS"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _contractID As Integer
        Private _budgetHolderID As Integer
        Private _clientID As Integer
        Private _dateFrom As Date
        Private _dateTo As Date
        Private _showNewButton As Boolean
        Private _showViewButton As Boolean
        Private _showTerminateButton As Boolean
        Private _showReinstateButton As Boolean
        Private _required As Boolean
        Private _description As String = "The list below displays the results of your Direct Payment Contract enquiry."
        Private _headerLabelWidth As Unit = New Unit(10, UnitType.Em)
        Private _currentStep As Integer
        Private _ShowCreatePaymentsButton As Boolean
        Private _isSDS As TriState
        Private _showCopyButton As Boolean

        Public Property HeaderLabelWidth() As Unit
            Get
                Return _headerLabelWidth
            End Get
            Set(ByVal value As Unit)
                _headerLabelWidth = value
            End Set
        End Property

        Public Property BaseUrl() As String Implements ISelectorWizardStep.BaseUrl
            Get
                Return _baseUrl
            End Get
            Set(ByVal Value As String)
                _baseUrl = Value
            End Set
        End Property

        Public Property DbConnection() As System.Data.SqlClient.SqlConnection Implements ISelectorWizardStep.DbConnection
            Get
                Return _dbConnection
            End Get
            Set(ByVal Value As System.Data.SqlClient.SqlConnection)
                _dbConnection = Value
            End Set
        End Property

        Public Property Description() As String Implements ISelectorWizardStep.Description
            Get
                Return _description
            End Get
            Set(ByVal Value As String)
                _description = Value
            End Set
        End Property

        Public Property IsCurrentStep() As Boolean Implements ISelectorWizardStep.IsCurrentStep
            Get
                Return _isCurrentStep
            End Get
            Set(ByVal Value As Boolean)
                _isCurrentStep = Value
            End Set
        End Property

        Public Property QueryString() As System.Collections.Specialized.NameValueCollection Implements ISelectorWizardStep.QueryString
            Get
                Return New NameValueCollection(_queryString)
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                _queryString = Value
                '++ Pull out the required params from the query string..
                _budgetHolderID = Utils.ToInt32(_queryString(QS_BUDGETHOLDERID))
                _clientID = Utils.ToInt32(_queryString(QS_CLIENTID))
                If Not _queryString(QS_ISSDS) Is Nothing Then
                    ' ++ if issds is present on the querystring but is empty, default to both
                    If Convert.ToString(_queryString(QS_ISSDS)).Length = 0 Then
                        _isSDS = TriState.UseDefault
                    Else
                        If _queryString(QS_ISSDS) <> TriState.False And _queryString(QS_ISSDS) <> TriState.True _
                        And _queryString(QS_ISSDS) <> TriState.UseDefault Then
                            _isSDS = TriState.UseDefault
                        Else
                            _isSDS = _queryString(QS_ISSDS)
                        End If
                    End If
                Else
                    _isSDS = TriState.UseDefault
                    _queryString.Add(QS_ISSDS, TriState.UseDefault)
                End If

                Using drs As New DateRangeStep()
                    With drs
                        .QueryString = Me.QueryString
                        _dateFrom = .DateFrom
                        _dateTo = .DateTo
                    End With
                End Using
                    
                _currentStep = Utils.ToInt32(_queryString(QS_CURRENTSTEP))
            End Set
        End Property

        Public Property StepIndex() As Integer Implements ISelectorWizardStep.StepIndex
            Get
                Return _stepIndex
            End Get
            Set(ByVal Value As Integer)
                _stepIndex = Value
            End Set
        End Property

        Public ReadOnly Property Title() As String Implements ISelectorWizardStep.Title
            Get
                Return "Results"
            End Get
        End Property

        Public ReadOnly Property BeforeNavigateJS() As String Implements ISelectorWizardStep.BeforeNavigateJS
            Get
                Return String.Empty
            End Get
        End Property

        Public Property Required() As Boolean Implements ISelectorWizardStep.Required
            Get
                Return _required
            End Get
            Set(ByVal Value As Boolean)
                _required = Value
            End Set
        End Property

        Public Property ShowNewButton() As Boolean
            Get
                Return _showNewButton
            End Get
            Set(ByVal value As Boolean)
                _showNewButton = value
            End Set
        End Property

        Public Property ShowViewButton() As Boolean
            Get
                Return _showViewButton
            End Get
            Set(ByVal value As Boolean)
                _showViewButton = value
            End Set
        End Property

        Public Property ShowReinstateButton() As Boolean
            Get
                Return _showReinstateButton
            End Get
            Set(ByVal value As Boolean)
                _showReinstateButton = value
            End Set
        End Property

        Public Property ShowTerminateButton() As Boolean
            Get
                Return _showTerminateButton
            End Get
            Set(ByVal value As Boolean)
                _showTerminateButton = value
            End Set
        End Property


        Public Property IsSDS() As TriState
            Get
                Return _isSDS
            End Get
            Set(ByVal value As TriState)
                _isSDS = value
            End Set
        End Property


        ''' <summary>
        ''' Gets or sets a value indicating whether [show create payments button].
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if [show create payments button]; otherwise, <c>false</c>.
        ''' </value>
        Public Property ShowCreatePaymentsButton() As Boolean
            Get
                Return _ShowCreatePaymentsButton
            End Get
            Set(ByVal value As Boolean)
                _ShowCreatePaymentsButton = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets a value indicating whether [show copy button].
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if [show copy payments button]; otherwise, <c>false</c>.
        ''' </value>
        Public Property ShowCopyButton() As Boolean
            Get
                Return _showCopyButton
            End Get
            Set(ByVal value As Boolean)
                _showCopyButton = value
            End Set
        End Property

        Public Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep

        End Sub

        Public Sub New()
            _showNewButton = True
            _showViewButton = True
            _showTerminateButton = False
            _showReinstateButton = False
            _ShowCreatePaymentsButton = False
        End Sub

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

        End Sub

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage
            Dim lbl As Label = New Label
            Dim text As Label = New Label
            Dim link As HyperLink = New HyperLink
            Dim reader As SqlDataReader = Nothing
            Dim qs As NameValueCollection

            '++ Label..
            lbl.Text = "DP Contract"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", _headerLabelWidth.ToString())
            controls.Add(lbl)

            text.Style.Add("float", "left")

            qs = Me.QueryString
            _contractID = Utils.ToInt32(qs.Get(QS_CONTRACTID))
            If _contractID = 0 Then
                text.Text = "All"
            Else
                Try
                    Dim dpc As DPContract = New DPContract(Me.DbConnection, "", "")
                    msg = dpc.Fetch(_contractID)
                    If Not msg.Success Then Return msg
                    text.Text = String.Format("{0}", dpc.Number)

                Catch ex As Exception
                    msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected
                    Return msg
                Finally
                    If Not reader Is Nothing Then reader.Close()
                End Try
            End If
            controls.Add(text)

            '++ Add the link..
            If Me.IsCurrentStep AndAlso _contractID > 0 Then
                '++ All..
                link.Text = "All DP Contracts"
                qs.Remove(QS_CONTRACTID)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                '++ Change..
                link.Text = "Change DP Contract"
                qs.Remove(QS_CURRENTSTEP)
                qs.Add(QS_CURRENTSTEP, Me.StepIndex)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            End If
            If Not String.IsNullOrEmpty(link.Text) Then
                link.Style.Add("float", "right")
                controls.Add(link)
            End If

            msg = New ErrorMessage
            msg.Success = True

            Return msg

        End Function

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

            Dim msg As ErrorMessage = New ErrorMessage
            Dim dpContractList As DPContractSelector = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/DPContractSelector.ascx")
            dpContractList.InitControl(wizard.Page, _clientID, _
                                       _budgetHolderID, _dateFrom, _dateTo, _
                                       _showNewButton, _showViewButton, _
                                       _showReinstateButton, _showTerminateButton, _
                                       _contractID, _ShowCreatePaymentsButton, False, _showCopyButton, IsSDS)
            controls.Add(dpContractList)
            msg.Success = True
            Return msg

        End Function
    End Class

#End Region

End Namespace