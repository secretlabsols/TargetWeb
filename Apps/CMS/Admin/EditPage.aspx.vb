
Imports FreeTextBoxControls
Imports System.Configuration.ConfigurationManager
Imports System.Collections.Specialized
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.Utils
Imports Target.Web.Apps.CMS.Controls

Namespace Apps.CMS.Admin

    ''' <summary>
    ''' Screen to allow editing of a CMS page.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Partial Class EditPage
        Inherits Target.Web.Apps.BasePage

#Region " Page Load "

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.CMS"), "Admin: CMS: Edit Page")

            With wizHeader
                .PageID = Target.Library.Utils.ToInt32(Request.QueryString("pageID"))
                .EnableEditPage = True
            End With

            If Not IsPostBack Then
                CreateToolbars()
                If wizHeader.PageID <> 0 Then
                    Dim cmsPage As WebCMSPage = New WebCMSPage(Me.DbConnection)
                    Dim msg As ErrorMessage = cmsPage.Fetch(wizHeader.PageID)
                    If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                    txtTitle.Text = cmsPage.Title
                    txtSubTitle.Text = cmsPage.SubTitle
                    ftbContent.Text = cmsPage.Content
                End If
                'Me.SetFocus(txtTitle)
            End If

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "linkToFileStoreFileUrl", WrapClientScript("var linkToFileStoreFileUrl = ""ModalDialogWrapper.axd?../../FileStore/Admin/SelectItem.aspx?treeMode=" & Target.Web.Apps.FileStore.Controls.WebFileStoreTreeMode.FileStoreFileSelect & """;var selectPageUrl = ""ModalDialogWrapper.axd?SelectItem.aspx?treeMode=" & WebCMSTreeMode.CMSPageSelect & """;"))

        End Sub

#End Region

#Region " CreateToolbars "

        Private Sub CreateToolbars()

            ' create the toolbars here in code rather than in the ASPX as VS.NET corrupts the definitions when switching 
            ' to/from design mode
            Dim tb1 As Toolbar = New Toolbar
            With tb1.Items
                .Add(New ParagraphMenu)
                .Add(New FontFacesMenu)
                .Add(New FontSizesMenu)
                .Add(New FontForeColorsMenu)
                .Add(New FontBackColorsMenu)
                .Add(New SymbolsMenu)
                ' pull HTML list items from web.config
                Dim htmlMenuItems As NameValueCollection = GetSection("Apps.CMS.FreeTextBox.InsertHtmlMenu")
                If Not htmlMenuItems Is Nothing AndAlso htmlMenuItems.Count > 0 Then
                    Dim htmlMenu As InsertHtmlMenu = New InsertHtmlMenu
                    For Each key As String In htmlMenuItems.AllKeys
                        htmlMenu.AddParsedSubObject(New ToolbarListItem(key, htmlMenuItems(key)))
                    Next
                    .Add(htmlMenu)
                End If
            End With
            ftbContent.Toolbars.Add(tb1)

            Dim tb2 As Toolbar = New Toolbar
            With tb2.Items
                .Add(New Bold)
                .Add(New Italic)
                .Add(New Underline)
                .Add(New StrikeThrough)
                .Add(New SuperScript)
                .Add(New SubScript)
            End With
            ftbContent.Toolbars.Add(tb2)

            Dim tb3 As Toolbar = New Toolbar
            With tb3.Items
                .Add(New JustifyLeft)
                .Add(New JustifyRight)
                .Add(New JustifyCenter)
                .Add(New JustifyFull)
                .Add(New BulletedList)
                .Add(New NumberedList)
                .Add(New Indent)
                .Add(New Outdent)
            End With
            ftbContent.Toolbars.Add(tb3)

            Dim tb4 As Toolbar = New Toolbar
            With tb4.Items
                .Add(New Cut)
                .Add(New Copy)
                .Add(New Paste)
                .Add(New Delete)
                .Add(New Undo)
                .Add(New Redo)
            End With
            ftbContent.Toolbars.Add(tb4)

            Dim tb5 As Toolbar = New Toolbar
            With tb5.Items
                Dim but1 As CreateLink = New CreateLink
                but1.Title = "Create a link to another web site"
                .Add(but1)
                Dim but2 As ToolbarButton = New ToolbarButton("Create a link to a file in the file store", "folderLink")
                but2.ScriptBlock = "LinkToFileStoreFile();"
                .Add(but2)
                Dim but3 As ToolbarButton = New ToolbarButton("Create a link to a page in the CMS", "cmsPageLink")
                but3.ScriptBlock = "LinkToCMSPage();"
                .Add(but3)
                .Add(New Unlink)
                .Add(New ToolbarSeparator)
                Dim but4 As ToolbarButton = New ToolbarButton("Insert an image from the file store", "insertimagefromgallery")
                but4.ScriptBlock = "InsertImageFromFileStore();"
                .Add(but4)
                .Add(New InsertRule)
                .Add(New InsertTable)
                .Add(New InsertDate)
                .Add(New InsertTime)
            End With
            ftbContent.Toolbars.Add(tb5)

            Dim tb6 As Toolbar = New Toolbar
            With tb6.Items
                .Add(New SelectAll)
                .Add(New RemoveFormat)
            End With
            ftbContent.Toolbars.Add(tb6)

            Dim tb7 As Toolbar = New Toolbar
            With tb7.Items
                .Add(New Print)
                .Add(New Preview)
            End With
            ftbContent.Toolbars.Add(tb7)

        End Sub

#End Region

#Region " btnNext_Click "

        Private Sub btnNext_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNext.Click

            If IsValid Then

                Dim pageID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("pageID"))
                Dim cmsPage As WebCMSPage = New WebCMSPage(Me.DbConnection)
                Dim msg As ErrorMessage

                If pageID <> 0 Then
                    msg = cmsPage.Fetch(pageID)
                    If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                End If

                With cmsPage
                    .Title = txtTitle.Text
                    .SubTitle = txtSubTitle.Text
                    .Content = ftbContent.Text
                    If pageID = 0 Then
                        .WebCMSFolderID = -1
                        .WebNavMenuItemID = -1
                    End If
                    msg = .Save()
                    If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                    pageID = .ID
                End With

                'Response.Redirect(String.Format("PageLocation.aspx?pageID={0}&showPageID={0}", pageID))

            End If

        End Sub

#End Region

    End Class

End Namespace