
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.Utils
Imports Target.Web.Apps.CMS.Controls
Imports Target.Web.Apps.Navigation.Controls

Namespace Apps.CMS.Admin

    ''' <summary>
    ''' Screen to placement of a CMS page within the application menu.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Partial Class PageMenu
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.CMS"), "Admin: CMS: Page Menu")

            Dim msg As ErrorMessage

            With wizHeader
                .PageID = Target.Library.Utils.ToInt32(Request.QueryString("pageID"))
            End With

            Dim cmsPage As WebCMSPage = New WebCMSPage(Me.DbConnection)
            With cmsPage
                msg = .Fetch(wizHeader.PageID)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
            End With

            With tree
                .ShowMenuID = cmsPage.WebNavMenuItemID
                .NewMenuUrl = "~/Apps/CMS/CMSGetPage.axd?id=" & wizHeader.PageID
                .CMSPageID = wizHeader.PageID
                .MoveDownMenuID = Target.Library.Utils.ToInt32(Request.QueryString("moveDownMenuID"))
                .MoveUpMenuID = Target.Library.Utils.ToInt32(Request.QueryString("moveUpMenuID"))
            End With

            If cmsPage.WebNavMenuItemID > 0 Then
                litTitle.Text = "Please move the selected menu to the required position in the menu structure and click on ""Next"".<br>If the menu item is no longer required click on the ""Delete"" icon."
            Else
                litTitle.Text = "If a menu item is required for this page, click on the ""New Menu"" icon to create it."
            End If

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "nextUrl", WrapClientScript(String.Format("var nextUrl = ""ViewPage.aspx?pageID={0}"";", wizHeader.PageID)))


        End Sub

    End Class

End Namespace