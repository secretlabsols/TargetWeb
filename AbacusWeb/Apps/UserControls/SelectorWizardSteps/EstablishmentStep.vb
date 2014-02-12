
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
    '''     ColinD      19/07/2011  D12140 - Service Register step now listed as Provider
    '''     MikeVO      13/04/2011  SDS issue #415 - corrected default DateFrom behaviour (reversing SDS issue #322).
    '''     ColinD      23/11/2010  SDS322 - Added DefaultDateFrom property
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
                'Title = String.Empty
                'Select Case _mode
                '    Case EstablishmentSelectorMode.Establishments
                '        Return "Select a Provider"
                '    Case EstablishmentSelectorMode.DomProviders
                '        Return "Select a Domiciliary Provider"
                '    Case EstablishmentSelectorMode.DayCareProviders
                '        Return "Select a Provider"
                '    Case Else
                '        ThrowInvalidMode()
                'End Select
                Return "Select a Care Provider"
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
                    Case EstablishmentSelectorMode.Establishments
                        link.Text = "All Providers"
                    Case EstablishmentSelectorMode.DomProviders
                        link.Text = "All Providers"
                    Case EstablishmentSelectorMode.DayCareProviders
                        link.Text = "All Providers"
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

End Namespace