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
    Partial Public Class InPlaceServiceGroupSelector
        Inherits System.Web.UI.UserControl

#Region " Private variables & properties "

        Private _sgID As Integer
        Private _requiredErrMsg As String = "Please enter/select a service group"
        Private _enabled As Boolean = True
        Public Property ServiceGroupText() As String
            Get
                Return txtName.Value
            End Get
            Set(ByVal value As String)
                txtName.Value = value
            End Set
        End Property

        Public Property ServiceGroupID() As Integer
            Get
                Return _sgID
            End Get
            Set(ByVal value As Integer)
                _sgID = value
                LoadServiceGroup()
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

            txtName.Attributes.Add("onchange", String.Format("InPlaceServiceGroupSelector_ClearStoredID('{0}');", Me.ClientID))
            btnFind.Attributes.Add("onclick", String.Format("InPlaceServiceGroupSelector_btnFind_Click('{0}');", Me.ClientID))

            If Not Page.ClientScript.IsClientScriptIncludeRegistered(Me.GetType(), SCRIPT_LIBRARY) Then
                Page.ClientScript.RegisterClientScriptInclude(Me.GetType(), SCRIPT_LIBRARY, WebUtils.GetVirtualPath("AbacusWeb/Apps/InPlaceSelectors/InPlaceServiceGroupSelector.js"))
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
        End Sub

        Private Sub LoadServiceGroup()
            Dim msg As ErrorMessage
            Dim sg As ServiceGroup
            Dim thePage As BasePage = CType(Me.Page, BasePage)

            If Me.ServiceGroupID > 0 Then
                sg = New ServiceGroup(thePage.DbConnection, String.Empty, String.Empty)
                msg = sg.Fetch(Me.ServiceGroupID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtName.Value = sg.Description
                hidID.Value = Me.ServiceGroupID
            Else
                txtName.Value = String.Empty
                hidID.Value = String.Empty
            End If
        End Sub

    End Class
End Namespace

