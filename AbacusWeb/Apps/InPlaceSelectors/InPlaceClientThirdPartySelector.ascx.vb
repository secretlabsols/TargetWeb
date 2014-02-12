
Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Web.Apps.UserControls
Imports Target.Web.Apps

Namespace Apps.InPlaceSelectors

    ''' <summary>
    ''' Outputs an in-place, client third party selector.
    ''' </summary>
    ''' <remarks></remarks>
    Partial Public Class InPlaceClientThirdPartySelector
        Inherits System.Web.UI.UserControl

#Region " Private variables & properties "

        Private _thirdPartyID As Integer

        Public Property Client_Selector() As InPlaceSelectors.InPlaceClientSelector
            Get
                Return CType(Me.clientSelector, InPlaceSelectors.InPlaceClientSelector)
            End Get
            Set(ByVal value As InPlaceSelectors.InPlaceClientSelector)
                Me.clientSelector = value
            End Set
        End Property

        Public Property ThirdPartyID() As Integer
            Get
                Return _thirdPartyID
            End Get
            Set(ByVal value As Integer)
                _thirdPartyID = value
                LoadTP()
            End Set
        End Property

        Public Property ThirdPartyLabelWidth() As Unit
            Get
                Return lblThirdParty.Width
            End Get
            Set(ByVal value As Unit)
                lblThirdParty.Width = value
            End Set
        End Property

        Public Property ThirdPartyLabelText() As String
            Get
                Return lblThirdParty.Text
            End Get
            Set(ByVal value As String)
                lblThirdParty.Text = value
            End Set
        End Property

        Public Property Required() As Boolean
            Get
                Return valRequired.Visible
            End Get
            Set(ByVal value As Boolean)
                valRequired.Visible = value
                Me.Client_Selector.Required = value
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

            txtTPName.Attributes.Add("onchange", String.Format("InPlaceClientThirdPartySelector_ClearStoredID('{0}');", Me.ClientID))
            btnTPFind.Attributes.Add("onclick", String.Format("InPlaceClientThirdPartySelector_btnFindTP_Click('{0}');", Me.ClientID))

            If Not Page.ClientScript.IsClientScriptIncludeRegistered(Me.GetType(), SCRIPT_LIBRARY) Then
                Page.ClientScript.RegisterClientScriptInclude(Me.GetType(), SCRIPT_LIBRARY, WebUtils.GetVirtualPath("AbacusWeb/Apps/InPlaceSelectors/InPlaceClientThirdPartySelector.js"))
            End If

            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.Mru.WebSvc.Mru))
            Page.ClientScript.RegisterClientScriptInclude(GetType(Target.Library.Web.Controls.MruList), "Library2", WebUtils.GetVirtualPath("Library/Javascript/MruList.js"))

        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If Me.Required Then
                With valRequired
                    .ControlToValidate = hidID.ID
                    .Display = ValidatorDisplay.Dynamic
                    .ErrorMessage = "Please select a service user"
                End With
            End If
        End Sub

        Private Sub LoadTP()
            Dim msg As ErrorMessage
            Dim tP As Target.Abacus.Library.DataClasses.ThirdParty
            Dim thePage As BasePage = CType(Me.Page, BasePage)

            If Me.ThirdPartyID > 0 Then
                tP = New Target.Abacus.Library.DataClasses.ThirdParty(thePage.DbConnection)
                msg = tP.Fetch(Me.ThirdPartyID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtTPName.Value = String.Format("{0} {1}", tP.Title, tP.Surname)
                hidID.Value = Me.ThirdPartyID
            Else
                txtTPName.Value = String.Empty
                hidID.Value = String.Empty
            End If
        End Sub

    End Class

End Namespace