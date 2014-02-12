Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.InPlaceSelectors

    ''' <summary>
    ''' Outputs an in-place, External Account selector.
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    Partial Public Class InPlaceExternalAccountSelector
        Inherits System.Web.UI.UserControl

        Private _requiredErrMsg As String = "Select an External Account"
        Private _enabled As Boolean = True

#Region " Private variables & properties "

        Public Property ExternalAccountText() As String
            Get
                Return txtExternalAccount.Value
            End Get
            Set(ByVal value As String)
                txtExternalAccount.Value = value
            End Set
        End Property

        Public Property DisableExternalAccountTextBox() As Boolean
            Get
                Return txtExternalAccount.Disabled
            End Get
            Set(ByVal value As Boolean)
                txtExternalAccount.Disabled = value
            End Set
        End Property

        Public Property hidExternalAccountID() As String
            Get
                Return txtHidExternalAccountID.Value
            End Get
            Set(ByVal value As String)
                txtHidExternalAccountID.Value = value
                loadExternalAccount()
            End Set
        End Property

        Public WriteOnly Property ExternalAccountSelectorVisible() As Boolean
            Set(ByVal value As Boolean)
                btnFind.Visible = value
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

        Public Property Enabled() As Boolean
            Get
                Return _enabled
            End Get
            Set(ByVal value As Boolean)
                _enabled = value
                WebUtils.RecursiveDisable(Me.Controls, Not _enabled)
            End Set
        End Property

        Public ReadOnly Property textbox() As HtmlControls.HtmlInputText
            Get
                Return Me.txtExternalAccount
            End Get
        End Property

#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Const SCRIPT_LIBRARY As String = "Library"

            txtExternalAccount.Attributes.Add("onchange", String.Format("InPlaceExternalAccountSelector_ClearStoredID(""{0}"");", Me.ClientID))
            btnFind.Attributes.Add("onclick", String.Format("InPlaceExternalAccountSelector_btnFind_Click(""{0}"");", Me.ClientID))

            If Not Page.ClientScript.IsClientScriptIncludeRegistered(Me.GetType(), SCRIPT_LIBRARY) Then
                Page.ClientScript.RegisterClientScriptInclude(Me.GetType(), SCRIPT_LIBRARY, WebUtils.GetVirtualPath("AbacusExtranet/Apps/InPlaceSelectors/InPlaceExternalAccountSelector.js"))
            End If
            
        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If Me.Required Then
                With valRequired
                    .ControlToValidate = txtHidExternalAccountID.ID
                    .Display = ValidatorDisplay.Dynamic
                    .ErrorMessage = _requiredErrMsg
                    .Enabled = True
                End With
            Else
                With valRequired
                    .Enabled = False
                End With
            End If
            txtExternalAccount.Disabled = Not Me.Enabled
            btnFind.Disabled = Not Me.Enabled
        End Sub

        Private Sub loadExternalAccount()
            Dim msg As ErrorMessage = New ErrorMessage
            Dim thePage As BasePage = CType(Me.Page, BasePage)
            If hidExternalAccountID.Length > 0 Then
                Dim externalAccount As Target.Abacus.Library.DataClasses.Users = _
                New Target.Abacus.Library.DataClasses.Users(thePage.DbConnection)
                msg = externalAccount.Fetch(Me.hidExternalAccountID)
                If Not msg.Success Then Return
                txtHidExternalAccountID.Value = externalAccount.ID
                txtExternalAccount.Value = externalAccount.Name
            Else
                txtHidExternalAccountID.Value = String.Empty
                txtExternalAccount.Value = String.Empty
            End If
        End Sub


    End Class

End Namespace