
Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Web.Apps.UserControls
Imports Target.Web.Apps
Imports Target.Abacus.Library

Namespace Apps.InPlaceSelectors

	''' <summary>
	''' Outputs an in-place, domiciliary contract selector.
	''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MoTahir 10/10/2009 - D11681
    ''' MikeVO  10/02/2009  D11492 - change to validator setup.
    ''' </history>
	Partial Public Class InPlaceDomContractSelector
		Inherits System.Web.UI.UserControl

#Region " Private variables & properties "

		Private _contractID As Integer
        Private _enabled As Boolean = True
        Private _serviceGroupClassificationID As Integer = ServiceGroupClassifications.All
        Private _FrameworkTypeID As Nullable(Of FrameworkTypes)

        Public Property FrameworkTypeID() As Nullable(Of FrameworkTypes)
            Get
                Return _FrameworkTypeID
            End Get
            Set(ByVal value As Nullable(Of FrameworkTypes))
                _FrameworkTypeID = value
            End Set
        End Property

        Public Property ServiceGroupClassificationID() As Integer
            Get
                Return _serviceGroupClassificationID
            End Get
            Set(ByVal value As Integer)
                _serviceGroupClassificationID = value
            End Set
        End Property

		Public Property ContractID() As Integer
			Get
				Return _contractID
			End Get
			Set(ByVal value As Integer)
				_contractID = value
				LoadContract()
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
			End Set
		End Property

#End Region

		Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Const SCRIPT_LIBRARY As String = "Library"
            Dim frameWorkType As Integer = 0

            If FrameworkTypeID.HasValue Then

                frameWorkType = FrameworkTypeID.Value

            End If

            txtNumber.Attributes.Add("onchange", String.Format("ServiceGroupClassificationID='{1}'; FrameworkTypeID={2}; InPlaceDomContractSelector_ClearStoredID('{0}');", Me.ClientID, _serviceGroupClassificationID, frameWorkType))
            txtTitle.Attributes.Add("onchange", String.Format("ServiceGroupClassificationID='{1}'; FrameworkTypeID={2}; InPlaceDomContractSelector_ClearStoredID('{0}');", Me.ClientID, _serviceGroupClassificationID, frameWorkType))
            btnFind.Attributes.Add("onclick", String.Format("ServiceGroupClassificationID='{1}'; FrameworkTypeID={2}; InPlaceDomContractSelector_btnFind_Click('{0}');", Me.ClientID, _serviceGroupClassificationID, frameWorkType))

			If Not Page.ClientScript.IsClientScriptIncludeRegistered(Me.GetType(), SCRIPT_LIBRARY) Then
				Page.ClientScript.RegisterClientScriptInclude(Me.GetType(), SCRIPT_LIBRARY, WebUtils.GetVirtualPath("AbacusWeb/Apps/InPlaceSelectors/InPlaceDomContractSelector.js"))
            End If

            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.Mru.WebSvc.Mru))
            Page.ClientScript.RegisterClientScriptInclude(GetType(Target.Library.Web.Controls.MruList), "Library2", WebUtils.GetVirtualPath("Library/Javascript/MruList.js"))

		End Sub

		Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

			Const SCRIPT_STARTUP As String = "Startup"

			If Not Me.Enabled Then
				If Not Page.ClientScript.IsStartupScriptRegistered(Me.GetType(), SCRIPT_STARTUP) Then
					Page.ClientScript.RegisterStartupScript(Me.GetType(), SCRIPT_STARTUP, _
					 String.Format("InPlaceDomContractSelector_Enabled('{0}', {1});", Me.ClientID, Me.Enabled.ToString().ToLower()), _
					 True)
				End If
			End If

            If Me.Required Then
                SetupValidator()
            End If
        End Sub

        Private Sub SetupValidator()
            With valRequired
                .Display = ValidatorDisplay.Dynamic
                .ErrorMessage = "Please select a contract"
            End With
        End Sub

		Private Sub LoadContract()
			Dim msg As ErrorMessage
			Dim contract As DomContract
			Dim thePage As BasePage = CType(Me.Page, BasePage)

			If Me.ContractID > 0 Then
				contract = New DomContract(thePage.DbConnection, String.Empty, String.Empty)
				msg = contract.Fetch(Me.ContractID)
				If Not msg.Success Then WebUtils.DisplayError(msg)
				txtNumber.Value = contract.Number
				txtTitle.Value = contract.Title
				hidID.Value = Me.ContractID
			Else
				txtNumber.Value = String.Empty
				txtTitle.Value = String.Empty
				hidID.Value = String.Empty
			End If
        End Sub

        Public Function GetPostBackValue() As String
            Return Request.Form(hidID.UniqueID)
        End Function

	End Class

End Namespace