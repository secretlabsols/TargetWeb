
Imports System.Configuration.ConfigurationManager
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
    ''' Class	 : Apps.Jobs.JobStepInputDefaults
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Page to allow the user to view/edit the default values of job step inputs.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MikeVO  01/12/2008  D11444 - security overhaul.
    ''' 	MikeVO	11/07/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class JobStepInputDefaults
        Inherits Target.Web.Apps.BasePage

        Private Const TXT_DEFAULT_VALUE As String = "txtDefaultValue"

#Region " Page_Load "

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.JobStepInputs"), "Job Service - Job Step Input Default Values")

            Dim stdBut As StdButtonsBase = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With stdBut
                .AllowBack = False
                .AllowDelete = False
                .AllowNew = False
                .AllowFind = False
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.JobStepInputs.Edit"))
                .EditableControls.Add(fsControls.Controls)
                .AuditLogTableNames.Add("JobType_JobStepTypeInput")
            End With
            AddHandler stdBut.FindClicked, AddressOf FindClicked
            AddHandler stdBut.EditClicked, AddressOf FindClicked
            AddHandler stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler stdBut.CancelClicked, AddressOf FindClicked

        End Sub

#End Region

#Region " FindClicked "

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Const SP_NAME As String = "spxJobType_JobStepTypeInput_FetchForSite"

            Dim spParams As SqlParameter()
            Dim ds As DataSet, defaults As DataTable
            Dim tr As TableRow
            Dim td As TableCell
            Dim txt As TextBoxEx
            Dim sysInfo As SystemInfo = DataCache.SystemInfo(Me.DbConnection, Nothing, False)

            Try
                spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME, False)
                spParams(0).Value = sysInfo.LicenceNo
                ds = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams)
                defaults = ds.Tables(0)

                For Each row As DataRow In defaults.Rows

                    ' row
                    tr = New TableRow()
                    phDefaults.Controls.Add(tr)

                    ' job
                    td = New TableCell()
                    tr.Controls.Add(td)
                    td.Text = row("Job")

                    ' job step
                    td = New TableCell()
                    tr.Controls.Add(td)
                    td.Text = row("JobStep")

                    ' input name
                    td = New TableCell()
                    tr.Controls.Add(td)
                    td.Text = row("InputName")

                    ' default value
                    td = New TableCell()
                    tr.Controls.Add(td)
                    txt = New TextBoxEx()
                    With txt
                        .ID = String.Format("{0}{1}", TXT_DEFAULT_VALUE, row("ID"))
                        .Required = True
                        .RequiredValidatorErrMsg = "Please enter a default value"
                        .Text = row("DefaultValue")
                    End With
                    td.Controls.Add(txt)

                Next

            Catch ex As Exception
                WebUtils.DisplayError(Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber))    ' unexpected
            End Try

        End Sub

#End Region

#Region " SaveClicked "

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim inputDefault As JobType_JobStepTypeInput
            Dim trans As SqlTransaction = Nothing
            Dim startPos As Integer, endPos As Integer, id As Integer
            Dim name As String, defaultValue As String
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            Try
                trans = SqlHelper.GetTransaction(Me.DbConnection)

                For Each key As String In Request.Form.AllKeys

                    ' get the folder field for each watcher entry
                    If key.Contains(TXT_DEFAULT_VALUE) Then

                        ' extract the control name
                        startPos = key.IndexOf(TXT_DEFAULT_VALUE)
                        endPos = key.IndexOf("$", startPos)
                        name = key.Substring(startPos, endPos - startPos)
                        ' get the input id
                        id = Utils.ToInt32(name.Replace(TXT_DEFAULT_VALUE, String.Empty))
                        ' get the default values
                        defaultValue = Request.Form(key)

                        ' update the default value
                        inputDefault = New JobType_JobStepTypeInput(trans, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                        With inputDefault
                            msg = .Fetch(id)
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                            .DefaultValue = defaultValue
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