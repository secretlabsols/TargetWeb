Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports Target.Abacus.Library
Imports FreeTextBoxControls

Namespace Apps.Admin.Misc

    Partial Public Class EmailTemplates
        Inherits BasePage

        Private _stdBut As StdButtonsBase
        'Private _customToolbar As Toolbar
        'Private _htmlMenu As InsertHtmlMenu

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Administration.Miscellaneous.EmailTemplates"), "Email Templates")
            'Dim customToolbar As Toolbar
            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                .AllowNew = False
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Administration.Miscellaneous.EmailTemplates.Edit"))
                .AllowDelete = False
                With .SearchBy
                    .Add("Description", "Description")
                End With
                .EditableControls.Add(fsControls.Controls)
                .GenericFinderTypeID = GenericFinderType.EmailTemplates
                .AuditLogTableNames.Add("EmailTemplate")
            End With
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf FindClicked

            If Not Me.IsPostBack Then
                Dim tb1 As Toolbar = New Toolbar
                ftbContent.Toolbars.Insert(0, tb1)
                '    customToolbar = New Toolbar
                '    ftbContent.Toolbars.Add(customToolbar)
                '    With customToolbar.Items
                '        _htmlMenu = New InsertHtmlMenu()
                '        .Add(_htmlMenu)
                '    End With
            End If

        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)
            Dim msg As ErrorMessage

            Dim template As New EmailTemplate(Me.DbConnection, SecurityBL.GetCurrentUser().ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

            msg = template.Fetch(e.ItemID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            txtDescription.Text = template.Name
            txtSubject.Text = template.Subject
            ftbContent.Text = template.Message
            divContent.InnerHtml = template.Message

            CreateToolbars(e.ItemID)

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)
            Dim msg As ErrorMessage

            Dim template As New EmailTemplate(Me.DbConnection, SecurityBL.GetCurrentUser().ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

            If txtSubject.Text = String.Empty Or ftbContent.Text = String.Empty Then
                If txtSubject.Text = String.Empty Then
                    lblError.Text = "A Subject must be entered."
                End If

                If ftbContent.Text = String.Empty Then
                    lblError.Text = String.Format("{0}{1}{2}", lblError.Text, _
                                                  IIf(lblError.Text.Length > 0, vbCrLf, ""), _
                                                  "A Message must be entered")
                End If
            Else
                msg = template.Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                template.Subject = txtSubject.Text
                template.Message = ftbContent.Text

                msg = template.Save()
                If Not msg.Success Then WebUtils.DisplayError(msg)

                FindClicked(e)
            End If



        End Sub

        Private Sub Page_PreRenderComplete1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
            If _stdBut.ButtonsMode <> StdButtonsMode.Edit Then
                divFreebox.Style.Add("display", "none")
                divContent.Style.Add("display", "block")
            Else
                divFreebox.Style.Add("display", "block")
                divContent.Style.Add("display", "none")
            End If

        End Sub



        Private Sub CreateToolbars(ByVal emailTemplateID As Integer)

            Dim msg As ErrorMessage
            Dim placeholders As EmailTemplatePlaceholderCollection = Nothing

            msg = EmailTemplatePlaceholder.FetchList(conn:=Me.DbConnection, list:=placeholders, emailTemplateID:=emailTemplateID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            Dim tb1 As Toolbar = New Toolbar
            With tb1.Items
                Dim htmlMenu As InsertHtmlMenu = New InsertHtmlMenu()
                'Dim htmlMenu As ToolbarDropDownList = New ToolbarDropDownList()
                htmlMenu.Title = "Placeholders"
                htmlMenu.Items.Clear()
                For Each ph As EmailTemplatePlaceholder In placeholders

                    htmlMenu.AddParsedSubObject(New ToolbarListItem(ph.PlaceholderText & " – " & ph.Description, ph.PlaceholderText))
                Next
                .Add(htmlMenu)
            End With
            ftbContent.Toolbars.RemoveAt(0)
            ftbContent.Toolbars.Insert(0, tb1)


        End Sub



    End Class

End Namespace
