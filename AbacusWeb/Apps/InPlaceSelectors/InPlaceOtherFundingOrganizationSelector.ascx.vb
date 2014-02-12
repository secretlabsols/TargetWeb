Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Web.Apps.UserControls
Imports Target.Web.Apps

Namespace Apps.InPlaceSelectors

    ''' <summary>
    ''' Outputs an in-place, organisation selector.
    ''' </summary>
    ''' <remarks></remarks>
    Partial Public Class InPlaceOtherFundingOrganizationSelector
        Inherits System.Web.UI.UserControl

#Region " Private variables & properties "

        Private _organisationID As Integer
        Private _orgType As Integer

        Public Property OrgType() As Integer
            Get
                Return _orgType
            End Get
            Set(ByVal value As Integer)
                _orgType = value
            End Set
        End Property

        Public Property OrganisationID() As Integer
            Get
                Return _organisationID
            End Get
            Set(ByVal value As Integer)
                _organisationID = value
                LoadOrganisation()
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

            txtName.Attributes.Add("onchange", String.Format("InPlaceOtherFundingOrganizationSelector_ClearStoredID('{0}');", Me.ClientID))
            btnFind.Attributes.Add("onclick", String.Format("InPlaceOtherFundingOrganizationSelector_btnFind_Click('{0}', {1});", Me.ClientID, _orgType))

            If Not Page.ClientScript.IsClientScriptIncludeRegistered(Me.GetType(), SCRIPT_LIBRARY) Then
                Page.ClientScript.RegisterClientScriptInclude(Me.GetType(), SCRIPT_LIBRARY, WebUtils.GetVirtualPath("AbacusWeb/Apps/InPlaceSelectors/InPlaceOtherFundingOrganizationSelector.js"))
                'Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Web.Apps.UserControls.OtherFundingOrgSelector.Startup", _
                '    Target.Library.Web.Utils.WrapClientScript(String.Format("orgType={0};", _orgType)))
            End If
            

        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If Me.Required Then
                With valRequired
                    .ControlToValidate = hidID.ID
                    .Display = ValidatorDisplay.Dynamic
                    .ErrorMessage = "Please select an Organization."
                End With
            End If
        End Sub

        Private Sub LoadOrganisation()
            Dim msg As ErrorMessage
            Dim organisation As OtherFundingOrganization
            Dim thePage As BasePage = CType(Me.Page, BasePage)

            If Me.OrganisationID > 0 Then
                organisation = New OtherFundingOrganization(thePage.DbConnection)
                msg = organisation.Fetch(Me.OrganisationID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtName.Value = organisation.Name
                hidID.Value = Me.OrganisationID
            Else
                txtName.Value = String.Empty
                hidID.Value = String.Empty
            End If
        End Sub

    End Class

End Namespace