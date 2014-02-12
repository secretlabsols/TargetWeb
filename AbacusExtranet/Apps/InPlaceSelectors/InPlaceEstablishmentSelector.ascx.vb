Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps

Namespace Apps.InPlaceSelectors
    Partial Public Class InPlaceEstablishmentSelector
        Inherits System.Web.UI.UserControl


#Region " Private variables & properties "

        Private _mode As EstablishmentSelectorMode = EstablishmentSelectorMode.Unknown
        Private _estabID As Integer

        Public Property Mode() As EstablishmentSelectorMode
            Get
                Return _mode
            End Get
            Set(ByVal value As EstablishmentSelectorMode)
                _mode = value
            End Set
        End Property

        Public Property EstablishmentID() As Integer
            Get
                Return _estabID
            End Get
            Set(ByVal value As Integer)
                _estabID = value
                LoadProvider()
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


        Public WriteOnly Property TxtReference_Enabled() As Boolean
            Set(ByVal value As Boolean)
                txtReference.Disabled = Not value
            End Set
        End Property


        Public WriteOnly Property TxtName_Enabled() As Boolean
            Set(ByVal value As Boolean)
                txtName.Disabled = Not value
            End Set
        End Property

        Public WriteOnly Property btnFind_Enabled() As Boolean
            Set(ByVal value As Boolean)
                btnFind.Disabled = Not value
            End Set
        End Property

        Public WriteOnly Property EstablishmentSelector_Enabled() As Boolean
            Set(ByVal value As Boolean)
                txtReference.Disabled = Not value
                txtName.Disabled = Not value
                btnFind.Disabled = Not value
            End Set
        End Property

        ''' <summary>
        ''' Get the Provider Reference of Selectred Provider / Establishment
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ProviderReference() As String
            Get
                Return txtReference.Value
            End Get
        End Property

        ''' <summary>
        ''' Get Provider Name / Establishment Name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ProviderName() As String
            Get
                Return txtName.Value
            End Get
        End Property

#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Const SCRIPT_LIBRARY As String = "Library"

            txtReference.Attributes.Add("onchange", String.Format("InPlaceEstablishmentSelector_ClearStoredID(""{0}"");", Me.ClientID))
            txtName.Attributes.Add("onchange", String.Format("InPlaceEstablishmentSelector_ClearStoredID(""{0}"");", Me.ClientID))
            btnFind.Attributes.Add("onclick", String.Format("InPlaceEstablishmentSelector_btnFind_Click(""{0}"",{1});", Me.ClientID, Convert.ToInt32(EstablishmentSelectorMode.DomProviders)))

            If Not Page.ClientScript.IsClientScriptIncludeRegistered(Me.GetType(), SCRIPT_LIBRARY) Then
                Page.ClientScript.RegisterClientScriptInclude(Me.GetType(), SCRIPT_LIBRARY, WebUtils.GetVirtualPath("AbacusExtranet/Apps/InPlaceSelectors/InPlaceEstablishmentSelector.js"))
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
                .ErrorMessage = "Please select a provider"
            End With
        End Sub

        Private Sub LoadProvider()
            Dim msg As ErrorMessage
            Dim estab As Establishment
            Dim thePage As BasePage = CType(Me.Page, BasePage)

            If Me.EstablishmentID > 0 Then
                estab = New Establishment(thePage.DbConnection)
                msg = estab.Fetch(Me.EstablishmentID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtReference.Value = estab.AltReference
                txtName.Value = estab.Name
                hidID.Value = Me.EstablishmentID
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

    Public Enum EstablishmentSelectorMode
        Unknown = 0
        Establishments = 1
        ResidentialHomes = 2
        DomProviders = 3
        DayCareProviders = 4
    End Enum

End Namespace