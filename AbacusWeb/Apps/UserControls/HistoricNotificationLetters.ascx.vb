Imports System.Collections.Generic
Imports System.Text
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Abacus.Library.UserLetters
Imports Target.Abacus.Library.SDS

Namespace Apps.UserControls

    Partial Public Class HistoricNotificationLetters
        Inherits System.Web.UI.UserControl

#Region "Fields"

        Private Const _SelectorName As String = "HistoricNotificationLetters"
        Private Const _WebNavMenuItemCommandGenerateNotificationsKey As String = "AbacusIntranet.WebNavMenuItemCommand.ServiceUserEnquiry.GenerateSdsNotifications"

#End Region

#Region " Properties "

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
        ''' Gets or sets the client ID 
        ''' </summary>
        Private _FilterClientID As Integer
        Public Property FilterClientID() As Integer
            Get
                Return _FilterClientID
            End Get
            Set(ByVal value As Integer)
                _FilterClientID = value
            End Set
        End Property

#End Region

        ''' <summary>
        ''' Inits the control.
        ''' </summary>
        ''' <param name="thePage">The page.</param>
        Public Sub InitControl(ByVal thePage As BasePage)

            Dim js As New StringBuilder()

            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))

            ' add dialog JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))

            ' add list filter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))

            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath(String.Format("AbacusWeb/Apps/UserControls/{0}.js", _SelectorName)))

            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.SUCLevels))

            ' add filter properties to page in js format
            js.AppendFormat("{1}_FilterClientID = {0};", FilterClientID, _SelectorName)
            'js.AppendFormat("{1}_SelectedID = {0};", FilterSelectedID, _SelectorName)
            js.AppendFormat("{1}_CurrentPage = {0};", CurrentPage, _SelectorName)
            'js.AppendFormat("{1}_TickImg = '{0}';", Target.Library.Web.Utils.GetVirtualPath("Images/Complete.png"), _SelectorName)
            'js.AppendFormat("{1}_CrossImg = '{0}';", Target.Library.Web.Utils.GetVirtualPath("Images/CriticalError.png"), _SelectorName)
            'js.AppendFormat("{1}_WarningImg = '{0}';", Target.Library.Web.Utils.GetVirtualPath("Images/WarningHS.png"), _SelectorName)
            'js.AppendFormat("{1}_BtnNotifyID = '{0}';", btnNotify.ClientID, _SelectorName)
            js.AppendFormat("{1}_btnViewID = '{0}';", btnView.ClientID, _SelectorName)
            'js.AppendFormat("{1}_HasCanGenerateSdsContributionNotificationsPermission = {0};", HasCanGenerateSdsContributionNotificationsPermission.ToString().ToLower(), _SelectorName)

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), _
                                                       String.Format("Target.Abacus.Web.Apps.UserControls.{0}.Startup", _SelectorName), _
                                                       Target.Library.Web.Utils.WrapClientScript(js.ToString()))

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


         
        End Sub

    End Class

End Namespace