
Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Web.Apps.UserControls
Imports Target.Web.Apps

Namespace Apps.InPlaceSelectors

    ''' <summary>
    ''' Outputs an in-place, finance code selector.
    ''' </summary>
    ''' <remarks></remarks>
    Partial Public Class InPlaceFinanceCodeSelector
        Inherits System.Web.UI.UserControl

#Region " Private variables & properties "

        Private _requiredErrMsg As String = "Please enter/select a finance code"
        Private _enabled As Boolean = True
        Private _financeCodeOverriden As Boolean = False
        Private _FinanceCodeCategoryID As Integer = 0

        Public Property ExpenditureAccountGroupID() As Integer
            Get
                If IsNumeric(hidExpenditureAccountID.Value) Then
                    Return hidExpenditureAccountID.Value
                Else
                    Return 0
                End If
            End Get
            Set(ByVal value As Integer)
                hidExpenditureAccountID.Value = value
            End Set
        End Property



        Public Property FinanceCodeOverridden() As Boolean
            Get
                Return _financeCodeOverriden
            End Get
            Set(ByVal value As Boolean)
                _financeCodeOverriden = value
            End Set
        End Property

        Public Property FinanceCodeText() As String
            Get
                Return txtName.Value
            End Get
            Set(ByVal value As String)
                txtName.Value = value
            End Set
        End Property

        Public Property FinanceCodeCategoryID() As Integer
            Get
                Return _FinanceCodeCategoryID
            End Get
            Set(ByVal value As Integer)
                _FinanceCodeCategoryID = value
            End Set
        End Property

        Public Property FinanceCodeID() As Integer
            Get
                Return IIf(hidID.Value = "", 0, hidID.Value)
            End Get
            Set(ByVal value As Integer)
                hidID.Value = value
                LoadFinanceCode()
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

        Public Property RequiredValidatorErrMsg() As String
            Get
                Return _requiredErrMsg
            End Get
            Set(ByVal value As String)
                _requiredErrMsg = value
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
                Return hidID.UniqueID
            End Get
        End Property

        Public Property Enabled() As Boolean
            Get
                Return _enabled
            End Get
            Set(ByVal value As Boolean)
                _enabled = value
                WebUtils.RecursiveDisable(Me.Controls, Not _enabled)
            End Set
        End Property

#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Const SCRIPT_LIBRARY As String = "Library"

            txtName.Attributes.Add("onchange", String.Format("InPlaceFinanceCodeSelector_ClearStoredID('{0}');", Me.ClientID))
            btnFind.Attributes.Add("onclick", String.Format("InPlaceFinanceCodeSelector_btnFind_Click('{0}');", Me.ClientID))

            If Not Page.ClientScript.IsClientScriptIncludeRegistered(Me.GetType(), SCRIPT_LIBRARY) Then
                Page.ClientScript.RegisterClientScriptInclude(Me.GetType(), SCRIPT_LIBRARY, WebUtils.GetVirtualPath("AbacusWeb/Apps/InPlaceSelectors/InPlaceFinanceCodeSelector.js"))
            End If

        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If Me.Required Then
                With valRequired
                    .ControlToValidate = txtName.ID
                    .Display = ValidatorDisplay.Dynamic
                    .ErrorMessage = _requiredErrMsg
                End With
            End If
            txtName.Disabled = Not Me.Enabled
            btnFind.Disabled = Not Me.Enabled
            If _financeCodeOverriden Then
                txtName.Style.Add("background-color", "#FFFFCC")
                txtName.Attributes.Add("title", "The Finance codes have been overridden")
            End If
            hidCategoryID.Value = FinanceCodeCategoryID
        End Sub

        Private Sub LoadFinanceCode()
            Dim msg As ErrorMessage
            Dim fc As FinanceCode

            Dim thePage As BasePage = CType(Me.Page, BasePage)

            If Me.FinanceCodeID > 0 Then
                fc = New FinanceCode(thePage.DbConnection)
                msg = fc.Fetch(Me.FinanceCodeID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtName.Value = fc.Code
                hidID.Value = Me.FinanceCodeID
            Else
                txtName.Value = String.Empty
                hidID.Value = String.Empty
            End If

        End Sub

    End Class

End Namespace