
Imports System.Text
Imports Target.Library
Imports Target.Web.Apps

''' <summary>
''' Popup Master Page for the site.
''' </summary>
''' <remarks>Ported from .NET 1.1 version MasterPages\TabbedPageTemplate.ascx</remarks>
''' <history>
''' Blake   04/10/2013  #8178 - Added the registration of AjaxProTimeout.js for all pages used ExtJs
''' MoTahir 22/08/2013  A8118 - On  service User  commissioned services, popup opened on new button click does not work (highlighted by Sarah)
''' ColinD  12/08/2011  #6979 - Added UseJqueryAsync
''' MikeVO  30/09/2010  Support for BasePage.UseJQuery property.
''' MikeVO  13/10/2008  Tweaks to previous change.
''' MikeVO  07/10/2008  Added support for site-specific CSS stylesheet.
''' </history>
Partial Public Class Popup
    Inherits System.Web.UI.MasterPage

    Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

        Dim thePage As BasePage = DirectCast(Me.Page, BasePage)

        ' set paths
        litHtmlTitle.Text = DirectCast(Me.Page, Target.Web.Apps.BasePage).PageTitle

        ' add paths
        'CSS
        If thePage.UseExt Then
            thePage.CssLinks.Insert(0, Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/ExtJs4/Source/resources/css/ext-all.css"))
        End If
        thePage.CssLinks.Insert(0, thePage.Settings("PageTemplateCssStylesheetSkinUrl"))
        thePage.CssLinks.Insert(0, thePage.Settings("PageTemplateCssStylesheetLayoutUrl"))
        thePage.CssLinks.Insert(0, thePage.Settings("CssStylesheetSkinUrl"))
        thePage.CssLinks.Insert(0, thePage.Settings("CssStylesheetLayoutUrl"))
        ' site-specific CSS goes at the end
        If thePage.Settings.SettingExists("BrandingFolderPath") AndAlso thePage.Settings("BrandingFolderPath").Length > 0 Then
            thePage.CssLinks.Add(VirtualPathUtility.Combine(thePage.Settings("BrandingFolderPath"), "Styles.css"))
        End If

        If thePage.UseExt Then
            thePage.JsLinks.Insert(0, Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/ExtJs4/Source/bootstrap.js"))
            thePage.JsLinks.Insert(1, Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/ExtJs4/Custom/Defaults.js"))
            thePage.JsLinks.Insert(2, Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/AjaxProTimeout.js"))
        End If
        ' JS
        If thePage.UseJQuery OrElse thePage.UseJqueryUI OrElse thePage.UseJqueryTooltip OrElse thePage.UseJqueryTemplates Then
            thePage.JsLinks.Insert(0, Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/jQuery.js"))
        End If
        If thePage.UseJqueryTemplates Then
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/Templates/jquery.tmpl.min.js"))
        End If
        If thePage.UseJqueryAsync Then
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/Async/Jquery.Async.js"))
        End If
        If thePage.UseJqueryUI OrElse thePage.UseJqueryTooltip Then
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/UI/jquery-ui/js/jquery-ui-1.8.14.custom.min.js"))
            thePage.CssLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/UI/jquery-ui/css/custom-theme/jquery-ui-1.8.14.custom.css"))
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/UI/JqueryUI.js"))
            thePage.CssLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/UI/JqueryUI.css"))
        End If
        If thePage.UseJqueryTableFilter Then
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/TableFilter/Jquery.BindWithDelay.js"))
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/TableFilter/Jquery.TableFilter.js"))
        End If
        If thePage.UseJquerySearchableMenu Then
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/SearchableMenu/Jquery.TargetSystems.SearchableMenu.js"))
            thePage.CssLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/SearchableMenu/Jquery.TargetSystems.SearchableMenu.css"))
        End If
        If thePage.UseJqueryTableScroller Then
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/TableScroll/jquery.tablescroll.js"))
            thePage.CssLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/TableScroll/jquery.tablescroll.css"))
        End If
        If thePage.UseJqueryTextboxClearer OrElse thePage.UseJqueryTableFilter Then
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/TextboxClearer/Jquery.TextboxClearer.js"))
            thePage.CssLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/TextboxClearer/Jquery.TextboxClearer.css"))
        End If
        If thePage.UseJqueryTooltip Then
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/Tooltip/jquery.qtip.min.js"))
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/Tooltip/jquery.qtip.custom.js"))
            thePage.CssLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/Tooltip/jquery.qtip.min.css"))
            thePage.CssLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/Tooltip/jquery.qtip.custom.css"))
        End If
        If thePage.UseJqueryValidation Then
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/Validation/jquery.validate.min.js"))
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/Validation/jquery.validate.custom.js"))
            thePage.CssLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/Validation/jquery.validate.css"))
        End If
        thePage.JsLinks.Insert(0, Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/Utils.js"))

        ' output links
        ' CSS
        For Each css As String In thePage.CssLinks.ToArray().Distinct()
            litCssReferences.Text &= String.Format(BasePage.LINK_TEMPLATE, "stylesheet", "CSS", css)
        Next
        litCssReferences.Text &= String.Format(BasePage.CONDITIONAL_LINK_TEMPLATE, "stylesheet", "CSS", _
            Target.Library.Web.Utils.GetVirtualPath("Library/Css/OldIE.css"), "<!--[if lt IE 7]>", "<![endif]-->")
        ' JS
        For Each js As String In thePage.JsLinks.ToArray().Distinct()
            If js.StartsWith(BasePage.SCRIPT_START_TAG) Then
                litJSReferences.Text &= js
            Else
                litJSReferences.Text &= String.Format(BasePage.SCRIPT_TEMPLATE, js)
            End If
        Next

        ' extra CSS styles
        Dim customBrandingPath As String = Target.Library.Web.Utils.GetVirtualPath("Library/Controls/MasterPages/Default/Images/")
        If thePage.Settings.SettingExists("BrandingFolderPath") AndAlso thePage.Settings("BrandingFolderPath").Length > 0 Then
            customBrandingPath = thePage.Settings("BrandingFolderPath")
        End If

        litExtraCssStyles.Text = thePage.ExtraCssStyles
    End Sub

End Class