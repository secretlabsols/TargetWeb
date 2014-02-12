Imports System.Collections.Generic
Imports System.Text
Imports Target.Abacus.Library.InterfaceLogs
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps

Namespace Apps.UserControls

    ''' <summary>
    ''' Class representing a selector tool for InterfaceLog records
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''   ColinD   18/02/2010 D11874 - Created
    ''' </history>
    Partial Public Class InterfaceLogSelector
        Inherits System.Web.UI.UserControl

#Region "Fields"

        ' locals
        Private _FilterSelectedID As Integer = 0
        Private _FilterInterfaceLogTypes As InterfaceLogsBL.InterfaceLogType = 0
        Private _TableCaption As String = String.Empty
        Private _TableSummary As String = String.Empty

        ' constants
        Private Const _JsLibrary As String = "Library/JavaScript/"
        Private Const _SelectorName As String = "InterfaceLogSelector"

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the current page.
        ''' </summary>
        ''' <value>The current page.</value>
        Private ReadOnly Property CurrentPage() As Integer
            Get
                Dim page As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
                If page <= 0 Then page = 1
                Return page
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the interface log types to fetch.
        ''' </summary>
        ''' <value>The interface log types to fetch.</value>
        Public Property FilterInterfaceLogTypes() As InterfaceLogsBL.InterfaceLogType
            Get
                Return _FilterInterfaceLogTypes
            End Get
            Set(ByVal value As InterfaceLogsBL.InterfaceLogType)
                _FilterInterfaceLogTypes = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets selected id.
        ''' </summary>
        ''' <value>The selected id.</value>
        Public Property FilterSelectedID() As Integer
            Get
                Return _FilterSelectedID
            End Get
            Set(ByVal value As Integer)
                _FilterSelectedID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets my base page.
        ''' </summary>
        ''' <value>My base page.</value>
        Private ReadOnly Property MyBasePage() As BasePage
            Get
                Return CType(Page, BasePage)
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the table caption.
        ''' </summary>
        ''' <value>The table caption.</value>
        Public Property TableCaption() As String
            Get
                Return _TableCaption
            End Get
            Set(ByVal value As String)
                _TableCaption = value
            End Set
        End Property
        
        ''' <summary>
        ''' Gets or sets the table summary.
        ''' </summary>
        ''' <value>The table summary.</value>
        Public Property TableSummary() As String
            Get
                Return _TableSummary
            End Get
            Set(ByVal value As String)
                _TableSummary = value
            End Set
        End Property

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Inits the control.
        ''' </summary>
        Public Sub InitControl()

            SetupJavascriptLinks()

        End Sub

        ''' <summary>
        ''' Handles the PreRender event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            SetupJavascript()

        End Sub

#End Region

#Region "Functions"

        ''' <summary>
        ''' Setups the javascript.
        ''' </summary>
        Private Sub SetupJavascript()

            Dim js As New StringBuilder()

            ' add filter properties to page in js format
            js.AppendFormat("{1}_SelectedID = {0};", FilterSelectedID, _SelectorName)
            js.AppendFormat("var {1}_InterfaceLogTypes = {0};", CType(FilterInterfaceLogTypes, Integer), _SelectorName)
            js.AppendFormat("{1}_CurrentPage = {0};", CurrentPage, _SelectorName)
            js.AppendFormat("var {0}_QsInterfaceLogID = 'ilid';", _SelectorName)

            MyBasePage.ClientScript.RegisterStartupScript(Me.GetType(), _
                                                           String.Format("Target.Abacus.Web.Apps.UserControls.{0}.Startup", _SelectorName), _
                                                           Target.Library.Web.Utils.WrapClientScript(js.ToString()))

        End Sub

        ''' <summary>
        ''' Sets up the javascript links for this control.
        ''' </summary>
        Private Sub SetupJavascriptLinks()

            Dim js As New StringBuilder()

            ' add date utility JS
            MyBasePage.JsLinks.Add(WebUtils.GetVirtualPath(String.Format("{0}date.js", _JsLibrary)))

            ' add utility JS link
            MyBasePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath(String.Format("{0}WebSvcUtils.js", _JsLibrary)))

            ' add dialog JS
            MyBasePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath(String.Format("{0}Dialog.js", _JsLibrary)))

            ' add list filter JS
            MyBasePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath(String.Format("{0}ListFilter.js", _JsLibrary)))

            ' add page JS
            MyBasePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath(String.Format("AbacusWeb/Apps/UserControls/{0}.js", _SelectorName)))

            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.InterfaceLogs))

        End Sub

#End Region

    End Class

End Namespace
