
Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Web.Apps.UserControls
Imports Target.Web.Apps
Imports Target.Library.Web
Imports Target.Library.ApplicationBlocks.DataAccess

Namespace Apps.InPlaceSelectors

    ''' <summary>
    ''' Outputs an in-place, client selector.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MoTahir 16/12/2011  BTI556 - Column size issue in New Creditor Payments  
    ''' MikeVO  10/02/2009  D11492 - change to validator setup.
    ''' </history>
    Partial Public Class InPlaceClientSelector
        Inherits System.Web.UI.UserControl

#Region " Private Constants "
        ' Constants
        Private Const _JavascriptPath As String = "AbacusWeb/Apps/InPlaceSelectors/InPlaceClientSelector.js"
#End Region

#Region " Private variables & properties "

        Private _hideDebtorRef As Boolean
        Private _hideCreditorRef As Boolean

        Public Property ClientDetailID() As Integer
            Get
                Return Target.Library.Utils.ToInt32(hidID.Value)
            End Get
            Set(ByVal value As Integer)
                hidID.Value = value
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
        ''' <summary>
        ''' Gets the base web page.
        ''' </summary>
        ''' <value>The base web page.</value>
        Private ReadOnly Property BaseWebPage() As BasePage
            Get
                Return CType(Me.Page, BasePage)
            End Get
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

        Public Property hideDebtorRef() As Boolean
            Get
                Return _hideDebtorRef
            End Get
            Set(ByVal value As Boolean)
                _hideDebtorRef = value
            End Set
        End Property

        Public Property hideCreditorRef() As Boolean
            Get
                Return _hideCreditorRef
            End Get
            Set(ByVal value As Boolean)
                _hideCreditorRef = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the javascript path for this user control.
        ''' </summary>
        ''' <value>The javascript path.</value>
        Private ReadOnly Property JavascriptPath() As String
            Get
                Return Target.Library.Web.Utils.GetVirtualPath(_JavascriptPath)
            End Get
        End Property

#End Region

        ''' <summary>
        ''' Handles the Init event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init

            'ClientDetailID = Target.Library.Utils.ToInt32(Me.GetPostBackValue())    ' set the id early so its accessible in pages!

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            'Const SCRIPT_LIBRARY As String = "Library"
            txtReference.Attributes.Add("onchange", String.Format("InPlaceClientSelector_ClearStoredID('{0}');", Me.ClientID))
            txtName.Attributes.Add("onchange", String.Format("InPlaceClientSelector_ClearStoredID('{0}');", Me.ClientID))
            btnFind.Attributes.Add("onclick", String.Format("InPlaceClientSelector_btnFind_Click('{0}');", Me.ClientID))

            'If Not Page.ClientScript.IsClientScriptIncludeRegistered(Me.GetType(), SCRIPT_LIBRARY) Then
            '    Page.ClientScript.RegisterClientScriptInclude(Me.GetType(), SCRIPT_LIBRARY, JavascriptPath)
            'End If

            'Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Web.Apps.InPlaceSelectors.InPlaceClientSelector.Startup", _
            ' Target.Library.Web.Utils.WrapClientScript(String.Format( _
            '  "InPlaceClientSelector_hideDebtorRef={0};InPlaceClientSelector_hideCreditorRef={1};", _
            '  hideDebtorRef.ToString.ToLower, hideCreditorRef.ToString.ToLower) _
            ' ) _
            ')

            SetupJavascript()

            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.Mru.WebSvc.Mru))
            Page.ClientScript.RegisterClientScriptInclude(GetType(Target.Library.Web.Controls.MruList), "Library2", WebUtils.GetVirtualPath("Library/Javascript/MruList.js"))

        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If Me.Required Then
                SetupValidator()
            End If
            LoadClient()
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
            Dim connection As SqlClient.SqlConnection = thePage.DbConnection

            If connection Is Nothing Then
                ' if nowt the create new connection

                connection = SqlHelper.GetConnectionToAbacus()

            End If

            If Me.ClientDetailID > 0 Then
                client = New ClientDetail(connection, String.Empty, String.Empty)
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

        ''' <summary>
        ''' Setups the javascript.
        ''' </summary>
        Private Sub SetupJavascript()

            Dim startupJavascript As String

            ' create a script tht sets ids of controls for this user control
            startupJavascript = String.Format("InPlaceClientSelector_hideDebtorRef={0};" _
                                              & "InPlaceClientSelector_hideCreditorRef={1};" _
                                               , _
                                              hideDebtorRef.ToString.ToLower, _
                                              hideCreditorRef.ToString.ToLower)

            ' register javascript file for this user control
            BaseWebPage.JsLinks.Add(JavascriptPath)

            ' register startup script for this user control
            BaseWebPage.ClientScript.RegisterStartupScript(BaseWebPage.GetType(), _
                                                           "InPlaceClientSelector.Init", _
                                                           Target.Library.Web.Utils.WrapClientScript(startupJavascript))

        End Sub


    End Class

End Namespace