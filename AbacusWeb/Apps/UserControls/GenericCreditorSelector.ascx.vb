Imports System.Collections.Generic
Imports System.Text
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps

Namespace Apps.UserControls

    ''' <summary>
    ''' Class representing a selector tool for GenericCreditor records
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''   PaulW    15/03/2011 D11974 - View Creditor Screen
    '''   ColinD   10/02/2010 D11874 - Created
    ''' </history>
    Partial Public Class GenericCreditorSelector
        Inherits System.Web.UI.UserControl

#Region "Fields"

        ' locals
        Private _FilterSelectedID As Integer = 0

        ' constants
        Private Const _JsLibrary As String = "Library/Javascript/"
        Private Const _SelectorName As String = "GenericCreditorSelector"

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the current page.
        ''' </summary>
        ''' <value>The current page.</value>
        Private ReadOnly Property CurrentPage() As Integer
            Get
                Dim page As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
                ' default always to page 1 if not larger
                If page <= 0 Then page = 1
                Return page
            End Get
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
        ''' Gets or sets a value indicating whether [show MRU list].
        ''' </summary>
        ''' <value><c>true</c> if [show MRU list]; otherwise, <c>false</c>.</value>
        Public Property ShowMruList() As Boolean
            Get
                Return mru.Visible
            End Get
            Set(ByVal value As Boolean)
                mru.Visible = value
            End Set
        End Property

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Inits the control.
        ''' </summary>
        ''' <param name="thePage">The page.</param>
        Public Sub InitControl(ByVal thePage As BasePage, Optional ByVal showViewButton As Boolean = False)

            Dim js As New StringBuilder()


            GenericCreditors_btnView.Visible = showViewButton

            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath(String.Format("{0}WebSvcUtils.js", _JsLibrary)))

            ' add dialog JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath(String.Format("{0}Dialog.js", _JsLibrary)))

            ' add list filter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath(String.Format("{0}ListFilter.js", _JsLibrary)))

            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath(String.Format("AbacusWeb/Apps/UserControls/{0}.js", _SelectorName)))

            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.CreditorPayments))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.Mru.WebSvc.Mru))

            ' add filter properties to page in js format
            js.AppendFormat("{1}_SelectedID = {0};", FilterSelectedID, _SelectorName)
            js.AppendFormat("{1}_CurrentPage = {0};", CurrentPage, _SelectorName)

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), _
                                                       String.Format("Target.Abacus.Web.Apps.UserControls.{0}.Startup", _SelectorName), _
                                                       Target.Library.Web.Utils.WrapClientScript(js.ToString()))

        End Sub

#End Region

    End Class

End Namespace
