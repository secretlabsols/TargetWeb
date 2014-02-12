
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.Utils
Imports Target.Web.Apps.CMS.Controls

Namespace Apps.CMS.Admin

    ''' <summary>
    ''' CMS editor start screen.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Partial Class DefaultPage
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.CMS"), "Admin: CMS")

            Dim cmsPage As WebCMSPage
            Dim msg As ErrorMessage
            Dim deletePageID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("deletePageID"))

            With wizHeader
                .PageID = Target.Library.Utils.ToInt32(Request.QueryString("pageID"))
            End With

            ' delete page if necessary
            If deletePageID > 0 Then
                msg = CmsBL.DeletePage(Me.DbConnection, deletePageID)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                ' if we have just delete the current page then reset
                If wizHeader.PageID = deletePageID Then wizHeader.PageID = 0
            End If

            rdoCreate.Checked = (wizHeader.PageID = 0)
            rdoEditSelected.Visible = Not (wizHeader.PageID = 0)
            rdoEditSelected.Checked = Not (wizHeader.PageID = 0)
            lblEditSelected.Visible = Not (wizHeader.PageID = 0)

            If wizHeader.PageID <> 0 Then
                cmsPage = New WebCMSPage(Me.DbConnection)
                msg = cmsPage.Fetch(wizHeader.PageID)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                lblEditSelected.Text = String.Format("Edit ""{0}""", cmsPage.Title)
                Me.ClientScript.RegisterStartupScript(Me.GetType(), "editSelectedUrl", WrapClientScript("var editSelectedUrl = ""EditPage.aspx?pageId=" & wizHeader.PageID & """;"))
            End If

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "selectPageUrl", WrapClientScript("var selectPageUrl = ""ModalDialogWrapper.axd?SelectItem.aspx?treeMode=" & WebCMSTreeMode.CMSPageSelect & """;"))

        End Sub

    End Class

End Namespace