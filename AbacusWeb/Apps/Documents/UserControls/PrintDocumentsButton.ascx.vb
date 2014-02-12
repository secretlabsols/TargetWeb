
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.IO
Imports System.Text
Imports System.Web.Script.Serialization
Imports Target.Library.Web.Controls
Imports Target.Library.Web.Controls.SearchableMenu
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports System.ComponentModel

Namespace Apps.Documents.UserControls

    ''' <summary>
    ''' User control to print documents
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' IHS  29/03/11  D11960 - Created
    ''' </history>
    Partial Public Class PrintDocumentsButton
        Inherits System.Web.UI.UserControl

#Region " Private variables & properties "

        Private _printAll As Boolean = False
        Private _showAsLink As Boolean = False
        Private _position As SearchableMenuPosition = SearchableMenuPosition.TopRight
        Private _menuItems As List(Of PrintButtonMenuItem) = New List(Of PrintButtonMenuItem)(2)

        ''' <summary>
        ''' Enables and disables the button and menu on this control.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Enabled() As Boolean
            Get
                Return Not btnPrintDocuments.Disabled
            End Get
            Set(ByVal value As Boolean)
                btnPrintDocuments.Disabled = Not value
            End Set
        End Property

        ''' <summary>
        ''' shows or hides the button and menu on this control.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ShowButton() As Boolean
            Get
                Return btnPrintDocuments.Visible
            End Get
            Set(ByVal value As Boolean)
                btnPrintDocuments.Visible = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets if it's a "Print All" button
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PrintAll() As Boolean
            Get
                Return _printAll
            End Get
            Set(ByVal value As Boolean)
                _printAll = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the text displayed on the button.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ButtonText() As String
            Get
                Return btnPrintDocuments.InnerText
            End Get
            Set(ByVal value As String)
                btnPrintDocuments.InnerText = value
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
                Return btnPrintDocuments.ClientID
            End Get
        End Property

        Private _buttonWidth As String
        <Browsable(True)> _
        Public Property ButtonWidth() As String
            Get
                Return _buttonWidth
            End Get
            Set(ByVal value As String)
                _buttonWidth = value
            End Set
        End Property

#End Region

#Region " Page_Load "

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            With CType(Me.Page, BasePage)
                ' add CSS
                .CssLinks.Add(WebUtils.GetVirtualPath("AbacusWeb/Apps/Documents/UserControls/PrintButton.css"))

                ' add in the jquery library
                .UseJQuery = True

                ' add in the jquery ui library
                .UseJqueryUI = True

                ' add the jquery searchable menu
                .UseJquerySearchableMenu = True
            End With

            Me.Page.ClientScript.RegisterClientScriptInclude(Me.GetType(), "ScriptLibrary", WebUtils.GetVirtualPath("AbacusWeb/Apps/Documents/UserControls/PrintButton.js"))

        End Sub

#End Region

#Region " Page_PreRender "

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            Dim startupScript As StringBuilder = New StringBuilder()
            Dim jsSerializer As JavaScriptSerializer = New JavaScriptSerializer()
            Dim jsonMenuItems As String = "[]"

            If Me.PrintAll Then Me.ButtonText = "Print All"

            CreateMenuItem("Now", "PrintButton PrintButtonNow", True)
            CreateMenuItem("Later", "PrintButton PrintButtonLater", False)

            If Me.ShowAsLink Then btnPrintDocuments.Attributes.Add("class", "link transbg")

            ' serialize the object to a json string
            With jsSerializer
                If Not _menuItems Is Nothing Then
                    jsonMenuItems = .Serialize(_menuItems.ToArray())
                End If
            End With
            startupScript.AppendFormat("var {0}_menuItems = {1};", Me.btnPrintDocuments.ClientID, jsonMenuItems)

            ' init the control
            startupScript.AppendFormat("PrintButton_Init('{0}','{1}','{2}');", _
                                       Me.btnPrintDocuments.ClientID, _
                                       IIf(Me.Position = SearchableMenuPosition.ClickPoint, String.Empty, Me.Position.ToString()), _
                                       divDownloadContainer.ClientID _
            )

            Me.Page.ClientScript.RegisterStartupScript( _
                Me.GetType(), _
                Me.ClientID, _
                startupScript.ToString(), _
                True _
            )

            If Not String.IsNullOrEmpty(ButtonWidth) Then
                btnPrintDocuments.Attributes.Add("style", "width:" & ButtonWidth & ";")
            End If
        End Sub

#End Region

#Region " CreateMenuItem "

        Private Sub CreateMenuItem(ByVal text As String, _
                                   ByVal cssClass As String, _
                                   ByVal printNow As Boolean)

            Dim menuItem As PrintButtonMenuItem

            menuItem = New PrintButtonMenuItem()
            With menuItem
                .description = text
                .cssClass = cssClass
                .printAll = Me.PrintAll
                .printNow = printNow
            End With

            _menuItems.Add(menuItem)

        End Sub

#End Region

    End Class

End Namespace