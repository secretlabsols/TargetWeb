
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
	Partial Public Class InPlaceClientGroupSelector
		Inherits System.Web.UI.UserControl

#Region " Private variables & properties "

		Private _clientGroupID As Integer

		Public Property ClientGroupID() As Integer
			Get
				Return _clientGroupID
			End Get
			Set(ByVal value As Integer)
				_clientGroupID = value
				LoadClientGroup()
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

#End Region

		Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

			Const SCRIPT_LIBRARY As String = "Library"

			txtReference.Attributes.Add("onchange", String.Format("InPlaceClientGroupSelector_ClearStoredID('{0}');", Me.ClientID))
			txtName.Attributes.Add("onchange", String.Format("InPlaceClientGroupSelector_ClearStoredID('{0}');", Me.ClientID))
			btnFind.Attributes.Add("onclick", String.Format("InPlaceClientGroupSelector_btnFind_Click('{0}');", Me.ClientID))

			If Not Page.ClientScript.IsClientScriptIncludeRegistered(Me.GetType(), SCRIPT_LIBRARY) Then
				Page.ClientScript.RegisterClientScriptInclude(Me.GetType(), SCRIPT_LIBRARY, WebUtils.GetVirtualPath("AbacusWeb/Apps/InPlaceSelectors/InPlaceClientGroupSelector.js"))
			End If

		End Sub

		Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
			If Me.Required Then
				With valRequired
                    .Display = ValidatorDisplay.Dynamic
                    .ErrorMessage = "Please select a team"
                End With
			End If
		End Sub

		Private Sub LoadClientGroup()
			Dim msg As ErrorMessage
			Dim cg As ClientGroup
			Dim thePage As BasePage = CType(Me.Page, BasePage)

			If Me.ClientGroupID > 0 Then
				cg = New ClientGroup(thePage.DbConnection)
				msg = cg.Fetch(Me.ClientGroupID)
				If Not msg.Success Then WebUtils.DisplayError(msg)
				txtReference.Value = cg.AltReference
				txtName.Value = cg.Name
				hidID.Value = Me.ClientGroupID
			Else
				txtReference.Value = String.Empty
				txtName.Value = String.Empty
				hidID.Value = String.Empty
			End If
		End Sub

	End Class

End Namespace