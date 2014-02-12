
Imports System.Collections.Specialized
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Text
Imports System.Web.UI.WebControls
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps
Imports Target.SP.Library.SPClassesBL

Namespace Apps.UserControls.SelectorWizardSteps

#Region " ProviderStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.SelectorWizard.ProviderStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Implements the select provider step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      10/10/2006  Added Required() property.
    ''' 	[Mikevo]	06/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Friend Class ProviderStep
        Implements ISelectorWizardStep

        Const QS_PROVIDERID As String = "providerID"
        Const QS_CURRENTSTEP As String = "currentStep"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _providerID As Integer
        Private _required As Boolean
        Private _allowViewProvider As Boolean
        Private _description As String = "Please select a provider from the list below and then click ""Next""."

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
                Return _queryString
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                _queryString = Value
                ' pull out the required params
                _providerID = Utils.ToInt32(_queryString(QS_PROVIDERID))
            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

            Dim msg As ErrorMessage = New ErrorMessage
            Dim providerList As ProviderSelector = wizard.LoadControl("~/SPWeb/Apps/UserControls/ProviderSelector.ascx")
            providerList.InitControl(wizard.Page, _allowViewProvider, _providerID)
            controls.Add(providerList)
            msg.Success = True
            Return msg

        End Function

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls

            Const SP_NAME As String = "pr_FetchSPProvider"

            Dim msg As ErrorMessage
            Dim lbl As Label = New Label
            Dim text As Label = New Label
            Dim link As HyperLink = New HyperLink
            Dim reader As SqlDataReader = Nothing
            Dim spParams As SqlParameter()

            ' label
            lbl.Text = "Provider"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", "8em")
            controls.Add(lbl)

            text.Style.Add("float", "left")
            If _providerID = 0 Then
                text.Text = "All"
            Else
                Try
                    ' get the provider details
                    spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME, False)
                    spParams(0).Value = _providerID
                    reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams)
                    If (reader.HasRows) Then
                        reader.Read()
                        text.Text = String.Format("{0}: {1}", reader("Reference"), reader("Name"))
                    End If

                Catch ex As Exception
                    msg = Utils.CatchError(ex, "E0001")     ' unexpected
                    Return msg
                Finally
                    If Not reader Is Nothing Then reader.Close()
                End Try

            End If
            controls.Add(text)

            If _providerID > 0 Then
                ' add the link
                link.Style.Add("float", "right")
                If Me.IsCurrentStep Then
                    link.Text = "All Providers"
                    Me.QueryString.Remove(QS_PROVIDERID)
                    link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(Me.QueryString))
                Else
                    link.Text = "Change Provider"
                    Me.QueryString.Remove(QS_CURRENTSTEP)
                    Me.QueryString.Add(QS_CURRENTSTEP, Me.StepIndex)
                    link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(Me.QueryString))
                End If
                controls.Add(link)
            End If

            msg = New ErrorMessage
            msg.Success = True

            Return msg

        End Function

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
                Return "Select a Provider"
            End Get
        End Property

        Public ReadOnly Property BeforeNavigateJS() As String Implements ISelectorWizardStep.BeforeNavigateJS
            Get
                Return "ProviderStep_BeforeNavigate()"
            End Get
        End Property

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

            wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.SP.Apps.UserControls.SelectorWizard.ProviderStep.Startup", _
                Target.Library.Web.Utils.WrapClientScript( _
                    String.Format("ProviderStep_required={0};", _required.ToString().ToLower()) _
                ) _
            )

        End Sub

        Public Property Required() As Boolean Implements ISelectorWizardStep.Required
            Get
                Return _required
            End Get
            Set(ByVal Value As Boolean)
                _required = Value
            End Set
        End Property

        Public Property AllowViewProvider() As Boolean
            Get
                Return _allowViewProvider
            End Get
            Set(ByVal Value As Boolean)
                _allowViewProvider = Value
            End Set
        End Property

        Public Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep

        End Sub

    End Class

#End Region

#Region " ServiceStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.UserControls.SelectorWizardSteps.ServiceStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Implements the select service step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      10/10/2006  Added Required() property.
    ''' 	[Mikevo]	06/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Friend Class ServiceStep
        Implements ISelectorWizardStep

        Const QS_PROVIDERID As String = "providerID"
        Const QS_SERVICEID As String = "serviceID"
        Const QS_CURRENTSTEP As String = "currentStep"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _providerID As Integer
        Private _serviceID As Integer
        Private _required As Boolean
        Private _headerLink As HyperLink
        Private _showHeaderLink As Boolean = True
        Private _allowViewService As Boolean
        Private _description As String = "Please select a service from the list below and then click ""Next""."

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets whether the header link should be displayed.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	16/10/2006	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Property ShowHeaderLink() As Boolean
            Get
                Return _showHeaderLink
            End Get
            Set(ByVal Value As Boolean)
                _showHeaderLink = Value
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
                Return _queryString
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                _queryString = Value
                ' pull out the required params
                _providerID = Utils.ToInt32(_queryString(QS_PROVIDERID))
                _serviceID = Utils.ToInt32(_queryString(QS_SERVICEID))
            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

            Dim msg As ErrorMessage = New ErrorMessage
            Dim serviceList As ServiceSelector = wizard.LoadControl("~/SPWeb/Apps/UserControls/ServiceSelector.ascx")
            serviceList.InitControl(wizard.Page, _allowViewService, _providerID, _serviceID)
            controls.Add(serviceList)
            msg.Success = True
            Return msg

        End Function

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls

            Const SP_NAME As String = "pr_FetchSPService"

            Dim msg As ErrorMessage
            Dim lbl As Label = New Label
            Dim text As Label = New Label
            Dim reader As SqlDataReader = Nothing
            Dim spParams As SqlParameter()

            ' label
            lbl.Text = "Service"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", "8em")
            controls.Add(lbl)

            text.Style.Add("float", "left")
            If _serviceID = 0 Then
                text.Text = "All"
            Else
                Try
                    ' get the provider details
                    spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME, False)
                    spParams(0).Value = _serviceID
                    reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams)

                    If reader.HasRows Then
                        reader.Read()
                        text.Text = String.Format("{0}: {1}", reader("Reference"), reader("Name"))
                    End If

                Catch ex As Exception
                    msg = Utils.CatchError(ex, "E0001")     ' unexpected
                    Return msg
                Finally
                    If Not reader Is Nothing Then reader.Close()
                End Try

            End If
            controls.Add(text)

            If _serviceID > 0 AndAlso _showHeaderLink Then
                ' add the link
                _headerLink = New HyperLink
                _headerLink.Style.Add("float", "right")
                If Me.IsCurrentStep Then
                    _headerLink.Text = "All Services"
                    Me.QueryString.Remove(QS_SERVICEID)
                    _headerLink.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(Me.QueryString))
                Else
                    _headerLink.Text = "Change Service"
                    Me.QueryString.Remove(QS_CURRENTSTEP)
                    Me.QueryString.Add(QS_CURRENTSTEP, Me.StepIndex)
                    _headerLink.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(Me.QueryString))
                End If
                controls.Add(_headerLink)
            End If

            msg = New ErrorMessage
            msg.Success = True

            Return msg

        End Function

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
                Return "Select a Service"
            End Get
        End Property

        Public ReadOnly Property BeforeNavigateJS() As String Implements ISelectorWizardStep.BeforeNavigateJS
            Get
                Return "ServiceStep_BeforeNavigate()"
            End Get
        End Property

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

            wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.SP.Apps.UserControls.SelectorWizard.ServiceStep.Startup", _
                Target.Library.Web.Utils.WrapClientScript( _
                    String.Format("ServiceStep_required={0};", _required.ToString().ToLower()) _
                ) _
            )

        End Sub

        Public Property Required() As Boolean Implements ISelectorWizardStep.Required
            Get
                Return _required
            End Get
            Set(ByVal Value As Boolean)
                _required = Value
            End Set
        End Property

        Public Property AllowViewService() As Boolean
            Get
                Return _allowViewService
            End Get
            Set(ByVal Value As Boolean)
                _allowViewService = Value
            End Set
        End Property

        Public Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep

        End Sub

    End Class

#End Region

#Region " ClientStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.UserControls.SelectorWizardSteps.ClientStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Implements the select service user step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[Mikevo]	10/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Friend Class ClientStep
        Implements ISelectorWizardStep

        Const QS_SERVICEID As String = "serviceID"
        Const QS_CLIENTID As String = "clientID"
        Const QS_CURRENTSTEP As String = "currentStep"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _clientID As Integer
        Private _serviceID As Integer
        Private _required As Boolean
        Private _description As String = "Please select a service user from the list below and then click ""Next""."

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
                Return "ClientStep_BeforeNavigate()"
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

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

            wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.SP.Apps.UserControls.SelectorWizard.ClientStep.Startup", _
                Target.Library.Web.Utils.WrapClientScript( _
                    String.Format("ClientStep_required={0};", _required.ToString().ToLower()) _
                ) _
            )

        End Sub

        Public Property QueryString() As System.Collections.Specialized.NameValueCollection Implements ISelectorWizardStep.QueryString
            Get
                Return _queryString
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                _queryString = Value
                ' pull out the required params
                _serviceID = Utils.ToInt32(_queryString(QS_SERVICEID))
                _clientID = Utils.ToInt32(_queryString(QS_CLIENTID))
            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

            Dim msg As ErrorMessage = New ErrorMessage
            Dim clientList As ClientSelector = wizard.LoadControl("~/SPWeb/Apps/UserControls/ClientSelector.ascx")
            clientList.InitControl(wizard.Page, _serviceID, _clientID)
            controls.Add(clientList)
            msg.Success = True
            Return msg

        End Function

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls

            Const SP_NAME As String = "pr_FetchClientDetail"

            Dim msg As ErrorMessage
            Dim lbl As Label = New Label
            Dim text As Label = New Label
            Dim link As HyperLink = New HyperLink
            Dim reader As SqlDataReader = Nothing
            Dim spParams As SqlParameter()

            ' label
            lbl.Text = "Service User"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", "8em")
            controls.Add(lbl)

            text.Style.Add("float", "left")
            If _clientID = 0 Then
                text.Text = "All"
            Else
                Try
                    ' get the provider details
                    spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME, False)
                    spParams(0).Value = _clientID
                    reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams)

                    If reader.HasRows Then
                        reader.Read()
                        text.Text = String.Format("{0}: {1}", reader("Reference"), reader("Name"))
                    End If

                Catch ex As Exception
                    msg = Utils.CatchError(ex, "E0001")     ' unexpected
                    Return msg
                Finally
                    If Not reader Is Nothing Then reader.Close()
                End Try

            End If
            controls.Add(text)

            If _clientID > 0 Then
                ' add the link
                link.Style.Add("float", "right")
                If Me.IsCurrentStep Then
                    link.Text = "All Service Users"
                    Me.QueryString.Remove(QS_CLIENTID)
                    link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(Me.QueryString))
                Else
                    link.Text = "Change Service User"
                    Me.QueryString.Remove(QS_CURRENTSTEP)
                    Me.QueryString.Add(QS_CURRENTSTEP, Me.StepIndex)
                    link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(Me.QueryString))
                End If
                controls.Add(link)
            End If

            msg = New ErrorMessage
            msg.Success = True

            Return msg

        End Function

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
                Return "Select a Service User"
            End Get
        End Property

        Public Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep

        End Sub

    End Class

#End Region

#Region " PaymentEnquiryResultsStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.UserControls.SelectorWizardSteps.PaymentEnquiryResultsStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Class that implements the payment enquiry results step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      10/10/2006  Added Required() property.
    ''' 	[Mikevo]	06/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Friend Class PaymentEnquiryResultsStep
        Implements ISelectorWizardStep

        Const QS_PROVIDERID As String = "providerID"
        Const QS_SERVICEID As String = "serviceID"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETO As String = "dateTo"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _providerID As Integer
        Private _serviceID As Integer
        Private _dateFrom As Date
        Private _dateTo As Date
        Private _required As Boolean
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
                Return "true"
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
                Return _queryString
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                _queryString = Value
                ' pull out the required params
                _providerID = Utils.ToInt32(_queryString(QS_PROVIDERID))
                _serviceID = Utils.ToInt32(_queryString(QS_SERVICEID))
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETO)) Then _dateTo = _queryString(QS_DATETO)
            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

            Dim msg As ErrorMessage = New ErrorMessage
            Dim remittanceList As RemittanceSelector = wizard.LoadControl("~/SPWeb/Apps/UserControls/RemittanceSelector.ascx")
            remittanceList.InitControl(wizard.Page, _providerID, _serviceID, _dateFrom, _dateTo)
            controls.Add(remittanceList)
            msg.Success = True
            Return msg

        End Function

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage = New ErrorMessage
            msg.Success = True
            Return msg
        End Function

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

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

        End Sub

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

    End Class

#End Region

#Region " SUPaymentEnquiryResultsStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.UserControls.SelectorWizardSteps.SUPaymentEnquiryResultsStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Class that implements the service user payment enquiry results step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[Mikevo]	11/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Friend Class SUPaymentEnquiryResultsStep
        Implements ISelectorWizardStep

        Const QS_PROVIDERID As String = "providerID"
        Const QS_SERVICEID As String = "serviceID"
        Const QS_CLIENTID As String = "clientID"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETO As String = "dateTo"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _serviceID As Integer
        Private _clientID As Integer
        Private _dateFrom As Date
        Private _dateTo As Date
        Private _required As Boolean
        Private _description As String = "The list below displays the results of your service user payment enquiry."

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
                Return "true"
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
                Return _queryString
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                _queryString = Value
                ' pull out the required params
                _serviceID = Utils.ToInt32(_queryString(QS_SERVICEID))
                _clientID = Utils.ToInt32(_queryString(QS_CLIENTID))
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETO)) Then _dateTo = _queryString(QS_DATETO)
            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

            Dim msg As ErrorMessage = New ErrorMessage
            Dim detailLineList As RemittanceDetailSelector = wizard.LoadControl("~/SPWeb/Apps/UserControls/RemittanceDetailSelector.ascx")
            detailLineList.InitControl(wizard.Page, _serviceID, _clientID, _dateFrom, _dateTo)
            controls.Add(detailLineList)
            msg.Success = True
            Return msg

        End Function

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage = New ErrorMessage
            msg.Success = True
            Return msg
        End Function

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

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

        End Sub

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

    End Class

#End Region

#Region " ProviderPaymentsResultsStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.UserControls.SelectorWizardSteps.ProviderPaymentsResultsStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Class that implements the provider payments results step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[Mikevo]	11/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Friend Class ProviderPaymentsResultsStep
        Implements ISelectorWizardStep

        Const QS_PROVIDERID As String = "providerID"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETO As String = "dateTo"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _providerID As Integer
        Private _dateFrom As Date
        Private _dateTo As Date
        Private _required As Boolean
        Private _description As String = "The list below displays the results of your payment interface enquiry."

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
                Return "true"
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
                Return _queryString
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                _queryString = Value
                ' pull out the required params
                _providerID = Utils.ToInt32(_queryString(QS_PROVIDERID))
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETO)) Then _dateTo = _queryString(QS_DATETO)
            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

            Dim msg As ErrorMessage = New ErrorMessage
            Dim interfaceFileList As ProviderInterfaceFileSelector = wizard.LoadControl("~/SPWeb/Apps/UserControls/ProviderInterfaceFileSelector.ascx")
            interfaceFileList.InitControl(wizard.Page, _providerID, _dateFrom, _dateTo)
            controls.Add(interfaceFileList)
            msg.Success = True
            Return msg

        End Function

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage = New ErrorMessage
            msg.Success = True
            Return msg
        End Function

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

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

        End Sub

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

    End Class

#End Region

#Region " NewSUNotifEnterDetailsStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.UserControls.SelectorWizardSteps.NewSUNotifEnterDetailsStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Class that implements the new service user notification enter details step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[Mikevo]	16/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Friend Class NewSUNotifEnterDetailsStep
        Implements ISelectorWizardStep

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _enterDetailsControl As NewSUNotifEnterDetails
        Private _description As String = "Please enter the required details below and then click on ""Next""."

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
                Return "NewSUNotifEndDetailsStep_BeforeNavigate()"
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

        Public Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("SPWeb/Apps/UserControls/NewSUNotif.js"))
        End Sub

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

        End Sub

        Public Property QueryString() As System.Collections.Specialized.NameValueCollection Implements ISelectorWizardStep.QueryString
            Get
                Return _queryString
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                _queryString = Value
            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

            Dim msg As ErrorMessage = New ErrorMessage
            _enterDetailsControl = wizard.LoadControl("~/SPWeb/Apps/UserControls/NewSUNotifEnterDetails.ascx")
            _enterDetailsControl.ID = "ctlDetails"
            _enterDetailsControl.InitControl(wizard.Page)
            controls.Add(_enterDetailsControl)
            msg.Success = True
            Return msg

        End Function

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage = New ErrorMessage
            msg.Success = True
            Return msg
        End Function

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
                Return "Enter Service/Service User Details"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets the enter details user control.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	20/10/2006	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Property EnterDetailsControl() As NewSUNotifEnterDetails
            Get
                Return _enterDetailsControl
            End Get
            Set(ByVal Value As NewSUNotifEnterDetails)
                _enterDetailsControl = Value
            End Set
        End Property

    End Class

#End Region

#Region " NewSUNotifPrintDocStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.UserControls.SelectorWizardSteps.NewSUNotifPrintDocStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Class that implements the new service user notification print document step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[Mikevo]	16/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Friend Class NewSUNotifPrintDocStep
        Implements ISelectorWizardStep

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _description As String = _
            "The details you have entered have been saved.<br /><br />" & _
            "Please <a href=""javascript:NewSUNotifPrintDocStep_ViewNotif();"">print these details now</a> and obtain the required signatures from the service user.<br /><br />" & _
            "Once the signatures have been obtained, scan the document using the scanner attached to your computer and return to this screen through the <a href=""ListSUNotif.aspx"">Service User Notifications screen</a>. " & _
            "Note: if you do not have a scanner, you may be permitted to fax or post the printed document instead.<br /><br />" & _
            "Click on the ""Next"" button below to continue."

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
                Return "NewSUNotif_BeforeNavigate()"
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

        Public Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("SPWeb/Apps/UserControls/NewSUNotif.js"))
        End Sub

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

        End Sub

        Public Property QueryString() As System.Collections.Specialized.NameValueCollection Implements ISelectorWizardStep.QueryString
            Get
                Return _queryString
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                _queryString = Value
            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls
            Dim msg As ErrorMessage = New ErrorMessage
            msg.Success = True
            Return msg
        End Function

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage = New ErrorMessage
            msg.Success = True
            Return msg
        End Function

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
                Return "Print Notification"
            End Get
        End Property

    End Class

#End Region

#Region " NewSUNotifAttachDocStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.UserControls.SelectorWizardSteps.NewSUNotifAttachDocStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Class that implements the new service user notification attach document step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[Mikevo]	16/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Friend Class NewSUNotifAttachDocStep
        Implements ISelectorWizardStep

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _description As String = "Please click on the ""Browse..."" button below to locate and select the scanned notification document. Then click on the ""Finish"" button below to complete this notification."

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
                Return "NewSUNotifAttachDocStep_BeforeNavigate()"
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

        Public Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("SPWeb/Apps/UserControls/NewSUNotif.js"))
        End Sub

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

        End Sub

        Public Property QueryString() As System.Collections.Specialized.NameValueCollection Implements ISelectorWizardStep.QueryString
            Get
                Return _queryString
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                _queryString = Value
            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

            Dim msg As ErrorMessage = New ErrorMessage
            Dim upload As Target.Library.Web.Controls.FileUpload = New Target.Library.Web.Controls.FileUpload

            With upload
                .ID = "scannedNotifForm"
                .Caption = "Signed & Scanned Notification Form"
                .MaxFiles = 1
                .Width = "50%"
            End With
            controls.Add(New HtmlGenericControl("br"))
            controls.Add(upload)
            controls.Add(New HtmlGenericControl("br"))

            msg.Success = True
            Return msg

        End Function

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage = New ErrorMessage
            msg.Success = True
            Return msg
        End Function

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
                Return "Attach Notification"
            End Get
        End Property

    End Class

#End Region

#Region " NewSUNotifCompleteStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.UserControls.SelectorWizardSteps.NewSUNotifCompleteStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Class that implements the new service user notification complete step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[Mikevo]	16/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Friend Class NewSUNotifCompleteStep
        Implements ISelectorWizardStep

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _description As String = "Thank you. Your New Service User Notification has been successfully submitted for processing.<br /><br />You may track its progress using the <a href=""ListSUNotif.aspx"">Service User Notifications screen</a>."

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
                Return "true"
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

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

        End Sub

        Public Property QueryString() As System.Collections.Specialized.NameValueCollection Implements ISelectorWizardStep.QueryString
            Get
                Return _queryString
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                _queryString = Value
            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls
            Dim msg As ErrorMessage = New ErrorMessage
            msg.Success = True
            Return msg
        End Function

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage = New ErrorMessage
            msg.Success = True
            Return msg
        End Function

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
                Return "Complete"
            End Get
        End Property

        Public Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("SPWeb/Apps/UserControls/NewSUNotif.js"))
        End Sub

    End Class

#End Region

#Region " SubsidyEnquiryResultsStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.UserControls.SelectorWizardSteps.SubsidyEnquiryResultsStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Class that implements the Subsidy enquiry results step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[Paul]	30/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Friend Class SubsidyEnquiryResultsStep
        Implements ISelectorWizardStep

        Const QS_PROVIDERID As String = "providerID"
        Const QS_SERVICEID As String = "serviceID"
        Const QS_CLIENTID As String = "clientID"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETO As String = "dateTo"
        Const QS_STATUS As String = "status"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _providerID As Integer
        Private _serviceID As Integer
        Private _clientID As Integer
        Private _dateFrom As Date
        Private _dateTo As Date
        Private _status As Integer
        Private _required As Boolean
        Private _description As String = "The list below displays the results of your Subsidy enquiry."

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
                Return "true"
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
                Return _queryString
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                _queryString = Value
                ' pull out the required params
                _providerID = Utils.ToInt32(_queryString(QS_PROVIDERID))
                _serviceID = Utils.ToInt32(_queryString(QS_SERVICEID))
                _clientID = Utils.ToInt32(_queryString(QS_CLIENTID))
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETO)) Then _dateTo = _queryString(QS_DATETO)
                _status = Utils.ToInt32(_queryString(QS_STATUS))
                If _status = 0 Then _status = 5
            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

            Dim msg As ErrorMessage = New ErrorMessage
            Dim subsidyList As SubsidySelector = wizard.LoadControl("~/SPWeb/Apps/UserControls/SubsidySelector.ascx")
            subsidyList.InitControl(wizard.Page, _providerID, _serviceID, _clientID, _dateFrom, _dateTo, _status)
            controls.Add(subsidyList)
            msg.Success = True
            Return msg

        End Function

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage = New ErrorMessage
            msg.Success = True
            Return msg
        End Function

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

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

        End Sub

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

    End Class

#End Region

#Region " SubsidyFilterStep "

    Friend Class SubsidyFilterStep
        Inherits DateRangeStep

        Private Enum SubsidyStatus
            Active = 1
            Cancelled = 2
            Provisional = 4
            Documentary = 8
            Suspended = 16
        End Enum

        Const QS_ACTIVE As String = "Active"
        Const QS_PROVISIONAL As String = "Provisional"
        Const QS_SUSPENDED As String = "Suspended"
        Const QS_CANCELLED As String = "Cancelled"
        Const QS_DOCUMENTARY As String = "Documentary"
        Const QS_STATUS As String = "Status"
        Const QS_CURRENTSTEP As String = "currentStep"

        Private _Active As Boolean
        Private _Provisional As Boolean
        Private _Suspended As Boolean
        Private _Cancelled As Boolean
        Private _Documentary As Boolean
        Private _queryString As NameValueCollection

        Private _ActiveControl As CheckBoxEx = New CheckBoxEx
        Private _ProvisionalControl As CheckBoxEx = New CheckBoxEx
        Private _SuspendedControl As CheckBoxEx = New CheckBoxEx
        Private _CancelledControl As CheckBoxEx = New CheckBoxEx
        Private _DocumentaryControl As CheckBoxEx = New CheckBoxEx

        Private _description As String = "Please select a date range and status to filter the results on."

        Public Overrides ReadOnly Property Title() As String
            Get
                Return "Select a Date Range and Status."
            End Get
        End Property

        Public Overrides Sub InitStep(ByVal wizard As SelectorWizardBase)
            MyBase.InitStep(wizard)
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("SPWeb/Apps/UserControls/SubsidyFilterStep.js"))
        End Sub

        Public Overrides Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage
            Dim msg As ErrorMessage
            Dim spacerBr As Literal
            Dim spacerBr2 As Literal

            msg = MyBase.RenderContentControls(wizard, controls)
            If Not msg.Success Then
                Return msg
            End If

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            With _ActiveControl
                .ID = "chkActive"
                .Text = QS_ACTIVE
                .checkBoxCssClass = "chkBoxStyle"
                controls.Add(_ActiveControl)
            End With

            With _ProvisionalControl
                .ID = "chkProvisional"
                .Text = QS_PROVISIONAL
                .checkBoxCssClass = "chkBoxStyle"
                controls.Add(_ProvisionalControl)
            End With

            With _SuspendedControl
                .ID = "chkSuspended"
                .Text = QS_SUSPENDED
                .checkBoxCssClass = "chkBoxStyle"
                controls.Add(_SuspendedControl)
            End With

            With _CancelledControl
                .ID = "chkCancelled"
                .Text = QS_CANCELLED
                .checkBoxCssClass = "chkBoxStyle"
                controls.Add(_CancelledControl)
            End With

            With _DocumentaryControl
                .ID = "chkDocumentary"
                .Text = QS_DOCUMENTARY
                .checkBoxCssClass = "chkBoxStyle"
                controls.Add(_DocumentaryControl)
            End With

            spacerBr2 = New Literal
            spacerBr2.Text = "<br /><br />"
            controls.Add(spacerBr2)

            msg = New ErrorMessage
            msg.Success = True
            Return msg
        End Function

        Public Overrides Property Description() As String
            Get
                Return _description
            End Get
            Set(ByVal Value As String)
                _description = Value
            End Set
        End Property

        Public Overrides Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage
            Dim msg As ErrorMessage
            Dim lbl As Label = New Label
            Dim spacerBr As Literal
            Dim text As Label = New Label
            Dim link As HyperLink = New HyperLink
            Dim intStatus As Integer

            msg = MyBase.RenderHeaderControls(controls)
            If Not msg.Success Then
                Return msg
            End If

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            ' label
            lbl.Text = "Status"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", "8em")
            controls.Add(lbl)

            text.Style.Add("float", "left")
            If _queryString(QS_STATUS) <> 0 Then
                text.Text = GetStatusHeaderText(_queryString(QS_STATUS))
            Else
                text.Text = GetStatusHeaderText(5)
            End If
            controls.Add(text)

            If _queryString(QS_STATUS) <> 0 Then
                If Not Me.IsCurrentStep Then
                    link.Style.Add("float", "right")
                    link.Text = "Change Status"
                    Me.QueryString.Remove(QS_CURRENTSTEP)
                    Me.QueryString.Add(QS_CURRENTSTEP, Me.StepIndex)

                    If _queryString(QS_STATUS) <> 0 Then
                        intStatus = _queryString(QS_STATUS)
                        Me.QueryString.Remove(QS_STATUS)
                        Me.QueryString.Add(QS_STATUS, intStatus)
                    Else
                        Me.QueryString.Remove(QS_STATUS)
                        Me.QueryString.Add(QS_STATUS, 5)
                    End If
                    link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(Me.QueryString))
                    controls.Add(link)
                End If
            End If

            msg = New ErrorMessage
            msg.Success = True
            Return msg
        End Function

        Private Sub SetStatus(ByVal intValue As Integer)

            If Not (SubsidyStatus.Active And intValue) = 0 Then
                _ActiveControl.CheckBox.Checked = True
            Else
                _ActiveControl.CheckBox.Checked = False
            End If
            If Not (SubsidyStatus.Cancelled And intValue) = 0 Then
                _CancelledControl.CheckBox.Checked = True
            Else
                _CancelledControl.CheckBox.Checked = False
            End If
            If Not (SubsidyStatus.Provisional And intValue) = 0 Then
                _ProvisionalControl.CheckBox.Checked = True
            Else
                _ProvisionalControl.CheckBox.Checked = False
            End If
            If Not (SubsidyStatus.Documentary And intValue) = 0 Then
                _DocumentaryControl.CheckBox.Checked = True
            Else
                _DocumentaryControl.CheckBox.Checked = False
            End If
            If Not (SubsidyStatus.Suspended And intValue) = 0 Then
                _SuspendedControl.CheckBox.Checked = True
            Else
                _SuspendedControl.CheckBox.Checked = False
            End If
        End Sub

        Private Function GetStatusHeaderText(ByVal intValue As Integer) As String
            Dim strHeaderText As String = ""

            If Not (SubsidyStatus.Active And intValue) = 0 Then
                If strHeaderText.Length > 0 Then
                    strHeaderText = strHeaderText & ", "
                End If
                strHeaderText = strHeaderText & "Active"
            End If
            If Not (SubsidyStatus.Cancelled And intValue) = 0 Then
                If strHeaderText.Length > 0 Then
                    strHeaderText = strHeaderText & ", "
                End If
                strHeaderText = strHeaderText & "Cancelled"
            End If
            If Not (SubsidyStatus.Provisional And intValue) = 0 Then
                If strHeaderText.Length > 0 Then
                    strHeaderText = strHeaderText & ", "
                End If
                strHeaderText = strHeaderText & "Provisional"
            End If
            If Not (SubsidyStatus.Documentary And intValue) = 0 Then
                If strHeaderText.Length > 0 Then
                    strHeaderText = strHeaderText & ", "
                End If
                strHeaderText = strHeaderText & "Documentary"
            End If
            If Not (SubsidyStatus.Suspended And intValue) = 0 Then
                If strHeaderText.Length > 0 Then
                    strHeaderText = strHeaderText & ", "
                End If
                strHeaderText = strHeaderText & "Suspended"
            End If
            Return strHeaderText
        End Function

        Public Overrides ReadOnly Property BeforeNavigateJS() As String
            Get
                Return "SubsidyFilterStep_BeforeNavigate()"
            End Get
        End Property

        Public Overrides Property QueryString() As System.Collections.Specialized.NameValueCollection
            Get
                Return MyBase.QueryString
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                MyBase.QueryString = Value
                _queryString = Value
                If _queryString(QS_STATUS) <> 0 Then
                    SetStatus(_queryString(QS_STATUS))
                Else
                    SetStatus(5)
                End If
            End Set
        End Property
    End Class

#End Region

#Region " NewPIReturnAttachDocStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.UserControls.SelectorWizardSteps.NewSUNotifAttachDocStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Class that implements the new service user notification attach document step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[Paul]	21/02/2007	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Friend Class NewPIReturnAttachDocStep
        Implements ISelectorWizardStep

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _valResults As Label = New Label
        Private _textComment As TextBox = New TextBox
        Private _description As String = "Please click on the ""Browse..."" button below to locate and select a PI Workbook. Then click on the ""Finish"" button below to Validate and Upload the Workbook."

        Public Property txtComment() As TextBox
            Get
                Return _textComment
            End Get
            Set(ByVal Value As TextBox)
                _textComment = Value
            End Set
        End Property

        Public Property ValResults() As Label
            Get
                Return _valResults
            End Get
            Set(ByVal Value As Label)
                _valResults = Value
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
                Return "NewPIReturnAttachDocStep_BeforeNavigate()"
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

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

        End Sub

        Public Property QueryString() As System.Collections.Specialized.NameValueCollection Implements ISelectorWizardStep.QueryString
            Get
                Return _queryString
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                _queryString = Value
            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

            Dim msg As ErrorMessage = New ErrorMessage
            Dim upload As Target.Library.Web.Controls.FileUpload = New Target.Library.Web.Controls.FileUpload
            Dim fieldset As HtmlGenericControl = New HtmlGenericControl("fieldset")
            Dim legend As HtmlGenericControl = New HtmlGenericControl("legend")
            Dim div As HtmlGenericControl = New HtmlGenericControl("div")


            legend.InnerText = "Comment"
            fieldset.Style.Add("width", "50%")
            fieldset.Style.Add("text-align", "left")
            fieldset.Controls.Add(legend)
            With _textComment
                .TextMode = TextBoxMode.MultiLine
                .Rows = 3
                .Width = New Unit(100, UnitType.Percentage)
            End With
            div.Controls.Add(_textComment)
            fieldset.Controls.Add(div)
            With _valResults
                .CssClass = "errorText"
            End With
            With upload
                .ID = "uploadPIReturn"
                .Caption = "Upload PI Workbook"
                .MaxFiles = 1
                .Width = "50%"
            End With
            controls.Add(New HtmlGenericControl("br"))
            controls.Add(fieldset)
            controls.Add(New HtmlGenericControl("br"))
            controls.Add(upload)
            controls.Add(New HtmlGenericControl("br"))
            controls.Add(_valResults)

            msg.Success = True
            Return msg

        End Function

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage = New ErrorMessage
            msg.Success = True
            Return msg
        End Function

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
                Return "Upload PI Workbook"
            End Get
        End Property

        Public Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("SPWeb/Apps/UserControls/NewPIReturnAttachDocStep.js"))
        End Sub

    End Class

#End Region

#Region " FinancialYrQtrStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.UserControls.SelectorWizardSteps.FinancialYrQtrStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Class that implements the new service user notification enter details step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[Paul]	21/02/2007	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Friend Class FinancialYrQtrStep
        Implements ISelectorWizardStep
        Const QS_FINANCIALYEAR As String = "finYr"
        Const QS_QUARTER As String = "qtr"
        Const QS_CURRENTSTEP As String = "currentStep"

        Private _financialYear As String
        Private _quarter As String
        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _finYrQtrControl As FinancialYrQtrSelector
        Private _description As String = "Please select a Financial Year/Quarter and then click on ""Next""."

        Public Property BaseUrl() As String Implements ISelectorWizardStep.BaseUrl
            Get
                Return _baseUrl
            End Get
            Set(ByVal Value As String)
                _baseUrl = Value
            End Set
        End Property

        Public Overridable ReadOnly Property BeforeNavigateJS() As String Implements ISelectorWizardStep.BeforeNavigateJS
            Get
                Return "FinancialYrQtrStep_BeforeNavigate()"
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

        Public Overridable Property Description() As String Implements ISelectorWizardStep.Description
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

        Public Overridable Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

            wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.SP.Apps.UserControls.SelectorWizard.FinancialYrQtrStep.Startup", _
                Target.Library.Web.Utils.WrapClientScript( _
                    String.Format("FinancialYrQtrStep_required={0};", _required.ToString().ToLower()) _
                ) _
            )

        End Sub

        Public Overridable Property QueryString() As System.Collections.Specialized.NameValueCollection Implements ISelectorWizardStep.QueryString
            Get
                Return _queryString
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)

                _queryString = Value
                ' pull out the required params
                If Not _queryString(QS_FINANCIALYEAR) Is Nothing Then
                    ' if Financial Year is present on the querystring but is empty, default Previous FinancialYr Quarter
                    If Convert.ToString(_queryString(QS_FINANCIALYEAR)).Length <> 0 Then
                        '_financialYear = finYr
                        'Else
                        _financialYear = _queryString(QS_FINANCIALYEAR)
                    End If
                End If
                If Not _queryString(QS_QUARTER) Is Nothing Then
                    ' if Quarter is present on the querystring but is empty, default Previous FinancialYr Quarter
                    If Convert.ToString(_queryString(QS_QUARTER)).Length <> 0 Then
                        '_quarter = qtr
                        'Else
                        _quarter = _queryString(QS_QUARTER)
                    End If
                End If

                If (_financialYear = "") Or (_quarter = "") Then
                    GetPreviousFinancialYrQuarter(ConnectionStrings("Abacus").ConnectionString, _financialYear, _quarter)
                End If
            End Set
        End Property

        Public Overridable Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

            Dim msg As ErrorMessage = New ErrorMessage
            _finYrQtrControl = wizard.LoadControl("~/SPWeb/Apps/UserControls/FinancialYrQtrSelector.ascx")
            _finYrQtrControl.ID = "ctlFinancialYearQuarter"
            _finYrQtrControl.InitControl(wizard.Page)
            _finYrQtrControl.FinancialYear = _financialYear
            _finYrQtrControl.Quarter = _quarter
            controls.Add(_finYrQtrControl)
            controls.Add(New HtmlGenericControl("br"))
            msg.Success = True
            Return msg

        End Function

        Public Overridable Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage
            Dim lbl As Label = New Label
            Dim text As Label = New Label
            Dim link As HyperLink = New HyperLink

            ' label
            lbl.Text = "Period"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", "8em")
            controls.Add(lbl)

            text.Style.Add("float", "left")
            If _financialYear = "" And _quarter = "" Then
                text.Text = "Not Selected"
            Else
                text.Text = _financialYear & " Q" & _quarter
            End If

            controls.Add(text)

            If (_financialYear <> "") And (_quarter <> "") Then
                ' add the link
                link.Style.Add("float", "right")
                If Me.IsCurrentStep Then
                    link.Text = "Clear Period"
                    Me.QueryString.Remove(QS_FINANCIALYEAR)
                    Me.QueryString.Remove(QS_QUARTER)
                    link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(Me.QueryString))
                Else
                    link.Text = "Change Period"
                    Me.QueryString.Remove(QS_CURRENTSTEP)
                    Me.QueryString.Add(QS_CURRENTSTEP, Me.StepIndex)
                    link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(Me.QueryString))
                End If
                controls.Add(link)
            End If

            msg = New ErrorMessage
            msg.Success = True

            Return msg
        End Function

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

        Public Overridable ReadOnly Property Title() As String Implements ISelectorWizardStep.Title
            Get
                Return "Enter Financial Year/Quarter Details"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets the enter details user control.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Paul]	21/02/2007	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Property FinancialYrQtrControl() As FinancialYrQtrSelector
            Get
                Return _finYrQtrControl
            End Get
            Set(ByVal Value As FinancialYrQtrSelector)
                _finYrQtrControl = Value
            End Set
        End Property

        Public Overridable Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("SPWeb/Apps/UserControls/FinancialYrQtrStep.js"))
        End Sub

    End Class

#End Region

#Region " NewPIReturnCompleteStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.UserControls.SelectorWizardSteps.NewPIReturnCompleteStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Class that implements the new service user notification complete step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[Paul]	23/02/2007	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Friend Class NewPIReturnCompleteStep
        Implements ISelectorWizardStep

        Const QS_PROVIDERID As String = "providerID"
        Const QS_SERVICEID As String = "serviceID"
        Const QS_FINANCIALYEAR As String = "finYr"
        Const QS_QUARTER As String = "qtr"

        Private _serviceID As Integer '= Utils.ToInt32(_queryString(QS_SERVICEID))
        Private _providerID As Integer '= Utils.ToInt32(_queryString(QS_PROVIDERID))
        Private _financialYear As String '= _queryString(QS_FINANCIALYEAR)
        Private _quarter As String '= _queryString(QS_QUARTER)
        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _description As String

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
                Return "true"
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
                _description = String.Format("Thank you. The PI Return has been successfully submitted for processing.<br /><br />You may track its progress using the <a href=""ListSubmittedPIReturns.aspx?provderID={0}&serviceID={1}&finYr={2}&qtr={3}&status=&currentStep=3"">List Submitted PI Returns</a>.", _providerID, _serviceID, _financialYear, _quarter)
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

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

        End Sub

        Public Property QueryString() As System.Collections.Specialized.NameValueCollection Implements ISelectorWizardStep.QueryString
            Get
                Return _queryString
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                _queryString = Value
                _serviceID = Utils.ToInt32(_queryString(QS_SERVICEID))
                _providerID = Utils.ToInt32(_queryString(QS_PROVIDERID))
                _financialYear = _queryString(QS_FINANCIALYEAR)
                _quarter = _queryString(QS_QUARTER)
            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls
            Dim msg As ErrorMessage = New ErrorMessage
            msg.Success = True
            Return msg
        End Function

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage = New ErrorMessage

            msg.Success = True
            Return msg
        End Function

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
                Return "Complete"
            End Get
        End Property

        Public Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep

        End Sub

    End Class

#End Region

#Region " FinancialYrQtrPIStatusStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.UserControls.SelectorWizardSteps.FinancialYrQtrPIStatusStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Class that implements the new service user notification enter details step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[Paul]	01/03/2007	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Friend Class FinancialYrQtrPIStatusStep
        Inherits FinancialYrQtrStep
        Const QS_FINANCIALYEAR As String = "finYr"
        Const QS_QUARTER As String = "qtr"
        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_STATUS As String = "status"

        Private Enum PIReturnStatus
            Declined = 1
            Submitted = 2
            Accepted = 3
        End Enum
        Private _status As String
        Private _financialYear As String
        Private _quarter As String
        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _finYrQtrControl As FinancialYrQtrSelector
        Private _statusCombo As DropDownListEx = New DropDownListEx
        Private _description As String = "Please select a Financial Year, Quarter and Status values.  Clicking on ""Next"" to continue."

        Public Overrides ReadOnly Property BeforeNavigateJS() As String
            Get
                Return "FinancialYrQtrPIStatusStep_BeforeNavigate()"
            End Get
        End Property

        Public Overrides Property Description() As String
            Get
                Return _description
            End Get
            Set(ByVal Value As String)
                _description = Value
            End Set
        End Property

        Public Overrides Sub InitStep(ByVal wizard As SelectorWizardBase)
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("SPWeb/Apps/UserControls/FinancialYrQtrPIStatusStep.js"))
        End Sub

        Public Overrides Sub PreRender(ByVal wizard As SelectorWizardBase)

            wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.SP.Apps.UserControls.SelectorWizard.FinancialYrQtrPIStatusStep.Startup", _
                Target.Library.Web.Utils.WrapClientScript( _
                    String.Format("FinancialYrQtrStep_required={0};", _required.ToString().ToLower()) _
                ) _
            )

        End Sub

        Public Overrides Property QueryString() As System.Collections.Specialized.NameValueCollection
            Get
                Return MyBase.QueryString
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                MyBase.QueryString = Value
                _queryString = Value
                If _queryString(QS_STATUS) <> "" Then
                    SetStatus(_queryString(QS_STATUS))
                Else
                    _status = ""
                End If
            End Set
        End Property

        Public Overrides Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage
            Dim msg As ErrorMessage = New ErrorMessage
            Dim fieldset As HtmlGenericControl = New HtmlGenericControl("fieldset")
            Dim legend As HtmlGenericControl = New HtmlGenericControl("legend")
            Dim div As HtmlGenericControl = New HtmlGenericControl("div")

            msg = MyBase.RenderContentControls(wizard, controls)
            If Not msg.Success Then
                Return msg
            End If

            'Add Status frame under the financial year/quarter selector
            legend.InnerText = "Status"
            fieldset.Style.Add("text-align", "left")
            fieldset.Controls.Add(legend)
            With _statusCombo.DropDownList
                _statusCombo.ID = "cboStatus"
                .Items.Insert(0, New ListItem("", ""))
                .Items.Insert(1, New ListItem("Submitted", "2"))
                .Items.Insert(2, New ListItem("Accepted", "3"))
                .Items.Insert(3, New ListItem("Declined", "1"))
                WebUtils.SetDropdownListValue(_statusCombo.DropDownList, _status)
            End With
            div.Controls.Add(_statusCombo)
            fieldset.Controls.Add(div)
            controls.Add(fieldset)
            controls.Add(New HtmlGenericControl("br"))

            msg.Success = True
            Return msg

        End Function

        Public Overrides Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage
            Dim msg As ErrorMessage
            Dim lbl As Label = New Label
            Dim text As Label = New Label
            Dim link As HyperLink = New HyperLink
            Dim spacerBr As Literal

            msg = MyBase.RenderHeaderControls(controls)
            If Not msg.Success Then
                Return msg
            End If

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            ' label
            lbl.Text = "Status"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", "8em")
            controls.Add(lbl)

            text.Style.Add("float", "left")

            Select Case _status
                Case PIReturnStatus.Submitted
                    text.Text = "Submitted"
                Case PIReturnStatus.Accepted
                    text.Text = "Accepted"
                Case PIReturnStatus.Declined
                    text.Text = "Declined"
                Case Else
                    text.Text = ""
            End Select

            controls.Add(text)

            ' add the link
            link.Style.Add("float", "right")
            If Me.IsCurrentStep Then
                link.Text = "All Status Values"
                Me.QueryString.Remove(QS_STATUS)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(Me.QueryString))
            Else
                link.Text = "Change Status"
                Me.QueryString.Remove(QS_CURRENTSTEP)
                Me.QueryString.Add(QS_CURRENTSTEP, Me.StepIndex)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(Me.QueryString))
            End If
            controls.Add(link)


            msg = New ErrorMessage
            msg.Success = True

            Return msg
        End Function

        Public Overrides ReadOnly Property Title() As String
            Get
                Return "Enter Financial Year, Quarter and Status Details"
            End Get
        End Property

        Private Sub SetStatus(ByVal intValue As Integer)

            WebUtils.SetDropdownListValue(_statusCombo.DropDownList, intValue)
            _status = intValue

        End Sub
    End Class

#End Region

#Region " SubmittedPIReturnsResultsStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.UserControls.SelectorWizardSteps.SubmittedPIReturnsResultsStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[paul]	05/03/2007	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Friend Class SubmittedPIReturnsResultsStep
        Implements ISelectorWizardStep

        Const QS_PROVIDERID As String = "providerID"
        Const QS_SERVICEID As String = "serviceID"
        Const QS_FINANCIALYEAR As String = "finYr"
        Const QS_QUARTER As String = "qtr"
        Const QS_STATUS As String = "status"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _providerID As Integer
        Private _serviceID As Integer
        Private _financialYear As String
        Private _quarter As String
        Private _status As Int32
        Private _required As Boolean
        Private _description As String = "The list below displays the results of your PI Submission List enquiry."

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
                Return "true"
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
                Return _queryString
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                _queryString = Value
                ' pull out the required params
                _providerID = Utils.ToInt32(_queryString(QS_PROVIDERID))
                _serviceID = Utils.ToInt32(_queryString(QS_SERVICEID))
                _financialYear = _queryString(QS_FINANCIALYEAR)
                _quarter = _queryString(QS_QUARTER)
                _status = Utils.ToInt32(_queryString(QS_STATUS))
            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

            Dim msg As ErrorMessage = New ErrorMessage
            Dim SubmissionQueueList As SubmittedPIReturnsList = wizard.LoadControl("~/SPWeb/Apps/UserControls/SubmittedPIReturnsList.ascx")
            SubmissionQueueList.InitControl(wizard.Page, _providerID, _serviceID, _financialYear, _quarter, _status)
            controls.Add(SubmissionQueueList)
            msg.Success = True
            Return msg

        End Function

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage = New ErrorMessage
            msg.Success = True
            Return msg
        End Function

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

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

        End Sub

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

    End Class

#End Region

#Region " FinYrQtrFromToPIStatusStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.UserControls.SelectorWizardSteps.FinYrQtrFromToPIStatusStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Class that implements the new service user notification enter details step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[Paul]	01/03/2007	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Friend Class FinYrQtrFromToPIStatusStep
        Implements ISelectorWizardStep
        Const QS_FINANCIALYEARFROM As String = "finYrFrom"
        Const QS_QUARTERFROM As String = "qtrFrom"
        Const QS_FINANCIALYEARTO As String = "finYrTo"
        Const QS_QUARTERTO As String = "qtrTo"
        Const QS_STATUS As String = "status"
        Const QS_CURRENTSTEP As String = "currentStep"

        Private _financialYearFrom As String
        Private _quarterFrom As String
        Private _financialYearTo As String
        Private _quarterTo As String
        Private _Status As String
        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _finYrQtrControlFrom As FinancialYrQtrSelector
        Private _finYrQtrControlTo As FinancialYrQtrSelector
        Private _statusCombo As DropDownListEx = New DropDownListEx
        Private _description As String = "Please select a Period From, Period To and Status values. Clicking on ""Next"" to continue."
        Private Enum PIReturnStatus
            Missing = 0
            Declined = 1
            Submitted = 2
            Accepted = 3
        End Enum

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
                Return "FinYrQtrFromToPIStatusStep_BeforeNavigate()"
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

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

            wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.SP.Apps.UserControls.SelectorWizard.FinYrQtrFromToPIStatusStep.Startup", _
                Target.Library.Web.Utils.WrapClientScript( _
                    String.Format("FinYrQtrFromToPIStatusStep_required={0};", _required.ToString().ToLower()) _
                ) _
            )

        End Sub

        Public Property QueryString() As System.Collections.Specialized.NameValueCollection Implements ISelectorWizardStep.QueryString
            Get
                Return _queryString
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                Dim prevFinancialYr As String = Nothing
                Dim prevQuarter As String = Nothing

                _queryString = Value
                ' pull out the required params
                If Not _queryString(QS_FINANCIALYEARFROM) Is Nothing Then
                    If Convert.ToString(_queryString(QS_FINANCIALYEARFROM)).Length <> 0 Then
                        _financialYearFrom = _queryString(QS_FINANCIALYEARFROM)
                    End If
                End If
                If Not _queryString(QS_QUARTERFROM) Is Nothing Then
                    If Convert.ToString(_queryString(QS_QUARTERFROM)).Length <> 0 Then
                        _quarterFrom = _queryString(QS_QUARTERFROM)
                    End If
                End If
                If Not _queryString(QS_FINANCIALYEARTO) Is Nothing Then
                    If Convert.ToString(_queryString(QS_FINANCIALYEARTO)).Length <> 0 Then
                        _financialYearTo = _queryString(QS_FINANCIALYEARTO)
                    End If
                End If
                If Not _queryString(QS_QUARTERTO) Is Nothing Then
                    If Convert.ToString(_queryString(QS_QUARTERTO)).Length <> 0 Then
                        _quarterTo = _queryString(QS_QUARTERTO)
                    End If
                End If
                If Not _queryString(QS_STATUS) Is Nothing Then
                    If Convert.ToString(_queryString(QS_STATUS)).Length <> 0 Then
                        _Status = _queryString(QS_STATUS)
                    End If
                End If

                If ((_financialYearFrom = "") And (_quarterFrom = "")) Or ((_financialYearTo = "") And (_quarterTo = "")) Then
                    GetPreviousFinancialYrQuarter(ConnectionStrings("Abacus").ConnectionString, prevFinancialYr, prevQuarter)
                    If ((_financialYearFrom = "") And (_quarterFrom = "")) Then
                        _financialYearFrom = prevFinancialYr
                        _quarterFrom = prevQuarter
                    End If
                    If ((_financialYearTo = "") And (_quarterTo = "")) Then
                        _financialYearTo = prevFinancialYr
                        _quarterTo = prevQuarter
                    End If
                End If
            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

            Dim msg As ErrorMessage = New ErrorMessage
            Dim fieldset As HtmlGenericControl = New HtmlGenericControl("fieldset")
            Dim legend As HtmlGenericControl = New HtmlGenericControl("legend")
            Dim div As HtmlGenericControl = New HtmlGenericControl("div")

            _finYrQtrControlFrom = wizard.LoadControl("~/SPWeb/Apps/UserControls/FinancialYrQtrSelector.ascx")
            _finYrQtrControlFrom.ID = "ctlFinancialYearQuarterFrom"
            _finYrQtrControlFrom.InitControl(wizard.Page)
            _finYrQtrControlFrom.FinancialYear = _financialYearFrom
            _finYrQtrControlFrom.Quarter = _quarterFrom
            _finYrQtrControlFrom.Caption = "Period From"
            controls.Add(_finYrQtrControlFrom)

            _finYrQtrControlTo = wizard.LoadControl("~/SPWeb/Apps/UserControls/FinancialYrQtrSelector.ascx")
            _finYrQtrControlTo.ID = "ctlFinancialYearQuarterTo"
            _finYrQtrControlTo.InitControl(wizard.Page)
            _finYrQtrControlTo.FinancialYear = _financialYearTo
            _finYrQtrControlTo.Quarter = _quarterTo
            _finYrQtrControlTo.Caption = "Period To"
            controls.Add(_finYrQtrControlTo)

            'Add Status frame under the financial year/quarter selector
            legend.InnerText = "Status"
            fieldset.Style.Add("text-align", "left")
            fieldset.Style.Add("margin-top", "1em")
            fieldset.Controls.Add(legend)
            With _statusCombo.DropDownList
                _statusCombo.ID = "cboStatus"
                .Items.Insert(0, New ListItem("", ""))
                .Items.Insert(1, New ListItem("Missing", "0"))
                .Items.Insert(2, New ListItem("Submitted", "2"))
                .Items.Insert(3, New ListItem("Accepted", "3"))
                .Items.Insert(4, New ListItem("Declined", "1"))
                WebUtils.SetDropdownListValue(_statusCombo.DropDownList, _Status)
            End With
            div.Controls.Add(_statusCombo)
            fieldset.Controls.Add(div)
            controls.Add(fieldset)
            controls.Add(New HtmlGenericControl("br"))

            msg.Success = True
            Return msg

        End Function

        Public Overridable Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage
            Dim lblFrom As Label = New Label
            Dim lblTo As Label = New Label
            Dim textFrom As Label = New Label
            Dim textTo As Label = New Label
            Dim link As HyperLink = New HyperLink
            Dim lbl As Label = New Label
            Dim text As Label = New Label
            Dim linkStatus As HyperLink = New HyperLink
            Dim spacerBr As Literal
            Dim space As HtmlGenericControl

            ' label
            lblFrom.Text = "Period From"
            lblFrom.Style.Add("float", "left")
            lblFrom.Style.Add("font-weight", "bold")
            lblFrom.Style.Add("width", "8em")
            controls.Add(lblFrom)

            textFrom.Style.Add("float", "left")
            lblFrom.Style.Add("width", "10em")
            If _financialYearFrom = "" And _quarterFrom = "" Then
                textFrom.Text = "Not Selected"
            Else
                textFrom.Text = _financialYearFrom & " Q" & _quarterFrom
            End If
            controls.Add(textFrom)

            space = New HtmlGenericControl("span")
            space.Style.Add("float", "left")
            space.InnerHtml = "&nbsp;&nbsp;"
            controls.Add(space)

            lblTo.Text = "Period To"
            lblTo.Style.Add("float", "left")
            lblTo.Style.Add("font-weight", "bold")
            lblTo.Style.Add("width", "6.5em")
            controls.Add(lblTo)

            textTo.Style.Add("float", "left")
            lblFrom.Style.Add("width", "8em")
            If _financialYearTo = "" And _quarterTo = "" Then
                textTo.Text = "Not Selected"
            Else
                textTo.Text = _financialYearTo & " Q" & _quarterTo
            End If
            controls.Add(textTo)

            If ((_financialYearFrom <> "") And (_quarterFrom <> "")) Or ((_financialYearTo <> "") And (_quarterTo <> "")) Then
                ' add the link
                link.Style.Add("float", "right")
                If Me.IsCurrentStep Then
                    link.Text = "Reset Period"
                    Me.QueryString.Remove(QS_FINANCIALYEARFROM)
                    Me.QueryString.Remove(QS_QUARTERFROM)
                    Me.QueryString.Remove(QS_FINANCIALYEARTO)
                    Me.QueryString.Remove(QS_QUARTERTO)
                    link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(Me.QueryString))
                Else
                    link.Text = "Change Period"
                    Me.QueryString.Remove(QS_CURRENTSTEP)
                    Me.QueryString.Add(QS_CURRENTSTEP, Me.StepIndex)
                    link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(Me.QueryString))
                End If
                controls.Add(link)
            End If

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            ' label
            lbl.Text = "Status"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", "8em")
            controls.Add(lbl)

            text.Style.Add("float", "left")

            Select Case _Status
                Case PIReturnStatus.Missing
                    text.Text = "Missing"
                Case PIReturnStatus.Submitted
                    text.Text = "Submitted"
                Case PIReturnStatus.Accepted
                    text.Text = "Accepted"
                Case PIReturnStatus.Declined
                    text.Text = "Declined"
                Case Else
                    text.Text = ""
            End Select

            controls.Add(text)

            ' add the link
            linkStatus.Style.Add("float", "right")
            If Me.IsCurrentStep Then
                linkStatus.Text = "All Status Values"
                Me.QueryString.Remove(QS_STATUS)
                linkStatus.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(Me.QueryString))
            Else
                linkStatus.Text = "Change Status"
                Me.QueryString.Remove(QS_CURRENTSTEP)
                Me.QueryString.Add(QS_CURRENTSTEP, Me.StepIndex)
                linkStatus.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(Me.QueryString))
            End If
            controls.Add(linkStatus)

            msg = New ErrorMessage
            msg.Success = True

            Return msg
        End Function

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

        Public Overridable ReadOnly Property Title() As String Implements ISelectorWizardStep.Title
            Get
                Return "Enter Financial Year/Quarter Details"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets the enter details user control.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Paul]	21/02/2007	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Property FinancialYrQtrControlFrom() As FinancialYrQtrSelector
            Get
                Return _finYrQtrControlFrom
            End Get
            Set(ByVal Value As FinancialYrQtrSelector)
                _finYrQtrControlFrom = Value
            End Set
        End Property

        Public Property FinancialYrQtrControlTo() As FinancialYrQtrSelector
            Get
                Return _finYrQtrControlTo
            End Get
            Set(ByVal Value As FinancialYrQtrSelector)
                _finYrQtrControlTo = Value
            End Set
        End Property

        Public Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("SPWeb/Apps/UserControls/FinYrQtrFromToPIStatusStep.js"))
        End Sub

    End Class


#End Region

#Region " PISubmissionStatusResultsStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.UserControls.SelectorWizardSteps.PISubmissionStatusResultsStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[paul]	05/03/2007	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Friend Class PISubmissionStatusResultsStep
        Implements ISelectorWizardStep

        Const QS_PROVIDERID As String = "providerID"
        Const QS_SERVICEID As String = "serviceID"
        Const QS_FINANCIALYEARFROM As String = "finYrFrom"
        Const QS_QUARTERFROM As String = "qtrFrom"
        Const QS_FINANCIALYEARTO As String = "finYrTo"
        Const QS_QUARTERTO As String = "qtrTo"
        Const QS_STATUS As String = "status"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _providerID As Integer
        Private _serviceID As Integer
        Private _financialYearFrom As String
        Private _quarterFrom As String
        Private _financialYearTo As String
        Private _quarterTo As String
        Private _status As String
        Private _required As Boolean
        Private _description As String = "The list below displays the results of your PI Submission Status List enquiry."

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
                Return "true"
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
                Return _queryString
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                _queryString = Value
                ' pull out the required params
                _providerID = Utils.ToInt32(_queryString(QS_PROVIDERID))
                _serviceID = Utils.ToInt32(_queryString(QS_SERVICEID))
                _financialYearFrom = _queryString(QS_FINANCIALYEARFROM)
                _quarterFrom = _queryString(QS_QUARTERFROM)
                _financialYearTo = _queryString(QS_FINANCIALYEARTO)
                _quarterTo = _queryString(QS_QUARTERTO)
                _status = _queryString(QS_STATUS)
            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

            Dim msg As ErrorMessage = New ErrorMessage
            Dim PISubmissionStatusList As PISubmissionStatusList = wizard.LoadControl("~/SPWeb/Apps/UserControls/PISubmissionStatusList.ascx")
            PISubmissionStatusList.InitControl(wizard.Page, _providerID, _serviceID, _financialYearFrom, _quarterFrom, _financialYearTo, _quarterTo, _status)
            controls.Add(PISubmissionStatusList)
            msg.Success = True
            Return msg

        End Function

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage = New ErrorMessage
            msg.Success = True
            Return msg
        End Function

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

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

        End Sub

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

    End Class

#End Region

End Namespace

