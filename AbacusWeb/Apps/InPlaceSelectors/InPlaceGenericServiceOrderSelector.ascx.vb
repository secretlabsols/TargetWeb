
Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Web.Apps.UserControls
Imports Target.Web.Apps

Namespace Apps.InPlaceSelectors

    ''' <summary>
    ''' Outputs an in-place, client selector.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' Paul W  10/10/2011  D11945A - Viewing Service Order In Extranet.
    ''' </history>
    Partial Public Class InPlaceGenericServiceOrderSelector
        Inherits System.Web.UI.UserControl

#Region " Private variables & properties "

        Private _genericServiceOrderID As Integer
        Private _clientID As Integer

        Public Property GenericServiceOrderID() As Integer
            Get
                _genericServiceOrderID = hidID.Value
                Return _genericServiceOrderID
            End Get
            Set(ByVal value As Integer)
                _genericServiceOrderID = value
                LoadServiceOrder()
            End Set
        End Property

        Public Property selectedClientID() As Integer
            Get
                Return _clientID
            End Get
            Set(ByVal value As Integer)
                _clientID = value
                hidClientID.Value = value
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

            txtReference.Attributes.Add("onchange", String.Format("InPlaceGenericServiceOrderSelector_ClearStoredID('{0}');", Me.ClientID))
            btnFind.Attributes.Add("onclick", String.Format("InPlaceGenericServiceOrderSelector_btnFind_Click('{0}');", Me.ClientID))

            If Not Page.ClientScript.IsClientScriptIncludeRegistered(Me.GetType(), SCRIPT_LIBRARY) Then
                Page.ClientScript.RegisterClientScriptInclude(Me.GetType(), SCRIPT_LIBRARY, WebUtils.GetVirtualPath("AbacusWeb/Apps/InPlaceSelectors/InPlaceGenericServiceOrderSelector.js"))
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
                .ErrorMessage = "Please select a service order"
            End With
        End Sub

        Private Sub LoadServiceOrder()
            Dim msg As ErrorMessage
            Dim dso As GenericServiceOrder
            Dim thePage As BasePage = CType(Me.Page, BasePage)

            If Me.GenericServiceOrderID > 0 Then
                dso = New GenericServiceOrder(thePage.DbConnection)
                msg = dso.Fetch(Me.GenericServiceOrderID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtReference.Value = dso.OrderRef
                hidID.Value = Me.GenericServiceOrderID
            Else
                txtReference.Value = String.Empty
                hidID.Value = String.Empty
            End If
        End Sub

        Public Function GetPostBackValue() As String
            Return Request.Form(hidID.UniqueID)
        End Function

    End Class

End Namespace
