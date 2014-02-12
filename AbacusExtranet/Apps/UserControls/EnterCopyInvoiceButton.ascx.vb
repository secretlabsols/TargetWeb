
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.IO
Imports System.Text
Imports Target.Library.Web.Controls
Imports Target.Library.Web.Controls.SearchableMenu
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports System.Web.Script.Serialization

Namespace Apps.UserControls

    ''' <summary>
    ''' user control to Add new manually entered visits or copy
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>MW</history>

    Partial Public Class EnterCopyInvoiceButton
        Inherits System.Web.UI.UserControl

#Region " Private variables and properties "

        Private _showAsLink As Boolean = False
        Private _showButton As Boolean
        Private _position As SearchableMenuPosition = SearchableMenuPosition.BottomLeft
        Private _menuItems As List(Of EnterCopyInvoiceButtonMenuItem) = New List(Of EnterCopyInvoiceButtonMenuItem)(2)

        ''' <summary>
        ''' Enable disable button
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Enabled() As Boolean
            Get
                Return Not btnAddProformaInvoice.Disabled
            End Get
            Set(ByVal value As Boolean)
                btnAddProformaInvoice.Disabled = Not value
            End Set
        End Property

        Public Property ShowButton() As Boolean
            Get
                Return _showButton
            End Get
            Set(ByVal value As Boolean)
                _showButton = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the position of the context menu.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Position() As SearchableMenuPosition
            Get
                Return _position
            End Get
            Set(ByVal value As SearchableMenuPosition)
                _position = value
            End Set
        End Property

        Public Property ShowAsLink() As Boolean
            Get
                ShowAsLink = _showAsLink
            End Get
            Set(ByVal value As Boolean)
                _showAsLink = value
            End Set
        End Property

        Public ReadOnly Property PrintButtonID() As String
            Get
                Return btnAddProformaInvoice.ClientID
            End Get
        End Property

#End Region

#Region " Page_Load "

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            With CType(Me.Page, BasePage)
               
                ' add in the jquery library
                .UseJQuery = True

                ' add in the jquery ui library
                .UseJqueryUI = True

                ' add the jquery searchable menu
                .UseJquerySearchableMenu = True
            End With

            Me.Page.ClientScript.RegisterClientScriptInclude(Me.GetType(), "ScriptLibrary", WebUtils.GetVirtualPath("AbacusExtranet/Apps/UserControls/EnterCopyInvoiceButton.js"))

        End Sub

#End Region

#Region " Page_PreRender "

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            Dim startupScript As StringBuilder = New StringBuilder()
            Dim jsSerializer As JavaScriptSerializer = New JavaScriptSerializer()
            Dim jsonMenuItems As String = "[]"

            CreateMenuItem("Create New", "EnterCopyInvoiceButton EnterCopyInvoiceButtonNew", True)
            CreateMenuItem("Copy Existing", "EnterCopyInvoiceButton EnterCopyInvoiceButtonCopy", False)

            If Me.ShowAsLink Then btnAddProformaInvoice.Attributes.Add("class", "link transbg")

            ' serialize the object to a json string
            With jsSerializer
                If Not _menuItems Is Nothing Then
                    jsonMenuItems = .Serialize(_menuItems.ToArray())
                End If
            End With
            startupScript.AppendFormat("var {0}_menuItems = {1};", Me.btnAddProformaInvoice.ClientID, jsonMenuItems)

            ' init the control
            startupScript.AppendFormat("EnterCopyInvoiceButton_Init(""{0}"",""{1}"",""{2}"");", _
                                       Me.btnAddProformaInvoice.ClientID, _
                                       IIf(Me.Position = SearchableMenuPosition.ClickPoint, String.Empty, Me.Position.ToString()), _
                                       divDownloadContainer.ClientID _
            )

            Me.Page.ClientScript.RegisterStartupScript( _
                Me.GetType(), _
                Me.ClientID, _
                startupScript.ToString(), _
                True _
            )

        End Sub

#End Region


#Region " CreateMenuItem "

        Private Sub CreateMenuItem(ByVal text As String, _
                                   ByVal cssClass As String, _
                                   ByVal createNew As Boolean)

            Dim menuItem As EnterCopyInvoiceButtonMenuItem

            menuItem = New EnterCopyInvoiceButtonMenuItem()
            With menuItem
                .description = text
                .cssClass = cssClass
                .createNew = createNew
            End With

            _menuItems.Add(menuItem)

        End Sub

#End Region

    End Class

End Namespace