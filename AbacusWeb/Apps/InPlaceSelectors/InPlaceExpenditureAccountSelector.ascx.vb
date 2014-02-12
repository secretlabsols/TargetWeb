Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Web.Apps.UserControls
Imports Target.Web.Apps

Namespace Apps.InPlaceSelectors

    ''' <summary>
    ''' Outputs an in-place, Expenditure Accounts selector.
    ''' </summary>
    ''' <remarks></remarks>
    Partial Public Class InPlaceExpenditureAccountSelector
        Inherits System.Web.UI.UserControl

#Region " Private variables & properties "

        Private _expenditureAccountGroupID As Integer
        Private _serviceType As Integer = 0
        Private _accountType As Integer = 0

        Public Property ServiceType() As Integer
            Get
                Return _serviceType
            End Get
            Set(ByVal value As Integer)
                _serviceType = value
            End Set
        End Property

        Public Property AccountType() As Integer
            Get
                Return _accountType
            End Get
            Set(ByVal value As Integer)
                _accountType = value
            End Set
        End Property

        Public Property ExpenditureAccountGroupID() As Integer
            Get
                Return IIf(hidID.Value = "", 0, hidID.Value)
            End Get
            Set(ByVal value As Integer)
                hidID.Value = value
                LoadExpenditureAccount()
            End Set
        End Property

        Public Property Required() As Boolean
            Get
                Return valRequired.Visible
            End Get
            Set(ByVal value As Boolean)
                valRequired.Visible = value
            End Set
        End Property

        Public Property ValidationGroup() As String
            Get
                Return valRequired.ValidationGroup
            End Get
            Set(ByVal value As String)
                valRequired.ValidationGroup = value
            End Set
        End Property

        Public ReadOnly Property HiddenFieldUniqueID() As String
            Get
                Return hidID.Value
            End Get
        End Property

#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            txtName.Attributes.Add("onchange", String.Format("InPlaceExpenditureAccountSelector_ClearStoredID('{0}');", Me.ClientID))
            btnFind.Attributes.Add("onclick", String.Format("InPlaceExpenditureAccountSelector_btnFind_Click('{0}');", Me.ClientID))

            System.Web.UI.ScriptManager.RegisterClientScriptInclude(Me, Me.[GetType](), "InplaceExpenditureSelector", WebUtils.GetVirtualPath("AbacusWeb/Apps/InPlaceSelectors/InPlaceExpenditureAccountSelector.js"))

            System.Web.UI.ScriptManager.RegisterStartupScript(Me, Me.[GetType](), "anyname", String.Format("InPlaceExpenditureAccountSelector_serviceType={0};InPlaceExpenditureAccountSelector_accountType={1};", _serviceType, _accountType), True)

        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If Me.Required Then
                With valRequired
                    .ControlToValidate = hidID.ID
                    .Display = ValidatorDisplay.Dynamic
                    .ErrorMessage = "Please select an expenditure account"
                End With
            End If
        End Sub

        Private Sub LoadExpenditureAccount()
            Dim msg As ErrorMessage
            Dim thePage As BasePage = CType(Me.Page, BasePage)
            Dim expAccGroup As ExpenditureAccountGroup = New ExpenditureAccountGroup(thePage.DbConnection)
            Dim finCode As FinanceCode = New FinanceCode(thePage.DbConnection)

            If Me.ExpenditureAccountGroupID > 0 Then
                msg = expAccGroup.Fetch(Me.ExpenditureAccountGroupID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                txtName.Value = expAccGroup.Description
                hidID.Value = Me.ExpenditureAccountGroupID

            Else
                txtName.Value = String.Empty
                hidID.Value = String.Empty
            End If
        End Sub

    End Class

End Namespace