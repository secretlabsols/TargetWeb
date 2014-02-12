
Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Web.Apps.UserControls
Imports Target.Web.Apps

Namespace Apps.InPlaceSelectors

	''' <summary>
	''' Outputs an in-place, team selector.
	''' </summary>
	''' <remarks></remarks>
	Partial Public Class InPlaceTeamSelector
		Inherits System.Web.UI.UserControl

#Region " Private variables & properties "

		Private _teamID As Integer
		Private _availableToRes As TriState = TriState.UseDefault
		Private _availableToDom As TriState = TriState.UseDefault

		Public Property TeamID() As Integer
			Get
				Return _teamID
			End Get
			Set(ByVal value As Integer)
				_teamID = value
				LoadTeam()
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

		Public Property AvailableToRes() As TriState
			Get
				Return _availableToRes
			End Get
			Set(ByVal value As TriState)
				_availableToRes = value
			End Set
		End Property

		Public Property AvailableToDom() As TriState
			Get
				Return _availableToDom
			End Get
			Set(ByVal value As TriState)
				_availableToDom = value
			End Set
		End Property

#End Region

		Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

			Const SCRIPT_LIBRARY As String = "Library"

			txtReference.Attributes.Add("onchange", String.Format("InPlaceTeamSelector_ClearStoredID('{0}');", Me.ClientID))
			txtName.Attributes.Add("onchange", String.Format("InPlaceTeamSelector_ClearStoredID('{0}');", Me.ClientID))
			btnFind.Attributes.Add("onclick", String.Format("InPlaceTeamSelector_btnFind_Click('{0}', '{1}', '{2}');", Me.ClientID, Me.AvailableToRes, Me.AvailableToDom))

			If Not Page.ClientScript.IsClientScriptIncludeRegistered(Me.GetType(), SCRIPT_LIBRARY) Then
				Page.ClientScript.RegisterClientScriptInclude(Me.GetType(), SCRIPT_LIBRARY, WebUtils.GetVirtualPath("AbacusWeb/Apps/InPlaceSelectors/InPlaceTeamSelector.js"))
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

		Private Sub LoadTeam()
			Dim msg As ErrorMessage
			Dim t As Team
			Dim thePage As BasePage = CType(Me.Page, BasePage)

			If Me.TeamID > 0 Then
				t = New Team(thePage.DbConnection)
				msg = t.Fetch(Me.TeamID)
				If Not msg.Success Then WebUtils.DisplayError(msg)
				txtReference.Value = t.AltReference
				txtName.Value = t.Name
				hidID.Value = Me.TeamID
			Else
				txtReference.Value = String.Empty
				txtName.Value = String.Empty
				hidID.Value = String.Empty
			End If
		End Sub

	End Class

End Namespace