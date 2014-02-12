
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

Namespace UserControls

    ''' <summary>
    ''' User control to display a Html button with context menu giving direct access to the specified report.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MikeVO  04/05/2011  SDS issue #624 - fix to #604.
    ''' MikeVO  20/04/2011  SDS issue #604 - improved menu positioning using dynamic menu container size.
    ''' Iftikhar  01/03/11  D11966 - added property IReportsButton.DownloadContainer.
    ''' MikeVO  15/12/2009  A4WA#5967 - changes to popup window when exporting direct to a format.
    ''' MikeVO  15/10/2009  D11546 - ported CreateContextMenuItem() code to Target.Library.Web.Utils
    ''' MikeVO  14/09/2009  D11602 - menu improvements.
    ''' MikeVO  12/05/2009  D11549 - created.
    ''' </history>
    Partial Public Class ReportsButton
        Inherits System.Web.UI.UserControl
        Implements IReportsButton

#Region " Private variables & properties "

        Private _reportID As Integer
        Private _reportToExcel As Boolean = True
        Private _reportToView As Boolean = True
        Private _reportToPdf As Boolean = True
        Private _showAsLink As Boolean = False
        Private _parameters As NameValueCollection = New NameValueCollection()
        Private _position As SearchableMenuPosition = SearchableMenuPosition.BottomLeft
        Private _menuItems As List(Of ReportsButtonMenuItem) = New List(Of ReportsButtonMenuItem)(3)

        ''' <summary>
        ''' Enables and disables the button and menu on this control.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Enabled() As Boolean Implements IReportsButton.Enabled
            Get
                Return Not btnReports.Disabled
            End Get
            Set(ByVal value As Boolean)
                btnReports.Disabled = Not value
            End Set
        End Property

        ''' <summary>
        ''' shows or hides the button and menu on this control.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ShowButton() As Boolean Implements IReportsButton.ShowButton
            Get
                Return btnReports.Visible
            End Get
            Set(ByVal value As Boolean)
                btnReports.Visible = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the ID of the WebReport record to permit access to.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ReportID() As Integer Implements IReportsButton.ReportID
            Get
                Return _reportID
            End Get
            Set(ByVal value As Integer)
                _reportID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets if a menu option is dsplayed to export the report to Excel.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ReportToView() As Boolean Implements IReportsButton.ReportToView
            Get
                Return _reportToView
            End Get
            Set(ByVal value As Boolean)
                _reportToView = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets if a menu option is dsplayed to export the report to Excel.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ReportToExcel() As Boolean Implements IReportsButton.ReportToExcel
            Get
                Return _reportToExcel
            End Get
            Set(ByVal value As Boolean)
                _reportToExcel = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets if a menu option is dsplayed to export the report to Pdf.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ReportToPdf() As Boolean Implements IReportsButton.ReportToPdf
            Get
                Return _reportToPdf
            End Get
            Set(ByVal value As Boolean)
                _reportToPdf = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the text displayed on the button.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ButtonText() As String Implements IReportsButton.ButtonText
            Get
                Return btnReports.InnerText
            End Get
            Set(ByVal value As String)
                btnReports.InnerText = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the list of parameters to pass to the report.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Parameters() As NameValueCollection Implements IReportsButton.Parameters
            Get
                Return _parameters
            End Get
            Set(ByVal value As NameValueCollection)
                _parameters = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the position of the context menu.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Position() As SearchableMenuPosition Implements IReportsButton.Position
            Get
                Return _position
            End Get
            Set(ByVal value As SearchableMenuPosition)
                _position = value
            End Set
        End Property

        Public Property ShowAsLink() As Boolean Implements IReportsButton.ShowAsLink
            Get
                ShowAsLink = _showAsLink
            End Get
            Set(ByVal value As Boolean)
                _showAsLink = value
            End Set
        End Property

        Public ReadOnly Property DownloadContainer() As HtmlGenericControl Implements IReportsButton.DownloadContainer
            Get
                Return divDownloadContainer
            End Get
        End Property

        ''' <summary>
        ''' The _button width
        ''' </summary>
        Private _buttonWidth As String
        ''' <summary>
        ''' Gets or sets the width of the button.
        ''' </summary>
        ''' <value>
        ''' The width of the button.
        ''' </value>
        <Browsable(True)> _
        Public Property ButtonWidth() As String Implements IReportsButton.ButtonWidth
            Get
                Return _buttonWidth
            End Get
            Set(ByVal value As String)
                _buttonWidth = value
            End Set
        End Property


        ''' <summary>
        ''' The _button height
        ''' </summary>
        Private _buttonHeight As String
        ''' <summary>
        ''' Gets or sets the height of the button.
        ''' </summary>
        ''' <value>
        ''' The height of the button.
        ''' </value>
        <Browsable(True)> _
        Public Property ButtonHeight() As String Implements IReportsButton.ButtonHeight
            Get
                Return _buttonHeight
            End Get
            Set(ByVal value As String)
                _buttonHeight = value
            End Set
        End Property

#End Region

#Region " Page_Load "

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


            With CType(Me.Page, BasePage)
                ' add CSS
                .CssLinks.Add(WebUtils.GetVirtualPath("Library/UserControls/ReportsButton.css"))

                ' add in the jquery library
                .UseJQuery = True

                ' add in the jquery ui library
                .UseJqueryUI = True

                ' add the jquery searchable menu
                .UseJquerySearchableMenu = True
            End With

            Me.Page.ClientScript.RegisterClientScriptInclude(Me.GetType(), "ScriptLibrary", WebUtils.GetVirtualPath("Library/UserControls/ReportsButton.js"))

           
        End Sub

#End Region

#Region " Page_PreRender "

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            Dim startupScript As StringBuilder = New StringBuilder("$(function() { ")
            Dim jsSerializer As JavaScriptSerializer = New JavaScriptSerializer()
            Dim jsonMenuItems As String = "[]"

            If Not String.IsNullOrEmpty(ButtonWidth) Then
                btnReports.Attributes.Add("style", "width:" & ButtonWidth & ";")
            End If

            If Not String.IsNullOrEmpty(ButtonHeight) Then
                btnReports.Attributes.Add("style", "height:" & ButtonHeight & ";")
            End If

            If Me.ReportID > 0 Then
                If Me.ReportToView Then
                    CreateReportsMenuItem("View", String.Empty, "ReportButton ReportButtonView")
                End If
                If Me.ReportToExcel Then
                    CreateReportsMenuItem("Export to Excel", "EXCEL", "ReportButton ReportButtonExcel")
                End If
                If Me.ReportToPdf Then
                    CreateReportsMenuItem("Export to PDF", "PDF", "ReportButton ReportButtonPdf")
                End If
            Else
                Me.Enabled = False
                ' we shouldn't need the script below to disable the button but we do, for some unknown reason...
                startupScript.AppendFormat("GetElement('{0}').disabled=true;", Me.btnReports.ClientID)
            End If

            If _showAsLink Then
                btnReports.Attributes.Add("class", "link transbg")
            End If

            ' serialize the object to a json string
            With jsSerializer
                If Not _menuItems Is Nothing Then
                    jsonMenuItems = .Serialize(_menuItems.ToArray())
                End If
            End With
            startupScript.AppendFormat("{0}_menuItems = {1};", Me.btnReports.ClientID, jsonMenuItems)

            ' init the control
            startupScript.AppendFormat("ReportsButton_Init('{0}','{1}','{2}');", _
                                       Me.btnReports.ClientID, _
                                       IIf(Me.Position = SearchableMenuPosition.ClickPoint, String.Empty, Me.Position.ToString()), _
                                       divDownloadContainer.ClientID _
            )

            startupScript.Append(" });")

            Me.Page.ClientScript.RegisterStartupScript( _
                Me.GetType(), _
                Me.ClientID, _
                startupScript.ToString(), _
                True _
            )

        End Sub

#End Region

#Region " CreateReportsMenuItem "

        Private Sub CreateReportsMenuItem(ByVal text As String, _
                                          ByVal exportFormat As String, _
                                          ByVal cssClass As String)

            Dim menuItem As ReportsButtonMenuItem

            menuItem = New ReportsButtonMenuItem()
            With menuItem
                .Description = text
                .CssClass = cssClass
                .url = BuildUrl(Me.ReportID, exportFormat, Me.Parameters)
            End With

            _menuItems.Add(menuItem)

        End Sub

        Private Function BuildUrl(ByVal reportID As Integer, _
                                  ByVal exportFormat As String, _
                                  ByVal parameters As NameValueCollection) As String

            Const REPORT_VIEWER_URL As String = "Apps/Reports/Viewer.aspx?"

            Dim url As StringBuilder

            url = New StringBuilder()
            With url
                ' base url
                .Append(WebUtils.GetVirtualPath(REPORT_VIEWER_URL))
                ' reportID
                .AppendFormat("rID={0}", reportID)
                ' export format
                If Not String.IsNullOrEmpty(exportFormat) Then
                    .AppendFormat("&rc:Command=Render&rc:Format={0}", exportFormat)
                End If
                ' report parameters
                For Each param As String In parameters
                    .AppendFormat("&{0}={1}", param, parameters(param))
                Next
            End With

            Return url.ToString()

        End Function

#End Region

    End Class

End Namespace
