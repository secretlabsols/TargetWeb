
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

#Region " DomContractReOpenWeeksResultStep "

    ''' <summary>
    ''' Class to provider the results wizard step when searching for a list of domiciliary contract re-opened weeks.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      25/09/2009  D11546 - changes to "selections" section links
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' </history>
    Friend Class DomContractReOpenWeeksResultStep
        Implements ISelectorWizardStep

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_ESTABLISHMENTID As String = "estabID"
        Const QS_CONTRACTID As String = "contractID"
        Const QS_WE_DATEFROM As String = "weDateFrom"
        Const QS_WE_DATETO As String = "weDateTo"
        Const QS_CLOSURE_DATEFROM As String = "closureDateFrom"
        Const QS_CLOSURE_DATETO As String = "closureDateTo"
        Const QS_WEEKID As String = "weekID"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _description As String = "The list below displays the list of domiciliary contract re-opened weeks that match your search criteria."
        Private _providerID As Integer, _contractID As Integer, _weekID As Integer
        Private _weDateFrom As Date, _weDateTo As Date, _closureDateFrom As Date, _closureDateTo As Date
        Private _showNewButton As Boolean

        Sub New()
            _showNewButton = True
        End Sub

        Public Property BaseUrl() As String Implements ISelectorWizardStep.BaseUrl
            Get
                Return _baseUrl
            End Get
            Set(ByVal value As String)
                _baseUrl = value
            End Set
        End Property

        Public ReadOnly Property BeforeNavigateJS() As String Implements ISelectorWizardStep.BeforeNavigateJS
            Get
                Return String.Empty
            End Get
        End Property

        Public Property DbConnection() As System.Data.SqlClient.SqlConnection Implements ISelectorWizardStep.DbConnection
            Get
                Return _dbConnection
            End Get
            Set(ByVal value As System.Data.SqlClient.SqlConnection)
                _dbConnection = value
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

        Public Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep

        End Sub

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

        Public Property QueryString() As System.Collections.Specialized.NameValueCollection Implements ISelectorWizardStep.QueryString
            Get
                Return New NameValueCollection(_queryString)
            End Get
            Set(ByVal value As System.Collections.Specialized.NameValueCollection)
                _queryString = value
                ' pull out the required params
                _providerID = Utils.ToInt32(_queryString(QS_ESTABLISHMENTID))
                _contractID = Utils.ToInt32(_queryString(QS_CONTRACTID))
                If Utils.IsDate(_queryString(QS_WE_DATEFROM)) Then _weDateFrom = _queryString(QS_WE_DATEFROM)
                If Utils.IsDate(_queryString(QS_WE_DATETO)) Then _weDateTo = _queryString(QS_WE_DATETO)
                If Utils.IsDate(_queryString(QS_CLOSURE_DATEFROM)) Then _closureDateFrom = _queryString(QS_CLOSURE_DATEFROM)
                If Utils.IsDate(_queryString(QS_CLOSURE_DATETO)) Then _closureDateTo = _queryString(QS_CLOSURE_DATETO)
                _weekID = Utils.ToInt32(_queryString(QS_WEEKID))
            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

            Dim msg As ErrorMessage = New ErrorMessage
            Dim weekList As DomContractReOpenedWeekSelector = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/DomContractReOpenedWeekSelector.ascx")
            weekList.InitControl(wizard.Page, _weekID, _providerID, _contractID, _weDateFrom, _weDateTo, _closureDateFrom, _closureDateTo, _showNewButton)
            controls.Add(weekList)
            msg.Success = True
            Return msg

        End Function

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage = New ErrorMessage()
            msg.Success = True
            Return msg
        End Function

        Public Property Required() As Boolean Implements ISelectorWizardStep.Required
            Get
                Return _required
            End Get
            Set(ByVal value As Boolean)
                _required = value
            End Set
        End Property

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
                Return "Results"
            End Get
        End Property

        Public Property ShowNewButton() As Boolean
            Get
                Return _showNewButton
            End Get
            Set(ByVal value As Boolean)
                _showNewButton = value
            End Set
        End Property

    End Class

#End Region

End Namespace