
Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Web.Apps.UserControls
Imports Target.Web.Apps

Namespace Apps.InPlaceSelectors

	''' <summary>
	''' Outputs an in-place, client group selector.
	''' </summary>
	''' <remarks></remarks>
	Partial Public Class InPlaceClientSubGroupSelector
		Inherits System.Web.UI.UserControl

#Region " Private variables & properties "

        Private _clientSubGroupID As Integer
        Private _isLocked As Boolean

        Public Property ClientSubGroupID() As Integer
            Get
                Return _clientSubGroupID
            End Get
            Set(ByVal value As Integer)
                _clientSubGroupID = value
                LoadClientSubGroup()
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
                Return hidID.UniqueID
            End Get
        End Property

        Public Property IsLocked() As Boolean
            Get
                Return _isLocked
            End Get
            Set(value As Boolean)
                If value Then
                    txtReference.Attributes.Add("readonly", "readonly")
                    txtReference.Style.Add("color", "#808080")
                    txtName.Attributes.Add("readonly", "readonly")
                    txtName.Style.Add("color", "#808080")
                    btnFind.Attributes.Add("disabled", "disabled")
                Else
                    txtReference.Attributes.Remove("readonly")
                    txtReference.Style.Remove("color")
                    txtName.Attributes.Remove("readonly")
                    txtName.Style.Remove("color")
                    btnFind.Attributes.Remove("disabled")
                End If

                _isLocked = value
            End Set
        End Property
#End Region

		Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

			Const SCRIPT_LIBRARY As String = "Library"

			txtReference.Attributes.Add("onchange", String.Format("InPlaceClientSubGroupSelector_ClearStoredID('{0}');", Me.ClientID))
			txtName.Attributes.Add("onchange", String.Format("InPlaceClientSubGroupSelector_ClearStoredID('{0}');", Me.ClientID))
			btnFind.Attributes.Add("onclick", String.Format("InPlaceClientSubGroupSelector_btnFind_Click('{0}');", Me.ClientID))

			If Not Page.ClientScript.IsClientScriptIncludeRegistered(Me.GetType(), SCRIPT_LIBRARY) Then
				Page.ClientScript.RegisterClientScriptInclude(Me.GetType(), SCRIPT_LIBRARY, WebUtils.GetVirtualPath("AbacusWeb/Apps/InPlaceSelectors/InPlaceClientSubGroupSelector.js"))
			End If

		End Sub

		Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
			If Me.Required Then
				With valRequired
                    .Display = ValidatorDisplay.Dynamic
                    .ErrorMessage = "Please select a client sub-group"
                End With
			End If
		End Sub

		Private Sub LoadClientSubGroup()
			Dim msg As ErrorMessage
            Dim csg As ClientSubGroup
            Dim thePage As BasePage = CType(Me.Page, BasePage)

            If Me.ClientSubGroupID > 0 Then
                csg = New ClientSubGroup(thePage.DbConnection, String.Empty, String.Empty)
                msg = csg.Fetch(Me.ClientSubGroupID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtReference.Value = csg.Reference
                txtName.Value = csg.Name
                hidID.Value = Me.ClientSubGroupID
            Else
                txtReference.Value = String.Empty
                txtName.Value = String.Empty
                hidID.Value = String.Empty
            End If
		End Sub

	End Class

End Namespace