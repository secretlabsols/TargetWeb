
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Web.Apps.Security

Namespace Apps.Jobs

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Apps.Jobs.FileWatcher
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Page to allow the user to view/edit existing file watcher records.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MikeVO  01/12/2008  D11444 - security overhaul.
    ''' 	MikeVO	20/06/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class FileWatcher
        Inherits Target.Web.Apps.BasePage

        Private Const TXT_FOLDER As String = "txtFolder"
        Private Const TXT_FILENAME As String = "txtFilename"

#Region " Page_Load "

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.FileWatcher"), "Job Service - File Watcher")

            Dim stdBut As StdButtonsBase = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With stdBut
                .AllowBack = False
                .AllowDelete = False
                .AllowNew = False
                .AllowFind = False
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.FileWatcher.Edit"))
                .EditableControls.Add(fsControls.Controls)
                .AuditLogTableNames.Add("JobFileWatcher")
            End With
            AddHandler stdBut.FindClicked, AddressOf FindClicked
            AddHandler stdBut.EditClicked, AddressOf FindClicked
            AddHandler stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler stdBut.CancelClicked, AddressOf FindClicked

        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim watchers As JobFilewatcherCollection = Nothing
            Dim tr As TableRow
            Dim td As TableCell
            Dim txt As TextBoxEx

            msg = JobFilewatcher.FetchList(Me.DbConnection, watchers, String.Empty, String.Empty)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            For Each w As JobFilewatcher In watchers

                ' row
                tr = New TableRow()
                phWatchers.Controls.Add(tr)

                ' folder
                td = New TableCell()
                tr.Controls.Add(td)
                txt = New TextBoxEx()
                With txt
                    .ID = String.Format("{0}{1}", TXT_FOLDER, w.ID)
                    .Width = New Unit(75, UnitType.Percentage)
                    .Required = True
                    .RequiredValidatorErrMsg = "Please enter a folder path"
                    .Text = w.Folder
                End With
                td.Controls.Add(txt)

                ' filename
                td = New TableCell()
                tr.Controls.Add(td)
                txt = New TextBoxEx()
                With txt
                    .ID = String.Format("{0}{1}", TXT_FILENAME, w.ID)
                    .Width = New Unit(40, UnitType.Percentage)
                    .Required = True
                    .RequiredValidatorErrMsg = "Please enter a filename pattern"
                    .Text = w.Filename
                End With
                td.Controls.Add(txt)

            Next

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim watcher As JobFilewatcher
            Dim trans As SqlTransaction = Nothing
            Dim startPos As Integer, endPos As Integer, id As Integer
            Dim name As String, folder As String, filename As String
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            Try
                trans = SqlHelper.GetTransaction(Me.DbConnection)

                For Each key As String In Request.Form.AllKeys

                    ' get the folder field for each watcher entry
                    If key.Contains(TXT_FOLDER) Then

                        ' extract the control name
                        startPos = key.IndexOf(TXT_FOLDER)
                        endPos = key.IndexOf("$", startPos)
                        name = key.Substring(startPos, endPos - startPos)
                        ' get the watcher id
                        id = Utils.ToInt32(name.Replace(TXT_FOLDER, String.Empty))
                        ' get the folder and filename values
                        folder = Request.Form(key)
                        filename = Request.Form(key.Replace(TXT_FOLDER, TXT_FILENAME))

                        ' update the watcher
                        watcher = New JobFilewatcher(trans, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                        With watcher
                            msg = .Fetch(id)
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                            .Folder = folder
                            .Filename = filename
                            msg = .Save()
                        End With

                    End If

                Next

                trans.Commit()

                FindClicked(e)

            Catch ex As Exception
                WebUtils.DisplayError(Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber))    ' unexpected
            Finally
                SqlHelper.RollbackTransaction(trans)
            End Try

        End Sub

#End Region

    End Class

End Namespace