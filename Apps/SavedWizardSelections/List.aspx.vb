
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.SavedWizardSelections

    ''' <summary>
    ''' Displays a paginated list of saved wizard selections.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' IHS     05/05/2011  A6714  - hiding btnBack when originating link was not "Manage selections...".
    ''' MikeVO  21/07/2009  D11651 - created.
    ''' </history>
    Partial Class List
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.SavedWizardSelections"), "Saved Wizard Selections")

            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            If currentPage <= 0 Then currentPage = 1

            divBtnBack.Visible = Not String.IsNullOrEmpty(Request.QueryString("backURL"))

            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Apps/SavedWizardSelections/List.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.SavedWizardSelections.WebSvc.Selections))

            Me.ClientScript.RegisterStartupScript( _
                Me.GetType(), _
                "Startup", _
                Target.Library.Web.Utils.WrapClientScript(String.Format( _
                    "currentPage={0};", _
                    currentPage) _
                ) _
            )

        End Sub

    End Class

End Namespace