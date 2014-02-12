
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

#Region " DPContractHiddenReportsStep "

    ''' <summary>
    ''' Implements the hidden reports step for the Direct Payment contracts wizard.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' JohnF   28/07/2010   Created (D11801)
    ''' </history>
    Friend Class DPContractHiddenReportsStep
        Implements ISelectorWizardStep, ISelectorWizardHiddenEndStep

        Const QS_BUDGETHOLDERID As String = "bhID"
        Const QS_CLIENTID As String = "clientID"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETO As String = "dateTo"
        Const QS_CONTRACTID As String = "dpcID"
        Const QS_CURRENTSTEP As String = "currentStep"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _description As String = "The available contract reports can be accessed below. The selections you have made above are applied to these reports."
        Private _title As String = "Reports"
        Private _contractID As Integer
        Private _budgetHolderID As Integer
        Private _clientID As Integer
        Private _dateFrom As Date
        Private _dateTo As Date

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
                '++ Pull out the required params..
                _clientID = Utils.ToInt32(_queryString(QS_CLIENTID))
                _budgetHolderID = Utils.ToInt32(_queryString(QS_BUDGETHOLDERID))
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETO)) Then _dateTo = _queryString(QS_DATETO)
                _contractID = Utils.ToInt32(_queryString(QS_CONTRACTID))
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
                Return _title
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

        Public Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep

        End Sub

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

        End Sub

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage = New ErrorMessage
            msg.Success = True
            Return msg
        End Function

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls
            Dim msg As ErrorMessage = New ErrorMessage
            Dim contractReports As DPContractReports = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/DPContractReports.ascx")

            With contractReports
                .ThePage = wizard.Page
                .clientDetailID = _clientID
                .budgetHolderID = _budgetHolderID
                .DateFrom = _dateFrom
                .DateTo = _dateTo
                .contractID = _contractID
            End With
            controls.Add(contractReports)
            msg.Success = True
            Return msg
        End Function

        Public ReadOnly Property ButtonText() As String Implements ISelectorWizardHiddenEndStep.ButtonText
            Get
                Return "Reports"
            End Get
        End Property
    End Class

#End Region

End Namespace