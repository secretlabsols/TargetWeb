Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports System.Data.SqlClient
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports RecurrenceGenerator
Imports Target.Web.Apps.Security

Namespace Apps.Jobs
    ''' ----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Apps.Jobs.JobScheduleMaintenance
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Page to maintain details of Job schedule.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MikeVO  08/01/2010  D11435 - user-job security
    '''     MikeVO  28/10/2009  A4WA#5867 - removed enabled checkbox default
    ''' 	PaulW	24/06/2009	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Class JobScheduleMaintenance
        Inherits Target.Web.Apps.BasePage

        Private _jobTypeID As Integer
        Private _stdBut As StdButtonsBase

#Region " Page_Load "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.JobScheduleMaintenance"), "Job Service - Job Schedule Maintenance")

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                .AllowBack = True
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.JobScheduleMaintenance.AddNew"))
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.JobScheduleMaintenance.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.JobScheduleMaintenance.Delete"))
                .AllowFind = False
                .AuditLogTableNames.Add("JobSchedule")
                .EditableControls.Add(fsControls.Controls)
            End With
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked
            AddHandler _stdBut.NewClicked, AddressOf NewClicked
            AddHandler _stdBut.BeforeModeChanged, AddressOf StdButBeforeModeChanged
            AddHandler _stdBut.AfterModeChanged, AddressOf StdButAfterModeChanged

            If Not IsPostBack Then
                PopulateDropdowns()
            End If

        End Sub

#End Region

#Region " PopulateDropdowns "

        Private Sub PopulateDropdowns()

            Const SP_NAME_JOBTYPES As String = "spxJobType_FetchListForScheduledJobs"

            Dim dsJobTypes As DataSet
            Dim spParams As SqlParameter()
            Dim sysInfo As SystemInfo = DataCache.SystemInfo(Me.DbConnection, Nothing, False)
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            ' load job type combo
            spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME_JOBTYPES, False)
            spParams(0).Value = sysInfo.LicenceNo
            spParams(1).Value = currentUser.ID
            dsJobTypes = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_JOBTYPES, spParams)
            With cboJobType.DropDownList
                .DataSource = dsJobTypes.Tables(0)
                .DataTextField = "Name"
                .DataValueField = "ID"
                .DataBind()
                .Items.Insert(0, String.Empty)
            End With

            cboJobType.SelectPostBackValue()
            _jobTypeID = Utils.ToInt32(cboJobType.DropDownList.SelectedValue)

        End Sub

#End Region

#Region " StdButtons_EventHandlers "

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)
            cboJobType.DropDownList.SelectedValue = ""
            chkEnabled.CheckBox.Checked = True
            CType(recPattern, Apps.UserControls.RecurrencePattern).SetDefaults()
        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)
            Dim schedule As JobSchedule = New JobSchedule(Me.DbConnection, String.Empty, String.Empty)
            Dim msg As ErrorMessage

            If e.ItemID > 0 Then
                msg = schedule.Fetch(e.ItemID)
                cboJobType.DropDownList.SelectedValue = schedule.JobTypeID
                txtDescription.Text = schedule.Description
                chkEnabled.CheckBox.Checked = schedule.Enabled
                CType(recPattern, Apps.UserControls.RecurrencePattern).SetRecurrencePattern(schedule.Pattern)
            End If

        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                cboJobType.DropDownList.SelectedIndex = -1
                txtDescription.Text = ""
                CType(recPattern, Apps.UserControls.RecurrencePattern).SetDefaults()
            Else
                FindClicked(e)
            End If
        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim schedule As New JobSchedule(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            Dim msg As ErrorMessage

            If e.ItemID > 0 Then
                msg = schedule.Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                msg = schedule.Delete
                If Not msg.Success Then
                    WebUtils.DisplayError(msg)
                Else
                    cboJobType.DropDownList.SelectedIndex = -1
                    txtDescription.Text = ""
                    CType(recPattern, Apps.UserControls.RecurrencePattern).SetDefaults()
                End If
            End If

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim schedule As New JobSchedule(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            Dim msg As ErrorMessage
            Dim message As String = ""

            If Not CType(recPattern, Apps.UserControls.RecurrencePattern).IsValid(message) Then
                lblError.Text = message
                e.Cancel = True
                Exit Sub
            End If


            If e.ItemID > 0 Then
                msg = schedule.Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If

            With schedule
                .Description = txtDescription.Value
                .JobTypeID = cboJobType.DropDownList.SelectedValue
                .Enabled = chkEnabled.CheckBox.Checked
                .Pattern = CType(recPattern, Apps.UserControls.RecurrencePattern).GetRecurringPattern
                .NextRunDate = RecurrenceHelper.GetNextDate(Date.Now, .Pattern)
                msg = .Save()
                If Not msg.Success Then
                    e.Cancel = True
                    WebUtils.DisplayError(msg)
                Else
                    e.ItemID = .ID
                    FindClicked(e)
                End If

            End With

        End Sub

        Private Sub StdButBeforeModeChanged(ByRef e As StdButtonBeforeModeEventArgs)
            CType(recPattern, Apps.UserControls.RecurrencePattern).InitControl(Convert.ToInt32(e.Mode))
        End Sub

        Private Sub StdButAfterModeChanged(ByVal e As StdButtonAfterModeEventArgs)
            CType(recPattern, Apps.UserControls.RecurrencePattern).InitControl(Convert.ToInt32(e.Mode))
        End Sub

#End Region

#Region " Page_PreRender "

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            CType(recPattern, Apps.UserControls.RecurrencePattern).InitControl(Convert.ToInt32(_stdBut.ButtonsMode))
        End Sub

#End Region

    End Class

End Namespace