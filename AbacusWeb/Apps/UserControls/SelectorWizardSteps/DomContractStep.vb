
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
    '''     MikeVO      23/05/2011  SDS issue #707 - defaulting date from.
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
        Private _FrameworkTypeID As Nullable(Of FrameworkTypes) = Nothing
        Sub New()
            _showHeaderControls = False
            _showNewButton = True
            _showViewButton = True
            _showCopyButton = True
            _showTerminateButton = False
            _showReinstateButton = False
            _showServiceUserColumn = False
        End Sub

        Public Property FrameworkTypeID() As Nullable(Of FrameworkTypes)
            Get
                Return _FrameworkTypeID
            End Get
            Set(ByVal value As Nullable(Of FrameworkTypes))
                _FrameworkTypeID = value
            End Set
        End Property

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

                Using drs As New DateRangeStep()
                    With drs
                        .QueryString = Me.QueryString
                        _dateFrom = .DateFrom
                        _dateTo = .DateTo
                    End With
                End Using

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
                                     _showReinstateButton, _showTerminateButton, _showServiceUserColumn, _contractID, _serviceGroupID, _serviceGroupClassificationID, FrameworkTypeID)
            controls.Add(contractList)
            msg.Success = True
            Return msg
        End Function

    End Class

#End Region

End Namespace