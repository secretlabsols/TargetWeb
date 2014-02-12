
Imports Target.Web.Apps.Security
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.Data.SqlClient
Imports System.Text
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.UserControls.SelectorWizardSteps

#Region " EstablishmentStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.SelectorWizardSteps.EstablishmentStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Implements the select establishment step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     WAQAS       22/03/2011  D13083
    '''     WAQAS       18/03/2011  D12082
    '''     Waqas       17/03/2011  D12081 - change title for Service User Payments Only
    '''     MikeVO      25/09/2009  D11546 - changes to "selections" section links
    '''     MikeVO      19/01/2009  A4WA#5161.
    '''     MikeVO      24/11/2008  A4WA#5029 - security fixes
    ''' 	[Mikevo]	07/09/2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Class EstablishmentStep
        Implements ISelectorWizardStep

        Public Const QS_ESTABLISHMENTID As String = "estabID"
        Const QS_CURRENTSTEP As String = "currentStep"

        Public Enum EstablishmentStepMode
            Establishments = 1
            ResidentialHomes = 2
            DomProviders = 3
        End Enum

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _establishmentID As Integer
        Private _required As Boolean
        Private _description As String = "Please select an establishment from the list below and then click ""Next""."
        Private _mode As EstablishmentStepMode = EstablishmentStepMode.Establishments
        Private _headerLabelWidth As Unit = New Unit(8, UnitType.Em)
        Private _currentStep As Integer
        Private _isCareProvider As Boolean = False

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
                _establishmentID = Utils.ToInt32(_queryString(QS_ESTABLISHMENTID))
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

        Public WriteOnly Property IsCareProvider() As Boolean
            Set(ByVal value As Boolean)
                _isCareProvider = value
            End Set
        End Property

        Public ReadOnly Property Title() As String Implements ISelectorWizardStep.Title
            Get
                Title = String.Empty
                Select Case _mode
                    Case EstablishmentStepMode.Establishments
                        Return "Select an Establishment"
                    Case EstablishmentStepMode.ResidentialHomes
                        Return "Select a Residential Home"
                    Case EstablishmentStepMode.DomProviders And Not _isCareProvider
                        Return "Select a Domiciliary Provider"
                    Case EstablishmentStepMode.DomProviders And _isCareProvider
                        Return "Select a Care Provider"
                    Case Else
                        ThrowInvalidMode()
                End Select
            End Get
        End Property


        Public ReadOnly Property BeforeNavigateJS() As String Implements ISelectorWizardStep.BeforeNavigateJS
            Get
                Return "EstablishmentStep_BeforeNavigate()"
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

        Public Property Mode() As EstablishmentStepMode
            Get
                Return _mode
            End Get
            Set(ByVal Value As EstablishmentStepMode)
                _mode = Value
            End Set
        End Property

        Public Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep

        End Sub

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

            wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Extranet.Apps.UserControls.SelectorWizardSteps.EstablishmentStep.Startup", _
                Target.Library.Web.Utils.WrapClientScript( _
                    String.Format("EstablishmentStep_required={0};EstablishmentStep_mode={1};", _
                        _required.ToString().ToLower(), Convert.ToInt32(_mode)) _
                ) _
            )

        End Sub

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls

            Const SP_NAME As String = "pr_FetchEstablishment"

            Dim msg As ErrorMessage
            Dim lbl As Label = New Label
            Dim text As Label = New Label
            Dim link As HyperLink = New HyperLink
            Dim reader As SqlDataReader = Nothing
            Dim spParams As SqlParameter()
            Dim qs As NameValueCollection

            ' label
            Select Case _mode
                Case EstablishmentStepMode.Establishments
                    lbl.Text = "Establishment"
                Case EstablishmentStepMode.ResidentialHomes
                    lbl.Text = "Home"
                Case EstablishmentStepMode.DomProviders
                    lbl.Text = "Provider"
                Case Else
                    ThrowInvalidMode()
            End Select
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", _headerLabelWidth.ToString())
            controls.Add(lbl)

            text.Style.Add("float", "left")
            If _establishmentID = 0 Then
                text.Text = "All"
            Else
                ' ensure user can view establishment
                Dim canViewType As TriState, canView As Boolean
                Select Case _mode
                    Case EstablishmentStepMode.Establishments
                        canViewType = TriState.UseDefault
                    Case EstablishmentStepMode.ResidentialHomes
                        canViewType = TriState.False
                    Case EstablishmentStepMode.DomProviders
                        canViewType = TriState.True
                    Case Else
                        ThrowInvalidMode()
                End Select
                msg = AbacusClassesBL.UserCanViewEstablishment(Me.DbConnection, SecurityBL.GetCurrentUser().ExternalUserID, _establishmentID, canViewType, canView)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                If Not canView Then
                    HttpContext.Current().Response.Redirect("~/Library/Errors/404.aspx")
                End If

                Try
                    ' get the establishment details
                    spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME, False)
                    spParams(0).Value = _establishmentID
                    reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams)
                    If (reader.HasRows) Then
                        reader.Read()
                        text.Text = String.Format("{0}: {1}", reader("AltReference"), reader("Name"))
                    End If
                Catch ex As Exception
                    msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected
                    Return msg
                Finally
                    If Not reader Is Nothing Then reader.Close()
                End Try

            End If
            controls.Add(text)

            ' add the link
            qs = Me.QueryString
            If Me.IsCurrentStep AndAlso _establishmentID > 0 Then
                ' all
                Select Case _mode
                    Case EstablishmentStepMode.Establishments
                        link.Text = "All Establishments"
                    Case EstablishmentStepMode.ResidentialHomes
                        link.Text = "All Homes"
                    Case EstablishmentStepMode.DomProviders
                        link.Text = "All Providers"
                    Case Else
                        ThrowInvalidMode()
                End Select
                qs.Remove(QS_ESTABLISHMENTID)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                Select Case _mode
                    Case EstablishmentStepMode.Establishments
                        link.Text = "Change Establishments"
                    Case EstablishmentStepMode.ResidentialHomes
                        link.Text = "Change Home"
                    Case EstablishmentStepMode.DomProviders
                        link.Text = "Change Provider"
                    Case Else
                        ThrowInvalidMode()
                End Select
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
            Dim establishmentList As EstablishmentSelector = wizard.LoadControl("~/AbacusExtranet/Apps/UserControls/EstablishmentSelector.ascx")
            establishmentList.InitControl(wizard.Page, _establishmentID)
            controls.Add(establishmentList)
            msg.Success = True
            Return msg

        End Function

        Private Sub ThrowInvalidMode()
            Throw New ArgumentOutOfRangeException("Mode", _mode, "Unrecognised Mode value.")
        End Sub

    End Class

#End Region

#Region " ResPaymentEnquiryResultsStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.Apps.UserControls.SelectorWizardSteps.ResPaymentEnquiryResultsStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Implements the residential payment enquiry results wizard step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MikeVO      25/09/2009  D11546 - changes to "selections" section links
    ''' 	[Mikevo]	07/09/2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Class ResPaymentEnquiryResultsStep
        Implements ISelectorWizardStep

        Const QS_ESTABLISHMENTID As String = "estabID"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETO As String = "dateTo"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _establishmentID As Integer
        Private _dateFrom As Date
        Private _dateTo As Date
        Private _description As String = "The list below displays the results of your payment enquiry."

        Public Property BaseUrl() As String Implements ISelectorWizardStep.BaseUrl
            Get
                Return _baseUrl
            End Get
            Set(ByVal Value As String)
                _baseUrl = Value
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
                _establishmentID = Utils.ToInt32(_queryString(QS_ESTABLISHMENTID))
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETO)) Then _dateTo = _queryString(QS_DATETO)
            End Set
        End Property

        Public Property Required() As Boolean Implements ISelectorWizardStep.Required
            Get
                Return _required
            End Get
            Set(ByVal Value As Boolean)
                _required = Value
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

        Public Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep

        End Sub

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

        End Sub

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls
            Dim msg As ErrorMessage = New ErrorMessage
            Dim remittanceList As RemittanceSelector = wizard.LoadControl("~/AbacusExtranet/Apps/UserControls/RemittanceSelector.ascx")
            remittanceList.InitControl(wizard.Page, _establishmentID, _dateFrom, _dateTo)
            controls.Add(remittanceList)
            msg.Success = True
            Return msg
        End Function

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage = New ErrorMessage
            msg.Success = True
            Return msg
        End Function


    End Class

#End Region

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
    '''     ColinD      25/03/2010  A4WA#6119 - change to "RenderHeaderControls" method to use the amended NameValueColection to create selection URL rather than the current requests query string NameValueColection.
    '''     MikeVO      25/09/2009  D11546 - changes to "selections" section links
    '''     MikeVO      19/01/2009  A4WA#5161.
    ''' 	[Mikevo]	12/09/2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Class ClientStep
        Implements ISelectorWizardStep

        Const QS_ESTABLISHMENTID As String = "estabID"
        Const QS_CONTRACTID As String = "contractID"
        Const QS_CLIENTID As String = "clientID"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETO As String = "dateTo"
        Const QS_CURRENTSTEP As String = "currentStep"
        Public Const QS_PSCHEDULE_ID As String = "pscheduleid"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _viewClientBaseUrl As String
        Private _isCurrentStep As Boolean
        Private _addHeaderControls As Boolean = True
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _establishmentID As Integer
        Private _contractID As Integer
        Private _clientID As Integer
        Private _dateFrom As Date
        Private _dateTo As Date
        Private _required As Boolean
        Private _description As String = "Please select a service user from the list below and then click ""Next""."
        Private _mode As ClientStepMode = ClientStepMode.Clients
        Private _headerLabelWidth As Unit = New Unit(8, UnitType.Em)
        Private _currentStep As Integer
        Private _pScheduleID As Integer

        Public Property BaseUrl() As String Implements ISelectorWizardStep.BaseUrl
            Get
                Return _baseUrl
            End Get
            Set(ByVal Value As String)
                _baseUrl = Value
            End Set
        End Property

        Public Property ViewClientBaseUrl() As String
            Get
                Return _viewClientBaseUrl
            End Get
            Set(ByVal Value As String)
                _viewClientBaseUrl = Value
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

        Public Property AddHeaderControls() As Boolean
            Get
                Return _addHeaderControls
            End Get
            Set(ByVal Value As Boolean)
                _addHeaderControls = Value
            End Set
        End Property

        Public Property QueryString() As System.Collections.Specialized.NameValueCollection Implements ISelectorWizardStep.QueryString
            Get
                Return New NameValueCollection(_queryString)
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                _queryString = Value
                ' pull out the required params
                _establishmentID = Utils.ToInt32(_queryString(QS_ESTABLISHMENTID))
                _contractID = Utils.ToInt32(_queryString(QS_CONTRACTID))
                _clientID = Utils.ToInt32(_queryString(QS_CLIENTID))
                _pScheduleID = Utils.ToInt32(_queryString(QS_PSCHEDULE_ID))
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETO)) Then _dateTo = _queryString(QS_DATETO)
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

        Public Property Mode() As ClientStepMode
            Get
                Return _mode
            End Get
            Set(ByVal Value As ClientStepMode)
                _mode = Value
            End Set
        End Property


        Private _showServiceOrderWithValidPeriod As Boolean = False
        Public Property showServiceOrderWithValidPeriod() As Boolean
            Get
                Return _showServiceOrderWithValidPeriod
            End Get
            Set(ByVal value As Boolean)
                _showServiceOrderWithValidPeriod = value
            End Set
        End Property

        Public Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep

        End Sub

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

            wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Extranet.Apps.UserControls.SelectorWizardSteps.ClientStep.Startup", _
                Target.Library.Web.Utils.WrapClientScript( _
                    String.Format("ClientStep_required={0};", _required.ToString().ToLower()) _
                ) _
            )

        End Sub

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls

            Const SP_NAME As String = "pr_FetchClientDetail"

            Dim msg As ErrorMessage
            Dim lbl As Label = New Label
            Dim text As Label = New Label
            Dim link As HyperLink = New HyperLink
            Dim reader As SqlDataReader = Nothing
            Dim spParams As SqlParameter()
            Dim qs As NameValueCollection

            If _addHeaderControls Then

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
                    ' ensure user can view client
                    Dim canViewType As UserCanViewClientCheckType, canView As Boolean
                    Select Case _mode
                        Case ClientStepMode.ResidentialClients
                            canViewType = UserCanViewClientCheckType.ResidentialClients
                        Case ClientStepMode.ClientsWithDomSvcOrders
                            canViewType = UserCanViewClientCheckType.ClientsWithDomSvcOrders
                        Case ClientStepMode.ClientsWithDomProviderInvoices
                            canViewType = UserCanViewClientCheckType.ClientsWithDomProviderInvoices
                        Case ClientStepMode.ClientsWithVisitBasedDomProviderInvoices
                            canViewType = UserCanViewClientCheckType.ClientsWithVisitBasedDomProviderInvoices
                        Case Else
                            ThrowInvalidMode()
                    End Select
                    msg = AbacusClassesBL.UserCanViewClient(Me.DbConnection, SecurityBL.GetCurrentUser().ExternalUserID, _clientID, canViewType, canView)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    If Not canView Then
                        HttpContext.Current().Response.Redirect("~/Library/Errors/404.aspx")
                    End If

                    Try
                        ' get the client details
                        spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME, False)
                        spParams(0).Value = _clientID
                        reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams)
                        If (reader.HasRows) Then
                            reader.Read()
                            text.Text = String.Format("{0}: {1}", reader("Reference"), reader("Name"))
                        End If

                    Catch ex As Exception
                        msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected
                        Return msg
                    Finally
                        If Not reader Is Nothing Then reader.Close()
                    End Try

                End If
                controls.Add(text)

                ' add the link
                qs = Me.QueryString
                If Me.IsCurrentStep AndAlso _clientID > 0 Then
                    ' all
                    link.Text = "All Service Users"
                    qs.Remove(QS_CLIENTID)
                    link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
                ElseIf _currentStep > Me.StepIndex Then
                    ' change
                    link.Text = "Change Service User"
                    qs.Remove(QS_CURRENTSTEP)
                    qs.Add(QS_CURRENTSTEP, Me.StepIndex)
                    link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
                End If
                If Not String.IsNullOrEmpty(link.Text) Then
                    link.Style.Add("float", "right")
                    controls.Add(link)
                End If

            End If

            msg = New ErrorMessage
            msg.Success = True

            Return msg

        End Function

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

            Dim msg As ErrorMessage = New ErrorMessage
            Dim clientList As ClientSelector = wizard.LoadControl("~/AbacusExtranet/Apps/UserControls/ClientSelector.ascx")
            clientList.showServiceOrderWithValidPeriod = showServiceOrderWithValidPeriod
            clientList.InitControl(wizard.Page, _establishmentID, _contractID, _clientID, _dateFrom, _dateTo, _viewClientBaseUrl, _mode)
            controls.Add(clientList)

            msg.Success = True
            Return msg

        End Function

        Private Sub ThrowInvalidMode()
            Throw New ArgumentOutOfRangeException("Mode", _mode, "Unrecognised Mode value.")
        End Sub

    End Class

#End Region

#Region " ResSUPaymentEnquiryResultsStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.Apps.UserControls.SelectorWizardSteps.ResSUPaymentEnquiryResultsStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Implements the residential service user payment enquiry results wizard step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MikeVO      25/09/2009  D11546 - changes to "selections" section links
    ''' 	[Mikevo]	13/09/2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Class ResSUPaymentEnquiryResultsStep
        Implements ISelectorWizardStep

        Const QS_ESTABLISHMENTID As String = "estabID"
        Const QS_CLIENTID As String = "clientID"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETO As String = "dateTo"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _establishmentID As Integer
        Private _clientID As Integer
        Private _dateFrom As Date
        Private _dateTo As Date
        Private _description As String = "The list below displays the results of your payment enquiry."

        Public Property BaseUrl() As String Implements ISelectorWizardStep.BaseUrl
            Get
                Return _baseUrl
            End Get
            Set(ByVal Value As String)
                _baseUrl = Value
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
                _establishmentID = Utils.ToInt32(_queryString(QS_ESTABLISHMENTID))
                _clientID = Utils.ToInt32(_queryString(QS_CLIENTID))
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETO)) Then _dateTo = _queryString(QS_DATETO)
            End Set
        End Property

        Public Property Required() As Boolean Implements ISelectorWizardStep.Required
            Get
                Return _required
            End Get
            Set(ByVal Value As Boolean)
                _required = Value
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

        Public Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep

        End Sub

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

        End Sub

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls
            Dim msg As ErrorMessage = New ErrorMessage
            Dim remittanceList As RemittanceDetailSelector = wizard.LoadControl("~/AbacusExtranet/Apps/UserControls/RemittanceDetailSelector.ascx")
            remittanceList.InitControl(wizard.Page, _establishmentID, _clientID, _dateFrom, _dateTo)
            controls.Add(remittanceList)
            msg.Success = True
            Return msg
        End Function

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage = New ErrorMessage
            msg.Success = True
            Return msg
        End Function

    End Class

#End Region

#Region " ResOccupancyEnquiryResultsStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.Apps.UserControls.SelectorWizardSteps.ResOccupancyEnquiryResultsStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Implements the residential Occupancy enquiry results wizard step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MikeVO  25/09/2009  D11546 - changes to "selections" section links
    ''' 	[Paul]	22/10/2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Class ResOccupancyEnquiryResultsStep
        Implements ISelectorWizardStep

        Const QS_ESTABLISHMENTID As String = "estabID"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETO As String = "dateTo"
        Const QS_MOVEMENT As String = "movement"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _establishmentID As Integer
        Private _dateFrom As Date
        Private _dateTo As Date
        Private _movement As Int16
        Private _description As String = "The list below displays the results of your occupancy enquiry."

        Public Property BaseUrl() As String Implements ISelectorWizardStep.BaseUrl
            Get
                Return _baseUrl
            End Get
            Set(ByVal Value As String)
                _baseUrl = Value
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
                _establishmentID = Utils.ToInt32(_queryString(QS_ESTABLISHMENTID))
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETO)) Then _dateTo = _queryString(QS_DATETO)
                _movement = Utils.ToInt32(_queryString(QS_MOVEMENT))
            End Set
        End Property

        Public Property Required() As Boolean Implements ISelectorWizardStep.Required
            Get
                Return _required
            End Get
            Set(ByVal Value As Boolean)
                _required = Value
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

        Public Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep

        End Sub

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

        End Sub

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls
            Dim msg As ErrorMessage = New ErrorMessage
            Dim occupancyList As OccupancySelector = wizard.LoadControl("~/AbacusExtranet/Apps/UserControls/OccupancySelector.ascx")
            occupancyList.InitControl(wizard.Page, _establishmentID, _dateFrom, _dateTo, _movement)
            controls.Add(occupancyList)
            msg.Success = True
            Return msg
        End Function

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage = New ErrorMessage
            msg.Success = True
            Return msg
        End Function

    End Class

#End Region

#Region " DateRangeMovementStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : UserControls.DateRangeMovementStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Class that implements the date range and movement step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MikeVO  25/09/2009  D11546 - changes to "selections" section links
    ''' 	[Paul]	12/10/2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DateRangeMovementStep
        Inherits DateRangeStep

        Const QS_MOVEMENT As String = "movement"
        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_ESTAB As String = "estabID"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer

        Private _movement As Int16
        Private _estabID As Int32
        Private _movementControl As RadioButtonList = New RadioButtonList
        Private _required As Boolean
        Private _description As String = "Please select a date range and movement to filter the results on."
        Private _currentStep As Integer

        Sub New()
            MyBase.new()
            MyBase.dateFrom = Date.Today
            MyBase.dateTo = Date.Today
            MyBase.ShowHeaderLink = False
        End Sub

        Public Overrides Property Description() As String
            Get
                Return _description
            End Get
            Set(ByVal Value As String)
                _description = Value
            End Set
        End Property

        Public Overrides Property QueryString() As System.Collections.Specialized.NameValueCollection
            Get
                Return MyBase.QueryString
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                MyBase.QueryString = Value
                _queryString = Value
                _movement = Target.Library.Utils.ToInt32(_queryString(QS_MOVEMENT))
                _estabID = Target.Library.Utils.ToInt32(_queryString(QS_ESTAB))
                _currentStep = Utils.ToInt32(_queryString(QS_CURRENTSTEP))
            End Set
        End Property

        Public Overrides ReadOnly Property BeforeNavigateJS() As String
            Get
                Return "DateRangeMovementStep_BeforeNavigate()"
            End Get
        End Property

        Public Overrides Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage

            Dim msg As ErrorMessage = New ErrorMessage
            Dim spacerBr As Literal

            MyBase.UseJquery = True
            msg = MyBase.RenderContentControls(wizard, controls)

            If Not msg.Success Then
                Return msg
            End If

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            With _movementControl
                .ID = "optMovement"
                .Items.Add("All")
                .Items.Add("Admissions Only")
                .Items.Add("Discharges Only")
                .Items.Add("Admissions and Discharges")
                .CssClass = "chkBoxStyle"
            End With
            controls.Add(_movementControl)

            _movementControl.Items(_movement).Selected = True

            msg.Success = True
            Return msg

        End Function

        Public Overrides Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage

            Dim msg As ErrorMessage
            Dim spacerBr As Literal
            Dim lbl As Label = New Label
            Dim text As Label = New Label
            Dim link As HyperLink = New HyperLink
            Dim qs As NameValueCollection

            'if a home has been selected show the date range link
            If _estabID <> 0 Then
                MyBase.ShowHeaderLink = True
            End If

            msg = MyBase.RenderHeaderControls(controls)
            If Not msg.Success Then
                Return msg
            End If

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            ' label
            lbl.Text = "Movement"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", "8em")
            controls.Add(lbl)

            text.Style.Add("float", "left")
            Select Case _movement
                Case 0
                    text.Text = "All"
                Case 1
                    text.Text = "Admissions Only"
                Case 2
                    text.Text = "Discharges Only"
                Case 3
                    text.Text = "Admissions and Discharges"
            End Select
            controls.Add(text)

            ' add the link
            qs = Me.QueryString
            If Me.IsCurrentStep AndAlso _movement <> 0 Then
                ' all
                link.Text = "All Movement"
                qs.Remove(QS_MOVEMENT)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change Movement"
                qs.Remove(QS_CURRENTSTEP)
                qs.Add(QS_CURRENTSTEP, MyBase.StepIndex)
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

        Public Overrides ReadOnly Property Title() As String
            Get
                Return "Select a Date Range and Movement"
            End Get
        End Property

        Public Overrides Sub PreRender(ByVal wizard As SelectorWizardBase)

            wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.SP.Apps.UserControls.SelectorWizard.DateRangeMovementStep.Startup", _
                Target.Library.Web.Utils.WrapClientScript( _
                    String.Format("SelectorWizard_dateFromID=""{0}"";SelectorWizard_dateToID=""{1}"";DateRangeStep_required={2};SelectorWizard_movementID=""{3}"";", _
                        MyBase.DateFromControl.ClientID, MyBase.DateToControl.ClientID, MyBase.Required.ToString().ToLower(), _movementControl.ClientID) _
                ) _
            )

        End Sub

        Public Overrides Sub InitStep(ByVal wizard As SelectorWizardBase)
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/UserControls/DateRangeMovementStep.js"))
        End Sub

    End Class

#End Region

#Region " DomContractStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.SelectorWizardSteps.DomContractStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Implements the select domiciliary contract step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MikeVO      25/09/2009  D11546 - changes to "selections" section links
    '''     MikeVO      24/11/2008  A4WA#5029 - security fixes
    ''' 	[Mikevo]	16/01/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Class DomContractStep
        Implements ISelectorWizardStep

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_ESTABLISHMENTID As String = "estabID"
        Const QS_CONTRACTTYPEID As String = "ctID"
        Const QS_CONTRACTGROUPID As String = "cgID"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETO As String = "dateTo"
        Public Const QS_CONTRACTID As String = "contractID"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _title As String = "Select a Contract"
        Private _description As String = "The list below displays the domiciliary contracts that match your search criteria."
        Private _estabID As Integer
        Private _contractType As DomContractType
        Private _contractGroupID As Integer
        Private _dateFrom As Date
        Private _dateTo As Date
        Private _showHeaderControls As Boolean
        Private _showNewButton As Boolean
        Private _showViewButton As Boolean
        Private _contractID As Integer
        Private _headerLabelWidth As Unit = New Unit(10, UnitType.Em)
        Private _currentStep As Integer

        Sub New()
            _showHeaderControls = False
            _showNewButton = True
            _showViewButton = True
        End Sub

        Public Property ShowHeaderControls() As Boolean
            Get
                Return _showHeaderControls
            End Get
            Set(ByVal value As Boolean)
                _showHeaderControls = value
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
                _estabID = Utils.ToInt32(_queryString(QS_ESTABLISHMENTID))
                _contractType = Utils.ToInt32(_queryString(QS_CONTRACTTYPEID))
                _contractGroupID = Utils.ToInt32(_queryString(QS_CONTRACTGROUPID))
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETO)) Then _dateTo = _queryString(QS_DATETO)
                _contractID = Utils.ToInt32(_queryString(QS_CONTRACTID))
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
                Return _title
            End Get
        End Property

        Public Sub SetTitle(ByVal value As String)
            _title = value
        End Sub

        Public ReadOnly Property BeforeNavigateJS() As String Implements ISelectorWizardStep.BeforeNavigateJS
            Get
                Return "DomContractStep_BeforeNavigate()"
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
              String.Format("DomContractStep_required={0};", _required.ToString().ToLower()) _
            ))
        End Sub

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls

            Dim msg As ErrorMessage = New ErrorMessage
            Dim qs As NameValueCollection

            If _showHeaderControls Then

                Dim lbl As Label = New Label
                Dim text As Label = New Label
                Dim link As HyperLink = New HyperLink
                Dim contract As DomContract

                ' label
                lbl.Text = "Contract"
                lbl.Style.Add("float", "left")
                lbl.Style.Add("font-weight", "bold")
                lbl.Style.Add("width", _headerLabelWidth.ToString())
                controls.Add(lbl)

                text.Style.Add("float", "left")
                If _contractID = 0 Then
                    text.Text = "All"
                Else
                    ' ensure user is permitted to view the contract
                    Dim canView As Boolean
                    msg = DomContractBL.UserCanViewContract(Me.DbConnection, SecurityBL.GetCurrentUser().ExternalUserID, _contractID, canView)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    If Not canView Then
                        HttpContext.Current().Response.Redirect("~/Library/Errors/404.aspx")
                    End If

                    contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
                    msg = contract.Fetch(_contractID)
                    If Not msg.Success Then Return msg
                    text.Text = String.Format("{0}: {1}", contract.Number, contract.Title)
                End If
                controls.Add(text)

                ' add the link
                qs = Me.QueryString
                If Me.IsCurrentStep AndAlso _contractID > 0 Then
                    ' all
                    link.Text = "All Contracts"
                    qs.Remove(QS_CONTRACTID)
                    link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
                ElseIf _currentStep > Me.StepIndex Then
                    ' change
                    link.Text = "Change Contract"
                    qs.Remove(QS_CURRENTSTEP)
                    qs.Add(QS_CURRENTSTEP, Me.StepIndex)
                    link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
                End If
                If Not String.IsNullOrEmpty(link.Text) Then
                    link.Style.Add("float", "right")
                    controls.Add(link)
                End If

            End If

            msg.Success = True
            Return msg

        End Function

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls
            Dim msg As ErrorMessage = New ErrorMessage
            Dim contractList As DomContractSelector = wizard.LoadControl("~/AbacusExtranet/Apps/UserControls/DomContractSelector.ascx")
            contractList.thePage = wizard.Page
            contractList.establishmentID = _estabID
            contractList.contractType = _contractType
            contractList.contractGroupID = _contractGroupID
            contractList.dateFrom = _dateFrom
            contractList.dateTo = _dateTo
            contractList.showNewButton = _showNewButton
            contractList.showViewButton = _showViewButton
            contractList.selectedContractID = _contractID
            controls.Add(contractList)
            msg.Success = True
            Return msg
        End Function

    End Class

#End Region

#Region " ServiceDeliveryFileFilterStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : UserControls.ServiceDeliveryFileFilterStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Class that implements the date range and status and submitted by filter step
    '''     for service delivery file enquiry.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MikeVO  25/09/2009  D11546 - changes to "selections" section links
    ''' 	[PaulW]	14/02/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ServiceDeliveryFileFilterStep
        Inherits DateRangeStep

        Const QS_USERID As String = "subBy"
        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_DELETED As String = "del"
        Const QS_PROCESSED As String = "proc"
        Const QS_AWAITING As String = "ap"
        Const QS_WORKINPROGRESS As String = "wip"
        Const QS_FAILED As String = "failed"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer

        Private _processedControl As CheckBoxEx = New CheckBoxEx
        Private _wipControl As CheckBoxEx = New CheckBoxEx
        Private _awaitingControl As CheckBoxEx = New CheckBoxEx
        Private _deletedControl As CheckBoxEx = New CheckBoxEx
        Private _failedControl As CheckBoxEx = New CheckBoxEx

        Private _submittedBy As DropDownListEx = New DropDownListEx
        Private _userID As Integer
        Private _required As Boolean
        Private _deleted As Boolean
        Private _processed As Boolean
        Private _awaitingProcessing As Boolean
        Private _workInProgress As Boolean
        Private _failed As Boolean
        Private _description As String = "Please select a date range and status to filter the results on."
        Private _currentStep As Integer

        Sub New()
            MyBase.new()
            MyBase.DateFrom = Date.MinValue
            MyBase.DateTo = Date.MaxValue
            MyBase.ShowHeaderLink = False
            MyBase.HeaderLabelWidth = New Unit(8, UnitType.Em)
            _processed = True
            _awaitingProcessing = True
            _workInProgress = True
            _failed = True
        End Sub

        Public Overrides Property Description() As String
            Get
                Return _description
            End Get
            Set(ByVal Value As String)
                _description = Value
            End Set
        End Property

        Public Overrides Property QueryString() As System.Collections.Specialized.NameValueCollection
            Get
                Return New NameValueCollection(_queryString)
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                MyBase.QueryString = Value
                _queryString = Value
                _userID = Utils.ToInt32(_queryString(QS_USERID))
                _deleted = Utils.ConvertXSBoolean(_queryString(QS_DELETED))
                If _queryString(QS_PROCESSED) <> "" Then
                    _processed = Utils.ConvertXSBoolean(_queryString(QS_PROCESSED))
                End If
                If _queryString(QS_AWAITING) <> "" Then
                    _awaitingProcessing = Utils.ConvertXSBoolean(_queryString(QS_AWAITING))
                End If
                If _queryString(QS_WORKINPROGRESS) <> "" Then
                    _workInProgress = Utils.ConvertXSBoolean(_queryString(QS_WORKINPROGRESS))
                End If
                If _queryString(QS_FAILED) <> "" Then
                    _failed = Utils.ConvertXSBoolean(_queryString(QS_FAILED))
                End If
                _currentStep = Utils.ToInt32(_queryString(QS_CURRENTSTEP))
            End Set
        End Property

        Public Overrides ReadOnly Property BeforeNavigateJS() As String
            Get
                Return "ServiceDeliveryFileFilterStep_BeforeNavigate()"
            End Get
        End Property

        Public Overrides Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage

            Const SP_FETCH_SUBMITTEDBY_LIST As String = "spxServiceDeliveryFile_FetchSubmittedByList"

            Dim msg As ErrorMessage = New ErrorMessage
            Dim spacerBr As Literal
            Dim userList As DataClasses.Collections.UsersCollection = Nothing
            Dim item As ListItem = New ListItem
            Dim spParams As SqlParameter()
            Dim reader As SqlDataReader = Nothing
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            'Add the Submitted by drop down list
            With _submittedBy
                .ID = "cboSubmittedBy"
                .LabelText = "Submitted By"
                .LabelWidth = "10em"
                .LabelBold = True
            End With
            controls.Add(_submittedBy)

            ' load submitted by combo
            Try
                spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_FETCH_SUBMITTEDBY_LIST, False)
                spParams(0).Value = currentUser.ExternalUserID
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_FETCH_SUBMITTEDBY_LIST, spParams)

                With _submittedBy.DropDownList
                    .DataSource = reader
                    .DataValueField = "FileUploadedByID"
                    .DataTextField = "FileUploadedBy"
                    .DataBind()
                    ' add blank entry
                    item.Value = 0
                    item.Text = ""
                    .Items.Insert(0, item)
                    .SelectedValue = _userID
                End With

                msg = New ErrorMessage()
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
            Finally
                SqlHelper.CloseReader(reader)
            End Try
            If Not msg.Success Then
                Return msg
            End If

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            'Add controls from the base class, basically the date controls
            MyBase.UseJquery = True
            msg = MyBase.RenderContentControls(wizard, controls, False)
            If Not msg.Success Then
                Return msg
            End If

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            'Add the checkbox controls for status
            With _awaitingControl
                .ID = "chkAwaiting"
                .Text = "Awaiting Processing"
                .CheckBoxCssClass = "chkBoxStyle"
                .CheckBox.TextAlign = TextAlign.Right
                controls.Add(_awaitingControl)
            End With
            With _wipControl
                .ID = "chkWip"
                .Text = "Work in Progress"
                .CheckBoxCssClass = "chkBoxStyle"
                .CheckBox.TextAlign = TextAlign.Right
                controls.Add(_wipControl)
            End With
            With _processedControl
                .ID = "chkProcessed"
                .Text = "Processed"
                .CheckBoxCssClass = "chkBoxStyle"
                .CheckBox.TextAlign = TextAlign.Right
                controls.Add(_processedControl)
            End With
            With _deletedControl
                .ID = "chkDeleted"
                .Text = "Deleted"
                .CheckBoxCssClass = "chkBoxStyle"
                .CheckBox.TextAlign = TextAlign.Right
                controls.Add(_deletedControl)
            End With
            With _failedControl
                .ID = "chkFailed"
                .Text = "Failed"
                .CheckBoxCssClass = "chkBoxStyle"
                .CheckBox.TextAlign = TextAlign.Right
                controls.Add(_failedControl)
            End With

            If _processed = True Then
                _processedControl.CheckBox.Checked = True
            End If
            If _workInProgress = True Then
                _wipControl.CheckBox.Checked = True
            End If
            If _awaitingProcessing = True Then
                _awaitingControl.CheckBox.Checked = True
            End If
            If _deleted = True Then
                _deletedControl.CheckBox.Checked = True
            End If
            If _failed = True Then
                _failedControl.CheckBox.Checked = True
            End If

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)
            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            msg.Success = True
            Return msg

        End Function

        Public Overrides Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage
            Dim msg As ErrorMessage
            Dim spacerBr As Literal
            Dim lbl As Label = New Label
            Dim lbl2 As Label = New Label
            Dim text As Label = New Label
            Dim submittedText As Label = New Label
            Dim link As HyperLink = New HyperLink
            Dim link2 As HyperLink = New HyperLink
            Dim qs As NameValueCollection

            MyBase.ShowHeaderLink = True

            msg = MyBase.RenderHeaderControls(controls)
            If Not msg.Success Then
                Return msg
            End If

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            ' label
            lbl2.Text = "Status"
            lbl2.Style.Add("float", "left")
            lbl2.Style.Add("font-weight", "bold")
            lbl2.Style.Add("width", "8em")
            controls.Add(lbl2)

            text.Style.Add("float", "left")
            If _awaitingProcessing = True Then
                text.Text = String.Format("{0}{1}", text.Text, "Awaiting Processing")
            End If
            If _workInProgress = True Then
                If text.Text <> "" Then text.Text = String.Format("{0}{1}", text.Text, ", ")
                text.Text = String.Format("{0}{1}", text.Text, "Work in Progress")
            End If
            If _processed = True Then
                If text.Text <> "" Then text.Text = String.Format("{0}{1}", text.Text, ", ")
                text.Text = String.Format("{0}{1}", text.Text, "Processed")
            End If
            If _deleted = True Then
                If text.Text <> "" Then text.Text = String.Format("{0}{1}", text.Text, ", ")
                text.Text = String.Format("{0}{1}", text.Text, "Deleted")
            End If
            If _failed = True Then
                If text.Text <> "" Then text.Text = String.Format("{0}{1}", text.Text, ", ")
                text.Text = String.Format("{0}{1}", text.Text, "Failed")
            End If
            'remove any leading spaces we may have
            text.Text = Trim(text.Text)
            controls.Add(text)

            ' add the link
            qs = Me.QueryString
            If Me.IsCurrentStep Then
                ' all
                link.Text = "Default Status Values"
                qs.Remove(QS_AWAITING)
                qs.Remove(QS_DELETED)
                qs.Remove(QS_PROCESSED)
                qs.Remove(QS_WORKINPROGRESS)
                qs.Remove(QS_FAILED)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change Status"
                qs.Remove(QS_CURRENTSTEP)
                qs.Add(QS_CURRENTSTEP, MyBase.StepIndex)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            End If
            If Not String.IsNullOrEmpty(link.Text) Then
                link.Style.Add("float", "right")
                controls.Add(link)
            End If

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            lbl.Text = "Submitted By"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", "8em")
            controls.Add(lbl)

            If _userID > 0 Then
                Dim user As WebSecurityUser = New WebSecurityUser(DbConnection)
                msg = user.Fetch(_userID)
                If msg.Success Then
                    submittedText.Text = String.Format("{0} {1}", user.FirstName, user.Surname)
                    controls.Add(submittedText)
                End If
            End If

            spacerBr = New Literal
            spacerBr.Text = "<div class=""clearer"" />"
            controls.Add(spacerBr)

            ' add the link
            qs = Me.QueryString
            If Me.IsCurrentStep AndAlso _userID > 0 Then
                ' all
                link2.Text = "All Submitted By"
                qs.Remove(QS_USERID)
                link2.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link2.Text = "Change Submitted By"
                qs.Remove(QS_CURRENTSTEP)
                qs.Add(QS_CURRENTSTEP, MyBase.StepIndex)
                link2.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            End If
            If Not String.IsNullOrEmpty(link2.Text) Then
                link2.Style.Add("float", "right")
                controls.Add(link2)
            End If

            msg = New ErrorMessage
            msg.Success = True

            Return msg

        End Function

        Public Overrides ReadOnly Property Title() As String
            Get
                Return "Select a Date Range and Status"
            End Get
        End Property

        Public Overrides Sub PreRender(ByVal wizard As SelectorWizardBase)

            wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.SP.Apps.UserControls.SelectorWizard.ServiceDeliveryFileFilterStep.Startup", _
                Target.Library.Web.Utils.WrapClientScript( _
                    String.Format("SelectorWizard_dateFromID=""{0}"";SelectorWizard_dateToID=""{1}"";DateRangeStep_required={2};SelectorWizard_SubmittedBy=""{3}"";", _
                        MyBase.DateFromControl.ClientID, MyBase.DateToControl.ClientID, MyBase.Required.ToString().ToLower(), _submittedBy.ClientID) _
                ) _
            )

        End Sub

        Public Overrides Sub InitStep(ByVal wizard As SelectorWizardBase)
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet\Apps\UserControls\ServiceDeliveryFileFilter.js"))
            CType(wizard.Page, Target.Web.Apps.BasePage).AddExtraCssStyle(".chkBoxStyle { float:left; margin-right:2em; }")
        End Sub

    End Class

#End Region

#Region " ServiceDeliveryFileEnquiryResultsStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.Apps.UserControls.SelectorWizardSteps.ServiceDeliveryFileEnquiryResultsStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Implements the service delivery file enquiry results wizard step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     ColinD  05/04/2010  D11755 - implement ability to filter by 'Failed' uploads
    '''     MikeVO  15/10/2009  D11546 - (actually implement!!!) and support changes to SubmittedBy filter
    '''     MikeVO  25/09/2009  D11546 - changes to "selections" section links
    '''     MikeVO  24/11/2008  A4WA#5033 - security fixes.
    ''' 	[PaulW]	14/02/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Class ServiceDeliveryFileEnquiryResultsStep
        Implements ISelectorWizardStep

        Const QS_USERID As String = "subBy"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETO As String = "dateTo"
        Const QS_DELETED As String = "del"
        Const QS_PROCESSED As String = "proc"
        Const QS_AWAITING As String = "ap"
        Const QS_WORKINPROGRESS As String = "wip"
        Const QS_FAILED As String = "failed"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _userID As Integer
        Private _dateFrom As Date
        Private _dateTo As Date
        Private _deleted As Boolean
        Private _processed As Boolean
        Private _awaitingProcessing As Boolean
        Private _workInProgress As Boolean
        Private _failed As Boolean
        Private _description As String = "The list below displays the results of your Service Delivery File enquiry."
        Private _userHasUploadServiceFileCommand As Boolean

        Public WriteOnly Property UserHasUploadServiceFileCommand() As Boolean
            Set(ByVal value As Boolean)
                _userHasUploadServiceFileCommand = value
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

        Public ReadOnly Property BeforeNavigateJS() As String Implements ISelectorWizardStep.BeforeNavigateJS
            Get
                Return String.Empty
            End Get
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
                _userID = Utils.ToInt32(_queryString(QS_USERID))
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETO)) Then _dateTo = _queryString(QS_DATETO)
                _deleted = Utils.ConvertXSBoolean(_queryString(QS_DELETED))
                _processed = Utils.ConvertXSBoolean(_queryString(QS_PROCESSED))
                _awaitingProcessing = Utils.ConvertXSBoolean(_queryString(QS_AWAITING))
                _workInProgress = Utils.ConvertXSBoolean(_queryString(QS_WORKINPROGRESS))
                _failed = Utils.ConvertXSBoolean(_queryString(QS_FAILED))
            End Set
        End Property

        Public Property Required() As Boolean Implements ISelectorWizardStep.Required
            Get
                Return _required
            End Get
            Set(ByVal Value As Boolean)
                _required = Value
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

        Public Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep

        End Sub

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

        End Sub

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls
            Dim msg As ErrorMessage = New ErrorMessage
            Dim serviceDeliveryfileList As ServicedeliveryFileSelector = wizard.LoadControl("~/AbacusExtranet/Apps/UserControls/ServiceDeliveryFileSelector.ascx")
            serviceDeliveryfileList.InitControl(wizard.Page, _userID, _dateFrom, _dateTo, _deleted, _processed, _awaitingProcessing, _workInProgress, _failed, _userHasUploadServiceFileCommand)
            controls.Add(serviceDeliveryfileList)
            msg.Success = True
            Return msg
        End Function

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage = New ErrorMessage
            msg.Success = True
            Return msg
        End Function


    End Class

#End Region

#Region " DomProformaInvoiceBatchFilterStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : UserControls.DomProformaInvoiceBatchFilterStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Class that implements batch type, status and status date range filters
    ''' for domicilary proforma invoice batch enquiries.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' ColinD  29/03/2010  A4W6156 - changes to "selections" section links if a file id is passed in query string
    ''' MikeVO  25/09/2009  D11546 - changes to "selections" section links
    ''' MvO  29/02/2008  Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DomProformaInvoiceBatchFilterStep
        Inherits DateRangeStep

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_BATCH_TYPE As String = "batchType"
        Const QS_BATCH_STATUS As String = "batchStatus"

        Const CTRL_BATCH_TYPE As String = "chkBatchType"
        Const CTRL_BATCH_STATUS As String = "chkBatchStatus"

        Private _dbConnection As SqlConnection
        Private _currentStep As Integer
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _deleted As Boolean
        Private _description As String = "Please select the batch type/status values and week ending date range to filter the results on."
        Private _tickedBatchType As DomProformaInvoiceBatchType
        Private _tickedBatchStatus As DomProformaInvoiceBatchStatus
        Private _visibleBatchType As DomProformaInvoiceBatchType
        Private _visibleBatchStatus As DomProformaInvoiceBatchStatus
        Private _batchTypeCheckboxes As List(Of CheckBoxEx), _batchStatusCheckboxes As List(Of CheckBoxEx)

        Sub New()
            MyBase.New()
            MyBase.DateFrom = Date.MinValue
            MyBase.DateTo = Date.MaxValue
            MyBase.ShowHeaderLink = False
            MyBase.HeaderLabelWidth = New Unit(8, UnitType.Em)
            ' by default, all type and status checkboxes are visible
            For Each t As DomProformaInvoiceBatchType In [Enum].GetValues(GetType(DomProformaInvoiceBatchType))
                Me.VisibleBatchType += t
            Next
            For Each s As DomProformaInvoiceBatchStatus In [Enum].GetValues(GetType(DomProformaInvoiceBatchStatus))
                Me.VisibleBatchStatus += s
            Next
            _batchTypeCheckboxes = New List(Of CheckBoxEx)
            _batchStatusCheckboxes = New List(Of CheckBoxEx)
        End Sub

        Public Property TickedBatchType() As DomProformaInvoiceBatchType
            Get
                Return _tickedBatchType
            End Get
            Set(ByVal value As DomProformaInvoiceBatchType)
                _tickedBatchType = value
            End Set
        End Property

        Public Property TickedBatchStatus() As DomProformaInvoiceBatchStatus
            Get
                Return _tickedBatchStatus
            End Get
            Set(ByVal value As DomProformaInvoiceBatchStatus)
                _tickedBatchStatus = value
            End Set
        End Property

        Public Property VisibleBatchType() As DomProformaInvoiceBatchType
            Get
                Return _visibleBatchType
            End Get
            Set(ByVal value As DomProformaInvoiceBatchType)
                _visibleBatchType = value
            End Set
        End Property

        Public Property VisibleBatchStatus() As DomProformaInvoiceBatchStatus
            Get
                Return _visibleBatchStatus
            End Get
            Set(ByVal value As DomProformaInvoiceBatchStatus)
                _visibleBatchStatus = value
            End Set
        End Property

        Public Overrides Property Description() As String
            Get
                Return _description
            End Get
            Set(ByVal Value As String)
                _description = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets File ID from the query string if one exists
        ''' </summary>
        ''' <value>File ID from query string</value>
        ''' <returns>File ID from query string</returns>
        ''' <remarks></remarks>
        Private ReadOnly Property FileID() As Nullable(Of Integer)
            Get
                Dim testFileID As Integer = -1

                If Not IsNothing(QueryString("fileid")) AndAlso QueryString("fileid").Trim().Length > 0 AndAlso Integer.TryParse(QueryString("fileid"), testFileID) Then

                    Return testFileID

                Else

                    Return Nothing

                End If
            End Get
        End Property

        Public Overrides Property QueryString() As System.Collections.Specialized.NameValueCollection
            Get
                Return New NameValueCollection(_queryString)
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                MyBase.QueryString = Value
                _queryString = Value
                _currentStep = Utils.ToInt32(_queryString(QS_CURRENTSTEP))
                If Utils.ToInt32(_queryString(QS_BATCH_TYPE)) > 0 Then _tickedBatchType = Utils.ToInt32(_queryString(QS_BATCH_TYPE))
                If Utils.ToInt32(_queryString(QS_BATCH_STATUS)) > 0 Then _tickedBatchStatus = Utils.ToInt32(_queryString(QS_BATCH_STATUS))
            End Set
        End Property

        Public Overrides ReadOnly Property BeforeNavigateJS() As String
            Get
                Return "DomProformaInvoiceBatchFilterStep_BeforeNavigate()"
            End Get
        End Property

        Public Overrides Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage

            Dim msg As ErrorMessage = New ErrorMessage
            Dim spacerBr As Literal
            Dim fs As HtmlGenericControl
            Dim legend As HtmlGenericControl

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            ' batch type
            fs = New HtmlGenericControl("FIELDSET")
            legend = New HtmlGenericControl("LEGEND")
            legend.InnerText = "Batch Type"
            fs.Controls.Add(legend)
            For Each t As DomProformaInvoiceBatchType In [Enum].GetValues(GetType(DomProformaInvoiceBatchType))
                If t And Me.VisibleBatchType Then
                    Dim chk As CheckBoxEx = New CheckBoxEx()
                    With chk
                        .ID = String.Format("{0}{1}", CTRL_BATCH_TYPE, Convert.ToInt32(t))
                        .Text = Utils.SplitOnCapitals([Enum].GetName(GetType(DomProformaInvoiceBatchType), t))
                        .checkBoxCssClass = "chkBoxStyle"
                        .CheckBox.TextAlign = TextAlign.Right
                        'If t And Me.TickedBatchType Then .CheckBox.Checked = True
                        .CheckBox.Checked = True
                        If FileID.HasValue Then chk.IsReadOnly = True
                    End With
                    fs.Controls.Add(chk)
                    _batchTypeCheckboxes.Add(chk)
                End If
            Next
            controls.Add(fs)

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            ' status
            fs = New HtmlGenericControl("FIELDSET")
            legend = New HtmlGenericControl("LEGEND")
            legend.InnerText = "Batch Status"
            fs.Controls.Add(legend)
            For Each s As DomProformaInvoiceBatchStatus In [Enum].GetValues(GetType(DomProformaInvoiceBatchStatus))
                If s And Me.VisibleBatchStatus Then
                    Dim chk As CheckBoxEx = New CheckBoxEx()
                    With chk
                        .ID = String.Format("{0}{1}", CTRL_BATCH_STATUS, Convert.ToInt32(s))
                        .Text = Utils.SplitOnCapitals([Enum].GetName(GetType(DomProformaInvoiceBatchStatus), s))
                        .checkBoxCssClass = "chkBoxStyle"
                        .CheckBox.TextAlign = TextAlign.Right
                        If s And Me.TickedBatchStatus Then .CheckBox.Checked = True
                    End With
                    fs.Controls.Add(chk)
                    _batchStatusCheckboxes.Add(chk)
                End If
            Next
            controls.Add(fs)

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            ' week ending
            fs = New HtmlGenericControl("FIELDSET")
            legend = New HtmlGenericControl("LEGEND")
            legend.InnerText = "Week Ending"
            fs.Controls.Add(legend)
            ' add controls from the base class, basically the date controls
            msg = MyBase.RenderContentControls(wizard, fs.Controls, False)
            If Not msg.Success Then
                Return msg
            End If
            controls.Add(fs)

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            msg.Success = True
            Return msg

        End Function

        Public Overrides Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage

            Dim msg As ErrorMessage
            Dim spacerBr As Literal
            Dim lbl As Label
            Dim text As Label
            Dim link As HyperLink
            Dim qs As NameValueCollection

            MyBase.ShowHeaderLink = True

            ' batch type
            lbl = New Label()
            lbl.Text = "Batch Type"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", "8em")
            controls.Add(lbl)
            ' add the list of ticked types
            text = New Label()
            text.Style.Add("float", "left")

            If FileID.HasValue = False Then

                For Each t As DomProformaInvoiceBatchType In [Enum].GetValues(GetType(DomProformaInvoiceBatchType))
                    If t And Me.TickedBatchType Then
                        text.Text = String.Format("{0}, {1}", text.Text, Utils.SplitOnCapitals([Enum].GetName(GetType(DomProformaInvoiceBatchType), t)))
                    End If
                Next

                ' remove leading ", "
                If text.Text.Length > 0 Then text.Text = text.Text.Substring(2)

            Else

                Dim interfaceFile As DataClasses.DomProviderInterfaceFile = New DataClasses.DomProviderInterfaceFile(Me.DbConnection)
                Dim interfaceFileMsg As ErrorMessage

                interfaceFileMsg = interfaceFile.Fetch(FileID.Value)

                If interfaceFileMsg.Success Then

                    text.Text = String.Format("Interface File ({0} : {1})", interfaceFile.Reference, interfaceFile.Description)

                Else

                    text.Text = "Interface File (Invalid identifier provided)"

                End If

            End If
            
            controls.Add(text)

            ' add the link
            link = New HyperLink()
            qs = Me.QueryString
            If Me.IsCurrentStep Then
                ' all
                link.Text = "Default Batch Types"
                qs.Remove(QS_BATCH_TYPE)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change Batch Type"
                qs.Remove(QS_CURRENTSTEP)
                qs.Add(QS_CURRENTSTEP, MyBase.StepIndex)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            End If
            If Not String.IsNullOrEmpty(link.Text) Then
                link.Style.Add("float", "right")
                controls.Add(link)
            End If

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            ' batch status
            lbl = New Label()
            lbl.Text = "Batch Status"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", "8em")
            controls.Add(lbl)
            ' add the list of ticked status values
            text = New Label()
            text.Style.Add("float", "left")
            For Each s As DomProformaInvoiceBatchStatus In [Enum].GetValues(GetType(DomProformaInvoiceBatchStatus))
                If s And Me.TickedBatchStatus Then
                    text.Text = String.Format("{0}, {1}", text.Text, Utils.SplitOnCapitals([Enum].GetName(GetType(DomProformaInvoiceBatchStatus), s)))
                End If
            Next
            ' remove leading ", "
            If text.Text.Length > 0 Then text.Text = text.Text.Substring(2)
            controls.Add(text)

            ' add the link
            link = New HyperLink()
            qs = Me.QueryString
            If Me.IsCurrentStep Then
                ' all
                link.Text = "Default Batch Status Values"
                qs.Remove(QS_BATCH_STATUS)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change Batch Status"
                qs.Remove(QS_CURRENTSTEP)
                qs.Add(QS_CURRENTSTEP, MyBase.StepIndex)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            End If
            If Not String.IsNullOrEmpty(link.Text) Then
                link.Style.Add("float", "right")
                controls.Add(link)
            End If

            ' add date controls from base class
            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            msg = MyBase.RenderHeaderControls(controls)
            If Not msg.Success Then
                Return msg
            End If

            msg = New ErrorMessage
            msg.Success = True

            Return msg

        End Function

        Public Overrides ReadOnly Property Title() As String
            Get
                Return "Select a Batch Type, Status and Status Date Range"
            End Get
        End Property

        Public Overrides Sub PreRender(ByVal wizard As SelectorWizardBase)

            Dim script As StringBuilder
            Dim scriptString As String

            script = New StringBuilder()
            script.AppendFormat("SelectorWizard_id=""{0}"";", wizard.ClientID)
            script.AppendFormat("SelectorWizard_dateFromID=""{0}"";", MyBase.DateFromControl.ClientID)
            script.AppendFormat("SelectorWizard_dateToID=""{0}"";", MyBase.DateToControl.ClientID)
            script.AppendFormat("DateRangeStep_required={0};", MyBase.Required.ToString().ToLower())
            script.AppendFormat("DomProformaInvoiceBatchFilterStep_batchTypePrefix=""{0}"";", CTRL_BATCH_TYPE)
            script.AppendFormat("DomProformaInvoiceBatchFilterStep_batchStatusPrefix=""{0}"";", CTRL_BATCH_STATUS)
            wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", script.ToString(), True)

            ' output array of batch type checkbox IDs
            script = New StringBuilder()
            For Each chk As CheckBoxEx In _batchTypeCheckboxes
                script.AppendFormat("""{0}"",", chk.ClientID)
            Next
            scriptString = script.ToString()
            If scriptString.Length > 0 Then scriptString = scriptString.Substring(0, scriptString.Length - 1)
            wizard.Page.ClientScript.RegisterArrayDeclaration("DomProformaInvoiceBatchFilterStep_batchTypeIDs", scriptString)

            ' output array of batch status checkbox IDs
            script = New StringBuilder()
            For Each chk As CheckBoxEx In _batchStatusCheckboxes
                script.AppendFormat("'{0}',", chk.ClientID)
            Next
            scriptString = script.ToString()
            If scriptString.Length > 0 Then scriptString = scriptString.Substring(0, scriptString.Length - 1)
            wizard.Page.ClientScript.RegisterArrayDeclaration("DomProformaInvoiceBatchFilterStep_batchStatusIDs", scriptString)

        End Sub

        Public Overrides Sub InitStep(ByVal wizard As SelectorWizardBase)
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/UserControls/DomProformaInvoiceBatchFilterStep.js"))
            CType(wizard.Page, Target.Web.Apps.BasePage).AddExtraCssStyle(".chkBoxStyle { float:left; margin-right:2em; }")
        End Sub

    End Class

#End Region

#Region " DomProviderInvoiceFilterStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : UserControls.DomProviderInvoiceFilterStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Class that implements Invoice Number, Weekending date range, status and 
    ''' status date range filters for domicilary  provider invoice enquiries.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' MikeVO  25/09/2009  D11546 - changes to "selections" section links
    ''' Paul  08/04/2008  Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DomProviderInvoiceFilterStep
        Inherits DateRangeStep

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_INVOICE_NUMBER As String = "invoiceNumber"
        Const QS_WEFROM As String = "weFrom"
        Const QS_WETO As String = "weTo"
        Const QS_PAID As String = "paid"
        Const QS_UNPAID As String = "unPaid"
        Const QS_AUTHORISED As String = "authorised"
        Const QS_SUSPENDED As String = "suspended"
        Const QS_RETRACTION As String = "retraction"

        Private _dbConnection As SqlConnection
        Private _currentStep As Integer
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _deleted As Boolean
        Private _weFrom As Date
        Private _weTo As Date
        Private _invoiceNumber As String
        Private _unpaid As Boolean
        Private _paid As Boolean
        Private _authorised As Boolean
        Private _suspended As Boolean
        Private _description As String = "Please select the criteria to filter the results on."
        Private _tickedInvoiceStatus As DomProformaInvoiceBatchStatus
        Private _visibleInvoiceStatus As DomProformaInvoiceBatchStatus
        Private _invoiceStatusCheckboxes As List(Of CheckBoxEx)
        Private _weFromControl As TextBoxEx = New TextBoxEx
        Private _weToControl As TextBoxEx = New TextBoxEx
        Private _invoiceNumberControl As TextBoxEx = New TextBoxEx
        Private _unPaidControl As CheckBoxEx = New CheckBoxEx
        Private _paidControl As CheckBoxEx = New CheckBoxEx
        Private _authorisedControl As CheckBoxEx = New CheckBoxEx
        Private _suspendedControl As CheckBoxEx = New CheckBoxEx
        Private _headerLabelWidth As Unit = New Unit(10, UnitType.Em)
        Private _HideRetractedAndRetractionInvoicesControl As CheckBox = New CheckBox
        Private _retraction As Boolean

        Sub New()
            MyBase.New()
            MyBase.DateFrom = Date.MinValue
            MyBase.DateTo = Date.MaxValue
            MyBase.ShowHeaderLink = False
            MyBase.HeaderLabelWidth = _headerLabelWidth

            ' by default, all status checkboxes are visible and selected
            _paid = True
            _unpaid = True
            _authorised = True
            _suspended = True

        End Sub


        Public Overrides Property HeaderLabelWidth() As Unit
            Get
                Return _headerLabelWidth
            End Get
            Set(ByVal value As Unit)
                _headerLabelWidth = value
                MyBase.HeaderLabelWidth = value
            End Set
        End Property

        Public Overrides Property Description() As String
            Get
                Return _description
            End Get
            Set(ByVal Value As String)
                _description = Value
            End Set
        End Property

        Public Overrides Property QueryString() As System.Collections.Specialized.NameValueCollection
            Get
                Return New NameValueCollection(_queryString)
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                MyBase.QueryString = Value
                _queryString = Value
                _currentStep = Utils.ToInt32(_queryString(QS_CURRENTSTEP))
                If (_queryString(QS_UNPAID) = Nothing) And (_queryString(QS_PAID) = Nothing) And _
                    (_queryString(QS_AUTHORISED) = Nothing) And (_queryString(QS_SUSPENDED) = Nothing) Then
                    _unpaid = True
                    _paid = True
                    _authorised = True
                    _suspended = True
                Else
                    _unpaid = IIf(_queryString(QS_UNPAID) = "true", True, False)
                    _paid = IIf(_queryString(QS_PAID) = "true", True, False)
                    _authorised = IIf(_queryString(QS_AUTHORISED) = "true", True, False)
                    _suspended = IIf(_queryString(QS_SUSPENDED) = "true", True, False)
                End If

                _invoiceNumber = _queryString(QS_INVOICE_NUMBER)
                If Target.Library.Utils.IsDate(_queryString(QS_WEFROM)) Then _weFrom = _queryString(QS_WEFROM)
                If Target.Library.Utils.IsDate(_queryString(QS_WETO)) Then _weTo = _queryString(QS_WETO)
                If (_queryString(QS_RETRACTION) = Nothing) Then
                    _retraction = True
                Else
                    _retraction = IIf(_queryString(QS_RETRACTION) = "true", True, False)
                End If

            End Set
        End Property

        Public Overrides ReadOnly Property BeforeNavigateJS() As String
            Get
                Return "DomProviderInvoiceFilterStep_BeforeNavigate()"
            End Get
        End Property

        Public Overrides Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage

            Dim msg As ErrorMessage = New ErrorMessage
            Dim spacerBr As Literal
            Dim fs As HtmlGenericControl
            Dim fs2 As HtmlGenericControl
            Dim legend As HtmlGenericControl

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            ' batch type
            fs = New HtmlGenericControl("FIELDSET")
            controls.Add(fs)
            legend = New HtmlGenericControl("LEGEND")
            legend.InnerText = "Filter Options"
            fs.Controls.Add(legend)

            With _invoiceNumberControl
                .LabelText = "Invoice No."
                .LabelWidth = "8em"
                .LabelBold = True
                .Text = _invoiceNumber
            End With
            AddHandler _invoiceNumberControl.AfterTextBoxControlAdded, AddressOf AfterTextBoxControlAdded
            fs.Controls.Add(_invoiceNumberControl)

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            fs.Controls.Add(spacerBr)

            With _weFromControl
                .ID = "dteWEFrom"
                .Format = TextBoxEx.TextBoxExFormat.DateFormat
                .LabelBold = True
                .LabelText = "W/E From"
                .LabelWidth = "8em"
                If Target.Library.Utils.IsDate(_weFrom) Then .Text = _weFrom
                fs.Controls.Add(_weFromControl)
            End With

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            fs.Controls.Add(spacerBr)

            With _weToControl
                .ID = "dteWETo"
                .Format = TextBoxEx.TextBoxExFormat.DateFormat
                .LabelBold = True
                .LabelText = "W/E To"
                .LabelWidth = "8em"
                If Target.Library.Utils.IsDate(_weTo) Then .Text = _weTo
                fs.Controls.Add(_weToControl)
            End With

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            fs.Controls.Add(spacerBr)

            ' status
            fs2 = New HtmlGenericControl("FIELDSET")
            legend = New HtmlGenericControl("LEGEND")
            legend.InnerText = "Invoice Status"
            fs2.Controls.Add(legend)

            With _unPaidControl
                .ID = "chkUnpaid"
                .Text = "Unpaid"
                .CheckBoxCssClass = "chkBoxStyle"
                .CheckBox.TextAlign = TextAlign.Right
                .CheckBox.Checked = _unpaid
            End With
            fs2.Controls.Add(_unPaidControl)

            With _authorisedControl
                .ID = "chkAuthorised"
                .Text = "Authorised"
                .CheckBoxCssClass = "chkBoxStyle"
                .CheckBox.TextAlign = TextAlign.Right
                .CheckBox.Checked = _authorised
            End With
            fs2.Controls.Add(_authorisedControl)

            With _paidControl
                .ID = "chkPaid"
                .Text = "Paid"
                .CheckBoxCssClass = "chkBoxStyle"
                .CheckBox.TextAlign = TextAlign.Right
                .CheckBox.Checked = _paid
            End With
            fs2.Controls.Add(_paidControl)

            With _suspendedControl
                .ID = "chkSuspended"
                .Text = "Suspended"
                .CheckBoxCssClass = "chkBoxStyle"
                .CheckBox.TextAlign = TextAlign.Right
                .CheckBox.Checked = _suspended
            End With
            fs2.Controls.Add(_suspendedControl)

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            fs2.Controls.Add(spacerBr)

            '
            ' add controls from the base class, basically the date controls
            msg = MyBase.RenderContentControls(wizard, fs2.Controls)
            If Not msg.Success Then
                Return msg
            End If
            fs.Controls.Add(fs2)

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            fs.Controls.Add(spacerBr)

            With _HideRetractedAndRetractionInvoicesControl
                .ID = "chkHideRetraction"
                .Text = "Hide retracted and retraction invoices"
                .TextAlign = TextAlign.Right
                .CssClass = "chkBoxStyle"
                .Checked = _retraction
            End With

            fs.Controls.Add(_HideRetractedAndRetractionInvoicesControl)

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            msg.Success = True
            Return msg

        End Function

        Private Sub AfterTextBoxControlAdded(ByVal sender As TextBoxEx)
            Dim anchor As HtmlAnchor = New HtmlAnchor
            Dim image As HtmlImage = New HtmlImage
            Dim space As Literal = New Literal

            space.Text = "&nbsp;"
            sender.Controls.Add(space)

            With image
                .Src = "~/Images/help16.png"
            End With

            With anchor
                .HRef = "javascript:ShowHelp();"
                .Controls.Add(image)
            End With

            sender.Controls.Add(anchor)
        End Sub

        Public Overrides Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage

            Dim msg As ErrorMessage
            Dim spacerBr As Literal
            Dim lbl As Label
            Dim text As Label
            Dim link As HyperLink
            Dim qs As NameValueCollection

            MyBase.ShowHeaderLink = True

            ' Invoice Number
            lbl = New Label()
            lbl.Text = "Invoice Number"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", _headerLabelWidth.ToString())
            controls.Add(lbl)
            text = New Label
            text.Style.Add("float", "left")
            text.Text = _invoiceNumber
            ' remove leading ", "
            'If text.Text.Length > 0 Then text.Text = text.Text.Substring(2)
            controls.Add(text)

            ' add the link
            link = New HyperLink()
            qs = Me.QueryString
            If Me.IsCurrentStep Then
                ' all
                link.Text = "All Invoice Numbers"
                qs.Remove(QS_INVOICE_NUMBER)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change Invoice Number"
                qs.Remove(QS_CURRENTSTEP)
                qs.Add(QS_CURRENTSTEP, MyBase.StepIndex)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            End If
            If Not String.IsNullOrEmpty(link.Text) Then
                link.Style.Add("float", "right")
                controls.Add(link)
            End If

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            'Weekending date range
            lbl = New Label()
            lbl.Text = "W/E Date Range"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", _headerLabelWidth.ToString())
            controls.Add(lbl)

            text = New Label
            text.Style.Add("float", "left")
            If Not Target.Library.Utils.IsDate(_weFrom) And Not Target.Library.Utils.IsDate(_weTo) Then
                text.Text = "All"
            Else
                If Target.Library.Utils.IsDate(_weFrom) Then
                    text.Text = String.Format("From {0}", _weFrom.ToString("dd/MM/yyyy"))
                End If
                If Target.Library.Utils.IsDate(_weTo) Then
                    If Target.Library.Utils.IsDate(_weFrom) Then text.Text &= " "
                    text.Text &= String.Format("To {0}", _weTo.ToString("dd/MM/yyyy"))
                End If
            End If
            controls.Add(text)

            ' add the link
            link = New HyperLink()
            qs = Me.QueryString
            If Me.IsCurrentStep AndAlso (Target.Library.Utils.IsDate(_weFrom) OrElse Target.Library.Utils.IsDate(_weTo)) Then
                ' all
                link.Text = "All Dates"
                qs.Remove(QS_WEFROM)
                qs.Remove(QS_WETO)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change W/E Date Range"
                qs.Remove(QS_CURRENTSTEP)
                qs.Add(QS_CURRENTSTEP, Me.StepIndex)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            End If
            If Not String.IsNullOrEmpty(link.Text) Then
                link.Style.Add("float", "right")
                controls.Add(link)
            End If

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            ' batch status
            lbl = New Label()
            lbl.Text = "Invoice Status"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", _headerLabelWidth.ToString())
            controls.Add(lbl)
            ' add the list of ticked status values
            text = New Label()
            text.Style.Add("float", "left")

            If _unpaid = True Then
                text.Text = String.Format("{0}, Unpaid", text.Text)
                Me.QueryString.Add(QS_UNPAID, "true")
            End If
            If _authorised = True Then
                text.Text = String.Format("{0}, Authorised", text.Text)
                Me.QueryString.Add(QS_AUTHORISED, "true")
            End If
            If _paid = True Then
                text.Text = String.Format("{0}, Paid", text.Text)
                Me.QueryString.Add(QS_PAID, "true")
            End If
            If _suspended = True Then
                text.Text = String.Format("{0}, Suspended", text.Text)
                Me.QueryString.Add(QS_SUSPENDED, "true")
            End If
            MyBase.QueryString = _queryString

            ' remove leading ", "
            If text.Text.Length > 0 Then text.Text = text.Text.Substring(2)
            controls.Add(text)

            ' add the link
            link = New HyperLink()
            qs = Me.QueryString
            If Me.IsCurrentStep Then
                ' default
                link.Text = "Default Invoice Status"
                qs.Remove(QS_AUTHORISED)
                qs.Remove(QS_PAID)
                qs.Remove(QS_SUSPENDED)
                qs.Remove(QS_UNPAID)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change Invoice Status"
                qs.Remove(QS_CURRENTSTEP)
                qs.Add(QS_CURRENTSTEP, MyBase.StepIndex)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            End If
            If Not String.IsNullOrEmpty(link.Text) Then
                link.Style.Add("float", "right")
                controls.Add(link)
            End If

            ' add date controls from base class
            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            msg = MyBase.RenderHeaderControls(controls)
            If Not msg.Success Then
                Return msg
            End If

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            lbl = New Label()
            lbl.Text = "Hide Retracted"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", _headerLabelWidth.ToString())
            controls.Add(lbl)

            ' add text
            text = New Label()
            text.Style.Add("float", "left")
            text.Text = IIf(_retraction, "Yes", "No")
            controls.Add(text)

            ' add the link
            link = New HyperLink()
            qs = Me.QueryString
            If Me.IsCurrentStep Then
                ' default
                link.Text = "Change filter"
                qs.Remove(QS_RETRACTION)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change filter"
                qs.Remove(QS_CURRENTSTEP)
                qs.Add(QS_CURRENTSTEP, MyBase.StepIndex)
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

        Public Overrides ReadOnly Property Title() As String
            Get
                Return "Select the criteria to filter the results on."
            End Get
        End Property

        Public Overrides Sub PreRender(ByVal wizard As SelectorWizardBase)

            Dim script As StringBuilder

            script = New StringBuilder()
            script.AppendFormat("SelectorWizard_id=""{0}"";", wizard.ClientID)
            script.AppendFormat("SelectorWizard_dateFromID=""{0}"";", MyBase.DateFromControl.ClientID)
            script.AppendFormat("SelectorWizard_dateToID=""{0}"";", MyBase.DateToControl.ClientID)
            script.AppendFormat("SelectorWizard_weFromID=""{0}"";", _weFromControl.ClientID)
            script.AppendFormat("SelectorWizard_weToID=""{0}"";", _weToControl.ClientID)
            script.AppendFormat("SelectorWizard_invoiceNumberID=""{0}"";", _invoiceNumberControl.ClientID)
            script.AppendFormat("DateRangeStep_required={0};", MyBase.Required.ToString().ToLower())
            script.AppendFormat("SelectorWizard_unPaidID=""{0}"";", _unPaidControl.ClientID)
            script.AppendFormat("SelectorWizard_paidID=""{0}"";", _paidControl.ClientID)
            script.AppendFormat("SelectorWizard_authorisedID=""{0}"";", _authorisedControl.ClientID)
            script.AppendFormat("SelectorWizard_suspendedID=""{0}"";", _suspendedControl.ClientID)
            script.AppendFormat("SelectorWizard_HideRetractedAndRetractionInvoicesID=""{0}"";", _HideRetractedAndRetractionInvoicesControl.ClientID)
            wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", script.ToString(), True)


        End Sub

        Public Overrides Sub InitStep(ByVal wizard As SelectorWizardBase)
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/UserControls/DomProviderInvoiceFilterStep.js"))
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/Dom/ProformaInvoice/CommentDialog.js"))
            CType(wizard.Page, Target.Web.Apps.BasePage).AddExtraCssStyle(".chkBoxStyle { float:left; margin-right:2em; }")
        End Sub

    End Class

#End Region

#Region " VisitAmendmentRequestEnquiryFilterStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : UserControls.VisitAmendmentRequestEnquiryFilterStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Class that implements Invoice Number, Weekending date range, status and 
    ''' status date range filters for domicilary  provider invoice enquiries.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' MikeVO  25/09/2009  D11546 - changes to "selections" section links
    ''' Paul  08/04/2008  Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class VisitAmendmentRequestEnquiryFilterStep
        Inherits DateRangeStep

        Private Enum Originators
            Council = 1
            Provider = 2
            Both = 3
        End Enum

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_REQ_DATEFROM As String = "reqDateFrom"
        Const QS_REQ_DATETO As String = "reqDateTo"
        Const QS_ORIGINATOR As String = "originator"
        Const QS_REQUESTEDBY As String = "reqByID"
        Const QS_STATUS As String = "status"
        Const QS_REQUESTEDBY_COMPANY As String = "reqbycompanyid"

        Const CTRL_STATUS As String = "chkStatus"

        Private _dbConnection As SqlConnection
        Private _currentStep As Integer
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _deleted As Boolean
        Private _originator As Originators
        Private _reqDateFrom As Date
        Private _reqDateTo As Date
        Private _reqByID As Integer
        Private _reqByCompanyID As Integer
        Private _description As String = "Please select the criteria to filter the results on."
        Private _tickedStatus As DomProviderInvoiceVisitAmendmentStatus
        Private _visibleStatus As DomProviderInvoiceVisitAmendmentStatus
        Private _councilOptionControl As RadioButton = New RadioButton
        Private _providerOptionControl As RadioButton = New RadioButton
        Private _bothOptionControl As RadioButton = New RadioButton
        Private _requestedByControl As DropDownListEx = New DropDownListEx
        Private _invoiceStatusCheckboxes As List(Of CheckBoxEx)
        Private _reqDateFromControl As TextBoxEx = New TextBoxEx
        Private _reqDateToControl As TextBoxEx = New TextBoxEx
        Private _headerLabelWidth As Unit = New Unit(10, UnitType.Em)
        Private _statusCheckboxes As List(Of CheckBox)
        Private _isCouncilUser As Boolean

        Sub New()
            MyBase.New()
            MyBase.DateFrom = Date.MinValue
            MyBase.DateTo = Date.MaxValue
            MyBase.ShowHeaderLink = False
            MyBase.HeaderLabelWidth = _headerLabelWidth
            _originator = Originators.Both

            For Each s As DomProviderInvoiceVisitAmendmentStatus In [Enum].GetValues(GetType(DomProviderInvoiceVisitAmendmentStatus))
                Me.VisibleStatus += s
            Next
            _statusCheckboxes = New List(Of CheckBox)

        End Sub

        Public Property VisibleStatus() As DomProviderInvoiceVisitAmendmentStatus
            Get
                Return _visibleStatus
            End Get
            Set(ByVal value As DomProviderInvoiceVisitAmendmentStatus)
                _visibleStatus = value
            End Set
        End Property

        Public Property TickedStatus() As DomProviderInvoiceVisitAmendmentStatus
            Get
                Return _tickedStatus
            End Get
            Set(ByVal value As DomProviderInvoiceVisitAmendmentStatus)
                _tickedStatus = value
            End Set
        End Property

        Public Overrides Property HeaderLabelWidth() As Unit
            Get
                Return _headerLabelWidth
            End Get
            Set(ByVal value As Unit)
                _headerLabelWidth = value
                MyBase.HeaderLabelWidth = value
            End Set
        End Property

        Public Overrides Property Description() As String
            Get
                Return _description
            End Get
            Set(ByVal Value As String)
                _description = Value
            End Set
        End Property

        Public Overrides Property QueryString() As System.Collections.Specialized.NameValueCollection
            Get
                Return MyBase.QueryString
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                MyBase.QueryString = Value
                _queryString = Value
                _currentStep = Utils.ToInt32(_queryString(QS_CURRENTSTEP))
                _reqByID = Utils.ToInt32(_queryString(QS_REQUESTEDBY))
                _reqByCompanyID = Utils.ToInt32(_queryString(QS_REQUESTEDBY_COMPANY))
                If Not _queryString(QS_ORIGINATOR) Is Nothing Then _originator = Utils.ToInt32(_queryString(QS_ORIGINATOR))
                If Not _queryString(QS_STATUS) Is Nothing Then Me.TickedStatus = _queryString(QS_STATUS)
                If Target.Library.Utils.IsDate(_queryString(QS_REQ_DATEFROM)) Then _reqDateFrom = _queryString(QS_REQ_DATEFROM)
                If Target.Library.Utils.IsDate(_queryString(QS_REQ_DATETO)) Then _reqDateTo = _queryString(QS_REQ_DATETO)
            End Set
        End Property

        Public Overrides ReadOnly Property BeforeNavigateJS() As String
            Get
                Return "VisitAmendmentRequestEnquiryFilterStep_BeforeNavigate()"
            End Get
        End Property

        Public Property IsCouncilUser() As Boolean
            Get
                Return _isCouncilUser
            End Get
            Set(ByVal value As Boolean)
                _isCouncilUser = value
            End Set
        End Property

        Public Overrides Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage


            Dim msg As ErrorMessage = New ErrorMessage
            Dim spacerBr As Literal
            Dim fs As HtmlGenericControl
            Dim fs1 As HtmlGenericControl
            Dim fs2 As HtmlGenericControl
            Dim legend As HtmlGenericControl
            Dim lbl As Label
            Dim firstControl As Boolean = True

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            ' batch type
            fs = New HtmlGenericControl("FIELDSET")
            controls.Add(fs)
            legend = New HtmlGenericControl("LEGEND")
            legend.InnerText = "Filter Options"
            fs.Controls.Add(legend)

            ' status
            fs1 = New HtmlGenericControl("FIELDSET")
            legend = New HtmlGenericControl("LEGEND")
            legend.InnerText = "Visit Details"
            fs1.Controls.Add(legend)

            lbl = New Label()
            lbl.Text = "Originator"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", "11em")
            fs1.Controls.Add(lbl)
            With _councilOptionControl
                .ID = "optCouncil"
                .GroupName = "Originator"
                .Text = "Council"
                .Checked = (_originator = Originators.Council)
                .TextAlign = TextAlign.Right
                .LabelAttributes.CssStyle.Add("float", "left")
                .InputAttributes.CssStyle.Add("float", "left")
                .LabelAttributes.CssStyle.Add("padding-top", "0.2em")
                .LabelAttributes.CssStyle.Add("padding-left", "0.5em")
                .InputAttributes.Add("onclick", "PopulateRequestedBy();")
                fs1.Controls.Add(_councilOptionControl)
            End With

            With _providerOptionControl
                .ID = "optProvider"
                .GroupName = "Originator"
                .Text = "Provider Organisation"
                .Checked = (_originator = Originators.Provider)
                .TextAlign = TextAlign.Right
                .LabelAttributes.CssStyle.Add("float", "left")
                .InputAttributes.CssStyle.Add("float", "left")
                .InputAttributes.CssStyle.Add("margin-left", "2em")
                .LabelAttributes.CssStyle.Add("padding-top", "0.2em")
                .LabelAttributes.CssStyle.Add("padding-left", "0.5em")
                .InputAttributes.Add("onclick", "PopulateRequestedBy();")
                fs1.Controls.Add(_providerOptionControl)
            End With

            With _bothOptionControl
                .ID = "optBoth"
                .GroupName = "Originator"
                .Text = "Both"
                .Checked = (_originator = Originators.Both)
                .TextAlign = TextAlign.Right
                .LabelAttributes.CssStyle.Add("float", "left")
                .InputAttributes.CssStyle.Add("float", "left")
                .InputAttributes.CssStyle.Add("margin-left", "2em")
                .LabelAttributes.CssStyle.Add("padding-top", "0.2em")
                .LabelAttributes.CssStyle.Add("padding-left", "0.5em")

                .InputAttributes.Add("onclick", "PopulateRequestedBy();")

                fs1.Controls.Add(_bothOptionControl)
            End With

            spacerBr = New Literal
            spacerBr.Text = "<br /><br />"
            fs1.Controls.Add(spacerBr)

            With _requestedByControl
                .ID = "cboRequestedBy"
                .LabelBold = True
                .LabelWidth = "11em"
                .LabelText = "Requested By"
                fs1.Controls.Add(_requestedByControl)
            End With

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            fs1.Controls.Add(spacerBr)

            With _reqDateFromControl
                .ID = "dteVisitDateFrom"
                .Format = TextBoxEx.TextBoxExFormat.DateFormat
                .LabelBold = True
                .LabelText = "Requested Date From"
                .LabelWidth = "11em"
                If Target.Library.Utils.IsDate(_reqDateFrom) Then .Text = _reqDateFrom
                fs1.Controls.Add(_reqDateFromControl)
            End With

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            fs1.Controls.Add(spacerBr)

            With _reqDateToControl
                .ID = "dteVisitDateTo"
                .Format = TextBoxEx.TextBoxExFormat.DateFormat
                .LabelBold = True
                .LabelText = "Requested Date To"
                .LabelWidth = "11em"
                If Target.Library.Utils.IsDate(_reqDateTo) Then .Text = _reqDateTo
                fs1.Controls.Add(_reqDateToControl)
            End With

            fs.Controls.Add(fs1)
            spacerBr = New Literal
            spacerBr.Text = "<br />"
            fs.Controls.Add(spacerBr)

            ' status
            fs2 = New HtmlGenericControl("FIELDSET")
            legend = New HtmlGenericControl("LEGEND")
            legend.InnerText = "Visit Amendment Status"
            fs2.Controls.Add(legend)

            lbl = New Label()
            lbl.Text = "Status"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Width = New Unit(8, UnitType.Em)
            fs2.Controls.Add(lbl)
            firstControl = True
            For Each s As DomProviderInvoiceVisitAmendmentStatus In [Enum].GetValues(GetType(DomProviderInvoiceVisitAmendmentStatus))
                If s And Me.VisibleStatus Then
                    Dim chk As CheckBox = New CheckBox()
                    With chk
                        .ID = String.Format("{0}{1}", CTRL_STATUS, Convert.ToInt32(s))
                        .Text = Utils.SplitOnCapitals([Enum].GetName(GetType(DomProviderInvoiceVisitAmendmentStatus), s))
                        .TextAlign = TextAlign.Right
                        .CssClass = "chkBoxStyle"
                        .LabelAttributes.CssStyle.Add("float", "left")
                        .InputAttributes.CssStyle.Add("float", "left")
                        If Not firstControl Then 'we dont want to put a margin on the first control
                            .InputAttributes.CssStyle.Add("margin-left", "1em")
                        End If
                        .LabelAttributes.CssStyle.Add("padding-top", "0.2em")
                        .LabelAttributes.CssStyle.Add("padding-left", "0.5em")
                        If s And Me.TickedStatus Then .Checked = True
                        firstControl = False
                    End With
                    fs2.Controls.Add(chk)
                    _statusCheckboxes.Add(chk)
                End If
            Next

            spacerBr = New Literal
            ' to allign the next coming date control
            spacerBr.Text = "<div>&nbsp;</div>"
            fs2.Controls.Add(spacerBr)

            ' add controls from the base class, basically the date controls
            msg = MyBase.RenderContentControls(wizard, fs2.Controls)
            If Not msg.Success Then
                Return msg
            End If

            fs.Controls.Add(fs2)

            spacerBr = New Literal
            spacerBr.Text = "<br/>"
            controls.Add(spacerBr)
           
            msg.Success = True
            Return msg

        End Function

        Public Overrides Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage

            Dim msg As ErrorMessage
            Dim spacerBr As Literal
            Dim lbl As Label
            Dim text As Label
            Dim link As HyperLink
            Dim webUser As WebSecurityUser = Nothing
            Dim webCompany As WebSecurityCompany = Nothing
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim qs As NameValueCollection

            MyBase.ShowHeaderLink = True

            'originator
            lbl = New Label()
            lbl.Text = "Originator"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", _headerLabelWidth.ToString())
            controls.Add(lbl)

            text = New Label
            text.Style.Add("float", "left")
            Select Case _originator
                Case Originators.Council
                    text.Text = "Council"
                Case Originators.Provider
                    text.Text = "Provider Organisation"
                Case Originators.Both
                    text.Text = "Both"
            End Select
            controls.Add(text)

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            'Requested By
            lbl = New Label()
            lbl.Text = "Requested By"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", _headerLabelWidth.ToString())
            controls.Add(lbl)

            text = New Label
            text.Style.Add("float", "left")
            If _reqByID > 0 Then
                If Me.IsCouncilUser Then
                    ' current user is a council user
                    Select Case _originator
                        Case Originators.Council
                            webUser = New WebSecurityUser(Me.DbConnection)
                        Case Originators.Provider
                            webCompany = New WebSecurityCompany(Me.DbConnection)
                    End Select
                Else
                    If _originator = Originators.Provider Then
                        webUser = New WebSecurityUser(Me.DbConnection)
                    End If
                End If
                If Not webUser Is Nothing Then
                    msg = webUser.Fetch(_reqByID)
                    If Not msg.Success Then Return msg
                    text.Text = String.Format("{0}, {1}", webUser.Surname, webUser.FirstName)
                End If
                If Not webCompany Is Nothing Then
                    msg = webCompany.Fetch(_reqByID)
                    If Not msg.Success Then Return msg
                    text.Text = webCompany.Name
                End If
            End If
            controls.Add(text)

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            'Weekending date range
            lbl = New Label()
            lbl.Text = "Date Requested"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", _headerLabelWidth.ToString())
            controls.Add(lbl)

            text = New Label
            text.Style.Add("float", "left")
            If Not Target.Library.Utils.IsDate(_reqDateFrom) And Not Target.Library.Utils.IsDate(_reqDateTo) Then
                text.Text = "All"
            Else
                If Target.Library.Utils.IsDate(_reqDateFrom) Then
                    text.Text = String.Format("From {0}", _reqDateFrom.ToString("dd/MM/yyyy"))
                End If
                If Target.Library.Utils.IsDate(_reqDateTo) Then
                    If Target.Library.Utils.IsDate(_reqDateFrom) Then text.Text &= " "
                    text.Text &= String.Format("To {0}", _reqDateTo.ToString("dd/MM/yyyy"))
                End If
            End If
            controls.Add(text)

            ' add the link
            link = New HyperLink()
            qs = Me.QueryString
            If Me.IsCurrentStep AndAlso (Utils.IsDate(_reqDateFrom) OrElse Utils.IsDate(_reqDateTo)) Then
                ' all
                link.Text = "All Dates"
                qs.Remove(QS_REQ_DATEFROM)
                qs.Remove(QS_REQ_DATETO)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change Requested Date Range"
                qs.Remove(QS_CURRENTSTEP)
                qs.Add(QS_CURRENTSTEP, Me.StepIndex)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            End If
            If Not String.IsNullOrEmpty(link.Text) Then
                link.Style.Add("float", "right")
                controls.Add(link)
            End If

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            ' visit Status
            lbl = New Label()
            lbl.Text = "Visit Status"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Width = _headerLabelWidth
            controls.Add(lbl)
            ' add the list of ticked status values
            text = New Label()
            text.Style.Add("float", "left")
            For Each s As DomProviderInvoiceVisitAmendmentStatus In [Enum].GetValues(GetType(DomProviderInvoiceVisitAmendmentStatus))
                If s And Me.TickedStatus Then
                    text.Text = String.Format("{0}, {1}", text.Text, Utils.SplitOnCapitals([Enum].GetName(GetType(DomProviderInvoiceVisitAmendmentStatus), s)))
                End If
            Next
            ' remove leading ", "
            If text.Text.Length > 0 Then text.Text = text.Text.Substring(2)
            controls.Add(text)

            ' add the link
            link = New HyperLink()
            qs = Me.QueryString
            If Me.IsCurrentStep AndAlso Me.TickedStatus > 0 Then
                ' all
                link.Text = "Default Visit Status"
                qs.Remove(QS_STATUS)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change Visit Status"
                qs.Remove(QS_CURRENTSTEP)
                qs.Add(QS_CURRENTSTEP, MyBase.StepIndex)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            End If
            If Not String.IsNullOrEmpty(link.Text) Then
                link.Style.Add("float", "right")
                controls.Add(link)
            End If

            ' add date controls from base class
            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            msg = MyBase.RenderHeaderControls(controls)
            If Not msg.Success Then
                Return msg
            End If

            msg = New ErrorMessage
            msg.Success = True

            Return msg

        End Function

        Public Overrides ReadOnly Property Title() As String
            Get
                Return "Select the criteria to filter the results on."
            End Get
        End Property

        Public Overrides Sub PreRender(ByVal wizard As SelectorWizardBase)
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim script As StringBuilder
            Dim script2 As StringBuilder
            Dim scriptString As String

            script = New StringBuilder()
            script.AppendFormat("SelectorWizard_id=""{0}"";", wizard.ClientID)
            script.AppendFormat("SelectorWizard_dateFromID=""{0}"";", MyBase.DateFromControl.ClientID)
            script.AppendFormat("SelectorWizard_dateToID=""{0}"";", MyBase.DateToControl.ClientID)
            script.AppendFormat("SelectorWizard_reqDateFromID=""{0}"";", _reqDateFromControl.ClientID)
            script.AppendFormat("SelectorWizard_reqDateToID=""{0}"";", _reqDateToControl.ClientID)
            script.AppendFormat("SelectorWizard_requestedByID=""{0}"";", _requestedByControl.ClientID)
            script.AppendFormat("SelectorWizard_optCouncilID=""{0}"";", _councilOptionControl.ClientID)
            script.AppendFormat("SelectorWizard_optProviderID=""{0}"";", _providerOptionControl.ClientID)
            script.AppendFormat("SelectorWizard_optBothID=""{0}"";", _bothOptionControl.ClientID)
            script.AppendFormat("VisitAmendmentRequestEnquiryFilterStep_StatusPrefix=""{0}"";", CTRL_STATUS)
            script.AppendFormat("reqByID=""{0}"";", _reqByID)
            script.AppendFormat("reqByCompanyID=""{0}"";", _reqByCompanyID)
            script.AppendFormat("WebCompanyID={0};", user.WebSecurityCompanyID)
            'script.AppendFormat("statusValue={0};", Me.TickedStatus)
            script.AppendFormat("CouncilUser={0};PopulateRequestedBy();", Me.IsCouncilUser.ToString.ToLower)
            script.AppendFormat("DateRangeStep_required={0};", MyBase.Required.ToString().ToLower())

            ' output array of batch status checkbox IDs
            script2 = New StringBuilder()
            For Each chk As CheckBox In _statusCheckboxes
                script2.AppendFormat("""{0}"",", chk.ClientID)
            Next
            scriptString = script2.ToString()
            If scriptString.Length > 0 Then scriptString = scriptString.Substring(0, scriptString.Length - 1)
            wizard.Page.ClientScript.RegisterArrayDeclaration("VisitAmendmentRequestEnquiryFilterStep_StatusIDs", scriptString)


            wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", script.ToString(), True)


        End Sub

        Public Overrides Sub InitStep(ByVal wizard As SelectorWizardBase)
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/UserControls/VisitAmendmentRequestEnquiryFilterStep.js"))
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/Dom/ProformaInvoice/CommentDialog.js"))
            CType(wizard.Page, Target.Web.Apps.BasePage).AddExtraCssStyle(".chkBoxStyle { float:left; margin-right:2em; }")
        End Sub

    End Class

#End Region

#Region " WeekendingStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : UserControls.WeekendingStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Class that implements a weekending filter step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' MikeVO  25/09/2009  D11546 - changes to "selections" section links
    ''' Paul  14/04/2008  Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class WeekendingStep
        Implements ISelectorWizardStep

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_PROVIDER_ID As String = "estabID"
        Const QS_WEEKENDING As String = "weekending"

        Private _dbConnection As SqlConnection
        Private _currentStep As Integer
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _deleted As Boolean
        Private _weekending As Date
        Private _providerID As Integer
        Private _description As String = "Please select a valid Weekending date to filter the results on."
        Private _weekendingControl As TextBoxEx = New TextBoxEx
        Private _headerLabelWidth As Unit = New Unit(10, UnitType.Em)



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
                _providerID = Utils.ToInt32(_queryString(QS_PROVIDER_ID))
                If Target.Library.Utils.IsDate(_queryString(QS_WEEKENDING)) Then _weekending = _queryString(QS_WEEKENDING)
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
                Return "Select a Week Ending Date"
            End Get
        End Property

        Public ReadOnly Property BeforeNavigateJS() As String Implements ISelectorWizardStep.BeforeNavigateJS
            Get
                Return "WeekendingStep_BeforeNavigate()"
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
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/UserControls/WeekendingStep.js"))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.DomContract))
        End Sub

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender
            Dim script As StringBuilder

            script = New StringBuilder()
            script.AppendFormat("SelectorWizard_weekendingID=""{0}"";", _weekendingControl.ClientID)
            script.AppendFormat("stepRequired=""{0}"";", _required)
            wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", script.ToString(), True)

        End Sub

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls

            Dim msg As ErrorMessage
            Dim lbl As Label = New Label
            Dim text As Label = New Label
            Dim link As HyperLink = New HyperLink
            Dim qs As NameValueCollection

            'Weekending date range
            lbl = New Label()
            lbl.Text = "Weekending"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", _headerLabelWidth.ToString())
            controls.Add(lbl)

            text = New Label
            text.Style.Add("float", "left")
            If Not Target.Library.Utils.IsDate(_weekending) Then
                text.Text = "All"
            Else
                If Target.Library.Utils.IsDate(_weekending) Then
                    text.Text = _weekending.ToString("dd/MM/yyyy")
                End If
            End If
            controls.Add(text)

            ' add the link
            qs = Me.QueryString
            If Me.IsCurrentStep AndAlso Target.Library.Utils.IsDate(_weekending.ToString) Then
                ' all
                link.Text = "All Dates"
                qs.Remove(QS_WEEKENDING)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change Weekending Date"
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
            Dim msg As ErrorMessage
            Dim spacerBr As Literal

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            With _weekendingControl
                .ID = "dteWEFrom"
                .Format = TextBoxEx.TextBoxExFormat.DateFormat
                .LabelBold = True
                .LabelText = "Weekending"
                .LabelWidth = "8em"
                If Target.Library.Utils.IsDate(_weekending) Then .Text = _weekending
                controls.Add(_weekendingControl)
            End With

            msg = New ErrorMessage
            msg.Success = True
            Return msg

        End Function

    End Class

#End Region

#Region " CareWorkerStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.SelectorWizardSteps.CareWorkerStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Implements the select care Worker step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MikeVO  25/09/2009  D11546 - changes to "selections" section links
    ''' 	[Paul]	15/04/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Class CareWorkerStep
        Implements ISelectorWizardStep

        Const QS_PROVIDER_ID As String = "estabID"
        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_CAREWORKERID As String = "careWorkerID"
        Const QS_CLIENTID As String = "clientID"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _addHeaderControls As Boolean = True
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _providerID As Integer
        Private _clientID As Integer
        Private _careWorkerID As Integer
        Private _required As Boolean
        Private _description As String = "Please select a care worker from the list below and then click ""Next""."
        Private _headerLabelWidth As Unit = New Unit(8, UnitType.Em)
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

        Public Property AddHeaderControls() As Boolean
            Get
                Return _addHeaderControls
            End Get
            Set(ByVal Value As Boolean)
                _addHeaderControls = Value
            End Set
        End Property

        Public Property QueryString() As System.Collections.Specialized.NameValueCollection Implements ISelectorWizardStep.QueryString
            Get
                Return New NameValueCollection(_queryString)
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                _queryString = Value
                ' pull out the required params
                _providerID = Utils.ToInt32(_queryString(QS_PROVIDER_ID))
                _careWorkerID = Utils.ToInt32(_queryString(QS_CAREWORKERID))
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
                Return "Select a Care Worker"
            End Get
        End Property

        Public ReadOnly Property BeforeNavigateJS() As String Implements ISelectorWizardStep.BeforeNavigateJS
            Get
                Return "CareWorkerStep_BeforeNavigate()"
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

            wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Extranet.Apps.UserControls.SelectorWizardSteps.CareWorkerStep.Startup", _
                Target.Library.Web.Utils.WrapClientScript( _
                    String.Format("required={0};clientID={1};", _
                        _required.ToString().ToLower(), _clientID) _
                ) _
            )

        End Sub

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls

            Const SP_NAME As String = "spxCareWorker_Fetch"

            Dim msg As ErrorMessage
            Dim lbl As Label = New Label
            Dim text As Label = New Label
            Dim link As HyperLink = New HyperLink
            Dim reader As SqlDataReader = Nothing
            Dim spParams As SqlParameter()
            Dim qs As NameValueCollection

            If _addHeaderControls Then

                ' label
                lbl.Text = "Care Worker"
                lbl.Style.Add("float", "left")
                lbl.Style.Add("font-weight", "bold")
                lbl.Style.Add("width", _headerLabelWidth.ToString())
                controls.Add(lbl)

                text.Style.Add("float", "left")
                If _careWorkerID = 0 Then
                    text.Text = "All"
                Else
                    Try
                        ' get the client details
                        spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME, False)
                        spParams(0).Value = _careWorkerID
                        reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams)
                        If (reader.HasRows) Then
                            reader.Read()
                            text.Text = String.Format("{0}: {1}", reader("Reference"), reader("Name"))
                        End If

                    Catch ex As Exception
                        msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected
                        Return msg
                    Finally
                        If Not reader Is Nothing Then reader.Close()
                    End Try

                End If
                controls.Add(text)

                ' add the link
                qs = Me.QueryString
                If Me.IsCurrentStep AndAlso _careWorkerID > 0 Then
                    ' all
                    link.Text = "All Care Workers"
                    qs.Remove(QS_CAREWORKERID)
                    link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
                ElseIf _currentStep > Me.StepIndex Then
                    ' change
                    link.Text = "Change Care Worker"
                    qs.Remove(QS_CURRENTSTEP)
                    qs.Add(QS_CURRENTSTEP, Me.StepIndex)
                    link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
                End If
                If Not String.IsNullOrEmpty(link.Text) Then
                    link.Style.Add("float", "right")
                    controls.Add(link)
                End If

            End If

            msg = New ErrorMessage
            msg.Success = True

            Return msg

        End Function

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

            Dim msg As ErrorMessage = New ErrorMessage
            Dim careWorkerList As CareWorkerSelector = wizard.LoadControl("~/AbacusExtranet/Apps/UserControls/CareWorkerSelector.ascx")
            careWorkerList.InitControl(wizard.Page, _providerID, _careWorkerID)
            controls.Add(careWorkerList)
            msg.Success = True
            Return msg

        End Function

    End Class

#End Region

#Region " ManualDomProformaInvoiceBatchEnquiryResultsStep "

    Public Class ManualDomProformaInvoiceBatchEnquiryResultsStep
        Implements ISelectorWizardStep

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_ESTABLISHMENTID As String = "estabID"
        Const QS_CONTRACTID As String = "contractID"
        Const QS_CLIENTID As String = "clientID"
        Const QS_BATCH_TYPE As String = "batchType"
        Const QS_BATCH_STATUS As String = "batchStatus"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETO As String = "dateTo"
        Const QS_INVOICE_ID As String = "invoiceID"
        Const QS_PSCHEDULE_ID As String = "pscheduleid"

        Private _providerID As Integer
        Private _contractID As Integer
        Private _clientID As Integer
        Private _batchType As DomProformaInvoiceBatchType
        Private _batchStatus As DomProformaInvoiceBatchStatus
        Private _dateFrom As Date
        Private _dateTo As Date
        Private _invoiceID As Integer
        Private _pScheduleID As Integer

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _deleted As Boolean
        Private _description As String = "The list below displays the results of your Manual Domiciliary Pro forma Invoice enquiry."
        Private _defaultBatchType As DomProformaInvoiceBatchType
        Private _defaultBatchStatus As DomProformaInvoiceBatchStatus
        Private _displayOptions As ManualDomProformaInvoiceSelectorDisplayOptions

        Sub New()
            _displayOptions = New ManualDomProformaInvoiceSelectorDisplayOptions()
        End Sub

        Public Property DefaultBatchType() As DomProformaInvoiceBatchType
            Get
                Return _defaultBatchType
            End Get
            Set(ByVal value As DomProformaInvoiceBatchType)
                _defaultBatchType = value
            End Set
        End Property

        Public Property DefaultBatchStatus() As DomProformaInvoiceBatchStatus
            Get
                Return _defaultBatchStatus
            End Get
            Set(ByVal value As DomProformaInvoiceBatchStatus)
                _defaultBatchStatus = value
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

        Public Property QueryString() As NameValueCollection Implements ISelectorWizardStep.QueryString
            Get
                Return New NameValueCollection(_queryString)
            End Get
            Set(ByVal value As NameValueCollection)
                _queryString = value
                ' pull out the required params
                _providerID = Utils.ToInt32(_queryString(QS_ESTABLISHMENTID))
                _contractID = Utils.ToInt32(_queryString(QS_CONTRACTID))
                _clientID = Utils.ToInt32(_queryString(QS_CLIENTID))
                _pScheduleID = Utils.ToInt32(_queryString(QS_PSCHEDULE_ID))
                If Utils.ToInt32(_queryString(QS_BATCH_TYPE)) > 0 Then
                    _batchType = Utils.ToInt32(_queryString(QS_BATCH_TYPE))
                Else
                    _batchType = Me.DefaultBatchType
                End If
                If Utils.ToInt32(_queryString(QS_BATCH_STATUS)) > 0 Then
                    _batchStatus = Utils.ToInt32(_queryString(QS_BATCH_STATUS))
                Else
                    _batchStatus = Me.DefaultBatchStatus
                End If
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETO)) Then _dateTo = _queryString(QS_DATETO)
                _invoiceID = Utils.ToInt32(_queryString(QS_INVOICE_ID))
            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls
            Dim msg As ErrorMessage = New ErrorMessage
            Dim invoiceList As ManualDomProformaInvoiceSelector = wizard.LoadControl("~/AbacusExtranet/Apps/UserControls/ManualDomProformaInvoiceSelector.ascx")
            invoiceList.InitControl(wizard.Page, _
                                    _invoiceID, _
                                    _providerID, _
                                    _contractID, _
                                    _clientID, _
                                    _batchType, _
                                    _batchStatus, _
                                    _dateFrom, _
                                    _dateTo, _
                                    _pScheduleID, _
                                    _displayOptions)
            controls.Add(invoiceList)
            msg.Success = True
            Return msg
        End Function

        Public Function RenderHeaderControls(ByRef controls As ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage
            msg = New ErrorMessage()
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

        Public Property DisplayOptions() As ManualDomProformaInvoiceSelectorDisplayOptions
            Get
                Return _displayOptions
            End Get
            Set(ByVal value As ManualDomProformaInvoiceSelectorDisplayOptions)
                _displayOptions = value
            End Set
        End Property

    End Class

#End Region

#Region " DomProformaInvoiceBatchEnquiryResultsStep "

    Public Class DomProformaInvoiceBatchEnquiryResultsStep
        Implements ISelectorWizardStep

        Const QS_FILEID As String = "fileID"
        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_ESTABLISHMENTID As String = "estabID"
        Const QS_CONTRACTID As String = "contractID"
        Const QS_BATCH_TYPE As String = "batchType"
        Const QS_BATCH_STATUS As String = "batchStatus"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETO As String = "dateTo"
        Const QS_INVOICE_ID As String = "invoiceID"

        Private _fileID As Integer
        Private _providerID As Integer
        Private _contractID As Integer
        Private _batchType As DomProformaInvoiceBatchType
        Private _batchStatus As DomProformaInvoiceBatchStatus
        Private _dateFrom As Date
        Private _dateTo As Date
        Private _invoiceID As Integer

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _deleted As Boolean
        Private _description As String = "The list below displays the results of your Domiciliary Pro forma Invoice enquiry."
        Private _defaultBatchType As DomProformaInvoiceBatchType
        Private _defaultBatchStatus As DomProformaInvoiceBatchStatus
        Private _displayOptions As DomProformaInvoiceSelectorDisplayOptions

        Sub New()
            _displayOptions = New DomProformaInvoiceSelectorDisplayOptions()
        End Sub

        Public Property DefaultBatchType() As DomProformaInvoiceBatchType
            Get
                Return _defaultBatchType
            End Get
            Set(ByVal value As DomProformaInvoiceBatchType)
                _defaultBatchType = value
            End Set
        End Property

        Public Property DefaultBatchStatus() As DomProformaInvoiceBatchStatus
            Get
                Return _defaultBatchStatus
            End Get
            Set(ByVal value As DomProformaInvoiceBatchStatus)
                _defaultBatchStatus = value
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

        Public Property QueryString() As NameValueCollection Implements ISelectorWizardStep.QueryString
            Get
                Return New NameValueCollection(_queryString)
            End Get
            Set(ByVal value As NameValueCollection)
                _queryString = value
                ' pull out the required params
                If Utils.ToInt32(_queryString(QS_FILEID)) > 0 Then
                    _fileID = Utils.ToInt32(_queryString(QS_FILEID))
                Else
                    _fileID = 0
                End If
                _providerID = Utils.ToInt32(_queryString(QS_ESTABLISHMENTID))
                _contractID = Utils.ToInt32(_queryString(QS_CONTRACTID))
                If Utils.ToInt32(_queryString(QS_BATCH_TYPE)) > 0 Then
                    _batchType = Utils.ToInt32(_queryString(QS_BATCH_TYPE))
                Else
                    _batchType = Me.DefaultBatchType
                End If
                If Utils.ToInt32(_queryString(QS_BATCH_STATUS)) > 0 Then
                    _batchStatus = Utils.ToInt32(_queryString(QS_BATCH_STATUS))
                Else
                    _batchStatus = Me.DefaultBatchStatus
                End If
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETO)) Then _dateTo = _queryString(QS_DATETO)
                _invoiceID = Utils.ToInt32(_queryString(QS_INVOICE_ID))
            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls
            Dim msg As ErrorMessage = New ErrorMessage
            Dim invoiceList As DomProformaInvoiceSelector = wizard.LoadControl("~/AbacusExtranet/Apps/UserControls/DomProformaInvoiceSelector.ascx")
            invoiceList.InitControl(wizard.Page, _fileID, _invoiceID, _providerID, _contractID, _batchType, _batchStatus, _dateFrom, _dateTo, _displayOptions)
            controls.Add(invoiceList)
            msg.Success = True
            Return msg
        End Function

        Public Function RenderHeaderControls(ByRef controls As ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage
            msg = New ErrorMessage()
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

        Public Property DisplayOptions() As DomProformaInvoiceSelectorDisplayOptions
            Get
                Return _displayOptions
            End Get
            Set(ByVal value As DomProformaInvoiceSelectorDisplayOptions)
                _displayOptions = value
            End Set
        End Property

    End Class

#End Region

#Region " DomProviderInvoiceEnquiryResultsStep "

    Public Class DomProviderInvoiceEnquiryResultsStep
        Implements ISelectorWizardStep

        Const QS_FILEID As String = "fileID"
        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_ESTABLISHMENTID As String = "estabID"
        Const QS_CONTRACTID As String = "contractID"
        Const QS_CLIENTID As String = "clientID"
        Const QS_INVOICE_NUMBER As String = "invoiceNumber"
        Const QS_WE_FROM As String = "weFrom"
        Const QS_WE_TO As String = "weTo"
        Const QS_PAID As String = "paid"
        Const QS_UNPAID As String = "unPaid"
        Const QS_AUTHORISED As String = "authorised"
        Const QS_SUSPENDED As String = "suspended"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETO As String = "dateTo"
        Const QS_INVOICE_ID As String = "invoiceID"
        Const QS_RETRACTION As String = "retraction"
        Const QS_PSVIEW As String = "psview"

        Private _fileID As Integer
        Private _providerID As Integer
        Private _contractID As Integer
        Private _clientID As Integer
        Private _unPaid As Boolean
        Private _paid As Boolean
        Private _authorised As Boolean
        Private _suspended As Boolean
        Private _invoiceNumber As String
        Private _weekendingFrom As Date
        Private _weekendingTo As Date
        Private _dateFrom As Date
        Private _dateTo As Date
        Private _invoiceID As Integer

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _deleted As Boolean
        Private _description As String = "The list below displays the results of your Domiciliary Provider Invoice enquiry."
        Private _defaultBatchStatus As DomProviderInvoiceStatus
        Private _enableRetractFeature As Boolean = False
        Private _userHasRetractCommand As Boolean
        Private _retraction As Boolean

        Public Property UserHasRetractCommand() As Boolean
            Get
                Return _userHasRetractCommand
            End Get
            Set(ByVal value As Boolean)
                _userHasRetractCommand = value
            End Set
        End Property

        Public Property EnableRetractFeature() As Boolean
            Get
                Return _enableRetractFeature
            End Get
            Set(ByVal value As Boolean)
                _enableRetractFeature = value
            End Set
        End Property


        Public Property DefaultStatus() As DomProviderInvoiceStatus
            Get
                Return _defaultBatchStatus
            End Get
            Set(ByVal value As DomProviderInvoiceStatus)
                _defaultBatchStatus = value
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


        Private _hideColumnContractNumber As Boolean = True
        Public Property HideColumnContractNumber() As Boolean
            Get
                Return _hideColumnContractNumber
            End Get
            Set(ByVal value As Boolean)
                _hideColumnContractNumber = value
            End Set
        End Property


        Private _hideColumnProviderReference As Boolean = True
        Public Property HideColumnProviderReference() As Boolean
            Get
                Return _hideColumnProviderReference
            End Get
            Set(ByVal value As Boolean)
                _hideColumnProviderReference = value
            End Set
        End Property


        Private _hideSUreference As Boolean
        Public Property HideSUReference() As Boolean
            Get
                Return _hideSUreference
            End Get
            Set(ByVal value As Boolean)
                _hideSUreference = value
            End Set
        End Property

        Private _showPaymentSchedule As Boolean = True
        Public Property showPaymentSchedule() As Boolean
            Get
                Return _showPaymentSchedule
            End Get
            Set(ByVal value As Boolean)
                _showPaymentSchedule = value
            End Set
        End Property


        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

        End Sub

        Public Property QueryString() As NameValueCollection Implements ISelectorWizardStep.QueryString
            Get
                Return New NameValueCollection(_queryString)
            End Get
            Set(ByVal value As NameValueCollection)
                _queryString = value
                ' pull out the required params
                If Utils.ToInt32(_queryString(QS_FILEID)) > 0 Then
                    _fileID = Utils.ToInt32(_queryString(QS_FILEID))
                Else
                    _fileID = 0
                End If
                _providerID = Utils.ToInt32(_queryString(QS_ESTABLISHMENTID))
                _contractID = Utils.ToInt32(_queryString(QS_CONTRACTID))
                _clientID = Utils.ToInt32(_queryString(QS_CLIENTID))
                If (_queryString(QS_UNPAID) = Nothing) And (_queryString(QS_PAID) = Nothing) And _
                    (_queryString(QS_AUTHORISED) = Nothing) And (_queryString(QS_SUSPENDED) = Nothing) Then
                    _unPaid = True
                    _paid = True
                    _authorised = True
                    _suspended = True
                Else
                    _paid = IIf(_queryString(QS_PAID) = "true", True, False)
                    _unPaid = IIf(_queryString(QS_UNPAID) = "true", True, False)
                    _authorised = IIf(_queryString(QS_AUTHORISED) = "true", True, False)
                    _suspended = IIf(_queryString(QS_SUSPENDED) = "true", True, False)
                End If
                _invoiceNumber = _queryString(QS_INVOICE_NUMBER)
                If Utils.IsDate(_queryString(QS_WE_FROM)) Then _weekendingFrom = _queryString(QS_WE_FROM)
                If Utils.IsDate(_queryString(QS_WE_TO)) Then _weekendingTo = _queryString(QS_WE_TO)
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETO)) Then _dateTo = _queryString(QS_DATETO)
                _invoiceID = Utils.ToInt32(_queryString(QS_INVOICE_ID))
                If _queryString(QS_RETRACTION) = Nothing Then
                    _retraction = True
                Else
                    _retraction = IIf(_queryString(QS_RETRACTION), True, False)
                End If
            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls
            Dim msg As ErrorMessage = New ErrorMessage
            If Not _enableRetractFeature Then
                _userHasRetractCommand = False
            End If
            Dim invoiceList As DomProviderInvoiceSelector = wizard.LoadControl("~/AbacusExtranet/Apps/UserControls/DomProviderInvoiceSelector.ascx")
            invoiceList.HideColumnContractNumber = HideColumnContractNumber
            invoiceList.HideColumnProviderReference = HideColumnProviderReference
            invoiceList.HideColumnSUReference = HideSUReference

            invoiceList.thePage = wizard.Page
            invoiceList.selectedInvoiceID = _invoiceID
            invoiceList.providerID = _providerID
            invoiceList.serviceUserID = _clientID
            invoiceList.contractID = _contractID
            invoiceList.invoiceNumber = _invoiceNumber
            invoiceList.weekendingFrom = _weekendingFrom
            invoiceList.weekendingTo = _weekendingTo
            invoiceList.unPaid = _unPaid
            invoiceList.paid = _paid
            invoiceList.authorised = _authorised
            invoiceList.suspended = _suspended
            invoiceList.dateFrom = _dateFrom
            invoiceList.dateTo = _dateTo
            invoiceList.userHasRetractCommand = _userHasRetractCommand
            invoiceList.hideRetraction = _retraction
            invoiceList.pscheduleId = Nothing
            invoiceList.showPaymentSchedule = showPaymentSchedule

            controls.Add(invoiceList)
            msg.Success = True
            Return msg
        End Function

        Public Function RenderHeaderControls(ByRef controls As ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage
            msg = New ErrorMessage()
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

    End Class

#End Region

#Region " PaymentScheduleEnquiryResultsStep "

    Public Class PaymentScheduleEnquiryResultsStep
        Implements ISelectorWizardStep

        Const QS_FILEID As String = "fileID"
        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_ESTABLISHMENTID As String = "estabID"
        Const QS_CONTRACTID As String = "contractID"
        Const QS_REFERENCE As String = "ref"
        Const QS_PERIODFROM As String = "periodfrom"
        Const QS_PERIODTO As String = "periodto"
        Const QS_VISITYES As String = "visityes"
        Const QS_VISITNO As String = "visitno"
        Const QS_PFNONE As String = "pfnone"
        Const QS_PFAWAIT As String = "pfawait"
        Const QS_PFVER As String = "pfver"
        Const QS_INVUNPAID As String = "invunpaid"
        Const QS_INVSUSP As String = "invsusp"
        Const QS_INVAUTH As String = "invauth"
        Const QS_INVPAID As String = "invpaid"
        Const QS_VARAWAIT As String = "varawait"
        Const QS_VARVER As String = "varver"
        Const QS_VARDEC As String = "vardec"
        Const QS_PSID As String = "psid"


        Private _fileID As Integer
        Private _providerID As Integer
        Private _contractID As Integer
        Private _reference As String
        Private _periodFrom As Nullable(Of DateTime)
        Private _periodTo As Nullable(Of DateTime)
        Private _visitYes As Boolean
        Private _visitNo As Boolean
        Private _pfNone As Boolean
        Private _pfAwait As Boolean
        Private _pfVer As Boolean
        Private _invUnpaid As Boolean
        Private _invSus As Boolean
        Private _invauth As Boolean
        Private _invPaid As Boolean
        Private _varAwait As Boolean
        Private _varVer As Boolean
        Private _varDec As Boolean
        Private _psId As Integer

        Private _required As Boolean
        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _description As String = "The list below displays the payment schedule enquiry."
        Private _defaultBatchStatus As DomProviderInvoiceStatus
        Private _enableRetractFeature As Boolean = False
        Private _userHasRetractCommand As Boolean

        Public Property UserHasRetractCommand() As Boolean
            Get
                Return _userHasRetractCommand
            End Get
            Set(ByVal value As Boolean)
                _userHasRetractCommand = value
            End Set
        End Property

        Public Property EnableRetractFeature() As Boolean
            Get
                Return _enableRetractFeature
            End Get
            Set(ByVal value As Boolean)
                _enableRetractFeature = value
            End Set
        End Property

        Public Property DefaultStatus() As DomProviderInvoiceStatus
            Get
                Return _defaultBatchStatus
            End Get
            Set(ByVal value As DomProviderInvoiceStatus)
                _defaultBatchStatus = value
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

        Public Property QueryString() As NameValueCollection Implements ISelectorWizardStep.QueryString
            Get
                Return New NameValueCollection(_queryString)
            End Get
            Set(ByVal value As NameValueCollection)
                _queryString = value
                ' pull out the required params
                If Utils.ToInt32(_queryString(QS_FILEID)) > 0 Then
                    _fileID = Utils.ToInt32(_queryString(QS_FILEID))
                Else
                    _fileID = 0
                End If
                _providerID = Utils.ToInt32(_queryString(QS_ESTABLISHMENTID))
                _contractID = Utils.ToInt32(_queryString(QS_CONTRACTID))
                _reference = Utils.ToString(_queryString(QS_REFERENCE))
                '' if valid date then load else start with minimum date
                If Utils.IsDate(_queryString(QS_PERIODFROM)) Then
                    _periodFrom = _queryString(QS_PERIODFROM)
                Else
                    _periodFrom = Date.Now.AddYears(-1).ToShortDateString()
                End If
                '' if valid date then load else start with max date
                If Utils.IsDate(_queryString(QS_PERIODTO)) Then
                    _periodTo = _queryString(QS_PERIODTO)
                Else
                    _periodTo = Date.MaxValue
                End If
                If (_queryString(QS_PFNONE) = Nothing) And _
                    (_queryString(QS_PFVER) = Nothing) And _
                    (_queryString(QS_PFAWAIT) = Nothing) And _
                    (_queryString(QS_INVUNPAID) = Nothing) And _
                    (_queryString(QS_INVPAID) = Nothing) And _
                    (_queryString(QS_INVAUTH) = Nothing) And _
                    (_queryString(QS_INVSUSP) = Nothing) And _
                    (_queryString(QS_VARAWAIT) = Nothing) And _
                    (_queryString(QS_VARVER) = Nothing) And _
                    (_queryString(QS_VARDEC) = Nothing) Then
                    _pfNone = True
                    _pfVer = True
                    _pfAwait = True
                    _invUnpaid = True
                    _invPaid = True
                    _invauth = True
                    _invSus = True
                    _varAwait = True
                    _varVer = True
                    _varDec = True
                Else
                    _pfNone = IIf(_queryString(QS_PFNONE) = "true", True, False)
                    _pfVer = IIf(_queryString(QS_PFVER) = "true", True, False)
                    _pfAwait = IIf(_queryString(QS_PFAWAIT) = "true", True, False)
                    _invPaid = IIf(_queryString(QS_INVPAID) = "true", True, False)
                    _invUnpaid = IIf(_queryString(QS_INVUNPAID) = "true", True, False)
                    _invauth = IIf(_queryString(QS_INVAUTH) = "true", True, False)
                    _invSus = IIf(_queryString(QS_INVSUSP) = "true", True, False)
                    _varAwait = IIf(_queryString(QS_VARAWAIT) = "true", True, False)
                    _varVer = IIf(_queryString(QS_VARVER) = "true", True, False)
                    _varDec = IIf(_queryString(QS_VARDEC) = "true", True, False)
                End If
                If (_queryString(QS_VISITYES) = Nothing And _
                     _queryString(QS_VISITNO) = Nothing) Then

                    _visitYes = True
                    _visitNo = True
                Else
                    _visitYes = IIf(_queryString(QS_VISITYES) = "true", True, False)
                    _visitNo = IIf(_queryString(QS_VISITNO) = "true", True, False)
                End If
                _psId = Utils.ToInt32(_queryString(QS_PSID))
            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls
            Dim msg As ErrorMessage = New ErrorMessage
            If Not _enableRetractFeature Then
                _userHasRetractCommand = False
            End If
            Dim invoiceList As PaymentScheduleSelector = wizard.LoadControl("~/AbacusExtranet/Apps/UserControls/PaymentScheduleSelector.ascx")
            invoiceList.InitControl(wizard.Page, _
                                    _psId, _
                                    _providerID, _
                                    _contractID, _
                                    _reference, _
                                    _periodFrom, _
                                    _periodTo, _
                                    _visitYes, _
                                    _visitNo, _
                                    _pfNone, _
                                    _pfAwait, _
                                    _pfVer, _
                                    _invUnpaid, _
                                    _invSus, _
                                    _invauth, _
                                    _invPaid, _
                                    _varAwait, _
                                    _varVer, _
                                    _varDec _
                                    )
            controls.Add(invoiceList)
            msg.Success = True
            Return msg
        End Function

        Public Function RenderHeaderControls(ByRef controls As ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage
            msg = New ErrorMessage()
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

    End Class

#End Region

#Region " DomProviderInvoiceVisitsEnquiryResultsStep "

    ''' <summary>
    ''' Wizard step to display the result of a domiciliary provider invoiced visits enquiry. 
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MikeVO  19/10/2009  D11546 - changed weekending filter to a date range.
    ''' </history>
    Public Class DomProviderInvoiceVisitsEnquiryResultsStep
        Implements ISelectorWizardStep

        Const QS_FILEID As String = "fileID"
        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_ESTABLISHMENTID As String = "estabID"
        Const QS_CONTRACTID As String = "contractID"
        Const QS_CLIENTID As String = "clientID"
        Const QS_CAREWORKERID As String = "careWorkerID"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETO As String = "dateTo"

        Private _fileID As Integer
        Private _providerID As Integer
        Private _contractID As Integer
        Private _clientID As Integer
        Private _careWorkerID As Integer
        Private _dateFrom As Date
        Private _dateTo As Date

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _deleted As Boolean
        Private _description As String = "The list below displays the results of your Invoiced Visits enquiry."
        Private _defaultBatchStatus As DomProviderInvoiceStatus

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

        Public Property QueryString() As NameValueCollection Implements ISelectorWizardStep.QueryString
            Get
                Return New NameValueCollection(_queryString)
            End Get
            Set(ByVal value As NameValueCollection)
                _queryString = value
                ' pull out the required params
                If Utils.ToInt32(_queryString(QS_FILEID)) > 0 Then
                    _fileID = Utils.ToInt32(_queryString(QS_FILEID))
                Else
                    _fileID = 0
                End If
                _providerID = Utils.ToInt32(_queryString(QS_ESTABLISHMENTID))
                _contractID = Utils.ToInt32(_queryString(QS_CONTRACTID))
                _clientID = Utils.ToInt32(_queryString(QS_CLIENTID))
                _careWorkerID = Utils.ToInt32(_queryString(QS_CAREWORKERID))
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETO)) Then _dateTo = _queryString(QS_DATETO)
            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls
            Dim msg As ErrorMessage = New ErrorMessage
            Dim visitList As InvoicedVisitsSelector = wizard.LoadControl("~/AbacusExtranet/Apps/UserControls/InvoicedVisitsSelector.ascx")

            With visitList
                .thePage = wizard.Page
                .providerID = _providerID
                .contractID = _contractID
                .svcUserID = _clientID
                .careWorkerID = _careWorkerID
                .dateFrom = _dateFrom
                .dateTo = _dateTo

            End With

            controls.Add(visitList)
            msg.Success = True
            Return msg
        End Function

        Public Function RenderHeaderControls(ByRef controls As ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage
            msg = New ErrorMessage()
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

    End Class

#End Region

#Region " VisitAmendmentRequestEnquiryResultsStep "

    Public Class VisitAmendmentRequestEnquiryResultsStep
        Implements ISelectorWizardStep

        Public Enum Originators
            Council = 1
            Provider = 2
            Both = 3
        End Enum

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_PROVIDERID As String = "estabID"
        Const QS_CONTRACTID As String = "contractID"
        Const QS_REQ_DATE_FROM As String = "reqDateFrom"
        Const QS_REQ_DATE_TO As String = "reqDateTo"
        Const QS_REQUESTEDBY As String = "reqByID"
        Const QS_STATUS As String = "status"
        Const QS_STATUS_DATE_FROM As String = "dateFrom"
        Const QS_STATUS_DATE_TO As String = "dateTo"
        Const QS_ORIGINATOR As String = "originator"
        Const QS_REQUESTEDBY_COMPANY As String = "reqbycompanyid"
        Const QS_CLIENTID As String = "clientid"

        Private _providerID As Integer
        Private _contractID As Integer
        Private _originator As Originators
        Private _requestDateFrom As Date
        Private _requestDateTo As Date
        Private _reqByID As Integer
        Private _reqByCompanyID As Integer
        Private _status As DomProviderInvoiceVisitAmendmentStatus
        Private _statusDateFrom As Date
        Private _statusDateTo As Date

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _deleted As Boolean
        Private _description As String = "The list below displays the results of your Visit Amendment Request enquiry. "
        Private _isCouncilUser As Boolean
        Private _clientId As Integer

        Sub New()
            _originator = Originators.Both
            _status = DomProviderInvoiceVisitAmendmentStatus.AwaitingVerification Or DomProviderInvoiceVisitAmendmentStatus.Declined Or _
                DomProviderInvoiceVisitAmendmentStatus.Verified Or DomProviderInvoiceVisitAmendmentStatus.Processed Or _
                DomProviderInvoiceVisitAmendmentStatus.Invoiced
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

        Public Property QueryString() As NameValueCollection Implements ISelectorWizardStep.QueryString
            Get
                Return New NameValueCollection(_queryString)
            End Get
            Set(ByVal value As NameValueCollection)
                _queryString = value
                _providerID = Utils.ToInt32(_queryString(QS_PROVIDERID))
                _contractID = Utils.ToInt32(_queryString(QS_CONTRACTID))
                _clientId = Utils.ToInt32(_queryString(QS_CLIENTID))
                If Not _queryString(QS_STATUS) Is Nothing Then _status = Utils.ToInt32(_queryString(QS_STATUS))
                _reqByID = Utils.ToInt32(_queryString(QS_REQUESTEDBY))
                _reqByCompanyID = Utils.ToInt32(_queryString(QS_REQUESTEDBY_COMPANY))
                If Not _queryString(QS_ORIGINATOR) Is Nothing Then _originator = Utils.ToInt32(_queryString(QS_ORIGINATOR))
                If Utils.IsDate(_queryString(QS_REQ_DATE_FROM)) Then _requestDateFrom = _queryString(QS_REQ_DATE_FROM)
                If Utils.IsDate(_queryString(QS_REQ_DATE_TO)) Then _requestDateTo = _queryString(QS_REQ_DATE_TO)
                If Utils.IsDate(_queryString(QS_STATUS_DATE_FROM)) Then _statusDateFrom = _queryString(QS_STATUS_DATE_FROM)
                If Utils.IsDate(_queryString(QS_STATUS_DATE_TO)) Then _statusDateTo = _queryString(QS_STATUS_DATE_TO)
            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls
            Dim msg As ErrorMessage = New ErrorMessage
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim reqCompanyID As Integer
            Dim reqUserID As Integer
            Dim amendmentList As VisitAmendmentRequestSelector = wizard.LoadControl("~/AbacusExtranet/Apps/UserControls/VisitAmendmentRequestSelector.ascx")

            'If Me.IsCouncilUser Then
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(Me.DbConnection.ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)
            Me.IsCouncilUser = SecurityBL.IsCouncilUser(Me.DbConnection, settings, user.ExternalUserID)
            If Me.IsCouncilUser Then
                'Council user
                If _originator = Originators.Council Then
                    reqCompanyID = 0
                    reqUserID = _reqByID
                ElseIf _originator = Originators.Provider Then
                    reqCompanyID = _reqByCompanyID
                    reqUserID = 0
                Else
                    reqCompanyID = 0
                    reqUserID = 0
                End If

            Else
                'Must be a provider
                If _originator = Originators.Provider Then
                    reqCompanyID = 0
                    reqUserID = _reqByID
                Else
                    reqCompanyID = 0
                    reqUserID = 0
                End If
            End If

            'amendmentList.InitControl(wizard.Page, _
            '                          _providerID, _
            '                          _contractID, _
            '                          _requestDateFrom, _
            '                          _requestDateTo, _
            '                          _status, _
            '                          _statusDateFrom, _
            '                          _statusDateTo, _
            '                          reqCompanyID, _
            '                          reqUserID, _
            '                          _clientId, _
            '                          _originator)
            With amendmentList
                .thePage = wizard.Page
                .providerID = _providerID
                .contractID = _contractID
                .requestDateFrom = _requestDateFrom
                .requestDateTo = _requestDateTo
                .status = _status
                .statusDateFrom = _statusDateFrom
                .statusDateTo = _statusDateTo
                .reqCompanyID = reqCompanyID
                .reqUserID = reqUserID
                .serviceUserID = _clientId
                .Originator = _originator
            End With

            controls.Add(amendmentList)
            msg.Success = True
            Return msg

        End Function

        Public Function RenderHeaderControls(ByRef controls As ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage
            msg = New ErrorMessage()
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

        Public Property IsCouncilUser() As Boolean
            Get
                Return _isCouncilUser
            End Get
            Set(ByVal value As Boolean)
                _isCouncilUser = value
            End Set
        End Property

    End Class

#End Region

#Region " GenericServiceOrderResultsStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.Apps.UserControls.SelectorWizardSteps.GenericServiceOrderResultsStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Implements the Generic Service Order results wizard step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[PaulW]	13/10/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Class GenericServiceOrderResultsStep
        Implements ISelectorWizardStep

        Const QS_ESTABLISHMENTID As String = "estabID"
        Const QS_CONTRACTID As String = "contractID"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETO As String = "dateTo"
        Const QS_MOVEMENT As String = "movement"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _establishmentID As Integer
        Private _contractID As Integer
        Private _dsoID As Integer
        Private _dateFrom As Date
        Private _dateTo As Date
        Private _movement As Integer = 0
        Private _description As String = "The list below displays the results of your payment enquiry."

        Public Property BaseUrl() As String Implements ISelectorWizardStep.BaseUrl
            Get
                Return _baseUrl
            End Get
            Set(ByVal Value As String)
                _baseUrl = Value
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
                _establishmentID = Utils.ToInt32(_queryString(QS_ESTABLISHMENTID))
                _contractID = Utils.ToInt32(_queryString(QS_CONTRACTID))
                'If Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                'If Utils.IsDate(_queryString(QS_DATETO)) Then _dateTo = _queryString(QS_DATETO)

                Using drs As New DateRangeStep()
                    With drs
                        .QueryString = Me.QueryString
                        _dateFrom = .DateFrom
                        _dateTo = .DateTo
                        If Not Utils.IsDate(_dateTo) Then
                            _dateTo = Date.Today
                        End If
                    End With
                End Using

                _movement = Utils.ToInt32(_queryString(QS_MOVEMENT))
            End Set
        End Property

        Public Property Required() As Boolean Implements ISelectorWizardStep.Required
            Get
                Return _required
            End Get
            Set(ByVal Value As Boolean)
                _required = Value
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

        Public Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep

        End Sub

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

        End Sub

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls
            Dim msg As ErrorMessage = New ErrorMessage
            Dim dsoList As GenericServiceOrderSelector = wizard.LoadControl("~/AbacusExtranet/Apps/UserControls/GenericServiceOrderSelector.ascx")
            dsoList.thePage = wizard.Page
            dsoList.establishmentId = _establishmentID
            dsoList.contractId = _contractID
            dsoList.dateFrom = _dateFrom
            dsoList.dateTo = _dateTo
            dsoList.movement = _movement
            dsoList.selectedServiceOrderId = _dsoID

            controls.Add(dsoList)
            msg.Success = True
            Return msg
        End Function

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage = New ErrorMessage
            msg.Success = True
            Return msg
        End Function


    End Class

#End Region

#Region " DateRangeDSOMovementStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : UserControls.DateRangeDSOMovementStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Class that implements the date range and DSO movement step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[PaulW]	14/10/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DateRangeDSOMovementStep
        Inherits DateRangeStep

        Const QS_MOVEMENT As String = "movement"
        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_ESTAB As String = "estabID"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer

        Private _movement As Int16
        Private _estabID As Int32
        Private _movementControl As RadioButtonList = New RadioButtonList
        Private _required As Boolean
        Private _description As String = "Please select a date range and movement to filter the results on."
        Private _currentStep As Integer

        Sub New()
            MyBase.new()
            MyBase.DateFrom = Date.Today
            MyBase.DateTo = Date.Today
            MyBase.DateToControl.MinimumValue = Date.Today
            MyBase.DateToControl.MaximumValue = Date.Today.AddMonths(1)
            MyBase.UseJquery = True
            MyBase.ShowHeaderLink = False
            MyBase.AllowChangeDateLinkText = True
            MyBase.DateLinkTextFirststep = "Change Date"
        End Sub

        Public Overrides Property Description() As String
            Get
                Return _description
            End Get
            Set(ByVal Value As String)
                _description = Value
            End Set
        End Property

        Public Overrides Property QueryString() As System.Collections.Specialized.NameValueCollection
            Get
                Return MyBase.QueryString
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                MyBase.QueryString = Value
                _queryString = Value
                _movement = Target.Library.Utils.ToInt32(_queryString(QS_MOVEMENT))
                _estabID = Target.Library.Utils.ToInt32(_queryString(QS_ESTAB))
                _currentStep = Utils.ToInt32(_queryString(QS_CURRENTSTEP))
            End Set
        End Property

        Public Overrides ReadOnly Property BeforeNavigateJS() As String
            Get
                Return "DateRangeMovementStep_BeforeNavigate()"
            End Get
        End Property

        Public Overrides Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage

            Dim msg As ErrorMessage = New ErrorMessage
            Dim spacerBr As Literal
            Dim fsPeriod As New HtmlGenericControl("fieldset")
            Dim fsMovement As New HtmlGenericControl("fieldset")
            Dim legend1 As New HtmlGenericControl("legend")
            Dim legend2 As New HtmlGenericControl("legend")
            Dim lblMax As New HtmlGenericControl("label")

            spacerBr = New Literal
            spacerBr.Text = "<div style='height:4px;'></div>"
            controls.Add(spacerBr)

            legend1.InnerText = "Period"
            fsPeriod.Controls.Add(legend1)

            msg = MyBase.RenderContentControls(wizard, fsPeriod.Controls)
            If Not msg.Success Then
                Return msg
            End If

            MyBase.DateToControl.MinimumValue = CDate(MyBase.DateToControl.Text)
            MyBase.DateToControl.MaximumValue = CDate(MyBase.DateToControl.Text).AddMonths(1)

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            fsPeriod.Controls.Add(spacerBr)

            lblMax.InnerText = "maximum of 1 month"
            lblMax.Attributes.CssStyle("font-style") = "italic"
            lblMax.Attributes.CssStyle("padding-left") = "10em"
            lblMax.Attributes.Add("class", "warningText")
            fsPeriod.Controls.Add(lblMax)

            controls.Add(fsPeriod)

            spacerBr = New Literal
            spacerBr.Text = "<div style='height:4px;'></div>"
            controls.Add(spacerBr)

            legend2.InnerText = "Movement"
            fsMovement.Controls.Add(legend2)

            With _movementControl
                .ID = "optMovement"
                .Items.Add("Show all service orders open in the period")
                .Items.Add("Show service orders that start in the period")
                .Items.Add("Show service orders that end in the period")
                .Items.Add("Show service orders that start and end in the period")
                .CssClass = "chkBoxStyle"
            End With
            fsMovement.Controls.Add(_movementControl)
            controls.Add(fsMovement)

            _movementControl.Items(_movement).Selected = True

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            msg.Success = True
            Return msg

        End Function

        Public Overrides Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage

            Dim msg As ErrorMessage
            Dim spacerBr As Literal
            Dim lbl As Label = New Label
            Dim text As Label = New Label
            Dim link As HyperLink = New HyperLink
            Dim qs As NameValueCollection
            

            'if a home has been selected show the date range link
            If _estabID <> 0 Then
                MyBase.ShowHeaderLink = True
            End If

            '' date Range custom Step

            msg = MyBase.RenderHeaderControls(controls)
            If Not msg.Success Then
                Return msg
            End If

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            ' label
            lbl.Text = "Movement"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", "8em")
            controls.Add(lbl)

            text.Style.Add("float", "left")
            text.Style.Add("padding-left", "2em")
            Select Case _movement
                Case 0
                    text.Text = "Show all service orders open in the period"
                Case 1
                    text.Text = "Show service orders that start in the period"
                Case 2
                    text.Text = "Show service orders that end in the period"
                Case 3
                    text.Text = "Show service orders that start and end in the period"
            End Select
            controls.Add(text)

            ' add the link
            qs = Me.QueryString
            If Me.IsCurrentStep AndAlso _movement = 0 Then
                ' all
                link.Text = "All Movement"
                qs.Remove(QS_MOVEMENT)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change Movement"
                qs.Remove(QS_CURRENTSTEP)
                qs.Add(QS_CURRENTSTEP, MyBase.StepIndex)
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

        Public Overrides ReadOnly Property Title() As String
            Get
                Return "Select a Date Range and Movement"
            End Get
        End Property

        Public Overrides Sub PreRender(ByVal wizard As SelectorWizardBase)

            wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.SP.Apps.UserControls.SelectorWizard.DateRangeMovementStep.Startup", _
                Target.Library.Web.Utils.WrapClientScript( _
                    String.Format("SelectorWizard_dateFromID=""{0}"";SelectorWizard_dateToID=""{1}"";DateRangeStep_required={2};SelectorWizard_movementID=""{3}"";", _
                        MyBase.DateFromControl.ClientID, MyBase.DateToControl.ClientID, MyBase.Required.ToString().ToLower(), _movementControl.ClientID) _
                ) _
            )

        End Sub

        Public Overrides Sub InitStep(ByVal wizard As SelectorWizardBase)
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/UserControls/DateRangeDSOMovementStep.js"))
        End Sub

    End Class

#End Region

#Region " ServiceOrderHiddenReportsStep "

    ''' <summary>
    ''' Implements the hidden reports step for the service order wizard.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO    25/09/2009  D11546 - changes to "selections" section links
    '''     MikeVO    23/09/2009  D11701 - added support for order-level reporting.
    ''' </history>
    Friend Class ServiceOrderHiddenReportsStep
        Implements ISelectorWizardStep, ISelectorWizardHiddenEndStep

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_DSOID As String = "dsoID"
        Const QS_ESTABLISHMENTID As String = "estabID"
        Const QS_CONTRACTID As String = "contractID"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETO As String = "dateTo"
        Const QS_MOVEMENT As String = "movement"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _description As String = "The available service order reports can be accessed below. The selections you have made above are applied to these reports."
        Private _dsoID As Integer
        Private _estabID As Integer
        Private _contractID As Integer
        Private _movement As Int16
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
                ' pull out the required params
                _dsoID = Utils.ToInt32(_queryString(QS_DSOID))
                _estabID = Utils.ToInt32(_queryString(QS_ESTABLISHMENTID))
                _contractID = Utils.ToInt32(_queryString(QS_CONTRACTID))
                _movement = Utils.ToInt32(_queryString(QS_MOVEMENT))

                Using drs As New DateRangeStep()
                    With drs
                        .QueryString = Me.QueryString
                        _dateFrom = .DateFrom
                        _dateTo = .DateTo
                        If Not Utils.IsDate(_dateTo) Then
                            _dateTo = Date.Today
                        End If
                    End With
                End Using
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
                Return "Reports"
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
            Dim orderReports As ServiceOrderReports = wizard.LoadControl("~/AbacusExtranet/Apps/UserControls/ServiceOrderReports.ascx")
            orderReports.ThePage = wizard.Page
            orderReports.EstablishmentID = _estabID
            orderReports.ContractID = _contractID
            orderReports.DateFrom = _dateFrom
            orderReports.DateTo = _dateTo
            orderReports.Movement = _movement
            orderReports.DsoID = _dsoID
            controls.Add(orderReports)
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