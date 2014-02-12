
Imports System.Configuration.ConfigurationManager
Imports System.Text
Imports Target.Library
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.Security.Collections
Imports Target.Web.Apps.Navigation
Imports WebUtils = Target.Library.Web.Utils

''' <summary>
''' Default Master Page for the site.
''' </summary>
''' <remarks>Ported from .NET 1.1 version MasterPages\PageTemplate.ascx</remarks>
''' <history>
''' Blake   04/10/2013  #8178 - Added the registration of AjaxProTimeout.js for all pages used ExtJs
''' ColinD  12/08/2011  #6979 - Added UseJqueryAsync
''' ColinD  19/07/2011  D12140 - Added additional Jquery plugins.
''' MikeVO  06/04/2011  SDS issue#483 - do not show About link for Extranet.
''' MikeVO  30/09/2010  Support for validation summary controls.
''' MikeVO  30/09/2010  Support for BasePage.UseJQuery property.
''' MoTahir 28/06/2010  D11829 - Licensing
''' MikeVO  11/12/2009  A4WA#5967 - fix to top level menu width when resizing text.
''' MikeVO  22/10/2009  D11710 - added tooltip to image menu items.
''' MikeVO  11/09/2009  D11602 - menu improvements.
''' MikeVO  30/01/2009  A4WA#5200 - guard against username enumeration.
''' MikeVO  23/12/2008  Fix to menu root UL element needing id attribute for IE6.
''' MikeVO  01/12/2008  D11444 - security overhaul.
''' MikeVO  13/10/2008  Tweaks to previous change.
''' MikeVO  07/10/2008  Added support for site-specific CSS stylesheet.
''' </history>
Partial Public Class DefaultMaster
    Inherits System.Web.UI.MasterPage

    Private _thePage As BasePage

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        _thePage = DirectCast(Me.Page, BasePage)
    End Sub

    Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

        Dim msg As ErrorMessage
        Dim menu As vwWebSecurityUser_WebNavMenuItemCollection = Nothing
        Dim litPageTitle As Literal = New Literal
        Dim extraCss As StringBuilder

        ' page title
        litHtmlTitle.Text = String.Format("{0}: {1}", _
            _thePage.Settings.Setting("SiteName"), _
            _thePage.PageTitle)
        litPageTitle.Text = _thePage.PageTitle
        MPPageTitle.Controls.Add(litPageTitle)

        ' home link
        litHomeLink.Text = String.Format(BasePage.LINK_TEMPLATE, "home", "Home", _
            _thePage.Settings.Setting("HomePageUrl"))

        ' help link
        lnkHelpLink.NavigateUrl = _thePage.HelpUrl

        ' add paths
        'CSS
        If _thePage.UseExt Then
            _thePage.CssLinks.Insert(0, Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/ExtJs4/Source/resources/css/ext-all.css"))
        End If
        _thePage.CssLinks.Insert(0, _thePage.Settings("PageTemplateCssStylesheetSkinUrl"))
        _thePage.CssLinks.Insert(0, _thePage.Settings("PageTemplateCssStylesheetLayoutUrl"))
        _thePage.CssLinks.Insert(0, _thePage.Settings("CssStylesheetSkinUrl"))
        _thePage.CssLinks.Insert(0, _thePage.Settings("CssStylesheetLayoutUrl"))
        ' site-specific CSS goes at the end
        If _thePage.Settings.SettingExists("BrandingFolderPath") AndAlso _thePage.Settings("BrandingFolderPath").Length > 0 Then
            _thePage.CssLinks.Add(VirtualPathUtility.Combine(_thePage.Settings("BrandingFolderPath"), "Styles.css"))
        End If

        If _thePage.UseExt Then
            _thePage.JsLinks.Insert(0, Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/ExtJs4/Source/bootstrap.js"))
            _thePage.JsLinks.Insert(1, Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/ExtJs4/Custom/Defaults.js"))
        End If
        ' JS
        If _thePage.UseJQuery OrElse _thePage.UseJqueryUI OrElse _thePage.UseJqueryTooltip OrElse _thePage.UseJqueryTemplates Then
            _thePage.JsLinks.Insert(0, Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/jQuery.js"))
        End If
        If _thePage.UseJqueryTemplates Then
            _thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/Templates/jquery.tmpl.min.js"))
        End If
        If _thePage.UseJqueryAsync Then
            _thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/Async/Jquery.Async.js"))
        End If
        If _thePage.UseJqueryUI OrElse _thePage.UseJqueryTooltip Then
            _thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/UI/jquery-ui/js/jquery-ui-1.8.14.custom.min.js"))
            _thePage.CssLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/UI/jquery-ui/css/custom-theme/jquery-ui-1.8.14.custom.css"))
            _thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/UI/JqueryUI.js"))
            _thePage.CssLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/UI/JqueryUI.css"))
        End If
        If _thePage.UseJqueryTableFilter Then
            _thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/TableFilter/Jquery.BindWithDelay.js"))
            _thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/TableFilter/Jquery.TableFilter.js"))
        End If
        If _thePage.UseJquerySearchableMenu Then
            _thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/SearchableMenu/Jquery.TargetSystems.SearchableMenu.js"))
            _thePage.CssLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/SearchableMenu/Jquery.TargetSystems.SearchableMenu.css"))
        End If
        If _thePage.UseJqueryTableScroller Then
            _thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/TableScroll/jquery.tablescroll.js"))
            _thePage.CssLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/TableScroll/jquery.tablescroll.css"))
        End If
        If _thePage.UseJqueryTextboxClearer OrElse _thePage.UseJqueryTableFilter Then
            _thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/TextboxClearer/Jquery.TextboxClearer.js"))
            _thePage.CssLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/TextboxClearer/Jquery.TextboxClearer.css"))
        End If
        If _thePage.UseJqueryTooltip Then
            _thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/Tooltip/jquery.qtip.min.js"))
            _thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/Tooltip/jquery.qtip.custom.js"))
            _thePage.CssLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/Tooltip/jquery.qtip.min.css"))
            _thePage.CssLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/Tooltip/jquery.qtip.custom.css"))
        End If
        If _thePage.UseJqueryValidation Then
            _thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/Validation/jquery.validate.min.js"))
            _thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/Validation/jquery.validate.custom.js"))
            _thePage.CssLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/Validation/jquery.validate.css"))
        End If
        _thePage.JsLinks.Insert(0, Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/Utils.js"))

        SetAjaxProTimeout()

        ' output links
        ' CSS
        For Each css As String In _thePage.CssLinks.ToArray().Distinct()
            litCssReferences.Text &= String.Format(BasePage.LINK_TEMPLATE, "stylesheet", "CSS", css)
        Next
        litCssReferences.Text &= String.Format(BasePage.CONDITIONAL_LINK_TEMPLATE, "stylesheet", "CSS", _
            Target.Library.Web.Utils.GetVirtualPath("Library/Css/OldIE.css"), "<!--[if lt IE 7]>", "<![endif]-->")
        ' JS
        For Each js As String In _thePage.JsLinks.ToArray().Distinct()
            If js.StartsWith(BasePage.SCRIPT_START_TAG) Then
                litJSReferences.Text &= js
            Else
                litJSReferences.Text &= String.Format(BasePage.SCRIPT_TEMPLATE, js)
            End If
        Next

        If _thePage.Settings.CurrentApplicationID = ApplicationName.AbacusIntranet Then
            imgHeaderLogo.ImageUrl = _thePage.CurrentTheme & "IntranetLogo.png"
            imgHeaderLogo.AlternateText = "Intranet"
            imgHeaderLogo.Attributes.Add("title", "Intranet")

        ElseIf _thePage.Settings.CurrentApplicationID = ApplicationName.AbacusExtranet Then
            imgHeaderLogo.ImageUrl = _thePage.CurrentTheme & "ExtranetLogo.png"
            imgHeaderLogo.AlternateText = "Extranet"
            imgHeaderLogo.Attributes.Add("title", "Extranet")

        End If

        lnkHeaderLogo.NavigateUrl = _thePage.Settings("HomePageUrl")
        lnkAccessibility.NavigateUrl = Target.Library.Web.Utils.GetVirtualPath("Accessibility.aspx")
        If _thePage.Settings.CurrentApplicationID = ApplicationName.AbacusIntranet Then
            lnkAbout.NavigateUrl = Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/Admin/about/info.aspx")
        Else
            lnkAbout.Visible = False
        End If

        ' extra CSS styles
        Dim customBrandingPath As String = Target.Library.Web.Utils.GetVirtualPath("Library/Controls/MasterPages/Default/Images/")
        If _thePage.Settings.SettingExists("BrandingFolderPath") AndAlso _thePage.Settings("BrandingFolderPath").Length > 0 Then
            customBrandingPath = _thePage.Settings("BrandingFolderPath")
        End If
        extraCss = New StringBuilder()
        With extraCss
        End With
        _thePage.AddExtraCssStyle(extraCss.ToString())
        litExtraCssStyles.Text = _thePage.ExtraCssStyles

        ' is test system?
        testSystem.Visible = Convert.ToBoolean(AppSettings("TestSystem"))

        If _thePage.RenderMenu Then
            ' get menu
            msg = NavigationBL.GetMenu(_thePage.DbConnection, _thePage.Settings.CurrentApplicationID, menu)
            If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
            ' create menu
            CreateMenu(menu, NavigationBL.ROOT_MENU_ID, phMenu.Controls)
        Else
            menuContainer.Visible = False
        End If

    End Sub

    Private Sub CreateMenu(ByVal menu As vwWebSecurityUser_WebNavMenuItemCollection, _
                           ByVal parentID As Integer, _
                           ByRef menuControls As ControlCollection)


        ' pixel measurements
        Const TOP_LEVEL_MENU_BOUNDARY_IMAGE_WIDTH As Integer = 10
        Const TOP_LEVEL_MENU_IMAGE_PADDING_WIDTH As Integer = 2

        ' em measurements
        Const SUB_MENU_SECTION_WIDTH As Decimal = 13.5
        Const SUB_MENU_MAX_COLUMNS As Decimal = 4.0
        Const SUB_MENU_CONTAINER_PADDING As Decimal = 4.5

        Const TOP_LEVEL_CSS_CLASS As String = "topLevel"

        Dim ul As HtmlGenericControl = Nothing
        Dim li As HtmlGenericControl
        Dim ulCreated As Boolean = False
        Dim createdHeaderCount As Integer
        Dim topLevelMenuCount As Integer, topLevelMenuWidthPx As Integer
        Dim topLevelMenuWidthEm As Decimal, subMenuCalculatedWidthEm As Decimal, maxSubMenuWidthEm As Decimal
        Dim menuImagePath As String

        menuImagePath = _thePage.CurrentTheme

        ' get the width of the top level menu items
        For Each mi As vwWebSecurityUser_WebNavMenuItem In menu
            If mi.WebNavMenuItemParentID = parentID Then
                topLevelMenuCount += 1
                topLevelMenuWidthPx += mi.WebNavMenuItemImageFileWidth
            End If
        Next
        ' add on the two boundary images and account for the border spacing
        topLevelMenuWidthPx = topLevelMenuWidthPx + (TOP_LEVEL_MENU_BOUNDARY_IMAGE_WIDTH * 2) + (TOP_LEVEL_MENU_IMAGE_PADDING_WIDTH * topLevelMenuCount)
        topLevelMenuWidthEm = WebUtils.ConvertPxToEm(topLevelMenuWidthPx)

        ' get the maximum permitted width for the sub menu
        maxSubMenuWidthEm = SUB_MENU_MAX_COLUMNS * SUB_MENU_SECTION_WIDTH

        ' for each menu at this level
        For Each mi As vwWebSecurityUser_WebNavMenuItem In menu
            If mi.WebNavMenuItemParentID = parentID Then
                ' create this "menu" if required
                If Not ulCreated Then
                    ul = New HtmlGenericControl("ul")
                    ' if this is a root menu item, we need to give an ID to the ul element
                    If parentID = NavigationBL.ROOT_MENU_ID Then
                        ul.Attributes.Add("id", "newNav")
                    End If
                    menuControls.Add(ul)
                    ulCreated = True
                End If
                ' create this menu item
                li = New HtmlGenericControl("li")
                li.Attributes.Add("class", TOP_LEVEL_CSS_CLASS)
                If Not String.IsNullOrEmpty(mi.WebNavMenuItemImageFile) Then
                    li.Style.Add( _
                        "background-image", _
                        String.Format("url({0})", _
                            VirtualPathUtility.Combine( _
                                menuImagePath, _
                                mi.WebNavMenuItemImageFile _
                            ) _
                        ) _
                    )
                End If
                ul.Controls.Add(li)
                ' create the menu item
                CreateMenuLink(mi, _
                               TOP_LEVEL_CSS_CLASS, _
                               mi.WebNavMenuItemImageFileWidth, _
                               "px", _
                               li.Controls _
                )

                If parentID = NavigationBL.ROOT_MENU_ID AndAlso mi.WebNavMenuItemSubMenuCount > 0 Then

                    Dim subNav As HtmlGenericControl = New HtmlGenericControl("div")
                    subNav.Attributes.Add("class", "subnav")
                    li.Controls.Add(subNav)

                    Dim subNavTop As HtmlGenericControl = New HtmlGenericControl("div")
                    subNavTop.Attributes.Add("class", "subnavTop")
                    subNav.Controls.Add(subNavTop)

                    ' iframe needed for ie6 to lay menu over the top of window-ed elements such as SELECT
                    Dim iframe As Literal = New Literal()
                    iframe.Text = "<!--[if lte IE 6]><iframe src=""javascript:false;"" frameBorder=""0"" tabindex=""-1""></iframe><![endif]-->"
                    li.Controls.Add(iframe)

                    ' create sub menu content
                    createdHeaderCount = CreateSubNavContentHeaders(menu, _
                                                                   mi.WebNavMenuItemID, _
                                                                   SUB_MENU_SECTION_WIDTH, _
                                                                   SUB_MENU_MAX_COLUMNS, _
                                                                   subNavTop.Controls)

                    ' set width of sub menu
                    subMenuCalculatedWidthEm = createdHeaderCount * SUB_MENU_SECTION_WIDTH
                    subNav.Style.Add( _
                        "width", _
                        String.Format( _
                            "{0}em", _
                            Math.Max(topLevelMenuWidthEm, Math.Min(subMenuCalculatedWidthEm, maxSubMenuWidthEm)) + SUB_MENU_CONTAINER_PADDING _
                        ) _
                    )


                End If

            End If
        Next

    End Sub

    Private Function CreateSubNavContentHeaders(ByVal menu As vwWebSecurityUser_WebNavMenuItemCollection, _
                                                ByVal parentID As Integer, _
                                                ByVal width As Decimal, _
                                                ByVal columnLimit As Integer, _
                                                ByRef menuControls As ControlCollection) As Integer
        Dim li As HtmlGenericControl = Nothing
        Dim dl As HtmlGenericControl
        Dim dt As HtmlGenericControl
        Dim createdHeaderCount As Integer, createdItemCount As Integer
        Dim subnavContent As HtmlGenericControl = Nothing

        ' for each menu at this level
        For Each mi As vwWebSecurityUser_WebNavMenuItem In menu
            If mi.WebNavMenuItemParentID = parentID Then
                ' create a new UL when column limit is reached
                If createdHeaderCount Mod columnLimit = 0 Then
                    subnavContent = New HtmlGenericControl("ul")
                    subnavContent.Attributes.Add("class", "subnavContent")
                    menuControls.Add(subnavContent)
                End If
                ' create this "sub-menu section"
                li = New HtmlGenericControl("li")
                subnavContent.Controls.Add(li)
                li.Style.Add("width", String.Format("{0}em", width))
                ' create the section
                dl = New HtmlGenericControl("dl")
                li.Controls.Add(dl)
                ' create the header
                dt = New HtmlGenericControl("dt")
                dl.Controls.Add(dt)
                ' create the link
                CreateMenuLink(mi, Nothing, width, "em", dt.Controls)
                createdHeaderCount += 1
                ' create sub menu section items
                createdItemCount = CreateSubNavContentItems(menu, mi.WebNavMenuItemID, dl.Controls)
            End If
        Next

        Return createdHeaderCount

    End Function

    Private Function CreateSubNavContentItems(ByVal menu As vwWebSecurityUser_WebNavMenuItemCollection, _
                                         ByVal parentID As Integer, _
                                         ByRef menuControls As ControlCollection) As Integer
        Dim dd As HtmlGenericControl
        Dim createdItemCount As Integer

        For Each mi As vwWebSecurityUser_WebNavMenuItem In menu
            If mi.WebNavMenuItemParentID = parentID Then
                ' create the item
                dd = New HtmlGenericControl("dd")
                menuControls.Add(dd)
                ' create the link
                CreateMenuLink(mi, Nothing, 0, "em", dd.Controls)
                createdItemCount += 1
            End If
        Next

        Return createdItemCount

    End Function

    Private Sub CreateMenuLink(ByVal menuItem As vwWebSecurityUser_WebNavMenuItem, _
                               ByVal cssClass As String, _
                               ByVal width As Decimal, _
                               ByVal widthUnit As String, _
                               ByRef menuControls As ControlCollection)

        Dim menuLink As HtmlAnchor
        Dim span As HtmlGenericControl

        ' if we don't have a url and we don't have an image
        If String.IsNullOrEmpty(menuItem.WebNavMenuItemURL) AndAlso String.IsNullOrEmpty(menuItem.WebNavMenuItemImageFile) Then
            span = New HtmlGenericControl("span")
            span.InnerHtml = menuItem.WebNavMenuItemName
            If Not String.IsNullOrEmpty(cssClass) Then
                span.Attributes.Add("class", cssClass)
            End If
            If width > 0 Then
                span.Style.Add("width", String.Format("{0}{1}", width, widthUnit))
            End If
            menuControls.Add(span)
        Else
            menuLink = New HtmlAnchor()
            ' text or image?
            If String.IsNullOrEmpty(menuItem.WebNavMenuItemImageFile) Then
                menuLink.InnerHtml = menuItem.WebNavMenuItemName
            Else
                ' images are defined in the parent container but we should still output hidden text to aid accessibility
                span = New HtmlGenericControl("span")
                span.Attributes.Add("class", "hidden")
                span.InnerHtml = menuItem.WebNavMenuItemName
                menuLink.Controls.Add(span)
                ' add tooltip to image menus items
                menuLink.Title = menuItem.WebNavMenuItemName
            End If

            If Not menuItem.WebNavMenuItemURL Is Nothing AndAlso menuItem.WebNavMenuItemURL.Length > 0 Then
                menuLink.HRef = menuLink.ResolveUrl(menuItem.WebNavMenuItemURL)
            Else
                menuLink.HRef = "javascript:void(0);"
            End If
            If Not String.IsNullOrEmpty(cssClass) Then
                menuLink.Attributes.Add("class", cssClass)
            End If
            If width > 0 Then
                menuLink.Style.Add("width", String.Format("{0}{1}", width, widthUnit.ToString()))
            End If
            menuControls.Add(menuLink)
        End If

    End Sub

#Region "Configure AjaxPro Timeout"

    ''' <summary>
    ''' Includes the relevant script the allow the timeout for AjaxPro web service calls to be set
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetAjaxProTimeout()
        If Not IsAjaxProEnabled() Then
            Exit Sub
        End If

        Dim script As String = String.Format("setAjaxProTimeout({0})", _thePage.AjaxProTimeout.Value)
        _thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/AjaxProTimeout.js?v=2"))
        AjaxControlToolkit.ToolkitScriptManager.RegisterStartupScript(_thePage, _
                                                                      _thePage.GetType(), _
                                                                      "AjaxProTimeout", _
                                                                      script, _
                                                                      True)
    End Sub

    ''' <summary>
    ''' Checks the see if AjaxPro is configured on the page
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function IsAjaxProEnabled() As Boolean
        Return _thePage.UseExt AndAlso _thePage.AjaxProTimeout.HasValue
    End Function

#End Region

End Class