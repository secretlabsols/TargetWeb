
Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Web.Apps.UserControls
Imports Target.Web.Apps

Namespace Apps.InPlaceSelectors

	''' <summary>
	''' Outputs an in-place, care manager selector.
	''' </summary>
	''' <remarks></remarks>
	Partial Public Class InPlaceCareManagerSelector
		Inherits System.Web.UI.UserControl

#Region " Private variables & properties "

		Private _careManagerID As Integer

		Public Property CareManagerID() As Integer
			Get
				Return _careManagerID
			End Get
			Set(ByVal value As Integer)
				_careManagerID = value
				LoadCareManager()
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

			txtReference.Attributes.Add("onchange", String.Format("InPlaceCareManagerSelector_ClearStoredID('{0}');", Me.ClientID))
			txtName.Attributes.Add("onchange", String.Format("InPlaceCareManagerSelector_ClearStoredID('{0}');", Me.ClientID))
			btnFind.Attributes.Add("onclick", String.Format("InPlaceCareManagerSelector_btnFind_Click('{0}');", Me.ClientID))

			If Not Page.ClientScript.IsClientScriptIncludeRegistered(Me.GetType(), SCRIPT_LIBRARY) Then
				Page.ClientScript.RegisterClientScriptInclude(Me.GetType(), SCRIPT_LIBRARY, WebUtils.GetVirtualPath("AbacusWeb/Apps/InPlaceSelectors/InPlaceCareManagerSelector.js"))
			End If

		End Sub

		Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
			If Me.Required Then
				With valRequired
                    .Display = ValidatorDisplay.Dynamic
                    .ErrorMessage = "Please select a care manager"
                End With
			End If
		End Sub

		Private Sub LoadCareManager()
			Dim msg As ErrorMessage
			Dim cm As CareManager
			Dim thePage As BasePage = CType(Me.Page, BasePage)

			If Me.CareManagerID > 0 Then
				cm = New CareManager(thePage.DbConnection)
				msg = cm.Fetch(Me.CareManagerID)
				If Not msg.Success Then WebUtils.DisplayError(msg)
				txtReference.Value = cm.AltReference
				txtName.Value = cm.Name
				hidID.Value = Me.CareManagerID
			Else
				txtReference.Value = String.Empty
				txtName.Value = String.Empty
				hidID.Value = String.Empty
			End If
		End Sub

	End Class

End Namespace