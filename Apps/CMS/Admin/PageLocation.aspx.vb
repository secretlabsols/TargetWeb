
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.Utils
Imports Target.Web.Apps.CMS.Controls

Namespace Apps.CMS.Admin

    ''' <summary>
    ''' Screen to allow the placement of a CMS page within the folder structure.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Partial Class PageLocation
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.CMS"), "Admin: CMS: Page Location")

            Dim cmsPage As WebCMSPage
            Dim folderID As Integer

            With wizHeader
                .PageID = Target.Library.Utils.ToInt32(Request.QueryString("pageID"))
            End With

            With tree
                .TreeMode = WebCMSTreeMode.CMSFolderAdmin
                .ShowPageID = Target.Library.Utils.ToInt32(Request.QueryString("showPageID"))
                .ShowFolderID = Target.Library.Utils.ToInt32(Request.QueryString("showFolderID"))
                .MovePageID = Target.Library.Utils.ToInt32(Request.QueryString("movePageID"))
                .MoveFolderID = Target.Library.Utils.ToInt32(Request.QueryString("moveFolderID"))
                .MoveTargetID = Target.Library.Utils.ToInt32(Request.QueryString("moveTargetID"))
            End With

            If Not IsPostBack Then
                ' get the page and its folder
                cmsPage = New WebCMSPage(Me.DbConnection)
                Dim msg As ErrorMessage = cmsPage.Fetch(wizHeader.PageID)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                folderID = cmsPage.WebCMSFolderID
                Dim folder As WebCMSFolder = New WebCMSFolder(Me.DbConnection)
                msg = folder.Fetch(folderID)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)

                litPageTitle.Text = String.Format("Move the page ""{0}"" to the required folder and click on ""Next"".", cmsPage.Title)

            End If

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "nextUrl", WrapClientScript(String.Format("var nextUrl = ""PageMenu.aspx?pageID={0}"";", wizHeader.PageID)))

        End Sub

    End Class

End Namespace