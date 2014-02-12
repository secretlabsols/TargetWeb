Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.InPlaceSelectors
    Partial Public Class InPlaceDcrDomContractSelector
        Inherits System.Web.UI.UserControl

        Private _requiredErrMsg As String = "Select contract"
        Private _enabled As Boolean = True


        Public Property SelectedContractID() As String
            Get
                Return txtHidselectedDcrDomContractId.Value
            End Get
            Set(ByVal value As String)
                txtHidselectedDcrDomContractId.Value = value
            End Set
        End Property

        Public Property SelectedContractTitle() As String
            Get
                Return txtContractTitle.Value
            End Get
            Set(ByVal value As String)
                txtContractTitle.Value = value
            End Set
        End Property

        Public Property SelectedContractNumber() As String
            Get
                Return txtContractNumber.Value
            End Get
            Set(ByVal value As String)
                txtContractNumber.Value = value
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

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Const SCRIPT_LIBRARY As String = "Library"

            txtContractNumber.Attributes.Add("onchange", String.Format("InPlaceDcrDomContractSelector_ClearStoredID(""{0}"");", Me.ClientID))
            txtContractTitle.Attributes.Add("onchange", String.Format("InPlaceDcrDomContractSelector_ClearStoredID(""{0}"");", Me.ClientID))
            btnFind.Attributes.Add("onclick", String.Format("InPlaceDcrDomContractSelector_btnFind_Click(""{0}"");", Me.ClientID))

            If Not Page.ClientScript.IsClientScriptIncludeRegistered(Me.GetType(), SCRIPT_LIBRARY) Then
                Page.ClientScript.RegisterClientScriptInclude(Me.GetType(), SCRIPT_LIBRARY, WebUtils.GetVirtualPath("AbacusExtranet/Apps/InPlaceSelectors/InPlaceDcrDomContractSelector.js"))
            End If

        End Sub


        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If Me.Required Then
                With valRequired
                    .ControlToValidate = txtContractNumber.ID
                    .ControlToValidate = txtContractTitle.ID
                    .Display = ValidatorDisplay.Dynamic
                    .ErrorMessage = _requiredErrMsg
                End With
            End If
            txtContractNumber.Disabled = Not Me.Enabled
            txtContractTitle.Disabled = Not Me.Enabled
            btnFind.Disabled = Not Me.Enabled
        End Sub

    End Class

End Namespace