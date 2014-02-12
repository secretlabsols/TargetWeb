
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

#Region " EstablishmentStep "

    ''' -----------------------------------------------------------------------------
	''' Project	 : Target.Abacus.Web
	''' Class	 : Abacus.Web.SelectorWizardSteps.EstablishmentStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Implements the select establishment step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     Paul W      29/06/2010  D11795 - SDS, Generic Contracts and Service Orders
    '''     MoTahir     06/10/09    D11681
    '''     MikeVO      25/09/2009  D11546 - changes to "selections" section links
	''' 	[Mikevo]	08/01/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Class EstablishmentStep
        Implements ISelectorWizardStep

        Const QS_ESTABLISHMENTID As String = "estabID"
        Const QS_CURRENTSTEP As String = "currentStep"

		Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _establishmentID As Integer
        Private _required As Boolean
        Private _description As String = "Please select a provider from the list below and then click ""Next""."
        Private _mode As EstablishmentSelectorMode = EstablishmentSelectorMode.Establishments
        Private _headerLabelWidth As Unit = New Unit(10, UnitType.Em)
        Private _currentStep As Integer

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

        Public ReadOnly Property Title() As String Implements ISelectorWizardStep.Title
            Get
                Title = String.Empty
                Select Case _mode
					Case EstablishmentSelectorMode.Establishments
                        Return "Select a Provider"
					Case EstablishmentSelectorMode.DomProviders
                        Return "Select a Domiciliary Provider"
                    Case EstablishmentSelectorMode.DayCareProviders
                        Return "Select a Day Care Provider"
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

		Public Property Mode() As EstablishmentSelectorMode
			Get
				Return _mode
			End Get
			Set(ByVal Value As EstablishmentSelectorMode)
				_mode = Value
			End Set
		End Property

        Public Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep

        End Sub

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

			wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Web.Apps.UserControls.SelectorWizardSteps.EstablishmentStep.Startup", _
			 Target.Library.Web.Utils.WrapClientScript( _
			  String.Format("EstablishmentStep_required={0}", _required.ToString().ToLower()) _
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
				Case EstablishmentSelectorMode.Establishments
                    lbl.Text = "Provider"
				Case EstablishmentSelectorMode.DomProviders
                    lbl.Text = "Provider"
                Case EstablishmentSelectorMode.DayCareProviders
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
                Try
                    ' get the establishment details
                    spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME, False)
                    spParams(0).Value = _establishmentID
                    reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams)
                    If (reader.HasRows) Then
                        reader.Read()
                        text.Text = String.Format("{0}: {1}", reader("AltReference"), reader("Name"))

                        ' store MRU provider
                        Dim mruManager As Target.Library.Web.MostRecentlyUsedManager = New Target.Library.Web.MostRecentlyUsedManager(HttpContext.Current)
                        mruManager("PROVIDERS")(_establishmentID.ToString()) = text.Text
                        mruManager.Save(HttpContext.Current)

                    End If

                Catch ex As Exception
                    msg = Utils.CatchError(ex, "E0001")     ' unexpected
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
                    Case EstablishmentSelectorMode.Establishments
                        link.Text = "All Providers"
                    Case EstablishmentSelectorMode.DomProviders
                        link.Text = "All Providers"
                    Case EstablishmentSelectorMode.DayCareProviders
                        link.Text = "All Day Care Providers"
                    Case Else
                        ThrowInvalidMode()
                End Select
                qs.Remove(QS_ESTABLISHMENTID)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                Select Case _mode
                    Case EstablishmentSelectorMode.Establishments
                        link.Text = "Change Provider"
                    Case EstablishmentSelectorMode.DomProviders
                        link.Text = "Change Provider"
                    Case EstablishmentSelectorMode.DayCareProviders
                        link.Text = "Change Day Care Provider"
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
			Dim establishmentList As EstablishmentSelector = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/EstablishmentSelector.ascx")
			establishmentList.InitControl(wizard.Page, _establishmentID, _mode)
            controls.Add(establishmentList)
            msg.Success = True
            Return msg

        End Function

        Private Sub ThrowInvalidMode()
            Throw New ArgumentOutOfRangeException("Mode", _mode, "Unrecognised Mode value.")
        End Sub

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
                client = New ClientDetail(Me.DbConnection)
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

#Region " RateFrameworkStep "

	''' <summary>
	''' Implements the select rate framework wizard step.
	''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      25/09/2009  D11546 - changes to "selections" section links
    ''' </history>
	Friend Class RateFrameworkStep
		Implements ISelectorWizardStep

		Const QS_FRAMEWORKID As String = "frameworkID"
		Const QS_CURRENTSTEP As String = "currentStep"

		Private _dbConnection As SqlConnection
		Private _baseUrl As String
		Private _isCurrentStep As Boolean
		Private _queryString As NameValueCollection
		Private _frameworkID As Integer
		Private _stepIndex As Integer
		Private _required As Boolean
        Private _description As String = "Please select a rate framework from the list below and then click ""Finish""."
        Private _showFrameworkDetails As Boolean
        Private _currentStep As Integer

        Sub New()
            _showFrameworkDetails = False
        End Sub

        Public Property ShowFrameworkDetails() As Boolean
            Get
                Return _showFrameworkDetails
            End Get
            Set(ByVal value As Boolean)
                _showFrameworkDetails = value
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
				Return "RateFrameworkStep_BeforeNavigate()"
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

			wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Web.Apps.UserControls.SelectorWizardSteps.RateFrameworkStep.Startup", _
			 Target.Library.Web.Utils.WrapClientScript(String.Format("RateFrameworkStep_required={0};", _required.ToString().ToLower())))

		End Sub

		Public Property QueryString() As System.Collections.Specialized.NameValueCollection Implements ISelectorWizardStep.QueryString
			Get
                Return New NameValueCollection(_queryString)
			End Get
			Set(ByVal value As System.Collections.Specialized.NameValueCollection)
				_queryString = value
				' pull out the required params
                _frameworkID = Utils.ToInt32(_queryString(QS_FRAMEWORKID))
                _currentStep = Utils.ToInt32(_queryString(QS_CURRENTSTEP))
			End Set
		End Property

		Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

			Dim msg As ErrorMessage = New ErrorMessage
			Dim frameworkList As RateFrameworkSelector = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/RateFrameworkSelector.ascx")
			frameworkList.InitControl(wizard.Page, _frameworkID)
			controls.Add(frameworkList)
			msg.Success = True
			Return msg

		End Function

		Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls

            Dim framework As DomRateFramework = Nothing
			Dim msg As ErrorMessage
			Dim lbl As Label = New Label
			Dim text As Label = New Label
            Dim link As HyperLink = New HyperLink
            Dim spacerBr As LiteralControl
            Dim qs As NameValueCollection

			' label
			lbl.Text = "Rate Framework"
			lbl.Style.Add("float", "left")
			lbl.Style.Add("font-weight", "bold")
			lbl.Style.Add("width", "10em")
			controls.Add(lbl)

			text.Style.Add("float", "left")
			If _frameworkID = 0 Then
				text.Text = "All"
			Else
				framework = New DomRateFramework(Me.DbConnection, String.Empty, String.Empty)
				msg = framework.Fetch(_frameworkID)
                If Not msg.Success Then Return msg
            End If
			controls.Add(text)

            ' add the link
            qs = Me.QueryString
            If Me.IsCurrentStep AndAlso _frameworkID > 0 Then
                ' all
                link.Text = "All Rate Frameworks"
                qs.Remove(QS_FRAMEWORKID)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change Rate Framework"
                qs.Remove(QS_CURRENTSTEP)
                qs.Add(QS_CURRENTSTEP, Me.StepIndex)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            End If
            If Not String.IsNullOrEmpty(link.Text) Then
                link.Style.Add("float", "right")
                controls.Add(link)
            End If

			If _frameworkID > 0 Then
				' add the extra details
                If Me.ShowFrameworkDetails Then
                    ' name
                    spacerBr = New LiteralControl("<br />")
                    controls.Add(spacerBr)
                    lbl = New Label()
                    lbl.Text = "Name"
                    lbl.Style.Add("float", "left")
                    lbl.Style.Add("font-weight", "bold")
                    lbl.Style.Add("width", "10em")
                    controls.Add(lbl)

                    text = New Label()
                    text.Text = framework.Description
                    controls.Add(text)

                    ' abbrev
                    spacerBr = New LiteralControl("<br />")
                    controls.Add(spacerBr)
                    lbl = New Label()
                    lbl.Text = "Abbreviation"
                    lbl.Style.Add("float", "left")
                    lbl.Style.Add("font-weight", "bold")
                    lbl.Style.Add("width", "10em")
                    controls.Add(lbl)

                    text = New Label()
                    text.Text = framework.Abbreviation
                    controls.Add(text)

                    ' enh rate days
                    spacerBr = New LiteralControl("<br />")
                    controls.Add(spacerBr)
                    lbl = New Label()
                    lbl.Text = "Enhanced Days"
                    lbl.Style.Add("float", "left")
                    lbl.Style.Add("font-weight", "bold")
                    lbl.Style.Add("width", "10em")
                    controls.Add(lbl)

                    text = New Label()
                    text.Text = Utils.BooleanToYesNo(framework.UseEnhancedRateDays)
                    controls.Add(text)

                    ' redundant
                    spacerBr = New LiteralControl("<br />")
                    controls.Add(spacerBr)
                    lbl = New Label()
                    lbl.Text = "Redundant"
                    lbl.Style.Add("float", "left")
                    lbl.Style.Add("font-weight", "bold")
                    lbl.Style.Add("width", "10em")
                    controls.Add(lbl)

                    text = New Label()
                    text.Text = Utils.BooleanToYesNo(framework.Redundant)
                    controls.Add(text)
                End If

            End If

			msg = New ErrorMessage
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
				Return "Select a Rate Framework"
			End Get
		End Property
	End Class

#End Region

#Region " RateCategoryStep "

	''' <summary>
	''' Implements the select rate category wizard step.
	''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      25/09/2009  D11546 - changes to "selections" section links
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' </history>
	Friend Class RateCategoryStep
		Implements ISelectorWizardStep

		Const QS_FRAMEWORKID As String = "frameworkID"
		Const QS_CURRENTSTEP As String = "currentStep"
		Const QS_CATEGORYID As String = "categoryID"

		Private _dbConnection As SqlConnection
		Private _baseUrl As String
		Private _isCurrentStep As Boolean
		Private _queryString As NameValueCollection
		Private _frameworkID As Integer
		Private _categoryID As Integer
		Private _stepIndex As Integer
		Private _required As Boolean
        Private _description As String = "Please select a rate category from the list and use the buttons below."
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
				Return "RateCategoryStep_BeforeNavigate()"
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
				_frameworkID = Utils.ToInt32(_queryString(QS_FRAMEWORKID))
				_categoryID = Utils.ToInt32(_queryString(QS_CATEGORYID))
			End Set
		End Property

		Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

			Dim msg As ErrorMessage = New ErrorMessage
			Dim categoryList As RateCategorySelector = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/RateCategorySelector.ascx")
            categoryList.InitControl(wizard.Page, _frameworkID, _categoryID, _showNewButton)
			controls.Add(categoryList)
			msg.Success = True
			Return msg

		End Function

		Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
			Dim msg As ErrorMessage= New ErrorMessage
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
				Return "Select a Rate Category"
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

#Region " DomContractFilterStep "

	''' <summary>
	''' Implements a wizard step to select domiciliary contract tyep, group and period.
	''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      25/09/2009  D11546 - changes to "selections" section links
    ''' </history>
	Friend Class DomContractFilterStep
		Inherits DateRangeStep

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_SERVICEGROUPID As String = "svcGroupID"
		Const QS_ESTABLISHMENTID As String = "estabID"
		Const QS_CONTRACTTYPEID As String = "ctID"
        Const QS_CONTRACTGROUPID As String = "cgID"
        Const QS_CONTRACTENDREASONID As String = "cerID"

		Private _queryString As NameValueCollection
		Private _estabID As Integer
		Private _contractType As DomContractType
        Private _contractGroupID As Integer
        Private _serviceGroupID As Integer
        Private _contractEndReasonID As Integer
		Private _cboContractType As DropDownListEx
        Private _cboContractGroup As DropDownListEx
        Private _cboContractEndReason As DropDownListEx
        Private _cboServiceGroup As DropDownListEx
        Private _currentStep As Integer

		Sub New()
			MyBase.New()
            Me.Description = "Please select the contract type, group, period and end reason to filter the results on."
			Me.DateFrom = Date.Today
            Me.ShowHeaderLink = True
			Me.HeaderLabelWidth = New Unit(10, UnitType.Em)
			Me.ContentLabelWidth = New Unit(10, UnitType.Em)
		End Sub

		Public Overrides Property QueryString() As System.Collections.Specialized.NameValueCollection
			Get
                Return New NameValueCollection(_queryString)
			End Get
			Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
				MyBase.QueryString = Value
				_queryString = Value
				_estabID = Utils.ToInt32(_queryString(QS_ESTABLISHMENTID))
				_contractType = Utils.ToInt32(_queryString(QS_CONTRACTTYPEID))
                _contractGroupID = Utils.ToInt32(_queryString(QS_CONTRACTGROUPID))
                _contractEndReasonID = Utils.ToInt32(_queryString(QS_CONTRACTENDREASONID))
                _serviceGroupID = Utils.ToInt32(_queryString(QS_SERVICEGROUPID))
                _currentStep = Utils.ToInt32(_queryString(QS_CURRENTSTEP))
			End Set
		End Property

		Public Overrides Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage

			Dim msg As ErrorMessage = New ErrorMessage
			Dim spacerBr As Literal
            Dim groups As GenericContractGroupCollection = Nothing
            Dim reasons As ContractEndReasonCollection = Nothing
            Dim user As WebSecurityUser
            Dim svcGroups As vwWebSecurityRole_ServiceGroupCollection = Nothing

            user = SecurityBL.GetCurrentUser()

			spacerBr = New Literal
			spacerBr.Text = "<br />"
			controls.Add(spacerBr)

            'Service Group
            _cboServiceGroup = New DropDownListEx()
            With _cboServiceGroup
                .LabelText = "Service Group"
                .LabelWidth = Me.ContentLabelWidth.ToString()
                .LabelBold = True
                msg = DomContractBL.GetServiceGroupsAvailableToUser(Me.DbConnection, user.ID, svcGroups)
                If Not msg.Success Then Return msg
                With .DropDownList
                    .Items.Clear()
                    .DataSource = svcGroups
                    .DataTextField = "ServiceGroupDescription"
                    .DataValueField = "ServiceGroupID"
                    .DataBind()
                    ' insert a blank at the top
                    .Items.Insert(0, New ListItem(String.Empty, 0))
                    .SelectedValue = Utils.ToInt32(_serviceGroupID)
                End With
            End With
            controls.Add(_cboServiceGroup)

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)


			' contract type
			_cboContractType = New DropDownListEx()
			With _cboContractType
				.LabelText = "Contract Type"
				.LabelWidth = Me.ContentLabelWidth.ToString()
				.LabelBold = True
				With .DropDownList.Items
					.Clear()
					For Each value As DomContractType In [Enum].GetValues(GetType(DomContractType))
						If value = DomContractType.Unknown Then
							.Add(New ListItem(String.Empty, Convert.ToInt32(value)))
						Else
							.Add(New ListItem(Utils.SplitOnCapitals([Enum].GetName(GetType(DomContractType), value)), Convert.ToInt32(value)))
						End If
					Next
				End With
				.DropDownList.SelectedValue = Convert.ToInt32(_contractType)
			End With
			controls.Add(_cboContractType)

			spacerBr = New Literal
			spacerBr.Text = "<br />"
			controls.Add(spacerBr)

			' contract group
			_cboContractGroup = New DropDownListEx()
			With _cboContractGroup
				.LabelText = "Contract Group"
				.LabelWidth = Me.ContentLabelWidth.ToString()
				.LabelBold = True
				' get a list of non-redundant groups
                msg = GenericContractGroup.FetchList(Me.DbConnection, groups, String.Empty, String.Empty, TriState.False)
				If Not msg.Success Then Return msg
				With .DropDownList
					.Items.Clear()
					.DataSource = groups
					.DataTextField = "Description"
					.DataValueField = "ID"
					.DataBind()
					' insert a blank at the top
					.Items.Insert(0, New ListItem(String.Empty, 0))
                    .SelectedValue = Utils.ToInt32(_contractGroupID)
				End With
			End With
			controls.Add(_cboContractGroup)

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            ' contract End Reason
            _cboContractEndReason = New DropDownListEx()
            With _cboContractEndReason
                .LabelText = "End Reason"
                .LabelWidth = Me.ContentLabelWidth.ToString()
                .LabelBold = True
                ' get a list of non-redundant groups
                msg = ContractEndReason.FetchList(Me.DbConnection, reasons, String.Empty, String.Empty, TriState.False)
                If Not msg.Success Then Return msg
                With .DropDownList
                    .Items.Clear()
                    .DataSource = reasons
                    .DataTextField = "Description"
                    .DataValueField = "ID"
                    .DataBind()
                    ' insert a blank at the top
                    .Items.Insert(0, New ListItem(String.Empty, 0))
                    .Items.Insert(1, New ListItem("[None]", -1))
                    .Items.Insert(2, New ListItem("[Any]", -2))
                    .SelectedValue = Utils.ToInt32(_contractEndReasonID)

                End With
            End With
            controls.Add(_cboContractEndReason)

			msg = MyBase.RenderContentControls(wizard, controls)
			If Not msg.Success Then Return msg

			msg.Success = True
			Return msg

		End Function

		Public Overrides Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage

			Dim msg As ErrorMessage
			Dim spacerBr As Literal
			Dim lbl As Label
			Dim text As Label
			Dim link As HyperLink
            Dim contractGroup As GenericContractGroup = Nothing
            Dim contractEndReason As ContractEndReason = Nothing
            Dim svcGroup As ServiceGroup = Nothing
            Dim qs As NameValueCollection

			' if a provider has been selected show the date range link
			If _estabID <> 0 Then Me.ShowHeaderLink = True

            ' Service Group
            lbl = New Label()
            lbl.Text = "Service Group"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", Me.HeaderLabelWidth.ToString())
            controls.Add(lbl)

            text = New Label()
            text.Style.Add("float", "left")
            If _serviceGroupID = 0 Then
                text.Text = "All"
            Else
                svcGroup = New ServiceGroup(Me.DbConnection, String.Empty, String.Empty)
                msg = svcGroup.Fetch(_serviceGroupID)
                If Not msg.Success Then Return msg
                text.Text = svcGroup.Description
            End If
            controls.Add(text)

            ' add the link
            qs = Me.QueryString
            link = New HyperLink()
            If Me.IsCurrentStep AndAlso _serviceGroupID > 0 Then
                ' all
                link.Text = "All Service Groups"
                qs.Remove(QS_SERVICEGROUPID)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change Service Group"
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

			' contract type
			lbl = New Label()
			lbl.Text = "Contract Type"
			lbl.Style.Add("float", "left")
			lbl.Style.Add("font-weight", "bold")
			lbl.Style.Add("width", Me.HeaderLabelWidth.ToString())
			controls.Add(lbl)

			text = New Label()
			text.Style.Add("float", "left")
			text.Text = IIf(_contractType = DomContractType.Unknown, "All", Utils.SplitOnCapitals([Enum].GetName(GetType(DomContractType), _contractType)))
			controls.Add(text)

			link = New HyperLink()
            If Me.IsCurrentStep AndAlso _contractType <> DomContractType.Unknown Then
                ' all
                link.Text = "All Contract Types"
                Me.QueryString.Remove(QS_CONTRACTTYPEID)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(Me.QueryString))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change Contract Type"
                Me.QueryString.Remove(QS_CURRENTSTEP)
                Me.QueryString.Add(QS_CURRENTSTEP, MyBase.StepIndex)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(Me.QueryString))
            End If
            If Not String.IsNullOrEmpty(link.Text) Then
                link.Style.Add("float", "right")
                controls.Add(link)
            End If

			spacerBr = New Literal
			spacerBr.Text = "<br />"
			controls.Add(spacerBr)

			' contract group
			lbl = New Label()
			lbl.Text = "Contract Group"
			lbl.Style.Add("float", "left")
			lbl.Style.Add("font-weight", "bold")
			lbl.Style.Add("width", Me.HeaderLabelWidth.ToString())
			controls.Add(lbl)

			If _contractGroupID > 0 Then
                contractGroup = New GenericContractGroup(Me.DbConnection, String.Empty, String.Empty)
				msg = contractGroup.Fetch(_contractGroupID)
				If Not msg.Success Then Return msg
			End If

			text = New Label()
			text.Style.Add("float", "left")
			If contractGroup Is Nothing Then
				text.Text = "All"
			Else
				text.Text = contractGroup.Description
			End If
			controls.Add(text)

			link = New HyperLink()
            If Me.IsCurrentStep AndAlso _contractGroupID > 0 Then
                ' all
                link.Text = "All Contract Groups"
                Me.QueryString.Remove(QS_CONTRACTGROUPID)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(Me.QueryString))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change Contract Group"
                Me.QueryString.Remove(QS_CURRENTSTEP)
                Me.QueryString.Add(QS_CURRENTSTEP, MyBase.StepIndex)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(Me.QueryString))
            End If
            If Not String.IsNullOrEmpty(link.Text) Then
                link.Style.Add("float", "right")
                controls.Add(link)
            End If

			spacerBr = New Literal
			spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            ' contract end reason
            lbl = New Label()
            lbl.Text = "End Reason"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", Me.HeaderLabelWidth.ToString())
            controls.Add(lbl)

            If _contractEndReasonID > 0 Then
                contractEndReason = New ContractEndReason(Me.DbConnection, String.Empty, String.Empty)
                msg = contractEndReason.Fetch(_contractEndReasonID)
                If Not msg.Success Then Return msg
            End If

            text = New Label()
            text.Style.Add("float", "left")
            If contractEndReason Is Nothing Then
                If _contractEndReasonID = -1 Then
                    text.Text = "None"
                ElseIf _contractEndReasonID = -2 Then
                    text.Text = "Any"
                Else
                    text.Text = "All"
                End If
            Else
                text.Text = contractEndReason.Description
            End If
            controls.Add(text)

            link = New HyperLink()
            If Me.IsCurrentStep AndAlso _contractEndReasonID <> 0 Then
                ' all
                link.Text = "All End Reasons"
                Me.QueryString.Remove(QS_CONTRACTENDREASONID)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(Me.QueryString))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change End Reason"
                Me.QueryString.Remove(QS_CURRENTSTEP)
                Me.QueryString.Add(QS_CURRENTSTEP, MyBase.StepIndex)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(Me.QueryString))
            End If
            If Not String.IsNullOrEmpty(link.Text) Then
                link.Style.Add("float", "right")
                controls.Add(link)
            End If

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

			' date range
			msg = MyBase.RenderHeaderControls(controls)
			If Not msg.Success Then Return msg

			msg = New ErrorMessage
			msg.Success = True

			Return msg

		End Function

		Public Overrides ReadOnly Property Title() As String
			Get
                Return "Select a Contract Type, Group, Period and End Reason"
			End Get
		End Property

		Public Overrides Sub PreRender(ByVal wizard As SelectorWizardBase)

            wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Web.Apps.UserControls.SelectorWizard.DomContractFilterStep.Startup", _
             Target.Library.Web.Utils.WrapClientScript( _
              String.Format("DomContractFilterStep_dateFromID='{0}';DomContractFilterStep_dateToID='{1}';DomContractFilterStep_contractTypeID='{2}';DomContractFilterStep_contractGroupID='{3}';DomContractFilterStep_contractEndReasonID='{4}';DomContractFilterStep_serviceGroupID='{5}';", _
               MyBase.DateFromControl.ClientID, MyBase.DateToControl.ClientID, _cboContractType.ClientID, _cboContractGroup.ClientID, _cboContractEndReason.ClientID, _cboServiceGroup.ClientID) _
             ) _
            )

        End Sub

        Public Overrides Sub InitStep(ByVal wizard As SelectorWizardBase)
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/DomContractFilterStep.js"))
        End Sub

        Public Overrides ReadOnly Property BeforeNavigateJS() As String
            Get
                Return "DomContractFilterStep_BeforeNavigate()"
            End Get
        End Property

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
    '''     MoTahir     09/11/2009  D11681
    '''     MikeVO      25/09/2009  D11546 - changes to "selections" section links
    '''     MikeVO      22/04/2008  A4WA#5395 - changed default display for Terminate/Reinstate buttons to False.
    '''     MikeVO      04/04/2008  Added service user column support.
    ''' 	[Mikevo]	16/01/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Class DomContractStep
        Implements ISelectorWizardStep

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_ESTABLISHMENTID As String = "estabID"
        Const QS_CONTRACTTYPEID As String = "ctID"
        Const QS_CONTRACTGROUPID As String = "cgID"
        Const QS_SERVICEGROUPID As String = "svcGroupID"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETo As String = "dateTo"
        Const QS_CONTRACTID As String = "contractID"
        Const QS_CONTRACTENDREASONID As String = "cerID"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _description As String = "The list below displays the domiciliary contracts that match your search criteria."
        Private _title As String = "Results"
        Private _estabID As Integer
        Private _contractType As DomContractType
        Private _contractGroupID As Integer
        Private _serviceGroupID As Integer
        Private _dateFrom As Date
        Private _dateTo As Date
        Private _contractEndReasonID As Integer
        Private _showHeaderControls As Boolean
        Private _showNewButton As Boolean
        Private _showViewButton As Boolean
        Private _showCopyButton As Boolean
        Private _showTerminateButton As Boolean
        Private _showReinstateButton As Boolean
        Private _contractID As Integer
        Private _showServiceUserColumn As Boolean
        Private _headerLabelWidth As Unit = New Unit(10, UnitType.Em)
        Private _currentStep As Integer
        Private _serviceGroupClassificationID As Integer = 0
        Sub New()
            _showHeaderControls = False
            _showNewButton = True
            _showViewButton = True
            _showCopyButton = True
            _showTerminateButton = False
            _showReinstateButton = False
            _showServiceUserColumn = False
        End Sub
        Public Property ServiceGroupClassificationID() As Integer
            Get
                Return _serviceGroupClassificationID
            End Get
            Set(ByVal value As Integer)
                _serviceGroupClassificationID = value
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

        Public Property ShowHeaderControls() As Boolean
            Get
                Return _showHeaderControls
            End Get
            Set(ByVal value As Boolean)
                _showHeaderControls = value
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

        Public Property ShowCopyButton() As Boolean
            Get
                Return _showCopyButton
            End Get
            Set(ByVal value As Boolean)
                _showCopyButton = value
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

        Public Property ShowServiceUserColumn() As Boolean
            Get
                Return _showServiceUserColumn
            End Get
            Set(ByVal value As Boolean)
                _showServiceUserColumn = value
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
                _contractEndReasonID = Utils.ToInt32(_queryString(QS_CONTRACTENDREASONID))
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETo)) Then _dateTo = _queryString(QS_DATETo)
                _contractID = Utils.ToInt32(_queryString(QS_CONTRACTID))
                _serviceGroupID = Utils.ToInt32(_queryString(QS_SERVICEGROUPID))
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
                    contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
                    msg = contract.Fetch(_contractID)
                    If Not msg.Success Then Return msg
                    text.Text = String.Format("{0}: {1}", contract.Number, contract.Title)

                    ' store MRU provider
                    Dim mruManager As Target.Library.Web.MostRecentlyUsedManager = New Target.Library.Web.MostRecentlyUsedManager(HttpContext.Current)
                    mruManager("DOM_CONTRACTS")(_contractID.ToString()) = text.Text
                    mruManager.Save(HttpContext.Current)

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
            Dim contractList As DomContractSelector = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/DomContractSelector.ascx")
            contractList.InitControl(wizard.Page, _estabID, _contractType, _contractGroupID, _dateFrom, _dateTo, _
                                     _contractEndReasonID, _showNewButton, _showViewButton, _showCopyButton, _
                                     _showReinstateButton, _showTerminateButton, _showServiceUserColumn, _contractID, _serviceGroupID, _serviceGroupClassificationID)
            controls.Add(contractList)
            msg.Success = True
            Return msg
        End Function

    End Class

#End Region

#Region " ContractStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.SelectorWizardSteps.ContractStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Implements the select contract step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	Paul W      29/06/2010  D11795 - SDS, Generic Contracts and Service Orders
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Class ContractStep
        Implements ISelectorWizardStep

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_ESTABLISHMENTID As String = "estabID"
        Const QS_CONTRACTGROUPID As String = "cgID"
        Const QS_SERVICEGROUPID As String = "svcGroupID"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETo As String = "dateTo"
        Const QS_CONTRACTID As String = "contractID"
        Const QS_CONTRACTENDREASONID As String = "cerID"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _description As String = "The list below displays the contracts that match your search criteria."
        Private _title As String = "Results"
        Private _estabID As Integer
        Private _contractGroupID As Integer
        Private _serviceGroupID As Integer
        Private _dateFrom As Date
        Private _dateTo As Date
        Private _contractEndReasonID As Integer
        Private _showHeaderControls As Boolean
        Private _showNewButton As Boolean
        Private _showViewButton As Boolean
        Private _showCopyButton As Boolean
        Private _showTerminateButton As Boolean
        Private _showReinstateButton As Boolean
        Private _contractID As Integer
        Private _showServiceUserColumn As Boolean
        Private _headerLabelWidth As Unit = New Unit(10, UnitType.Em)
        Private _currentStep As Integer
        Private _serviceGroupClassificationID As Integer = 0
        Sub New()
            _showHeaderControls = False
            _showNewButton = True
            _showViewButton = True
            _showCopyButton = True
            _showTerminateButton = False
            _showReinstateButton = False
            _showServiceUserColumn = False
        End Sub
        Public Property ServiceGroupClassificationID() As Integer
            Get
                Return _serviceGroupClassificationID
            End Get
            Set(ByVal value As Integer)
                _serviceGroupClassificationID = value
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

        Public Property ShowHeaderControls() As Boolean
            Get
                Return _showHeaderControls
            End Get
            Set(ByVal value As Boolean)
                _showHeaderControls = value
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

        Public Property ShowCopyButton() As Boolean
            Get
                Return _showCopyButton
            End Get
            Set(ByVal value As Boolean)
                _showCopyButton = value
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

        Public Property ShowServiceUserColumn() As Boolean
            Get
                Return _showServiceUserColumn
            End Get
            Set(ByVal value As Boolean)
                _showServiceUserColumn = value
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
                _contractGroupID = Utils.ToInt32(_queryString(QS_CONTRACTGROUPID))
                _contractEndReasonID = Utils.ToInt32(_queryString(QS_CONTRACTENDREASONID))
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETo)) Then _dateTo = _queryString(QS_DATETo)
                _contractID = Utils.ToInt32(_queryString(QS_CONTRACTID))
                _serviceGroupID = Utils.ToInt32(_queryString(QS_SERVICEGROUPID))
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
                Return "ContractStep_BeforeNavigate()"
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
              String.Format("ContractStep_required={0};", _required.ToString().ToLower()) _
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
                    contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
                    msg = contract.Fetch(_contractID)
                    If Not msg.Success Then Return msg
                    text.Text = String.Format("{0}: {1}", contract.Number, contract.Title)

                    ' store MRU provider
                    Dim mruManager As Target.Library.Web.MostRecentlyUsedManager = New Target.Library.Web.MostRecentlyUsedManager(HttpContext.Current)
                    mruManager("CONTRACTS")(_contractID.ToString()) = text.Text
                    mruManager.Save(HttpContext.Current)

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
            Dim contractList As ContractSelector = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/ContractSelector.ascx")
            contractList.InitControl(wizard.Page, _estabID, _contractGroupID, _dateFrom, _dateTo, _
                                     _contractEndReasonID, _showNewButton, _showViewButton, _showCopyButton, _
                                     _showReinstateButton, _showTerminateButton, _showServiceUserColumn, _
                                     _contractID, _serviceGroupID, _serviceGroupClassificationID)
            controls.Add(contractList)
            msg.Success = True
            Return msg
        End Function

    End Class

#End Region

#Region " RegisterStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.SelectorWizardSteps.RegisterStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Implements the select register step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MoTahir      07/10/2009 - D11681
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Class RegisterStep
        Implements ISelectorWizardStep

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_ESTABLISHMENTID As String = "estabID"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETO As String = "dateTo"
        Const QS_REGISTERID As String = "registerid"
        Const QS_CONTRACTID As String = "contractid"
        Const QS_INPROGRESS As String = "status_ip"
        Const QS_SUBMITTED As String = "status_s"
        Const QS_AMENDED As String = "status_a"
        Const QS_PROCESSED As String = "status_p"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _description As String = "The list below displays the completed registers that match your search criteria."
        Private _title As String = "Results"
        Private _estabID As Integer
        Private _dateFrom As Date
        Private _dateTo As Date
        Private _inProgress As Boolean
        Private _submitted As Boolean
        Private _amended As Boolean
        Private _processed As Boolean
        Private _showHeaderControls As Boolean
        Private _showNewButton As Boolean
        Private _showViewButton As Boolean
        Private _registerID As Integer
        Private _contractID As Integer
        Private _headerLabelWidth As Unit = New Unit(10, UnitType.Em)
        Private _currentStep As Integer

        Sub New()
            _showHeaderControls = False
            _showNewButton = True
            _showViewButton = True
        End Sub

        Public Property HeaderLabelWidth() As Unit
            Get
                Return _headerLabelWidth
            End Get
            Set(ByVal value As Unit)
                _headerLabelWidth = value
            End Set
        End Property

        Public Property ShowHeaderControls() As Boolean
            Get
                Return _showHeaderControls
            End Get
            Set(ByVal value As Boolean)
                _showHeaderControls = value
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
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETO)) Then _dateTo = _queryString(QS_DATETO)
                _registerID = Utils.ToInt32(_queryString(QS_REGISTERID))
                _currentStep = Utils.ToInt32(_queryString(QS_CURRENTSTEP))
                _contractID = Utils.ToInt32(_queryString(QS_CONTRACTID))
                If (_queryString(QS_INPROGRESS) = Nothing) And (_queryString(QS_SUBMITTED) = Nothing) And _
                    (_queryString(QS_AMENDED) = Nothing) And (_queryString(QS_PROCESSED) = Nothing) Then
                    _inProgress = True
                    _submitted = True
                    _amended = True
                    _processed = True
                Else
                    _inProgress = IIf(_queryString(QS_INPROGRESS) = "true", True, False)
                    _submitted = IIf(_queryString(QS_SUBMITTED) = "true", True, False)
                    _amended = IIf(_queryString(QS_AMENDED) = "true", True, False)
                    _processed = IIf(_queryString(QS_PROCESSED) = "true", True, False)
                End If
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
                Return "RegisterStep_BeforeNavigate()"
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
              String.Format("RegisterStep_required={0};", _required.ToString().ToLower()) _
            ))
        End Sub

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls

            Dim msg As ErrorMessage = New ErrorMessage

            If _showHeaderControls Then
                Dim text As Label = New Label
                Dim reg As Register
                Dim est As Establishment = Nothing
                Dim contract As Target.Abacus.Library.DataClasses.DomContract = Nothing
                Dim regRows As RegisterRowCollection = Nothing
                Dim contractID As Integer = 0
                Dim lbl As Label = New Label

                ' label
                lbl.Text = "Register"
                lbl.Style.Add("float", "left")
                lbl.Style.Add("font-weight", "bold")
                lbl.Style.Add("width", _headerLabelWidth.ToString())
                controls.Add(lbl)

                text.Style.Add("float", "left")
                If _registerID = 0 Then
                    text.Text = "All"
                Else
                    reg = New Register(Me.DbConnection, String.Empty, String.Empty)
                    msg = reg.Fetch(_registerID)
                    If Not msg.Success Then Return msg

                    est = New Establishment(Me.DbConnection)
                    msg = est.Fetch(reg.ProviderID)

                    msg = RegisterRow.FetchList(Me.DbConnection, regRows, String.Empty, String.Empty)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    For Each row As RegisterRow In regRows
                        contractID = row.DomContractID
                        If contractID > 0 Then Exit For
                    Next

                    contract = New Target.Abacus.Library.DataClasses.DomContract(Me.DbConnection, String.Empty, String.Empty)
                    msg = contract.Fetch(contractID)

                    text.Text = String.Format("{0}: {1}: {2}", [String].Format("{0:dd/MM/yyyy}", reg.WeekEnding), est.Name, contract.Number)

                    ' store MRU provider
                    Dim mruManager As Target.Library.Web.MostRecentlyUsedManager = New Target.Library.Web.MostRecentlyUsedManager(HttpContext.Current)
                    mruManager("ATTENDANCEREGISTERS")(_registerID.ToString()) = text.Text
                    mruManager.Save(HttpContext.Current)

                End If
                controls.Add(text)
            End If

            msg.Success = True
            Return msg

        End Function

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls
            Dim msg As ErrorMessage = New ErrorMessage
            Dim registerList As RegisterSelector = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/RegisterSelector.ascx")
            registerList.InitControl(wizard.Page, _estabID, _dateFrom, _dateTo, _showNewButton, _showViewButton, _
                                     _registerID, _contractID, _inProgress, _submitted, _amended, _processed)
            controls.Add(registerList)
            msg.Success = True
            Return msg
        End Function

    End Class

#End Region

#Region " RegisterFilterStep "

    ''' <summary>
    ''' Implements a wizard step to select register period.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MoTahir      07/10/2009  D11681
    ''' </history>
    Friend Class RegisterFilterStep
        Inherits DateRangeStep

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_ESTABLISHMENTID As String = "estabID"
        Const QS_INPROGRESS As String = "status_ip"
        Const QS_SUBMITTED As String = "status_s"
        Const QS_AMENDED As String = "status_a"
        Const QS_PROCESSED As String = "status_p"

        Private _queryString As NameValueCollection
        Private _estabID As Integer
        Private _inProgress As Boolean
        Private _submitted As Boolean
        Private _amended As Boolean
        Private _processed As Boolean
        Private _currentStep As Integer

        Private _chkInprogressControl As CheckBoxEx = New CheckBoxEx
        Private _chkSubmittedControl As CheckBoxEx = New CheckBoxEx
        Private _chkAmendedControl As CheckBoxEx = New CheckBoxEx
        Private _chkProcessedControl As CheckBoxEx = New CheckBoxEx

        Sub New()
            MyBase.New()
            Me.Description = "Please select the period to filter the results on."
            Me.DateFrom = Nothing
            Me.ShowHeaderLink = True
            Me.HeaderLabelWidth = New Unit(10, UnitType.Em)
            Me.ContentLabelWidth = New Unit(10, UnitType.Em)
            _inProgress = True
            _submitted = True
            _amended = True
            _processed = True
        End Sub

        Public Overrides Property QueryString() As System.Collections.Specialized.NameValueCollection
            Get
                Return New NameValueCollection(_queryString)
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                MyBase.QueryString = Value
                _queryString = Value
                _estabID = Utils.ToInt32(_queryString(QS_ESTABLISHMENTID))
                _currentStep = Utils.ToInt32(_queryString(QS_CURRENTSTEP))
                If (_queryString(QS_INPROGRESS) = Nothing) And (_queryString(QS_SUBMITTED) = Nothing) And _
                    (_queryString(QS_AMENDED) = Nothing) And (_queryString(QS_PROCESSED) = Nothing) Then
                    _inProgress = True
                    _submitted = True
                    _amended = True
                    _processed = True
                Else
                    _inProgress = IIf(_queryString(QS_INPROGRESS) = "true", True, False)
                    _submitted = IIf(_queryString(QS_SUBMITTED) = "true", True, False)
                    _amended = IIf(_queryString(QS_AMENDED) = "true", True, False)
                    _processed = IIf(_queryString(QS_PROCESSED) = "true", True, False)
                End If
            End Set
        End Property

        Public Overrides Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage

            Dim msg As ErrorMessage = New ErrorMessage
            Dim spacerBr As Literal
            Dim user As WebSecurityUser
            Dim lbl As Label

            user = SecurityBL.GetCurrentUser()

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            msg = MyBase.RenderContentControls(wizard, controls)
            If Not msg.Success Then Return msg

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            ' label
            lbl = New Label()
            lbl.Text = "Register Status"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", MyBase.ContentLabelWidth.ToString)
            controls.Add(lbl)

            With _chkInprogressControl
                .ID = "chkInProgress"
                .Text = "In Progress"
                .CheckBoxCssClass = "chkBoxStyle"
                .CheckBox.TextAlign = TextAlign.Right
                .CheckBox.Checked = _inProgress
            End With
            controls.Add(_chkInprogressControl)

            With _chkSubmittedControl
                .ID = "chkSubmitted"
                .Text = "Submitted"
                .CheckBoxCssClass = "chkBoxStyle"
                .CheckBox.TextAlign = TextAlign.Right
                .CheckBox.Checked = _submitted
            End With
            controls.Add(_chkSubmittedControl)

            With _chkAmendedControl
                .ID = "chkAmended"
                .Text = "Amended"
                .CheckBoxCssClass = "chkBoxStyle"
                .CheckBox.TextAlign = TextAlign.Right
                .CheckBox.Checked = _amended
            End With
            controls.Add(_chkAmendedControl)


            With _chkProcessedControl
                .ID = "chkProcessed"
                .Text = "Processed"
                .CheckBoxCssClass = "chkBoxStyle"
                .CheckBox.TextAlign = TextAlign.Right
                .CheckBox.Checked = _processed
            End With
            controls.Add(_chkProcessedControl)

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
            
            ' if a provider has been selected show the date range link
            If _estabID <> 0 Then Me.ShowHeaderLink = True

            ' date range
            msg = MyBase.RenderHeaderControls(controls)
            If Not msg.Success Then Return msg

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            ' batch status
            lbl = New Label()
            lbl.Text = "Register Status"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", MyBase.HeaderLabelWidth.ToString())
            controls.Add(lbl)
            ' add the list of ticked status values
            text = New Label()
            text.Style.Add("float", "left")

            If _inProgress = True Then
                text.Text = String.Format("{0}, In Progress", text.Text)
                Me.QueryString.Add(QS_INPROGRESS, "true")
            End If
            If _submitted = True Then
                text.Text = String.Format("{0}, Submitted", text.Text)
                Me.QueryString.Add(QS_SUBMITTED, "true")
            End If
            If _amended = True Then
                text.Text = String.Format("{0}, Amended", text.Text)
                Me.QueryString.Add(QS_AMENDED, "true")
            End If
            If _processed = True Then
                text.Text = String.Format("{0}, Processed", text.Text)
                Me.QueryString.Add(QS_PROCESSED, "true")
            End If

            ' remove leading ", "
            If text.Text.Length > 0 Then text.Text = text.Text.Substring(2)
            controls.Add(text)

            ' add the link
            link = New HyperLink()
            qs = Me.QueryString
            If Me.IsCurrentStep Then
                ' all
                link.Text = "All Register Status Values"
                qs.Remove(QS_INPROGRESS)
                qs.Remove(QS_SUBMITTED)
                qs.Remove(QS_AMENDED)
                qs.Remove(QS_PROCESSED)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change Register Status"
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
                Return "Select a Period"
            End Get
        End Property

        Public Overrides Sub PreRender(ByVal wizard As SelectorWizardBase)

            wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Web.Apps.UserControls.SelectorWizard.RegisterFilterStep.Startup", _
             Target.Library.Web.Utils.WrapClientScript( _
              String.Format("RegisterFilterStep_dateFromID='{0}';RegisterFilterStep_dateToID='{1}';RegisterFilterStep_InProgressID='{2}';RegisterFilterStep_SubmittedID='{3}'; RegisterFilterStep_AmendedID='{4}'; RegisterFilterStep_ProcessedID='{5}';", _
               MyBase.DateFromControl.ClientID, MyBase.DateToControl.ClientID, _chkInprogressControl.ClientID, _chkSubmittedControl.ClientID, _chkAmendedControl.ClientID, _chkProcessedControl.ClientID) _
             ) _
            )

        End Sub

        Public Overrides Sub InitStep(ByVal wizard As SelectorWizardBase)
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/RegisterFilterStep.js"))
        End Sub

        Public Overrides ReadOnly Property BeforeNavigateJS() As String
            Get
                Return "RegisterFilterStep_BeforeNavigate()"
            End Get
        End Property

    End Class

#End Region

#Region " ServiceOrderStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.SelectorWizardSteps.DomServiceOrderStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Implements the select domiciliary service order step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     Paul W      29/06/2010  D11795 - SDS, Generic Contracts and Service Orders
    '''     ColinD      22/04/2010  D11807 - added copy button functionality
    '''     MikeVO      15/12/2009  A4WA#5967 - fix when the specified DSO on the querystring does not exist.
    '''     MikeVO      25/09/2009  D11546 - changes to "selections" section links
    '''     MikeVO      23/09/2009  D11701 - added header controls.
    ''' 	JohnF   	09/02/2009	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Class ServiceOrderStep
        Implements ISelectorWizardStep

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_DSOID As String = "dsoID"
        Const QS_ESTABLISHMENTID As String = "estabID"
        Const QS_CONTRACTID As String = "contractID"
        Const QS_CLIENTID As String = "clientID"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETo As String = "dateTo"
        Const QS_SERVICEGROUPID As String = "svcGroupID"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _description As String = "The list below displays the service orders that match your search criteria."
        Private _dsoID As Integer
        Private _estabID As Integer
        Private _contractID As Integer
        Private _clientID As Integer
        Private _serviceGroupID As Integer
        Private _dateFrom As Date
        Private _dateTo As Date
        Private _showNewButton As Boolean
        Private _showCopyButton As Boolean
        Private _showHeaderControls As Boolean
        Private _headerLabelWidth As Unit
        Private _currentStep As Integer

        Sub New()
            _showCopyButton = False
            _showNewButton = True
            _showHeaderControls = False
            _headerLabelWidth = New Unit(10, UnitType.Em)
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
                _dsoID = Utils.ToInt32(_queryString(QS_DSOID))
                _estabID = Utils.ToInt32(_queryString(QS_ESTABLISHMENTID))
                _contractID = Utils.ToInt32(_queryString(QS_CONTRACTID))
                _clientID = Utils.ToInt32(_queryString(QS_CLIENTID))
                _serviceGroupID = Utils.ToInt32(_queryString(QS_SERVICEGROUPID))
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETo)) Then _dateTo = _queryString(QS_DATETo)
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
                Return "serviceOrderStep_BeforeNavigate()"
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

        Public Property ShowCopyButton() As Boolean
            Get
                Return _showCopyButton
            End Get
            Set(ByVal value As Boolean)
                _showCopyButton = value
            End Set
        End Property

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

        Public Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep

        End Sub

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender
            wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", _
             Target.Library.Web.Utils.WrapClientScript( _
              String.Format("serviceOrderStep_required={0};", _required.ToString().ToLower()) _
            ))
        End Sub

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls

            Dim msg As ErrorMessage = New ErrorMessage
            Dim qs As NameValueCollection

            If _showHeaderControls Then

                Dim lbl As Label = New Label
                Dim text As Label = New Label
                Dim link As HyperLink = New HyperLink
                Dim dso As DomServiceOrder

                ' label
                lbl.Text = "Service Order"
                lbl.Style.Add("float", "left")
                lbl.Style.Add("font-weight", "bold")
                lbl.Style.Add("width", _headerLabelWidth.ToString())
                controls.Add(lbl)

                text.Style.Add("float", "left")
                If _dsoID = 0 Then
                    text.Text = "All"
                Else
                    dso = New DomServiceOrder(Me.DbConnection, String.Empty, String.Empty)
                    msg = dso.Fetch(_dsoID)
                    If msg.Success Then text.Text = dso.OrderReference
                End If
                controls.Add(text)

                ' add the link
                qs = Me.QueryString
                If Me.IsCurrentStep AndAlso _dsoID > 0 Then
                    ' all
                    link.Text = "All Service Orders"
                    qs.Remove(QS_DSOID)
                    link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
                ElseIf _currentStep > Me.StepIndex Then
                    ' change
                    link.Text = "Change Service Order"
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
            Dim soList As ServiceOrderSelector = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/ServiceOrderSelector.ascx")
            soList.InitControl(wizard.Page, _dsoID, _estabID, _contractID, _clientID, _dateFrom, _dateTo, _serviceGroupID, _showNewButton, _showCopyButton)
            controls.Add(soList)
            msg.Success = True
            Return msg
        End Function

    End Class

#End Region

#Region " DebtorInvoiceTypeFilterStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : UserControls.DebtorInvoiceTypeFilterStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Class that implements invoice type criteria filtering for debtor invoice enquiries.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' MikeVO    25/09/2009  D11546 - changes to "selections" section links
    ''' MikeVO    23/07/2009  D11651 - parameter consolidation
    ''' JohnF     29/04/2009  Created (D11604)
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DebtorInvoiceTypeFilterStep
        Implements ISelectorWizardStep

        Const QS_CURRENTSTEP As String = "currentStep"

        Private _dbConnection As SqlConnection
        Private _currentStep As Integer
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _description As String = "Please select the criteria to filter the invoices on."
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
                Return "DebtorInvoiceTypeSelector_BeforeNavigate()"
            End Get
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

            Dim msg As ErrorMessage = New ErrorMessage
            Dim invType As DebtorInvoiceTypeSelector = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/DebtorInvoiceTypeSelector.ascx")
            invType.InitControl(wizard.Page, _qsParser, Me.DbConnection)
            controls.Add(invType)
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
            lbl.Text = "Invoice Types"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", _headerLabelWidth.ToString())
            controls.Add(lbl)

            text = New Label()
            text.Style.Add("float", "left")
            text.Style.Add("width", _headerDescWidth.ToString())
            text.Text = _qsParser.SelectedInvoiceTypesDesc
            controls.Add(text)

            ' add the link
            qs = Me.QueryString
            If Me.IsCurrentStep Then
                ' all
                link.Text = "All Invoice Types"
                qs.Remove(WizardScreenParameters.QS_INVOICES_RES)
                qs.Remove(WizardScreenParameters.QS_INVOICES_DOM)
                qs.Remove(WizardScreenParameters.QS_INVOICES_LD)
                qs.Remove(WizardScreenParameters.QS_INVOICES_CLIENT)
                qs.Remove(WizardScreenParameters.QS_INVOICES_TP)
                qs.Remove(WizardScreenParameters.QS_INVOICES_PROP)
                qs.Remove(WizardScreenParameters.QS_INVOICES_OLA)
                qs.Remove(WizardScreenParameters.QS_INVOICES_PEN_COLL)
                qs.Remove(WizardScreenParameters.QS_INVOICES_HOME_COLL)
                qs.Remove(WizardScreenParameters.QS_INVOICES_STD)
                qs.Remove(WizardScreenParameters.QS_INVOICES_MAN)
                qs.Remove(WizardScreenParameters.QS_INVOICES_SDS)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change Invoice Types"
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
                Return "Select the invoice types to filter the results on."
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

#Region " DebtorInvoiceGeneralFilterStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : UserControls.DebtorInvoiceGeneralFilterStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Class that implements Invoice Number, Weekending date range, status and 
    ''' status date range filters for domicilary provider invoice enquiries.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' JohnF     20/10/2009  D11684 - Zero-value and Debtors-excluded properties added
    ''' MikeVO    25/09/2009  D11546 - changes to "selections" section links
    ''' MikeVO    23/07/2009  D11651 - parameter consolidation
    ''' JohnF     30/04/2009  Created (D11604)
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DebtorInvoiceGeneralFilterStep
        Implements ISelectorWizardStep

        Const QS_CURRENTSTEP As String = "currentStep"

        Private _dbConnection As SqlConnection
        Private _currentStep As Integer
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _description As String = "Please select the criteria to filter the invoices on."
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
                Return "DebtorInvoiceGeneralSelector_BeforeNavigate()"
            End Get
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

            Dim msg As ErrorMessage = New ErrorMessage
            Dim invType As DebtorInvoiceGeneralSelector = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/DebtorInvoiceGeneralSelector.ascx")
            invType.InitControl(wizard.Page, _qsParser)
            controls.Add(invType)
            msg.Success = True
            Return msg

        End Function

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls

            Dim msg As ErrorMessage
            Dim lbl As Label
            Dim text As Label
            Dim link As HyperLink
            Dim spacerBr As Literal
            Dim qs As NameValueCollection

            Me.QueryString = _queryString

            lbl = New Label()
            lbl.Text = "Creation Dates"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", _headerLabelWidth.ToString())
            controls.Add(lbl)

            text = New Label()
            text.Style.Add("float", "left")
            text.Text = _qsParser.SelectedCreationDatesDesc
            controls.Add(text)

            ' add the link
            link = New HyperLink()
            qs = Me.QueryString
            If Me.IsCurrentStep Then
                ' all
                link.Text = "All Dates"
                qs.Remove(WizardScreenParameters.QS_INVOICES_DATE_FROM)
                qs.Remove(WizardScreenParameters.QS_INVOICES_DATE_TO)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change Dates"
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

            '++ Add the list of ticked "Other Settings"..
            lbl = New Label()
            lbl.Text = "Other Settings"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", _headerLabelWidth.ToString())
            controls.Add(lbl)

            text = New Label()
            text.Style.Add("float", "left")
            text.Style.Add("width", _headerDescWidth.ToString())
            text.Text = _qsParser.SelectedOtherSettingsDesc
            controls.Add(text)

            ' add the link
            link = New HyperLink()
            qs = Me.QueryString
            If Me.IsCurrentStep Then
                ' all
                link.Text = "All Other Settings"
                qs.Remove(WizardScreenParameters.QS_INVOICES_ACTUAL)
                qs.Remove(WizardScreenParameters.QS_INVOICES_PROVISIONAL)
                qs.Remove(WizardScreenParameters.QS_INVOICES_RETRACTED)
                qs.Remove(WizardScreenParameters.QS_INVOICES_VIA_RETRACT)
                qs.Remove(WizardScreenParameters.QS_INVOICES_ZERO_VALUE)
                qs.Remove(WizardScreenParameters.QS_INVOICES_BATCH_SEL)
                qs.Remove(WizardScreenParameters.QS_INVOICES_EXCLUDE)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change Settings"
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
                Return "Select the invoice criteria to filter the results on."
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

#Region " DebtorInvoiceResultsStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : UserControls.DebtorInvoiceResultsStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Class that lists the debtor invoices satisying the criteria 
    ''' specified on the previous wizard steps.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' MikeVO    25/09/2009  D11546 - changes to "selections" section links
    ''' MikeVO    24/09/2009  D11701 - added header controls
    ''' MikeVO    23/07/2009  D11651 - parameter consolidation
    ''' JohnF     01/05/2009  Created (D11604)
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DebtorInvoiceResultsStep
        Implements ISelectorWizardStep

        Const QS_CURRENTSTEP As String = "currentStep"

        Private _dbConnection As SqlConnection
        Private _currentStep As Integer

        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _description As String = "The list below displays the results of your Debtor Invoice enquiry."
        Private _qsParser As WizardScreenParameters
        Private _showHeaderControls As Boolean
        Private _headerLabelWidth As Unit

        Sub New()
            _showHeaderControls = False
            _headerLabelWidth = New Unit(10, UnitType.Em)
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
                Return "DebtorInvoiceResults_BeforeNavigate()"
            End Get
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

            Dim msg As ErrorMessage = New ErrorMessage
            Dim invType As DebtorInvoiceResults = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/DebtorInvoiceResults.ascx")
            invType.InitControl(wizard.Page, _qsParser)
            controls.Add(invType)
            msg.Success = True
            Return msg

        End Function

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls

            Dim msg As ErrorMessage = New ErrorMessage
            Dim qs As NameValueCollection

            If _showHeaderControls Then

                Dim lbl As Label = New Label
                Dim text As Label = New Label
                Dim link As HyperLink = New HyperLink
                Dim inv As Invoice

                ' label
                lbl.Text = "Invoice"
                lbl.Style.Add("float", "left")
                lbl.Style.Add("font-weight", "bold")
                lbl.Style.Add("width", _headerLabelWidth.ToString())
                controls.Add(lbl)

                text.Style.Add("float", "left")
                If _qsParser.InvoiceID = 0 Then
                    text.Text = "All"
                Else
                    inv = New Invoice(Me.DbConnection)
                    msg = inv.Fetch(_qsParser.InvoiceID)
                    If Not msg.Success Then Return msg
                    text.Text = inv.InvoiceNumber
                End If
                controls.Add(text)


                ' add the link
                qs = Me.QueryString
                If Me.IsCurrentStep AndAlso _qsParser.InvoiceID > 0 Then
                    ' all
                    link.Text = "All Invoices"
                    qs.Remove(WizardScreenParameters.QS_INVOICES_ID)
                    link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
                ElseIf _currentStep > Me.StepIndex Then
                    ' change
                    link.Text = "Change Invoice"
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

#Region " DomProviderInvoiceFilterStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : UserControls.DomProviderInvoiceFilterStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Class that implements Invoice Number, Weekending date range, status and 
    ''' status date range filters for domicilary provider invoice enquiries.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' MikeVO    25/09/2009  D11546 - changes to "selections" section links
    ''' JohnF     11/02/2009  Created (D11494)
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DomProviderInvoiceFilterStep
        Inherits DateRangeStep

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_INVOICE_NUMBER As String = "invoiceNumber"
        Const QS_INVOICE_REF As String = "invoiceRef"
        Const QS_WEFROM As String = "weFrom"
        Const QS_WETO As String = "weTo"
        Const QS_PAID As String = "paid"
        Const QS_UNPAID As String = "unPaid"
        Const QS_AUTHORIZED As String = "authorized"
        Const QS_SUSPENDED As String = "suspended"
        Const QS_STATUSFROM As String = "dateFrom"
        Const QS_STATUSTO As String = "dateTo"
        Const QS_EXCLUDE As String = "exclude"

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
        Private _invoiceRef As String
        Private _unpaid As Boolean
        Private _paid As Boolean
        Private _authorized As Boolean
        Private _suspended As Boolean
        Private _statusFrom As Date
        Private _statusTo As Date
        Private _excluded As String
        Private _description As String = "Please select the criteria to filter the results on."
        Private _tickedInvoiceStatus As DomProformaInvoiceBatchStatus
        Private _visibleInvoiceStatus As DomProformaInvoiceBatchStatus
        Private _invoiceStatusCheckboxes As List(Of CheckBoxEx)
        Private _weFromControl As TextBoxEx = New TextBoxEx
        Private _weToControl As TextBoxEx = New TextBoxEx
        Private _invoiceNumberControl As TextBoxEx = New TextBoxEx
        Private _invoiceRefControl As TextBoxEx = New TextBoxEx
        Private _unPaidControl As CheckBoxEx = New CheckBoxEx
        Private _paidControl As CheckBoxEx = New CheckBoxEx
        Private _authorizedControl As CheckBoxEx = New CheckBoxEx
        Private _suspendedControl As CheckBoxEx = New CheckBoxEx
        Private _excludedAll As RadioButton = New RadioButton
        Private _excludedNonExcludeOnly As RadioButton = New RadioButton
        Private _excludedExcludedOnly As RadioButton = New RadioButton
        Private _headerLabelWidth As Unit = New Unit(10, UnitType.Em)

        Sub New()
            MyBase.New()
            MyBase.DateFrom = Date.MinValue
            MyBase.DateTo = Date.MaxValue
            MyBase.ShowHeaderLink = False
            MyBase.HeaderLabelWidth = _headerLabelWidth

            ' by default, all status checkboxes are visible and selected
            _paid = True
            _unpaid = True
            _authorized = True
            _suspended = True
            _excluded = "null"
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
                Return MyBase.QueryString
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                MyBase.QueryString = Value
                _queryString = Value
                _currentStep = Utils.ToInt32(_queryString(QS_CURRENTSTEP))
                If (_queryString(QS_UNPAID) = Nothing) And (_queryString(QS_PAID) = Nothing) And _
                    (_queryString(QS_AUTHORIZED) = Nothing) And (_queryString(QS_SUSPENDED) = Nothing) And _
                    (_queryString(QS_EXCLUDE) = Nothing) Then
                    _unpaid = True
                    _paid = True
                    _authorized = True
                    _suspended = True
                    _excluded = "null"
                Else
                    _unpaid = IIf(_queryString(QS_UNPAID) = "true", True, False)
                    _paid = IIf(_queryString(QS_PAID) = "true", True, False)
                    _authorized = IIf(_queryString(QS_AUTHORIZED) = "true", True, False)
                    _suspended = IIf(_queryString(QS_SUSPENDED) = "true", True, False)
                    _excluded = IIf(_queryString(QS_EXCLUDE) = "" Or _queryString(QS_EXCLUDE) = "null", "null", _queryString(QS_EXCLUDE))
                End If

                _invoiceNumber = _queryString(QS_INVOICE_NUMBER)
                _invoiceRef = _queryString(QS_INVOICE_REF)
                If Target.Library.Utils.IsDate(_queryString(QS_WEFROM)) Then _weFrom = _queryString(QS_WEFROM)
                If Target.Library.Utils.IsDate(_queryString(QS_WETO)) Then _weTo = _queryString(QS_WETO)
                If Target.Library.Utils.IsDate(_queryString(QS_STATUSFROM)) Then _statusFrom = _queryString(QS_STATUSFROM)
                If Target.Library.Utils.IsDate(_queryString(QS_STATUSTO)) Then _statusTo = _queryString(QS_STATUSTO)
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
            Dim fs3 As HtmlGenericControl
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
            AddHandler _invoiceNumberControl.AfterTextBoxControlAdded, AddressOf AfterTextBoxControlAdded_InvoiceNumber
            fs.Controls.Add(_invoiceNumberControl)

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            fs.Controls.Add(spacerBr)

            With _invoiceRefControl
                .LabelText = "Invoice Ref."
                .LabelWidth = "8em"
                .LabelBold = True
                .Text = _invoiceRef
            End With
            AddHandler _invoiceRefControl.AfterTextBoxControlAdded, AddressOf AfterTextBoxControlAdded_InvoiceReference
            fs.Controls.Add(_invoiceRefControl)

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

            With _authorizedControl
                .ID = "chkAuthorized"
                .Text = "Authorised"
                .CheckBoxCssClass = "chkBoxStyle"
                .CheckBox.TextAlign = TextAlign.Right
                .CheckBox.Checked = _authorized
            End With
            fs2.Controls.Add(_authorizedControl)

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

            ' add controls from the base class, basically the date controls
            msg = MyBase.RenderContentControls(wizard, fs2.Controls)
            If Not msg.Success Then
                Return msg
            End If
            fs.Controls.Add(fs2)

            ' status
            fs3 = New HtmlGenericControl("FIELDSET")
            legend = New HtmlGenericControl("LEGEND")
            legend.InnerText = "Excluded from Creditors"
            fs3.Controls.Add(legend)

            With _excludedAll
                .ID = "optExcAllInvoices"
                .Text = "Show all invoices"
                .TextAlign = TextAlign.Left
                .GroupName = "optExcluded"
                If _excluded = String.Empty OrElse _excluded.ToLower = "null" Then
                    .Checked = True
                Else
                    .Checked = False
                End If
                .LabelAttributes.Add("style", "width:19em;")
            End With

            fs3.Controls.Add(_excludedAll)

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            fs3.Controls.Add(spacerBr)

            With _excludedNonExcludeOnly
                .ID = "optExcOnlyNonExcluded"
                .Text = "Show non creditors-excluded only"
                .TextAlign = TextAlign.Left
                .GroupName = "optExcluded"
                If _excluded = String.Empty OrElse _excluded.ToLower = "null" Then
                    .Checked = False
                Else
                    .Checked = Not Convert.ToBoolean(_excluded)
                End If
                .LabelAttributes.Add("style", "width:19em;")
            End With
            fs3.Controls.Add(_excludedNonExcludeOnly)

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            fs3.Controls.Add(spacerBr)

            With _excludedExcludedOnly
                .ID = "optExcOnlyExcluded"
                .Text = "Show creditors-excluded only"
                .TextAlign = TextAlign.Left
                .GroupName = "optExcluded"
                If _excluded = String.Empty OrElse _excluded.ToLower = "null" Then
                    .Checked = False
                Else
                    .Checked = Convert.ToBoolean(_excluded)
                End If
                .LabelAttributes.Add("style", "width:19em;")
            End With
            fs3.Controls.Add(_excludedExcludedOnly)
            fs.Controls.Add(fs3)

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            msg.Success = True
            Return msg

        End Function

        Private Sub AfterTextBoxControlAdded_InvoiceNumber(ByVal sender As TextBoxEx)
            AfterTextBoxControlAdded(sender, "Invoice Number")
        End Sub

        Private Sub AfterTextBoxControlAdded_InvoiceReference(ByVal sender As TextBoxEx)
            AfterTextBoxControlAdded(sender, "Invoice Reference")
        End Sub

        Private Sub AfterTextBoxControlAdded(ByVal sender As TextBoxEx, ByVal filterName As String)
            Dim anchor As HtmlAnchor = New HtmlAnchor
            Dim image As HtmlImage = New HtmlImage
            Dim space As Literal = New Literal

            space.Text = "&nbsp;"
            sender.Controls.Add(space)

            With image
                .Src = "~/Images/help16.png"
            End With

            With anchor
                .HRef = String.Format("javascript:ShowHelp('{0}');", filterName)
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
            If Not _invoiceNumber Is Nothing AndAlso _invoiceNumber.Length > 0 Then
                text.Text = _invoiceNumber
            Else
                text.Text = "All"
            End If
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

            ' Invoice Ref.
            lbl = New Label()
            lbl.Text = "Invoice Ref."
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", _headerLabelWidth.ToString())
            controls.Add(lbl)
            text = New Label
            text.Style.Add("float", "left")
            If Not _invoiceRef Is Nothing AndAlso _invoiceRef.Length > 0 Then
                text.Text = _invoiceRef
            Else
                text.Text = "All"
            End If
            controls.Add(text)

            ' add the link
            link = New HyperLink()
            qs = Me.QueryString
            If Me.IsCurrentStep Then
                ' all
                link.Text = "All Invoice Refs."
                qs.Remove(QS_INVOICE_REF)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change Invoice Ref."
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
            If Me.IsCurrentStep Then
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
            If _authorized = True Then
                text.Text = String.Format("{0}, Authorised", text.Text)
                Me.QueryString.Add(QS_AUTHORIZED, "true")
            End If
            If _paid = True Then
                text.Text = String.Format("{0}, Paid", text.Text)
                Me.QueryString.Add(QS_PAID, "true")
            End If
            If _suspended = True Then
                text.Text = String.Format("{0}, Suspended", text.Text)
                Me.QueryString.Add(QS_SUSPENDED, "true")
            End If
            If _excluded.ToLower = "true" Then
                text.Text = String.Format("{0}, Creditors-Excluded only", text.Text)
                Me.QueryString.Add(QS_EXCLUDE, "true")
            ElseIf _excluded.ToLower = "false" Then
                text.Text = String.Format("{0}, Non Creditors-Excluded only", text.Text)
                Me.QueryString.Add(QS_EXCLUDE, "false")
            Else
                Me.QueryString.Add(QS_EXCLUDE, "null")
            End If

            ' remove leading ", "
            If text.Text.Length > 0 Then text.Text = text.Text.Substring(2)
            controls.Add(text)

            ' add the link
            link = New HyperLink()
            qs = Me.QueryString
            If Me.IsCurrentStep Then
                ' all
                link.Text = "All Invoice Status Values"
                qs.Remove(QS_UNPAID)
                qs.Remove(QS_AUTHORIZED)
                qs.Remove(QS_PAID)
                qs.Remove(QS_SUSPENDED)
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

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            ' excluded from creditors
            lbl = New Label()
            lbl.Text = "Excluded"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", _headerLabelWidth.ToString())
            controls.Add(lbl)
            ' add the list of ticked status values
            text = New Label()
            text.Style.Add("float", "left")

            If _excluded.ToLower = "true" Then
                text.Text = String.Format("{0}, Creditors-Excluded only", text.Text)
                Me.QueryString.Add(QS_EXCLUDE, "true")
            ElseIf _excluded.ToLower = "false" Then
                text.Text = String.Format("{0}, Non Creditors-Excluded only", text.Text)
                Me.QueryString.Add(QS_EXCLUDE, "false")
            Else
                text.Text = String.Format("{0}, Show all invoices", text.Text)
                Me.QueryString.Add(QS_EXCLUDE, "null")
            End If
            MyBase.QueryString = _queryString

            ' remove leading ", "
            If text.Text.Length > 0 Then text.Text = text.Text.Substring(2)
            controls.Add(text)

            ' add the link
            link = New HyperLink()
            qs = Me.QueryString
            If Me.IsCurrentStep Then
                ' all
                link.Text = "Ignore Excluded Status"
                qs.Remove(QS_EXCLUDE)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change Excluded Status"
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

            Dim script As StringBuilder

            script = New StringBuilder()
            script.AppendFormat("SelectorWizard_id='{0}';", wizard.ClientID)
            script.AppendFormat("SelectorWizard_dateFromID='{0}';", MyBase.DateFromControl.ClientID)
            script.AppendFormat("SelectorWizard_dateToID='{0}';", MyBase.DateToControl.ClientID)
            script.AppendFormat("SelectorWizard_weFromID='{0}';", _weFromControl.ClientID)
            script.AppendFormat("SelectorWizard_weToID='{0}';", _weToControl.ClientID)
            script.AppendFormat("SelectorWizard_invoiceNumberID='{0}';", _invoiceNumberControl.ClientID)
            script.AppendFormat("SelectorWizard_invoiceRefID='{0}';", _invoiceRefControl.ClientID)
            script.AppendFormat("DateRangeStep_required={0};", MyBase.Required.ToString().ToLower())
            script.AppendFormat("SelectorWizard_unPaidID='{0}';", _unPaidControl.ClientID)
            script.AppendFormat("SelectorWizard_paidID='{0}';", _paidControl.ClientID)
            script.AppendFormat("SelectorWizard_authorizedID='{0}';", _authorizedControl.ClientID)
            script.AppendFormat("SelectorWizard_suspendedID='{0}';", _suspendedControl.ClientID)
            script.AppendFormat("SelectorWizard_optExcAllID='{0}';", _excludedAll.ClientID)
            script.AppendFormat("SelectorWizard_optExcNonExcludedOnlyID='{0}';", _excludedNonExcludeOnly.ClientID)
            script.AppendFormat("SelectorWizard_optExcExcludedOnlyID='{0}';", _excludedExcludedOnly.ClientID)
            wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", script.ToString(), True)


        End Sub

        Public Overrides Sub InitStep(ByVal wizard As SelectorWizardBase)
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/DomProviderInvoiceFilterStep.js"))
            CType(wizard.Page, Target.Web.Apps.BasePage).AddExtraCssStyle(".chkBoxStyle { float:left; margin-right:2em; }")
        End Sub

    End Class

#End Region

#Region " DomProviderInvoiceFilterResultsStep "

    ''' <summary>
    ''' Class that lists the domiciliary provider invoices satisfying the criteria 
    ''' specified on the previous wizard steps.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MikeVO    25/09/2009  D11546 - changes to "selections" section links
    ''' MikeVO    23/09/2009  D11701 - added header controls.
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DomProviderInvoiceFilterResultsStep
        Implements ISelectorWizardStep

        Const QS_FILEID As String = "fileID"
        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_ESTABLISHMENTID As String = "estabID"
        Const QS_CONTRACTID As String = "contractID"
        Const QS_CLIENTID As String = "clientID"
        Const QS_INVOICE_NUMBER As String = "invoiceNumber"
        Const QS_INVOICE_REF As String = "invoiceRef"
        Const QS_WE_FROM As String = "weFrom"
        Const QS_WE_TO As String = "weTo"
        Const QS_PAID As String = "paid"
        Const QS_UNPAID As String = "unPaid"
        Const QS_AUTHORIZED As String = "authorized"
        Const QS_SUSPENDED As String = "suspended"
        Const QS_EXCLUDE As String = "exclude"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETO As String = "dateTo"
        Const QS_INVOICE_ID As String = "invoiceID"

        Private _fileID As Integer
        Private _providerID As Integer
        Private _contractID As Integer
        Private _clientID As Integer
        Private _unPaid As Boolean
        Private _paid As Boolean
        Private _authorized As Boolean
        Private _suspended As Boolean
        Private _exclude As String
        Private _invoiceNumber As String
        Private _invoiceRef As String
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
        Private _showNewButton As Boolean
        Private _showHeaderControls As Boolean
        Private _headerLabelWidth As Unit
        Private _currentStep As Integer

        Sub New()
            _showHeaderControls = False
            _headerLabelWidth = New Unit(10, UnitType.Em)
        End Sub

        Public Property ShowNewButton() As Boolean
            Get
                Return _showNewButton
            End Get
            Set(ByVal value As Boolean)
                _showNewButton = value
            End Set
        End Property

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
                Return "DomProviderInvoiceStep_BeforeNavigate()"
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
                If (_queryString(QS_UNPAID) = Nothing) And (_queryString(QS_PAID) = Nothing) And _
                    (_queryString(QS_AUTHORIZED) = Nothing) And (_queryString(QS_SUSPENDED) = Nothing) And _
                    (_queryString(QS_EXCLUDE) = Nothing) Then
                    _unPaid = True
                    _paid = True
                    _authorized = True
                    _suspended = True
                    _exclude = "null"
                Else
                    _paid = IIf(_queryString(QS_PAID) = "true", True, False)
                    _unPaid = IIf(_queryString(QS_UNPAID) = "true", True, False)
                    _authorized = IIf(_queryString(QS_AUTHORIZED) = "true", True, False)
                    _suspended = IIf(_queryString(QS_SUSPENDED) = "true", True, False)
                    _exclude = IIf(_queryString(QS_EXCLUDE) = "" Or _queryString(QS_EXCLUDE) = "null", "null", _queryString(QS_EXCLUDE))
                End If
                _invoiceNumber = _queryString(QS_INVOICE_NUMBER)
                _invoiceRef = _queryString(QS_INVOICE_REF)
                If Utils.IsDate(_queryString(QS_WE_FROM)) Then _weekendingFrom = _queryString(QS_WE_FROM)
                If Utils.IsDate(_queryString(QS_WE_TO)) Then _weekendingTo = _queryString(QS_WE_TO)
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETO)) Then _dateTo = _queryString(QS_DATETO)
                _invoiceID = Utils.ToInt32(_queryString(QS_INVOICE_ID))
                _currentStep = Utils.ToInt32(_queryString(QS_CURRENTSTEP))
            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls
            Dim msg As ErrorMessage = New ErrorMessage
            Dim invoiceList As DomProviderInvoiceSelector = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/DomProviderInvoiceSelector.ascx")
            invoiceList.InitControl(wizard.Page, _invoiceID, _providerID, _contractID, _clientID, _
                                    _weekendingFrom, _weekendingTo, _invoiceNumber, _invoiceRef, _
                                    _dateFrom, _dateTo, _unPaid, _authorized, _paid, _suspended, _
                                    _showNewButton, _exclude)
            controls.Add(invoiceList)
            msg.Success = True
            Return msg
        End Function

        Public Function RenderHeaderControls(ByRef controls As ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls

            Dim msg As ErrorMessage = New ErrorMessage
            Dim qs As NameValueCollection

            If _showHeaderControls Then

                Dim lbl As Label = New Label
                Dim text As Label = New Label
                Dim link As HyperLink = New HyperLink
                Dim dpi As DomProviderInvoice

                ' label
                lbl.Text = "Invoice"
                lbl.Style.Add("float", "left")
                lbl.Style.Add("font-weight", "bold")
                lbl.Style.Add("width", _headerLabelWidth.ToString())
                controls.Add(lbl)

                text.Style.Add("float", "left")
                If _invoiceID = 0 Then
                    text.Text = "All"
                Else
                    dpi = New DomProviderInvoice(Me.DbConnection)
                    msg = dpi.Fetch(_invoiceID)
                    If Not msg.Success Then Return msg
                    text.Text = dpi.InvoiceNumber
                End If
                controls.Add(text)


                ' add the link
                qs = Me.QueryString
                If Me.IsCurrentStep AndAlso _invoiceID > 0 Then
                    ' all
                    link.Text = "All Invoices"
                    qs.Remove(QS_INVOICE_ID)
                    link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
                ElseIf _currentStep > Me.StepIndex Then
                    ' change
                    link.Text = "Change Invoice"
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

#Region " DomContractPeriodSysAccFilterStep "

    ''' <summary>
    ''' Implements a wizard step to select Suspension End Reason and period.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      25/09/2009  D11546 - changes to "selections" section links
    ''' </history>
    Friend Class DomContractPeriodSysAccFilterStep
        Inherits DateRangeStep

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_CONTRACTID As String = "contractID"
        Const QS_PERIODID As String = "periodID"
        Const QS_SYSACCID As String = "sysAccID"
        Const QS_ESTABLISHMENTID As String = "estabID"

        Private _description As String
        Private _queryString As NameValueCollection
        Private _currentStep As Integer
        Private _contractID As Integer
        Private _sysAccID As Integer
        Private _providerID As Integer
        Private _cboPeriod As DropDownListEx
        Private _cboSysAcc As DropDownListEx

        Sub New()
            MyBase.New()
            _description = "Please select a system account to filter the results on."
            'Me.DateFrom = Date.Today
            Me.ShowHeaderLink = False
            Me.HeaderLabelWidth = New Unit(10, UnitType.Em)
            Me.ContentLabelWidth = New Unit(10, UnitType.Em)
        End Sub

        Public Overrides Property QueryString() As System.Collections.Specialized.NameValueCollection
            Get
                Return MyBase.QueryString
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                MyBase.QueryString = Value
                _queryString = Value
                _contractID = Utils.ToInt32(_queryString(QS_CONTRACTID))
                _sysAccID = Utils.ToInt32(_queryString(QS_SYSACCID))
                _providerID = Utils.ToInt32(_queryString(QS_ESTABLISHMENTID))
                _currentStep = Utils.ToInt32(_queryString(QS_CURRENTSTEP))
            End Set
        End Property

        Public Overrides Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage

            Dim msg As ErrorMessage = New ErrorMessage
            Dim spacerBr As Literal
            Dim periods As DomContractPeriodCollection = Nothing

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            msg = MyBase.RenderContentControls(wizard, controls)
            If Not msg.Success Then Return msg

            ' get periods
            msg = DomContractPeriod.FetchList(Me.DbConnection, periods, String.Empty, String.Empty, _contractID)
            If Not msg.Success Then Return msg
            periods.Sort(New CollectionSorter("DateFrom", SortDirection.Descending))

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            ' system account (populated from client-side call to web svc)
            _cboSysAcc = New DropDownListEx()
            With _cboSysAcc
                .LabelText = "System Account"
                .LabelWidth = "10em"
                .LabelBold = True
                .DropDownList.Enabled = True
            End With
            controls.Add(_cboSysAcc)

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
            Dim contractGroup As GenericContractGroup = Nothing
            Dim endReason As ServiceOrderSuspensionReason = Nothing
            Dim qs As NameValueCollection
            Dim accounts As vwDomContractPeriodSystemAccountCollection = Nothing
            
            ' date range
            msg = MyBase.RenderHeaderControls(controls)
            If Not msg.Success Then Return msg

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            ' system account
            lbl = New Label()
            lbl.Text = "System Account"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", "10em")
            controls.Add(lbl)

            text = New Label()
            text.Style.Add("float", "left")
            If _sysAccID > 0 Then
                ' get sys acc
                msg = vwDomContractPeriodSystemAccount.FetchList(Me.DbConnection, accounts, 0, _sysAccID)
                If Not msg.Success Then Return msg
                text.Text = accounts(0).ClientName
            Else
                text.Text = "All"
            End If
            controls.Add(text)

            ' add the link
            link = New HyperLink()
            qs = Me.QueryString
            If Me.IsCurrentStep AndAlso _sysAccID > 0 Then
                ' all
                link.Text = "All System Accounts"
                qs.Remove(QS_SYSACCID)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change System Account"
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

        Public Overrides ReadOnly Property Title() As String
            Get
                Return "Select a System Account"
            End Get
        End Property

        Public Overrides Sub PreRender(ByVal wizard As SelectorWizardBase)
            Dim dateFrom As Date = Date.MinValue
            Dim dateTo As Date = Date.MaxValue

            If Utils.IsDate(MyBase.DateFromControl.Text) Then dateFrom = MyBase.DateFromControl.Text
            If Utils.IsDate(MyBase.DateToControl.Text) Then dateTo = MyBase.DateToControl.Text

            wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", _
             Target.Library.Web.Utils.WrapClientScript( _
              String.Format(";DomContractPeriodSysAccFilterStep_systemAccountID='{0}';DomContractPeriodSysAccFilterStep_providerID={1};DomContractPeriodSysAccFilterStep_selectedSystemAccount={2};DomContractPeriodSysAccFilterStep_dateFrom={3};DomContractPeriodSysAccFilterStep_dateTo={4};DomContractPeriodSysAccFilterStep_contractID={5};", _
               _cboSysAcc.ClientID, _providerID, _sysAccID, WebUtils.GetDateAsJavascriptString(dateFrom), WebUtils.GetDateAsJavascriptString(dateTo), _contractID) _
             ) _
            )

        End Sub

        Public Overrides Sub InitStep(ByVal wizard As SelectorWizardBase)
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/DomContractPeriodSysAccFilterStep.js"))
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/WebSvcUtils.js"))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DomContract))
        End Sub

        Public Overrides ReadOnly Property BeforeNavigateJS() As String
            Get
                Return "DomContractPeriodSysAccFilterStep_BeforeNavigate()"
            End Get
        End Property

    End Class

#End Region

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

#Region " DomContractReOpenWeeksFilterStep "

    ''' <summary>
    ''' Wizard step that allows the filtering of re-opened weeks on week-ending and expected closure dates.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      25/09/2009  D11546 - changes to "selections" section links
    ''' </history>
    Friend Class DomContractReOpenWeeksFilterStep
        Implements ISelectorWizardStep

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_WE_DATEFROM As String = "weDateFrom"
        Const QS_WE_DATETO As String = "weDateTo"
        Const QS_CLOSURE_DATEFROM As String = "closureDateFrom"
        Const QS_CLOSURE_DATETO As String = "closureDateTo"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _description As String = "Please enter the date ranges below to filter the list of re-opened weeks."
        Private _weDateFrom As Date, _weDateTo As Date, _closureDateFrom As Date, _closureDateTo As Date
        Private _weDateFromControl As TextBoxEx
        Private _weDateToControl As TextBoxEx
        Private _closureDateFromControl As TextBoxEx
        Private _closureDateToControl As TextBoxEx
        Private _currentStep As Integer

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
                Return "DomContractReOpenWeeksFilterStep_BeforeNavigate()"
            End Get
        End Property

        Public Property DbConnection() As SqlConnection Implements ISelectorWizardStep.DbConnection
            Get
                Return _dbConnection
            End Get
            Set(ByVal value As SqlConnection)
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
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/DomContractReOpenWeeksFilterStep.js"))
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

            wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.SP.Apps.UserControls.SelectorWizard.DateRangeStep.Startup", _
                WebUtils.WrapClientScript( _
                    String.Format("{0}_weDateFromID='{1}';{0}_weDateToID='{2}';{0}_closureDateFromID='{3}';{0}_closureDateToID='{4}';", _
                        "DomContractReOpenWeeksFilterStep", _weDateFromControl.ClientID, _weDateToControl.ClientID, _
                        _closureDateFromControl.ClientID, _closureDateToControl.ClientID) _
                ) _
            )

        End Sub

        Public Property QueryString() As NameValueCollection Implements ISelectorWizardStep.QueryString
            Get
                Return New NameValueCollection(_queryString)
            End Get
            Set(ByVal value As NameValueCollection)
                _queryString = value
                ' pull out the required params
                If Utils.IsDate(_queryString(QS_WE_DATEFROM)) Then _weDateFrom = _queryString(QS_WE_DATEFROM)
                If Utils.IsDate(_queryString(QS_WE_DATETO)) Then _weDateTo = _queryString(QS_WE_DATETO)
                If Utils.IsDate(_queryString(QS_CLOSURE_DATEFROM)) Then _closureDateFrom = _queryString(QS_CLOSURE_DATEFROM)
                If Utils.IsDate(_queryString(QS_CLOSURE_DATETO)) Then _closureDateTo = _queryString(QS_CLOSURE_DATETO)
                _currentStep = Utils.ToInt32(_queryString(QS_CURRENTSTEP))
            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

            Dim msg As ErrorMessage = New ErrorMessage()
            Dim fs As HtmlGenericControl
            Dim legend As HtmlGenericControl
            Dim lit As Literal

            lit = New Literal
            lit.Text = "<br />"
            controls.Add(lit)

            ' W/E date range
            fs = New HtmlGenericControl("FIELDSET")
            controls.Add(fs)
            fs.Style.Add("width", "40%")
            legend = New HtmlGenericControl("LEGEND")
            fs.Controls.Add(legend)
            legend.InnerText = "Re-Opened Week Ending Date"

            _weDateFromControl = New TextBoxEx()
            With _weDateFromControl
                .ID = "dteWEDateFrom"
                .Format = TextBoxEx.TextBoxExFormat.DateFormat
                .LabelBold = True
                .LabelText = "Between"
                .LabelWidth = "6em"
                If Target.Library.Utils.IsDate(_weDateFrom) Then .Text = _weDateFrom
                fs.Controls.Add(_weDateFromControl)
            End With

            _weDateToControl = New TextBoxEx()
            With _weDateToControl
                .ID = "dteWEDateTo"
                .Format = TextBoxEx.TextBoxExFormat.DateFormat
                .LabelBold = True
                .LabelText = "And"
                .LabelWidth = "6em"
                If Target.Library.Utils.IsDate(_weDateTo) Then .Text = _weDateTo
                fs.Controls.Add(_weDateToControl)
            End With

            lit = New Literal
            lit.Text = "<br /><br />"
            controls.Add(lit)

            ' expected closure date range
            fs = New HtmlGenericControl("FIELDSET")
            controls.Add(fs)
            fs.Style.Add("width", "40%")
            legend = New HtmlGenericControl("LEGEND")
            fs.Controls.Add(legend)
            legend.InnerText = "Expected Closure Date"

            _closureDateFromControl = New TextBoxEx()
            With _closureDateFromControl
                .ID = "dteClosureDateFrom"
                .Format = TextBoxEx.TextBoxExFormat.DateFormat
                .LabelBold = True
                .LabelText = "Between"
                .LabelWidth = "6em"
                If Target.Library.Utils.IsDate(_closureDateFrom) Then .Text = _closureDateFrom
                fs.Controls.Add(_closureDateFromControl)
            End With

            _closureDateToControl = New TextBoxEx()
            With _closureDateToControl
                .ID = "dteClosureDateTo"
                .Format = TextBoxEx.TextBoxExFormat.DateFormat
                .LabelBold = True
                .LabelText = "And"
                .LabelWidth = "6em"
                If Target.Library.Utils.IsDate(_closureDateTo) Then .Text = _closureDateTo
                fs.Controls.Add(_closureDateToControl)
            End With

            lit = New Literal
            lit.Text = "<br /><br />"
            controls.Add(lit)

            msg.Success = True
            Return msg

        End Function

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls

            Dim msg As ErrorMessage
            Dim lbl As Label
            Dim text As Label
            Dim link As HyperLink
            Dim lit As Literal
            Dim qs As NameValueCollection

            ' w/e dates
            lbl = New Label()
            lbl.Text = "Re-Opened Week Ending Date"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", "18em")
            controls.Add(lbl)

            text = New Label()
            text.Style.Add("float", "left")
            If Not Utils.IsDate(_weDateFrom) And Not Utils.IsDate(_weDateTo) Then
                text.Text = "All"
            Else
                If Utils.IsDate(_weDateFrom) Then
                    text.Text = String.Format("From {0}", _weDateFrom.ToString("dd/MM/yyyy"))
                End If
                If Utils.IsDate(_weDateTo) Then
                    If Utils.IsDate(_weDateFrom) Then text.Text &= " "
                    text.Text &= String.Format("To {0}", _weDateTo.ToString("dd/MM/yyyy"))
                End If
            End If
            controls.Add(text)


            ' add the link
            link = New HyperLink()
            qs = Me.QueryString
            If Me.IsCurrentStep AndAlso (Utils.IsDate(_weDateFrom) Or _
                                         Utils.IsDate(_weDateTo) Or _
                                         Utils.IsDate(_closureDateFrom) Or _
                                         Utils.IsDate(_closureDateTo)) Then
                ' all
                link.Text = "All Dates"
                qs.Remove(QS_WE_DATEFROM)
                qs.Remove(QS_WE_DATETO)
                qs.Remove(QS_CLOSURE_DATEFROM)
                qs.Remove(QS_CLOSURE_DATETO)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change Date Ranges"
                qs.Remove(QS_CURRENTSTEP)
                qs.Add(QS_CURRENTSTEP, Me.StepIndex)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            End If
            If Not String.IsNullOrEmpty(link.Text) Then
                link.Style.Add("float", "right")
                controls.Add(link)
            End If

            lit = New Literal
            lit.Text = "<br />"
            controls.Add(lit)

            ' expected closure dates
            lbl = New Label()
            lbl.Text = "Expected Closure Date"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", "18em")
            controls.Add(lbl)

            text = New Label()
            text.Style.Add("float", "left")
            If Not Utils.IsDate(_closureDateFrom) And Not Utils.IsDate(_closureDateTo) Then
                text.Text = "All"
            Else
                If Utils.IsDate(_closureDateFrom) Then
                    text.Text = String.Format("From {0}", _closureDateFrom.ToString("dd/MM/yyyy"))
                End If
                If Utils.IsDate(_closureDateTo) Then
                    If Utils.IsDate(_closureDateTo) Then text.Text &= " "
                    text.Text &= String.Format("To {0}", _closureDateTo.ToString("dd/MM/yyyy"))
                End If
            End If
            controls.Add(text)

            msg = New ErrorMessage
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
                Return "Enter Date Ranges"
            End Get
        End Property
    End Class

#End Region

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

#Region " SdsPersonalBudgetEnquiryResultStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.SelectorWizardSteps.SdsPersonalBudgetEnquiryResultStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Implements the select SDS personal budget step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MikeVO      25/09/2009  D11546 - changes to "selections" section links
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' 	[Mikevo]	07/07/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Class SdsPersonalBudgetEnquiryResultStep
        Implements ISelectorWizardStep

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_CLIENTID As String = "clientID"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETo As String = "dateTo"
        Const QS_BUDGETID As String = "budgetID"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _clientID As Integer
        Private _dateFrom As Date
        Private _dateTo As Date
        Private _selectedBudgetID As Integer
        Private _required As Boolean
        Private _description As String = "The list below displays the personal budgets that match your search criteria."
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
                _clientID = Utils.ToInt32(_queryString(QS_CLIENTID))
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETo)) Then _dateTo = _queryString(QS_DATETo)
                _selectedBudgetID = Utils.ToInt32(_queryString(QS_BUDGETID))
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

        Public Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep

        End Sub

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

        End Sub

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage = New ErrorMessage()
            msg.Success = True
            Return msg
        End Function

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

            Dim msg As ErrorMessage = New ErrorMessage
            Dim budgetList As SdsPersonalBudgetSelector = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/SdsPersonalBudgetSelector.ascx")
            budgetList.InitControl(wizard.Page, _clientID, _dateFrom, _dateTo, _selectedBudgetID, _showNewButton)
            controls.Add(budgetList)
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

#Region " PlannedCarePackageEnquiryResultStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.SelectorWizardSteps.PlannedCarePackageEnquiryResultStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Implements the select SDS planned care package step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MikeVO      25/09/2009  D11546 - changes to "selections" section links
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' 	[Mikevo]	10/07/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Class PlannedCarePackageEnquiryResultStep
        Implements ISelectorWizardStep

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_CLIENTID As String = "clientID"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETo As String = "dateTo"
        Const QS_PACKAGEID As String = "packageID"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _clientID As Integer
        Private _dateFrom As Date
        Private _dateTo As Date
        Private _selectedPackageID As Integer
        Private _required As Boolean
        Private _description As String = "The list below displays the planned care packages that match your search criteria."
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
                _clientID = Utils.ToInt32(_queryString(QS_CLIENTID))
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETo)) Then _dateTo = _queryString(QS_DATETo)
                _selectedPackageID = Utils.ToInt32(_queryString(QS_PACKAGEID))
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

        Public Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep

        End Sub

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

        End Sub

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
            Dim msg As ErrorMessage = New ErrorMessage()
            msg.Success = True
            Return msg
        End Function

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

            Dim msg As ErrorMessage = New ErrorMessage
            Dim packageList As PlannedCarePackageSelector = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/PlannedCarePackageSelector.ascx")
            packageList.InitControl(wizard.Page, _clientID, _dateFrom, _dateTo, _selectedPackageID, _showNewButton)
            controls.Add(packageList)
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

#Region " ServiceOrderSuspensionFilterStep "

    ''' <summary>
    ''' Implements a wizard step to select Suspension End Reason and period.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      25/09/2009  D11546 - changes to "selections" section links
    ''' </history>
    Friend Class ServiceOrderSuspensionFilterStep
        Inherits DateRangeStep

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_SUSPENSIONREASONID As String = "serID"

        Private _queryString As NameValueCollection
        Private _suspensionReasonID As Integer
        Private _cboSuspensionReason As DropDownListEx
        Private _currentStep As Integer

        Sub New()
            MyBase.New()
            Me.Description = "Please select the period and suspenision reason to filter the results on."
            'Me.DateFrom = Date.Today
            Me.ShowHeaderLink = False
            Me.HeaderLabelWidth = New Unit(12, UnitType.Em)
            Me.ContentLabelWidth = New Unit(12, UnitType.Em)
        End Sub

        Public Overrides Property QueryString() As System.Collections.Specialized.NameValueCollection
            Get
                Return MyBase.QueryString
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                MyBase.QueryString = Value
                _queryString = Value
                _suspensionReasonID = Utils.ToInt32(_queryString(QS_SUSPENSIONREASONID))
                _currentStep = Utils.ToInt32(_queryString(QS_CURRENTSTEP))
            End Set
        End Property

        Public Overrides Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage

            Dim msg As ErrorMessage = New ErrorMessage
            Dim spacerBr As Literal
            Dim groups As GenericContractGroupCollection = Nothing
            Dim reasons As ServiceOrderSuspensionReasonCollection = Nothing

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            ' contract End Reason
            _cboSuspensionReason = New DropDownListEx()
            With _cboSuspensionReason
                .LabelText = "Suspension Reason"
                .LabelWidth = Me.ContentLabelWidth.ToString()
                .LabelBold = True
                ' get a list of non-redundant groups
                msg = ServiceOrderSuspensionReason.FetchList(Me.DbConnection, reasons, String.Empty, String.Empty)
                If Not msg.Success Then Return msg
                With .DropDownList
                    .Items.Clear()
                    .DataSource = reasons
                    .DataTextField = "Description"
                    .DataValueField = "ID"
                    .DataBind()
                    ' insert a blank at the top
                    .Items.Insert(0, New ListItem(String.Empty, 0))
                    .SelectedValue = Utils.ToInt32(_suspensionReasonID)

                End With
            End With
            controls.Add(_cboSuspensionReason)

            msg = MyBase.RenderContentControls(wizard, controls)
            If Not msg.Success Then Return msg

            msg.Success = True
            Return msg

        End Function

        Public Overrides Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage

            Dim msg As ErrorMessage
            Dim spacerBr As Literal
            Dim lbl As Label
            Dim text As Label
            Dim link As HyperLink
            Dim contractGroup As GenericContractGroup = Nothing
            Dim endReason As ServiceOrderSuspensionReason = Nothing
            Dim qs As NameValueCollection

            ' contract end reason
            lbl = New Label()
            lbl.Text = "Suspension Reason"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", Me.HeaderLabelWidth.ToString())
            controls.Add(lbl)

            If _suspensionReasonID > 0 Then
                endReason = New ServiceOrderSuspensionReason(Me.DbConnection, String.Empty, String.Empty)
                msg = endReason.Fetch(_suspensionReasonID)
                If Not msg.Success Then Return msg
            End If

            text = New Label()
            text.Style.Add("float", "left")
            If endReason Is Nothing Then
                text.Text = "All"
            Else
                text.Text = endReason.Description
            End If
            controls.Add(text)

            ' add the link
            link = New HyperLink()
            qs = Me.QueryString
            If Me.IsCurrentStep AndAlso _suspensionReasonID > 0 Then
                ' all
                link.Text = "All Suspension Reasons"
                qs.Remove(QS_SUSPENSIONREASONID)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change Suspension Reason"
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

            ' date range
            msg = MyBase.RenderHeaderControls(controls)
            If Not msg.Success Then Return msg

            msg = New ErrorMessage
            msg.Success = True

            Return msg

        End Function

        Public Overrides ReadOnly Property Title() As String
            Get
                Return "Select a Period and Suspension Reason"
            End Get
        End Property

        Public Overrides Sub PreRender(ByVal wizard As SelectorWizardBase)

            wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Web.Apps.UserControls.SelectorWizard.DomContractFilterStep.Startup", _
             Target.Library.Web.Utils.WrapClientScript( _
              String.Format("ServiceOrderSuspensionFilterStep_dateFromID='{0}';ServiceOrderSuspensionFilterStep_dateToID='{1}';ServiceOrderSuspensionFilterStep_suspensionReasonID='{2}';", _
               MyBase.DateFromControl.ClientID, MyBase.DateToControl.ClientID, _cboSuspensionReason.ClientID) _
             ) _
            )

        End Sub

        Public Overrides Sub InitStep(ByVal wizard As SelectorWizardBase)
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/ServiceOrderSuspensionFilterStep.js"))
        End Sub

        Public Overrides ReadOnly Property BeforeNavigateJS() As String
            Get
                Return "ServiceOrderSuspensionFilterStep_BeforeNavigate()"
            End Get
        End Property

    End Class

#End Region

#Region " ServiceOrderSuspensionPeriodStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.SelectorWizardSteps.ServiceOrderSuspensionPeriodStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Implements the select Service Order Suspension Period Step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MikeVO  25/09/2009  D11546 - changes to "selections" section links
    ''' 	[Paul]	15/01/2009	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Class ServiceOrderSuspensionPeriodStep
        Implements ISelectorWizardStep

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_CLIENTID As String = "clientID"
        Const QS_SUSPENSIONREASONID As String = "serID"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETo As String = "dateTo"
        Const QS_SUSPENSIONID As String = "sospID"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _description As String = "The list below displays the service order suspensions that match your search criteria."
        Private _title As String = "Results"

        Private _dateFrom As Date
        Private _dateTo As Date
        Private _clientID As Integer
        Private _suspensionReasonID As Integer
        Private _showHeaderControls As Boolean
        Private _showNewButton As Boolean
        Private _showViewButton As Boolean
        Private _suspensionID As Integer
        Private _headerLabelWidth As Unit = New Unit(10, UnitType.Em)

        Sub New()
            _showHeaderControls = False
            _showNewButton = True
            _showViewButton = True
        End Sub

        Public Property HeaderLabelWidth() As Unit
            Get
                Return _headerLabelWidth
            End Get
            Set(ByVal value As Unit)
                _headerLabelWidth = value
            End Set
        End Property

        Public Property ShowHeaderControls() As Boolean
            Get
                Return _showHeaderControls
            End Get
            Set(ByVal value As Boolean)
                _showHeaderControls = value
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
                _suspensionReasonID = Utils.ToInt32(_queryString(QS_SUSPENSIONREASONID))
                _suspensionID = Utils.ToInt32(_queryString(QS_SUSPENSIONID))
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETo)) Then _dateTo = _queryString(QS_DATETo)
                _clientID = Utils.ToInt32(_queryString(QS_CLIENTID))
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
                Return "ServiceOrderSuspensionPeriodStep_BeforeNavigate()"
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
              String.Format("ServiceOrderSuspensionPeriodStep_required={0};", _required.ToString().ToLower()) _
            ))
        End Sub

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls

            Dim msg As ErrorMessage = New ErrorMessage

            msg.Success = True
            Return msg

        End Function

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls
            Dim msg As ErrorMessage = New ErrorMessage
            Dim suspensionList As ServiceOrderSuspensionSelector = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/ServiceOrderSuspensionSelector.ascx")
            suspensionList.InitControl(wizard.Page, _clientID, _dateFrom, _dateTo, _suspensionReasonID, _
                                      _showNewButton, _showViewButton, _suspensionID)
            controls.Add(suspensionList)
            msg.Success = True
            Return msg
        End Function

    End Class

#End Region

#Region " DomProviderInvoiceVisitsEnquiryResultsStep "

    Public Class DomProviderInvoiceVisitsEnquiryResultsStep
        Implements ISelectorWizardStep

        Const QS_FILEID As String = "fileID"
        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_ESTABLISHMENTID As String = "estabID"
        Const QS_CONTRACTID As String = "contractID"
        Const QS_CLIENTID As String = "clientID"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETO As String = "dateTo"


        Private _fileID As Integer
        Private _providerID As Integer
        Private _contractID As Integer
        Private _clientID As Integer
        Private _datefrom As Date
        Private _dateto As Date

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
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _datefrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETO)) Then _dateto = _queryString(QS_DATETO)

            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls
            Dim msg As ErrorMessage = New ErrorMessage
            Dim visitList As InvoicedVisitsSelector = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/InvoicedVisitsSelector.ascx")
            visitList.InitControl(wizard.Page, _providerID, _contractID, _clientID, _datefrom, _dateto)
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

#Region " DomContractHiddenReportsStep "

    ''' <summary>
    ''' Implements the hidden reports step for the domiciliary contracts wizard.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MikeVO  25/09/2009  D11546 - changes to "selections" section links
    ''' MikeVO  02/07/2009  A4WA#5562 - support for DomContractID.
    ''' </history>
    Friend Class DomContractHiddenReportsStep
        Implements ISelectorWizardStep, ISelectorWizardHiddenEndStep

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_ESTABLISHMENTID As String = "estabID"
        Const QS_CONTRACTTYPEID As String = "ctID"
        Const QS_CONTRACTGROUPID As String = "cgID"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETo As String = "dateTo"
        Const QS_CONTRACTID As String = "contractID"
        Const QS_CONTRACTENDREASONID As String = "cerID"
        Const QS_SERVICEGROUPID As String = "svcGroupID"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _description As String = "The available contract reports can be accessed below. The selections you have made above are applied to these reports."
        Private _title As String = "Reports"
        Private _estabID As Integer
        Private _contractType As DomContractType
        Private _contractGroupID As Integer
        Private _serviceGroupID As Integer
        Private _dateFrom As Date
        Private _dateTo As Date
        Private _contractEndReasonID As Integer
        Private _domContractID As Integer
        Private _showCapacityReport As Boolean
        Private _showSvcDelSummaryReport As Boolean

        Public Property ShowCapacityReport() As Boolean
            Get
                Return _showCapacityReport
            End Get
            Set(ByVal value As Boolean)
                _showCapacityReport = value
            End Set
        End Property

        Public Property ShowSvcDelSummaryReport() As Boolean
            Get
                Return _showSvcDelSummaryReport
            End Get
            Set(ByVal value As Boolean)
                _showSvcDelSummaryReport = value
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
                _serviceGroupID = Utils.ToInt32(_queryString(QS_SERVICEGROUPID))
                _contractEndReasonID = Utils.ToInt32(_queryString(QS_CONTRACTENDREASONID))
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETo)) Then _dateTo = _queryString(QS_DATETo)
                _domContractID = Utils.ToInt32(_queryString(QS_CONTRACTID))
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
            Dim contractReports As DomContractReports = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/DomContractReports.ascx")
            contractReports.InitControl(wizard.Page, _
                                        _estabID, _
                                        _contractType, _
                                        _contractGroupID, _
                                        _dateFrom, _
                                        _dateTo, _
                                        _contractEndReasonID, _
                                        _domContractID, _
                                        _serviceGroupID, _
                                        Me.ShowCapacityReport, _
                                        Me.ShowSvcDelSummaryReport)
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

#Region " RegisterHiddenReportsStep "

    ''' <summary>
    ''' Implements the hidden reports step for the register wizard.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' Mo Tahir  07/10/2009  D11681
    ''' </history>
    Friend Class RegisterHiddenReportsStep
        Implements ISelectorWizardStep, ISelectorWizardHiddenEndStep

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_ESTABLISHMENTID As String = "estabID"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETO As String = "dateTo"
        Const QS_REGISTERID As String = "registerid"
        Const QS_CONTRACTID As String = "contractid"
        Const QS_INPROGRESS As String = "status_ip"
        Const QS_SUBMITTED As String = "status_s"
        Const QS_AMENDED As String = "status_a"
        Const QS_PROCESSED As String = "status_p"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _description As String = "The available day care register reports can be accessed below. The selections you have made above are applied to these reports."
        Private _title As String = "Reports"
        Private _estabID As Integer
        Private _dateFrom As Date
        Private _dateTo As Date
        Private _inProgress As Boolean
        Private _submitted As Boolean
        Private _amended As Boolean
        Private _processed As Boolean
        Private _showHeaderControls As Boolean
        Private _showNewButton As Boolean
        Private _showViewButton As Boolean
        Private _registerID As Integer
        Private _contractID As Integer


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
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETO)) Then _dateTo = _queryString(QS_DATETO)
                _registerID = Utils.ToInt32(_queryString(QS_REGISTERID))
                _contractID = Utils.ToInt32(_queryString(QS_CONTRACTID))
                If (_queryString(QS_INPROGRESS) = Nothing) And (_queryString(QS_SUBMITTED) = Nothing) And _
                    (_queryString(QS_AMENDED) = Nothing) And (_queryString(QS_PROCESSED) = Nothing) Then
                    _inProgress = True
                    _submitted = True
                    _amended = True
                    _processed = True
                Else
                    _inProgress = IIf(_queryString(QS_INPROGRESS) = "true", True, False)
                    _submitted = IIf(_queryString(QS_SUBMITTED) = "true", True, False)
                    _amended = IIf(_queryString(QS_AMENDED) = "true", True, False)
                    _processed = IIf(_queryString(QS_PROCESSED) = "true", True, False)
                End If
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
            Dim regReports As RegisterReports = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/RegisterReports.ascx")
            regReports.InitControl(wizard.Page, _
                                        _estabID, _
                                        _contractID, _
                                        _registerID, _
                                        _dateFrom, _
                                        _dateTo, _
                                        _inProgress, _
                                        _submitted, _
                                        _amended, _
                                        _processed)
            controls.Add(regReports)
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
        Const QS_CLIENTID As String = "clientID"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETO As String = "dateTo"
        Const QS_SERVICEGROUPID As String = "svcGroupID"

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
        Private _clientID As Integer
        Private _serviceGroupID As Integer
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
                _clientID = Utils.ToInt32(_queryString(QS_CLIENTID))
                _ServiceGroupID = Utils.ToInt32(_queryString(QS_SERVICEGROUPID))
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETO)) Then _dateTo = _queryString(QS_DATETO)
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
            Dim orderReports As ServiceOrderReports = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/ServiceOrderReports.ascx")
            orderReports.InitControl(wizard.Page, _
                                    _estabID, _
                                    _contractID, _
                                    _clientID, _
                                    _dateFrom, _
                                    _dateTo, _
                                    _serviceGroupID, _
                                    _dsoID)
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

#Region " ServiceOrderSuspensionHiddenReportStep "

    ''' <summary>
    ''' Implements the hidden reports step for the service order suspensions wizard.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      25/09/2009  D11546 - changes to "selections" section links
    ''' </history>
    Friend Class ServiceOrderSuspensionHiddenReportStep
        Implements ISelectorWizardStep, ISelectorWizardHiddenEndStep

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_CLIENTID As String = "clientID"
        Const QS_SUSPENSIONREASONID As String = "serID"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETo As String = "dateTo"
        Const QS_SUSPENSIONID As String = "sospID"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _description As String = "The available service order suspension reports can be accessed below. The selections you have made above are applied to these reports."
        Private _title As String = "Reports"

        Private _dateFrom As Date
        Private _dateTo As Date
        Private _clientID As Integer
        Private _suspensionReasonID As Integer
        Private _suspensionID As Integer

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
                _suspensionReasonID = Utils.ToInt32(_queryString(QS_SUSPENSIONREASONID))
                _suspensionID = Utils.ToInt32(_queryString(QS_SUSPENSIONID))
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETo)) Then _dateTo = _queryString(QS_DATETo)
                _clientID = Utils.ToInt32(_queryString(QS_CLIENTID))
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
            Dim reports As ServiceOrderSuspensionReports = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/ServiceOrderSuspensionReports.ascx")
            reports.InitControl(wizard.Page, _
                                _clientID, _
                                _dateFrom, _
                                _dateTo, _
                                _suspensionReasonID _
            )
            controls.Add(reports)
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

#Region " DomProviderInvoiceHiddenReportStep "

    ''' <summary>
    ''' Implements the hidden reports step for the domiciliary provider invoices wizard.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO    25/09/2009  D11546 - changes to "selections" section links
    '''     MikeVO    23/09/2009  D11701 - added support for order-level reporting.
    ''' </history>
    Public Class DomProviderInvoiceHiddenReportStep
        Implements ISelectorWizardStep, ISelectorWizardHiddenEndStep

        Const QS_FILEID As String = "fileID"
        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_ESTABLISHMENTID As String = "estabID"
        Const QS_CONTRACTID As String = "contractID"
        Const QS_CLIENTID As String = "clientID"
        Const QS_INVOICE_NUMBER As String = "invoiceNumber"
        Const QS_INVOICE_REF As String = "invoiceRef"
        Const QS_WE_FROM As String = "weFrom"
        Const QS_WE_TO As String = "weTo"
        Const QS_PAID As String = "paid"
        Const QS_UNPAID As String = "unPaid"
        Const QS_AUTHORIZED As String = "authorized"
        Const QS_SUSPENDED As String = "suspended"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETO As String = "dateTo"
        Const QS_INVOICEID As String = "invoiceID"
        Const QS_EXCLUDE As String = "exclude"

        Private _fileID As Integer
        Private _providerID As Integer
        Private _contractID As Integer
        Private _clientID As Integer
        Private _unPaid As Boolean
        Private _paid As Boolean
        Private _authorized As Boolean
        Private _suspended As Boolean
        Private _exclude As String
        Private _invoiceNumber As String
        Private _invoiceRef As String
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
        Private _description As String = "The available provider invoice reports can be accessed below. The selections you have made above are applied to these reports."

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
                _fileID = Utils.ToInt32(_queryString(QS_FILEID))
                _providerID = Utils.ToInt32(_queryString(QS_ESTABLISHMENTID))
                _contractID = Utils.ToInt32(_queryString(QS_CONTRACTID))
                _clientID = Utils.ToInt32(_queryString(QS_CLIENTID))
                If (_queryString(QS_UNPAID) = Nothing) And (_queryString(QS_PAID) = Nothing) And _
                    (_queryString(QS_AUTHORIZED) = Nothing) And (_queryString(QS_SUSPENDED) = Nothing) And _
                    (_queryString(QS_EXCLUDE) = Nothing) Then
                    _unPaid = True
                    _paid = True
                    _authorized = True
                    _suspended = True
                    _exclude = "null"
                Else
                    _paid = IIf(_queryString(QS_PAID) = "true", True, False)
                    _unPaid = IIf(_queryString(QS_UNPAID) = "true", True, False)
                    _authorized = IIf(_queryString(QS_AUTHORIZED) = "true", True, False)
                    _suspended = IIf(_queryString(QS_SUSPENDED) = "true", True, False)
                    _exclude = IIf(_queryString(QS_EXCLUDE) = "" Or _queryString(QS_EXCLUDE) = "null", "null", _queryString(QS_EXCLUDE))
                End If
                _invoiceNumber = _queryString(QS_INVOICE_NUMBER)
                _invoiceRef = _queryString(QS_INVOICE_REF)
                If Utils.IsDate(_queryString(QS_WE_FROM)) Then _weekendingFrom = _queryString(QS_WE_FROM)
                If Utils.IsDate(_queryString(QS_WE_TO)) Then _weekendingTo = _queryString(QS_WE_TO)
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETO)) Then _dateTo = _queryString(QS_DATETO)
                _invoiceID = Utils.ToInt32(_queryString(QS_INVOICEID))
            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls
            Dim msg As ErrorMessage = New ErrorMessage
            Dim reports As DomProviderInvoiceReports = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/DomProviderInvoiceReports.ascx")
            reports.InitControl(wizard.Page, _
                                    _providerID, _
                                    _contractID, _
                                    _clientID, _
                                    _weekendingFrom, _
                                    _weekendingTo, _
                                    _invoiceNumber, _
                                    _invoiceRef, _
                                    _dateFrom, _
                                    _dateTo, _
                                    _unPaid, _
                                    _authorized, _
                                    _paid, _
                                    _suspended, _
                                    _invoiceID, _
                                    _exclude _
            )
            controls.Add(reports)
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
                Return "Reports"
            End Get
        End Property

        Public ReadOnly Property ButtonText() As String Implements ISelectorWizardHiddenEndStep.ButtonText
            Get
                Return "Reports"
            End Get
        End Property
    End Class

#End Region

#Region " DomProviderInvoiceVisitsHiddenReportsStep "

    ''' <summary>
    ''' Implements the hidden reports step for the domiciliary provider invoiced visits wizard.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      25/09/2009  D11546 - changes to "selections" section links
    ''' </history>
    Public Class DomProviderInvoiceVisitsHiddenReportsStep
        Implements ISelectorWizardStep, ISelectorWizardHiddenEndStep

        Const QS_FILEID As String = "fileID"
        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_ESTABLISHMENTID As String = "estabID"
        Const QS_CONTRACTID As String = "contractID"
        Const QS_CLIENTID As String = "clientID"
        Const QS_DATEFROM As String = "dateFrom"
        Const QS_DATETO As String = "dateTo"

        Private _fileID As Integer
        Private _providerID As Integer
        Private _contractID As Integer
        Private _clientID As Integer
        Private _datefrom As Date
        Private _dateto As Date

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _deleted As Boolean
        Private _description As String = "The available invoiced visits reports can be accessed below. The selections you have made above are applied to these reports."

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
                _fileID = Utils.ToInt32(_queryString(QS_FILEID))
                _providerID = Utils.ToInt32(_queryString(QS_ESTABLISHMENTID))
                _contractID = Utils.ToInt32(_queryString(QS_CONTRACTID))
                _clientID = Utils.ToInt32(_queryString(QS_CLIENTID))
                If Utils.IsDate(_queryString(QS_DATEFROM)) Then _datefrom = _queryString(QS_DATEFROM)
                If Utils.IsDate(_queryString(QS_DATETO)) Then _dateto = _queryString(QS_DATETO)
            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls
            Dim msg As ErrorMessage = New ErrorMessage()
            Dim reports As InvoicedVisitsReports = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/InvoicedVisitsReports.ascx")
            reports.InitControl(wizard.Page, _providerID, _contractID, _clientID, _datefrom, _dateto)
            controls.Add(reports)
            msg.Success = True
            Return msg
        End Function

        Public Function RenderHeaderControls(ByRef controls As ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls
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
                Return "Reports"
            End Get
        End Property

        Public ReadOnly Property ButtonText() As String Implements ISelectorWizardHiddenEndStep.ButtonText
            Get
                Return "Reports"
            End Get
        End Property
    End Class

#End Region

#Region " DebtorInvoiceHiddenReportStep "

    ''' <summary>
    ''' Implements the hidden reports step for the debtor invoices wizard.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MikeVO  25/09/2009  D11546 - changes to "selections" section links
    ''' MikeVO  22/07/2009  D11651 - parameters overhaul.
    ''' </history>
    Public Class DebtorInvoiceHiddenReportStep
        Implements ISelectorWizardStep, ISelectorWizardHiddenEndStep

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _deleted As Boolean
        Private _description As String = "The available debtor invoice reports can be accessed below. The selections you have made above are applied to these reports."
        Private _qsParser As WizardScreenParameters

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
                _qsParser = New WizardScreenParameters(_queryString)
            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls
            Dim msg As ErrorMessage = New ErrorMessage
            Dim reports As DebtorInvoiceReports = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/DebtorInvoiceReports.ascx")
            reports.InitControl(wizard.Page, _qsParser)
            controls.Add(reports)
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
                Return "Reports"
            End Get
        End Property

        Public ReadOnly Property ButtonText() As String Implements ISelectorWizardHiddenEndStep.ButtonText
            Get
                Return "Reports"
            End Get
        End Property
    End Class

#End Region

#Region " ManualPaymentDomProformaInvoiceHiddenReportsStep "

    ''' <summary>
    ''' Implements the hidden reports step for the domiciliary contracts wizard.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MikeVO  25/09/2009  D11546 - changes to "selections" section links
    ''' MikeVO  02/07/2009  A4WA#5562 - support for DomContractID.
    ''' </history>
    Friend Class ManualPaymentDomProformaInvoiceHiddenReportsStep
        Implements ISelectorWizardStep, ISelectorWizardHiddenEndStep

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_INVOICEID As String = "ID"
        Const QS_ESTABLISHMENTID As String = "estabID"
        Const QS_CONTRACTID As String = "contractID"
        Const QS_SYSACCID As String = "sysAccID"
        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _description As String = "The available contract reports can be accessed below. The selections you have made above are applied to these reports."
        Private _title As String = "Reports"
        Private _invoiceID As Integer
        Private _estabID As Integer
        Private _contractID As Integer
        Private _sysAccID As Integer
        Private _showCapacityReport As Boolean
        Private _showSvcDelSummaryReport As Boolean

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
            Dim manualPayReports As ManualPaymentDomProformaInvoiceReports = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/ManualPaymentDomProformaInvoiceReports.ascx")
            manualPayReports.InitControl(wizard.Page, _
                                        _estabID, _
                                        _contractID, _
                                        _invoiceID, _
                                        _sysAccID)
            controls.Add(manualPayReports)
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

#Region " ServiceOrderFilterStep "

    ''' <summary>
    ''' Implements a wizard step to select service Group and period.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     Paul W      29/06/2010  D11795 - SDS, Generic Contracts and Service Orders
    ''' </history>
    Friend Class ServiceOrderFilterStep
        Inherits DateRangeStep

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_SERVICEGROUPID As String = "svcGroupID"
        Const QS_ESTABLISHMENTID As String = "estabID"

        Private _queryString As NameValueCollection
        Private _estabID As Integer
        Private _serviceGroupID As Integer
        Private _cboServiceGroup As DropDownListEx
        Private _currentStep As Integer

        Sub New()
            MyBase.New()
            Me.Description = "Please select the service group and period to filter the results on."
            Me.DateFrom = Date.Today
            Me.ShowHeaderLink = True
            Me.HeaderLabelWidth = New Unit(10, UnitType.Em)
            Me.ContentLabelWidth = New Unit(10, UnitType.Em)
        End Sub

        Public Overrides Property QueryString() As System.Collections.Specialized.NameValueCollection
            Get
                Return New NameValueCollection(_queryString)
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                MyBase.QueryString = Value
                _queryString = Value
                _estabID = Utils.ToInt32(_queryString(QS_ESTABLISHMENTID))
                _serviceGroupID = Utils.ToInt32(_queryString(QS_SERVICEGROUPID))
                _currentStep = Utils.ToInt32(_queryString(QS_CURRENTSTEP))
            End Set
        End Property

        Public Overrides Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage

            Dim msg As ErrorMessage = New ErrorMessage
            Dim spacerBr As Literal
            Dim groups As GenericContractGroupCollection = Nothing
            Dim reasons As ContractEndReasonCollection = Nothing
            Dim user As WebSecurityUser
            Dim svcGroups As vwWebSecurityRole_ServiceGroupCollection = Nothing

            user = SecurityBL.GetCurrentUser()

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            'Service Group
            _cboServiceGroup = New DropDownListEx()
            With _cboServiceGroup
                .LabelText = "Service Group"
                .LabelWidth = Me.ContentLabelWidth.ToString()
                .LabelBold = True
                msg = DomContractBL.GetServiceGroupsAvailableToUser(Me.DbConnection, user.ID, svcGroups)
                If Not msg.Success Then Return msg
                With .DropDownList
                    .Items.Clear()
                    .DataSource = svcGroups
                    .DataTextField = "ServiceGroupDescription"
                    .DataValueField = "ServiceGroupID"
                    .DataBind()
                    ' insert a blank at the top
                    .Items.Insert(0, New ListItem(String.Empty, 0))
                    .SelectedValue = Utils.ToInt32(_serviceGroupID)
                End With
            End With
            controls.Add(_cboServiceGroup)

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            msg = MyBase.RenderContentControls(wizard, controls)
            If Not msg.Success Then Return msg

            msg.Success = True
            Return msg

        End Function

        Public Overrides Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage

            Dim msg As ErrorMessage
            Dim spacerBr As Literal
            Dim lbl As Label
            Dim text As Label
            Dim link As HyperLink
            Dim contractGroup As GenericContractGroup = Nothing
            Dim contractEndReason As ContractEndReason = Nothing
            Dim svcGroup As ServiceGroup = Nothing
            Dim qs As NameValueCollection

            ' if a provider has been selected show the date range link
            If _estabID <> 0 Then Me.ShowHeaderLink = True

            ' Service Group
            lbl = New Label()
            lbl.Text = "Service Group"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", Me.HeaderLabelWidth.ToString())
            controls.Add(lbl)

            text = New Label()
            text.Style.Add("float", "left")
            If _serviceGroupID = 0 Then
                text.Text = "All"
            Else
                svcGroup = New ServiceGroup(Me.DbConnection, String.Empty, String.Empty)
                msg = svcGroup.Fetch(_serviceGroupID)
                If Not msg.Success Then Return msg
                text.Text = svcGroup.Description
            End If
            controls.Add(text)

            ' add the link
            qs = Me.QueryString
            link = New HyperLink()
            If Me.IsCurrentStep AndAlso _serviceGroupID > 0 Then
                ' all
                link.Text = "All Service Groups"
                qs.Remove(QS_SERVICEGROUPID)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change Service Group"
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

            ' date range
            msg = MyBase.RenderHeaderControls(controls)
            If Not msg.Success Then Return msg

            msg = New ErrorMessage
            msg.Success = True

            Return msg

        End Function

        Public Overrides ReadOnly Property Title() As String
            Get
                Return "Select a Service Group and Period"
            End Get
        End Property

        Public Overrides Sub PreRender(ByVal wizard As SelectorWizardBase)

            wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Web.Apps.UserControls.SelectorWizard.ServiceOrderFilterStep.Startup", _
             Target.Library.Web.Utils.WrapClientScript( _
              String.Format("ServiceOrderFilterStep_dateFromID='{0}';ServiceOrderFilterStep_dateToID='{1}';ServiceOrderFilterStep_serviceGroupID='{2}';", _
               MyBase.DateFromControl.ClientID, MyBase.DateToControl.ClientID, _cboServiceGroup.ClientID) _
             ) _
            )

        End Sub

        Public Overrides Sub InitStep(ByVal wizard As SelectorWizardBase)
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/ServiceOrderFilterStep.js"))
        End Sub

        Public Overrides ReadOnly Property BeforeNavigateJS() As String
            Get
                Return "ServiceOrderFilterStep_BeforeNavigate()"
            End Get
        End Property

    End Class

#End Region

#Region " BudgetHolderStep "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.SelectorWizardSteps.BudgetHolderStep
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Implements the select Budget Holder wizard step.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	JohnF	15/07/2010	Created (D11801)
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Class BudgetHolderStep
        Implements ISelectorWizardStep

        Const QS_BUDGETHOLDERID As String = "bhID"
        Const QS_CURRENTSTEP As String = "currentStep"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _budgetHolderID As Integer
        Private _required As Boolean
        Private _description As String = "Please select a budget holder from the list below and then click ""Next""."
        Private _headerLabelWidth As Unit = New Unit(10, UnitType.Em)
        Private _currentStep As Integer

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
                Return "Select a Budget Holder"
            End Get
        End Property

        Public ReadOnly Property BeforeNavigateJS() As String Implements ISelectorWizardStep.BeforeNavigateJS
            Get
                Return "BudgetHolderStep_BeforeNavigate()"
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

            wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Web.Apps.UserControls.SelectorWizardSteps.BudgetHolderStep.Startup", _
             Target.Library.Web.Utils.WrapClientScript( _
              String.Format("BudgetHolderStep_required={0}", _required.ToString().ToLower()) _
             ) _
            )

        End Sub

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls

            Const SP_NAME As String = "spxThirdPartyBudgetHolder_FetchDetails"

            Dim msg As ErrorMessage
            Dim lbl As Label = New Label
            Dim text As Label = New Label
            Dim link As HyperLink = New HyperLink
            Dim reader As SqlDataReader = Nothing
            Dim spParams As SqlParameter()
            Dim qs As NameValueCollection

            '++ Label..
            lbl.Text = "Budget Holder"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", _headerLabelWidth.ToString())
            controls.Add(lbl)

            text.Style.Add("float", "left")
            If _budgetHolderID = 0 Then
                text.Text = "All"
            Else
                Try
                    '++ Get the budget holder details..
                    spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME, False)
                    spParams(0).Value = _budgetHolderID
                    reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams)
                    If (reader.HasRows) Then
                        reader.Read()
                        text.Text = String.Format("{0}: {1}", reader("Reference"), reader("BudgetHolderName"))
                        If reader("OrganisationName").ToString.Trim <> "" Then
                            text.Text = String.Concat(text.Text, String.Format(" ({0})", reader("OrganisationName").ToString.Trim))
                        End If
                    End If

                Catch ex As Exception
                    msg = Utils.CatchError(ex, "E0001")     ' unexpected
                    Return msg
                Finally
                    If Not reader Is Nothing Then reader.Close()
                End Try
            End If
            controls.Add(text)

            '++ Add the link..
            qs = Me.QueryString
            If Me.IsCurrentStep AndAlso _budgetHolderID > 0 Then
                '++ All..
                link.Text = "All Budget Holders"
                qs.Remove(QS_BUDGETHOLDERID)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                '++ Change..
                link.Text = "Change Budget Holder"
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
            Dim budgetHolderList As BudgetHolderSelector = wizard.LoadControl("~/AbacusWeb/Apps/UserControls/BudgetHolderSelector.ascx")
            budgetHolderList.InitControl(wizard.Page, _budgetHolderID)
            controls.Add(budgetHolderList)
            msg.Success = True
            Return msg

        End Function

    End Class

#End Region

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
    ''' 	JohnF	27/07/2010	Created (D11801)
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Class DPContractsResultsStep
        Implements ISelectorWizardStep

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
                If Not _queryString(QS_DATEFROM) Is Nothing Then
                    '++ If 'dateFrom' is present on the querystring but is empty, default to today..
                    If Convert.ToString(_queryString(QS_DATEFROM)).Length = 0 Then
                        _dateFrom = Date.Today
                    Else
                        If Target.Library.Utils.IsDate(_queryString(QS_DATEFROM)) Then _dateFrom = _queryString(QS_DATEFROM)
                    End If
                End If
                If Target.Library.Utils.IsDate(_queryString(QS_DATETO)) Then _dateTo = _queryString(QS_DATETO)
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

        Public Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep

        End Sub

        Public Sub New()
            _showNewButton = True
            _showViewButton = True
            _showTerminateButton = False
            _showReinstateButton = False
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
                    text.Text = String.Format("{0}: {1}", dpc.Number, dpc.Title)

                Catch ex As Exception
                    msg = Utils.CatchError(ex, "E0001")     ' unexpected
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
                                       _contractID)
            controls.Add(dpContractList)
            msg.Success = True
            Return msg

        End Function
    End Class

#End Region

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
                If Utils.IsDate(_queryString(QS_DATETo)) Then _dateTo = _queryString(QS_DATETo)
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
            contractReports.InitControl(wizard.Page, _
                                        _clientID, _
                                        _budgetHolderID, _
                                        _dateFrom, _
                                        _dateTo, _
                                        _contractID)
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