
Namespace Apps.CMS.Controls

Partial Class PageWizardHeader
    Inherits System.Web.UI.UserControl

#Region " Private variables, constants and properties "

        Private Const URL_SELECTPAGE As String = "../Admin/Default.aspx?pageID={0}"
        Private Const URL_EDITPAGE As String = "../Admin/EditPage.aspx?pageID={0}"
        Private Const URL_PAGELOCATION As String = "../Admin/PageLocation.aspx?pageID={0}&showPageID={0}"
        Private Const URL_PAGEMENU As String = "../Admin/PageMenu.aspx?pageID={0}"
        Private Const URL_VIEWPAGE As String = "../Admin/ViewPage.aspx?pageID={0}"

        Private _pageID As Integer

        Public Property PageID() As Integer
            Get
                Return _pageID
            End Get
            Set(ByVal Value As Integer)
                _pageID = Value

                Dim pageSet As Boolean = (_pageID <> 0)

                Me.EnableEditPage = pageSet
                Me.EnablePageLocation = pageSet
                Me.EnablePageMenu = pageSet
                Me.EnableViewPage = pageSet

                Me.lnkSelectPage.NavigateUrl = String.Format(URL_SELECTPAGE, _pageID)
                Me.lnkEditPage.NavigateUrl = String.Format(URL_EDITPAGE, _pageID)
                Me.lnkPageLocation.NavigateUrl = String.Format(URL_PAGELOCATION, _pageID)
                Me.lnkPageMenu.NavigateUrl = String.Format(URL_PAGEMENU, _pageID)
                Me.lnkViewPage.NavigateUrl = String.Format(URL_VIEWPAGE, _pageID)

            End Set
        End Property

        Public Property EnableEditPage() As Boolean
            Get
                Return Me.lnkEditPage.Enabled
            End Get
            Set(ByVal Value As Boolean)
                Me.lnkEditPage.Enabled = Value
                Me.lnkEditPage.CssClass = IIf(Value, "", "disabled")
            End Set
        End Property

        Public Property EnablePageLocation() As Boolean
            Get
                Return Me.lnkPageLocation.Enabled
            End Get
            Set(ByVal Value As Boolean)
                Me.lnkPageLocation.Enabled = Value
                Me.lnkPageLocation.CssClass = IIf(Value, "", "disabled")
            End Set
        End Property

        Public Property EnablePageMenu() As Boolean
            Get
                Return Me.lnkPageMenu.Enabled
            End Get
            Set(ByVal Value As Boolean)
                Me.lnkPageMenu.Enabled = Value
                Me.lnkPageMenu.CssClass = IIf(Value, "", "disabled")
            End Set
        End Property

        Public Property EnableViewPage() As Boolean
            Get
                Return Me.lnkViewPage.Enabled
            End Get
            Set(ByVal Value As Boolean)
                Me.lnkViewPage.Enabled = Value
                Me.lnkViewPage.CssClass = IIf(Value, "", "disabled")
            End Set
        End Property

#End Region

End Class

End Namespace