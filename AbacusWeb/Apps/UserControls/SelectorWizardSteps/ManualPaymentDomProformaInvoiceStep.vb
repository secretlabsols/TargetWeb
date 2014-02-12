
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

#Region " ManualPaymentDomProformaInvoiceStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.SelectorWizardSteps.ManualPaymentDomProformaInvoiceStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Implements the select manual payment domiciliary pro forma invoice step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MikeVO      25/09/2009  D11546 - changes to "selections" section links
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' 	[Mikevo]	07/04/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Class ManualPaymentDomProformaInvoiceStep
        Implements ISelectorWizardStep

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_INVOICEID As String = "dsoID"
        Const QS_ESTABLISHMENTID As String = "estabID"
        Const QS_CONTRACTID As String = "contractID"
        Const QS_SYSACCID As String = "sysAccID"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _description As String = "The list below displays the manual payment proforma invoices that match your search criteria."
        Private _invoiceID As Integer
        Private _estabID As Integer
        Private _contractID As Integer
        Private _sysAccID As Integer
        Private _showNewButton As Boolean

        Sub New()
            _showNewButton = True
        End Sub

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
                ' pull out the required params
                _invoiceID = Utils.ToInt32(_queryString(QS_INVOICEID))
                _estabID = Utils.ToInt32(_queryString(QS_ESTABLISHMENTID))
                _contractID = Utils.ToInt32(_queryString(QS_CONTRACTID))
                _sysAccID = Utils.ToInt32(_queryString(QS_SYSACCID))
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
                Return "ManualPaymentDomProformaInvoiceStep_BeforeNavigate()"
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
            wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", _
             Target.Library.Web.Utils.WrapClientScript( _
              String.Format("ManualPaymentDomProformaInvoiceStep_required={0};", _required.ToString().ToLower()) _
            ))
        End Sub

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage = New ErrorMessage
            msg.Success = True
            Return msg
        End Function

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls
            Dim msg As ErrorMessage = New ErrorMessage
            Dim dsoList As ManualPaymentDomProformaInvoiceSelector = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/ManualPaymentDomProformaInvoiceSelector.ascx")
            dsoList.InitControl(wizard.Page, _invoiceID, _estabID, _contractID, _sysAccID, _showNewButton)
            controls.Add(dsoList)
            msg.Success = True
            Return msg
        End Function

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