
Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Web.Apps.UserControls
Imports Target.Web.Apps

Namespace Apps.InPlaceSelectors

	''' <summary>
	''' Outputs an in-place, primary care trust selector.
	''' </summary>
	''' <remarks></remarks>
	Partial Public Class InPlacePctSelector
		Inherits System.Web.UI.UserControl

#Region " Private variables & properties "

		Private _pctID As Integer

		Public Property PctID() As Integer
			Get
				Return _pctID
			End Get
			Set(ByVal value As Integer)
				_pctID = value
				LoadPct()
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

        Public ReadOnly Property RequiredValidator() As RequiredFieldValidator
            Get
                Return valRequired
            End Get
        End Property

#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Const SCRIPT_LIBRARY As String = "Library"

            txtName.Attributes.Add("onchange", String.Format("InPlacePctSelector_ClearStoredID('{0}');", Me.ClientID))
            btnFind.Attributes.Add("onclick", String.Format("InPlacePctSelector_btnFind_Click('{0}');", Me.ClientID))

            If Not Page.ClientScript.IsClientScriptIncludeRegistered(Me.GetType(), SCRIPT_LIBRARY) Then
                Page.ClientScript.RegisterClientScriptInclude(Me.GetType(), SCRIPT_LIBRARY, WebUtils.GetVirtualPath("AbacusWeb/Apps/InPlaceSelectors/InPlacePctSelector.js"))
            End If

        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If Me.Required Then
                With valRequired
                    .Display = ValidatorDisplay.Dynamic
                    .ErrorMessage = "Please select a Clinical Commissioning Group"
                End With
            End If
        End Sub

        Private Sub LoadPct()
            Dim msg As ErrorMessage
            Dim pct As PrimaryCareTrust
            Dim thePage As BasePage = CType(Me.Page, BasePage)

            If Me.PctID > 0 Then
                pct = New PrimaryCareTrust(thePage.DbConnection)
                msg = pct.Fetch(Me.PctID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtName.Value = pct.Name
                hidID.Value = Me.PctID
            Else
                txtName.Value = String.Empty
                hidID.Value = String.Empty
            End If
        End Sub

    End Class

End Namespace