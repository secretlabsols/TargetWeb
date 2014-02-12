Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Web.Apps.UserControls
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.InPlaceSelectors

    ''' <summary>
    ''' Outputs an in-place, service group selector.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MoTahir       15/01/2013 D12092G - Old-style Debtor Invoices In Place Selector
    ''' </history>
    Partial Public Class InPlaceDebtorInvoiceSelector
        Inherits System.Web.UI.UserControl

#Region " Private variables & properties "

        Private _diID As Integer
        Private _requiredErrMsg As String = "Please enter/select a debtor invoice"
        Private _enabled As Boolean = True
        Public Property DebtorInvoiceText() As String
            Get
                Return txtName.Value
            End Get
            Set(ByVal value As String)
                txtName.Value = value
            End Set
        End Property

        Public Property DebtorInvoiceID() As Integer
            Get
                Return _diID
            End Get
            Set(ByVal value As Integer)
                _diID = value
                LoadDebtorInvoice()
            End Set
        End Property

        Public Property Required() As Boolean
            Get
                Return valRequired.Visible
            End Get
            Set(ByVal value As Boolean)
                valRequired.Visible = value
                If value Then SetupValidator()
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
                If Not value Is Nothing AndAlso value.Trim().Length > 0 Then SetupValidator()
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

            txtName.Attributes.Add("onchange", String.Format("InPlaceDebtorInvoiceSelector_ClearStoredID('{0}');", Me.ClientID))
            btnFind.Attributes.Add("onclick", String.Format("InPlaceDebtorInvoiceSelector_btnFind_Click('{0}');", Me.ClientID))

            If Not Page.ClientScript.IsClientScriptIncludeRegistered(Me.GetType(), SCRIPT_LIBRARY) Then
                Page.ClientScript.RegisterClientScriptInclude(Me.GetType(), SCRIPT_LIBRARY, WebUtils.GetVirtualPath("AbacusWeb/Apps/InPlaceSelectors/InPlaceDebtorInvoiceSelector.js"))
            End If

        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            txtName.Disabled = Not Me.Enabled
            btnFind.Disabled = Not Me.Enabled

            If Me.Required Then
                SetupValidator()
            End If
        End Sub

        Private Sub SetupValidator()
            With valRequired
                .Display = ValidatorDisplay.Dynamic
                .ErrorMessage = _requiredErrMsg
            End With
        End Sub

        Private Sub LoadDebtorInvoice()
            Dim msg As ErrorMessage
            Dim inv As Invoice
            Dim tAcc As TransactionAccount
            Dim tPar As Target.Abacus.Library.DataClasses.ThirdParty
            Dim cli As ClientDetail

            Dim thePage As BasePage = CType(Me.Page, BasePage)

            If Me.DebtorInvoiceID > 0 Then
                inv = New Invoice(thePage.DbConnection)
                msg = inv.Fetch(Me.DebtorInvoiceID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtRef.Value = inv.InvoiceNumber

                tAcc = New TransactionAccount(thePage.DbConnection, String.Empty, String.Empty)
                msg = tAcc.Fetch(inv.TransactionAccountID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                tPar = New Target.Abacus.Library.DataClasses.ThirdParty(thePage.DbConnection)
                If tAcc.ThirdPartyID > 0 Then
                    msg = tPar.Fetch(tAcc.ThirdPartyID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If

                cli = New ClientDetail(thePage.DbConnection, String.Empty, String.Empty)
                If tAcc.ClientID > 0 Then
                    msg = cli.Fetch(tAcc.ClientID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If

                If tPar.ID > 0 Then
                    txtName.Value = tPar.Title + " " + tPar.Surname
                    hidID.Value = Me.DebtorInvoiceID
                ElseIf cli.ID > 0 Then
                    txtName.Value = cli.Name
                    hidID.Value = Me.DebtorInvoiceID
                Else
                    txtName.Value = String.Empty
                    hidID.Value = String.Empty
                End If
            End If
        End Sub

    End Class
End Namespace

