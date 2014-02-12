
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
    '''     ColinD  15/04/2011  Updated SDS585 - Updated to ensure that redundant/non-redundant items are displayed.
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
                    msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected
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
            budgetHolderList.InitControl(wizard.Page, _budgetHolderID, Nothing, Nothing)
            controls.Add(budgetHolderList)
            msg.Success = True
            Return msg

        End Function

    End Class

#End Region

End Namespace