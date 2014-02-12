Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps

Namespace Apps.InPlaceSelectors

    Partial Public Class InPlaceClientSelector
        Inherits System.Web.UI.UserControl


#Region " Private variables & properties "

        Private _clientDetailID As Integer

        Public Property ClientDetailID() As Integer
            Get
                Return _clientDetailID
            End Get
            Set(ByVal value As Integer)
                _clientDetailID = value
                LoadClient()
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

        Public ReadOnly Property RequiredValidator() As RequiredFieldValidator
            Get
                Return valRequired
            End Get
        End Property

#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Const SCRIPT_LIBRARY As String = "Library"

            txtReference.Attributes.Add("onchange", String.Format("InPlaceClientSelector_ClearStoredID(""{0}"");", Me.ClientID))
            txtName.Attributes.Add("onchange", String.Format("InPlaceClientSelector_ClearStoredID(""{0}"");", Me.ClientID))
            btnFind.Attributes.Add("onclick", String.Format("InPlaceClientSelector_btnFind_Click(""{0}"");", Me.ClientID))

            If Not Page.ClientScript.IsClientScriptIncludeRegistered(Me.GetType(), SCRIPT_LIBRARY) Then
                Page.ClientScript.RegisterClientScriptInclude(Me.GetType(), SCRIPT_LIBRARY, WebUtils.GetVirtualPath("AbacusExtranet/Apps/InPlaceSelectors/InPlaceClientSelector.js"))
            End If

            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.Mru.WebSvc.Mru))
            Page.ClientScript.RegisterClientScriptInclude(GetType(Target.Library.Web.Controls.MruList), "Library2", WebUtils.GetVirtualPath("Library/Javascript/MruList.js"))

        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If Me.Required Then
                SetupValidator()
            End If
        End Sub

        Private Sub SetupValidator()
            With valRequired
                .Display = ValidatorDisplay.Dynamic
                .ErrorMessage = "Please select a service user"
            End With
        End Sub

        Private Sub LoadClient()
            Dim msg As ErrorMessage
            Dim client As ClientDetail
            Dim thePage As BasePage = CType(Me.Page, BasePage)

            If Me.ClientDetailID > 0 Then
                client = New ClientDetail(thePage.DbConnection, String.Empty, String.Empty)
                msg = client.Fetch(Me.ClientDetailID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtReference.Value = client.Reference
                txtName.Value = client.Name
                hidID.Value = Me.ClientDetailID
            Else
                txtReference.Value = String.Empty
                txtName.Value = String.Empty
                hidID.Value = String.Empty
            End If
        End Sub

        Public Function GetPostBackValue() As String
            Return Request.Form(hidID.UniqueID)
        End Function


    End Class

End Namespace