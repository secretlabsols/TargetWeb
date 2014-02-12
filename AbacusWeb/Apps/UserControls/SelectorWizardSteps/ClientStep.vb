
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

#Region " ClientStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.SelectorWizardSteps.ClientStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Implements the select client step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MikeVO      13/04/2011  SDS issue #415 - corrected default DateFrom behaviour (reversing SDS issue #345).
    '''     ColinD      23/11/2011  SDS345 - changes to add default date from date which can be passed to subsequent steps in url
    '''     MikeVO      25/09/2009  D11546 - changes to "selections" section links
    ''' 	[Mikevo]	12/09/2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Class ClientStep
        Implements ISelectorWizardStep

        Const QS_CLIENTID As String = "clientID"
        Const QS_BUDGETHOLDERID As String = "bhID"
        Const QS_CURRENTSTEP As String = "currentStep"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _clientID As Integer
        Private _budgetHolderID As Integer
        Private _required As Boolean
        Private _description As String = "Please select a service user from the list below and then click ""Next""."
        Private _headerLabelWidth As Unit = New Unit(10, UnitType.Em)
        Private _currentStep As Integer

        Public Property BaseUrl() As String Implements ISelectorWizardStep.BaseUrl
            Get
                Return _baseUrl
            End Get
            Set(ByVal Value As String)
                _baseUrl = Value
            End Set
        End Property

        Public Property HeaderLabelWidth() As Unit
            Get
                Return _headerLabelWidth
            End Get
            Set(ByVal value As Unit)
                _headerLabelWidth = value
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
                _clientID = Utils.ToInt32(_queryString(QS_CLIENTID))
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
                Return "Select a Service User"
            End Get
        End Property

        Public ReadOnly Property BeforeNavigateJS() As String Implements ISelectorWizardStep.BeforeNavigateJS
            Get
                Return "ClientStep_BeforeNavigate()"
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
              String.Format("ClientStep_required={0};", _required.ToString().ToLower()) _
            ))

        End Sub

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls

            Dim msg As ErrorMessage
            Dim lbl As Label = New Label
            Dim text As Label = New Label
            Dim link As HyperLink = New HyperLink
            Dim client As ClientDetail
            Dim qs As NameValueCollection

            ' label
            lbl.Text = "Service User"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", _headerLabelWidth.ToString())
            controls.Add(lbl)

            text.Style.Add("float", "left")
            If _clientID = 0 Then
                text.Text = "All"
            Else
                client = New ClientDetail(Me.DbConnection, String.Empty, String.Empty)
                msg = client.Fetch(_clientID)
                If Not msg.Success Then Return msg
                text.Text = String.Format("{0}: {1}", client.Reference, client.Name)

                ' store MRU client
                Dim mruManager As Target.Library.Web.MostRecentlyUsedManager = New Target.Library.Web.MostRecentlyUsedManager(HttpContext.Current)
                mruManager("SERVICE_USERS")(_clientID.ToString()) = text.Text
                mruManager.Save(HttpContext.Current)

            End If
            controls.Add(text)

            ' add the link
            qs = Me.QueryString
            If Me.IsCurrentStep AndAlso _clientID > 0 Then
                link.Text = "All Service Users"
                qs.Remove(QS_CLIENTID)
                qs.Remove(QS_BUDGETHOLDERID)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                link.Text = "Change Service User"
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
            Dim clientList As ClientSelector = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/ClientSelector.ascx")
            clientList.InitControl(wizard.Page, _clientID)
            controls.Add(clientList)
            msg.Success = True
            Return msg

        End Function

    End Class

#End Region

End Namespace